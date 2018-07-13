using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LitJson;
using OneGame.Lua;
using UnityEngine;
using UnityEngine.Networking;

namespace OneGame {
    using Type = Lua.Type;
    using Debug = OneGame.Logger;

    /// <summary>
    /// Handles script loading and referencing
    /// </summary>
    [CreateAssetMenu (menuName = "Scriptable Objects/Databases/Script Database")]
    public class ScriptCatalog : ScriptableObject {
        /// <summary>
        /// Is the script database ready?
        /// </summary>
        /// <returns></returns>
        public bool IsReady { get; private set; }

        /// <summary>
        /// The current set of available scripts
        /// </summary>
        /// <returns></returns>
        public ScriptData[] Scripts {
            get {
                var scriptArray = new ScriptData[scripts.Count];
                scripts.Values.CopyTo (scriptArray, 0);
                return scriptArray;
            }
        }

        [SerializeField, Header ("Urls")]
        private string scriptUrl;
        [SerializeField]
        private string urlPrefix;

        private Dictionary<uint, ScriptData> scripts;
        private Queue<Tuple<uint, string>> downloadQueue;
        private NativeComponentInfo[] availableComponents;

        private struct NativeComponentInfo {
            public Type type;
            public string name;
        }

        private void OnEnable () {
            downloadQueue = new Queue<Tuple<uint, string>> ();
            scripts = new Dictionary<uint, ScriptData> ();
            IsReady = false;

            // TODO: Move this to a bridge
            Database.OnScriptFindRequest += FindScript;
            Database.OnComponentFindRequest += FindComponent;
        }

        private void OnDisable () {
            downloadQueue.Clear ();

            Database.OnScriptFindRequest -= FindScript;
            Database.OnComponentFindRequest -= FindComponent;
        }

        /// <summary>
        /// Finds a component with a matching name
        /// </summary>
        public Type FindComponent (string name) {
            var normalizedName = name.ToLowerInvariant ();

            for (var i = 0; i < availableComponents.Length; ++i) {
                var component = availableComponents[i];

                if (component.name == normalizedName) {
                    return component.type;
                }
            }

            return default (Type);
        }

        /// <summary>
        /// Finds a script with a matching id
        /// </summary>
        /// <param name="id">The script id</param>
        public string FindScript (uint id) {
            ScriptData data;

            if (scripts.TryGetValue (id, out data)) {
                return data.script;
            }

            return string.Empty;
        }

        /// <summary>
        /// Loads lua scripts into the catalog
        /// </summary>
        public void LoadScripts () {
            FetchComponentsFromAssemblies ();

            IsReady = false;
            downloadQueue.Clear ();

            Debug.Log ("[Script Catalog] Downloading scripts..");

            var manifestWWW = UnityWebRequest.Get (scriptUrl).SendWebRequest ();
            manifestWWW.completed += (mc) => {
                // Generate script data
                var jsonData = JsonMapper.ToObject (DownloadHandlerBuffer.GetContent (manifestWWW.webRequest));
                var jsonArray = jsonData[0];

                for (var i = 0; i < jsonArray.Count; ++i) {
                    var datum = jsonArray[i];

                    var scriptData = new ScriptData {
                        id = (uint)datum["id"],
                        name = datum["name"].GetString (),
                    };

                    scripts.Add (scriptData.id, scriptData);

                    // Download the script
                    var url = string.Format ("{0}{1}", urlPrefix, datum["asset-path"]);
                    downloadQueue.Enqueue (new Tuple<uint, string> (scriptData.id, url));
                }

                DownloadFromQueue (downloadQueue);

                manifestWWW.webRequest.Dispose ();
            };
        }

        /// <summary>
        /// Downloads the scripts, one by one on a download queue
        /// </summary>
        private void DownloadFromQueue (Queue<Tuple<uint, string>> queue) {
            if (queue.Count > 0) {
                DownloadScript (queue.Peek (),
                    () => {
                        queue.Dequeue ();
                        DownloadFromQueue (queue);
                    },
                    () => {
                        Debug.LogWarningFormat (
                            "[Script Catalog] <color=red>Skipping '{0}'...this may break saves if it relies on this script</color>",
                            scripts[queue.Peek ().item1]);

                        queue.Dequeue ();
                        DownloadFromQueue (queue);
                    });
            } else {
                Debug.LogWarning ("[Script Catalog] <color=green>Download complete!</color>");
                IsReady = true;
            }
        }

        /// <summary>
        /// Downloads a script from the internet
        /// </summary>
        private void DownloadScript (Tuple<uint, string> scriptInfo, Action onSuccess, Action onFail) {
            var www = new UnityWebRequest (scriptInfo.item2);
            var downloadHandler = new DownloadHandlerBuffer ();
            www.downloadHandler = downloadHandler;

            Debug.LogFormat ("[Script Catalog] Downloading '{0}'...", scripts[scriptInfo.item1].name);

            var op = www.SendWebRequest ();

            op.completed += (c) => {
                if (www.isNetworkError || www.isHttpError) {

                    Debug.LogFormat ("[Script Catalog] <color=red>Error! Cannot download '{0}'!\n{1}...<color>",
                        scripts[scriptInfo.item1].name, www.error);

                    onFail?.Invoke ();
                } else {
                    UpdateCode (scriptInfo.item1, downloadHandler.text);

                    Debug.LogFormat ("[Script Catalog] Downloaded '{0}'", scripts[scriptInfo.item1].name);

                    onSuccess?.Invoke ();
                }

                www.Dispose ();
            };
        }

        /// <summary>
        /// Loads native components from avaiable assemblies
        /// </summary>
        private void FetchComponentsFromAssemblies () {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies ();
            var typeList = new List<NativeComponentInfo> ();

            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes ().Where (
                        t => {
                            return t != null && !t.IsAbstract && t.IsSubclassOf (typeof (NativeComponent));
                        }).Select<System.Type, NativeComponentInfo> (
                        s => new NativeComponentInfo {
                            type = new Type { type = s },
                            name = s.Name.ToLower ()
                        }); ;

                typeList.AddRange (types);
            }

            availableComponents = typeList.ToArray ();
        }



        /// <summary>
        /// Finds a script with a matching name
        /// </summary>
        /// <param name="name">The script's name</param>
        private string FindScript (string name) {
            var id = default (uint);
            uint.TryParse (name, System.Globalization.NumberStyles.HexNumber, null, out id);

            foreach (var value in scripts.Values) {
                if (value.id == id || value.name == name) {
                    return value.script;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Updates the lua script in the internal dictionary
        /// </summary>
        /// <param name="id">The id of the script</param>
        /// <param name="code">The lua code</param>
        private void UpdateCode (uint id, string code) {
            ScriptData scriptData;

            if (scripts.TryGetValue (id, out scriptData)) {
                scriptData.script = code;
                scripts[id] = scriptData;
            }
        }

    }
}