using System.Collections.Generic;
using UnityEngine.Networking;
using LitJson;
using System;

namespace OneGame {
    using DownloadQueue = Queue<Tuple<ScriptData, string>>;

    /// <summary>
    /// A collection of utility scripts involving the script catalog
    /// </summary>
    internal static class ScriptCatalogUtility {

        /// <summary>
        /// Downloads a series of scripts from the internet
        /// </summary>
        /// <param name="url">The url of the script manifest</param>
        /// <param name="urlPrefix">The root url</param>
        /// <param name="onScriptDownload">The event where a script was downloaded</param>
        /// <param name="onComplete">The event called when the entire process is complete</param>
        internal static void DownloadScripts (string url, string urlPrefix, Action<ScriptData> onScriptDownload, Action onComplete) {
            var www = UnityWebRequest.Get (url);
            var handler = new DownloadHandlerBuffer ();
            www.downloadHandler = handler;
            var op = www.SendWebRequest ();

            op.completed += c => {
                var json = JsonMapper.ToObject (handler.text);
                var array = json[0];
                var downloadQueue = new DownloadQueue ();

                for (var i = 0; i < array.Count; ++i) {
                    var datum = array[i];

                    var scriptData = new ScriptData {
                        id = (uint)datum["id"],
                        name = datum["name"].GetString ()
                    };

                    var scriptUrl = string.Format ("{0}{1}", urlPrefix, datum["asset-path"]);
                    downloadQueue.Enqueue (new Tuple<ScriptData, string> (scriptData, scriptUrl));
                }

                ProcessDownloadQueue (downloadQueue, onScriptDownload, onComplete);
            };
        }

        private static void ProcessDownloadQueue (DownloadQueue queue, Action<ScriptData> onSuccess, Action onDownloadComplete) {
            if (queue.Count > 0) {
                ProcessScriptDownload (queue.Peek (),
                s => {
                    onSuccess?.Invoke (s);
                    queue.Dequeue ();
                    ProcessDownloadQueue (queue, onSuccess, onDownloadComplete);
                },
                () => {
                    Logger.LogWarningFormat (
                        "[Script Catalog] <color=red>Skipping '{0}'...this may break saves if it relies on this script</color>",
                        queue.Peek ().item1.name);

                    queue.Dequeue ();
                    ProcessDownloadQueue (queue, onSuccess, onDownloadComplete);
                });
            } else {
                onDownloadComplete?.Invoke ();
            }
        }

        private static void ProcessScriptDownload (Tuple<ScriptData, string> scriptInfo, Action<ScriptData> onSuccess, Action onFail) {
            var www = new UnityWebRequest (scriptInfo.item2);
            var downloadHandler = new DownloadHandlerBuffer ();
            www.downloadHandler = downloadHandler;

            Logger.LogFormat ("[Script Catalog] Downloading '{0}'...", scriptInfo.item1.name);

            var op = www.SendWebRequest ();

            op.completed += (c) => {
                if (www.isNetworkError || www.isHttpError) {

                    Logger.LogFormat ("[Script Catalog] <color=red>Error! Cannot download '{0}'!\n{1}...<color>",
                        scriptInfo.item1.name, www.error);

                    onFail?.Invoke ();
                } else {
                    Logger.LogFormat ("[Script Catalog] Downloaded '{0}'", scriptInfo.item1.name);

                    onSuccess?.Invoke (
                        new ScriptData {
                            id = scriptInfo.item1.id,
                            name = scriptInfo.item1.name,
                            script = downloadHandler.text
                        });
                }

                www.Dispose ();
            };
        }
    }
}