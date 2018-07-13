using System;
using System.Collections.Generic;
using System.Globalization;
using LitJson;
using OneGame.Lua;
using UnityEngine;
using UnityEngine.Networking;

namespace OneGame {
    using Debug = OneGame.Logger;

    /// <summary>
    /// A localized database that contains items that the player can use to build worlds
    /// </summary>
    [CreateAssetMenu (menuName = "Scriptable Objects/Databases/Item Catalog")]
    public class ItemCatalog : ScriptableObject {
        /// <summary>
        /// The current manifest of elements in the game
        /// </summary>
        public ElementData[] Elements { get { return elements; } }

        /// <summary>
        /// The current instance of the catalog
        /// </summary>
        public static ItemCatalog Instance { get; private set; }

        /// <summary>
        /// The current loading status of the database. The database is minimally ready
        /// when its state is set to DatabaseStatus.RawDataLoaded
        /// </summary>
        public LoadingStatus Status {
            get {
                return downloadStatus == DownloadStatus.Completed ? LoadingStatus.Completed : LoadingStatus.NotLoaded;
            }
        }

        /// <summary>
        /// Describes the current loading status
        /// </summary>
        public enum LoadingStatus { NotLoaded, RawDataLoaded, Completed }
        private enum DownloadStatus { Idle, Downloading, Completed }

        /// <summary>
        /// A collection of metadata describing an AssetBundle
        /// </summary>
        private struct BundleData {
            public uint id;
            public string name;
            public string bundleName;
            public string path;
        }

        [SerializeField]
        private string assetListUrl;
        [SerializeField]
        private string urlPrefix;

        private ElementData[] elements;
        private Dictionary<uint, AssetBundle> assetBundles;
        private Dictionary<uint, string> assetPaths;
        private Queue<BundleData> downloadQueue;
        private DownloadStatus downloadStatus;
        private readonly object lockObj = new object ();

        private void OnEnable () {
            elements = new ElementData[0];
            downloadQueue = new Queue<BundleData> ();
            assetBundles = new Dictionary<uint, AssetBundle> ();
            assetPaths = new Dictionary<uint, string> ();
            downloadStatus = DownloadStatus.Idle;

            // TODO: Move the following block of code into a lua bridge
            Database.OnItemFindRequest += FindItem;

            Instance = this;
        }

        private void OnDisable () {
            AssetBundle.UnloadAllAssetBundles (true);
            assetBundles.Clear ();
            assetPaths.Clear ();

            // TODO: Move the following block of code into a lua bridge
            Database.OnItemFindRequest -= FindItem;
        }

        /// <summary>
        /// Loads an asset from the asset bundle
        /// </summary>
        /// <param name="id">The id of the asset</param>
        /// <typeparam name="T">The type to load</typeparam>
        public T GetAsset<T> (uint id) where T : UnityEngine.Object {
            var item = FindElementData (id);
            return GetAsset<T> (item);
        }

        public T GetAsset<T> (ElementData item) where T : UnityEngine.Object {
            AssetBundle bundle;

            if (assetBundles.TryGetValue (item.bundleId, out bundle)) {
                //TODO: Append the correct path
                var path = assetPaths[item.id] + ".prefab";
                var asset = bundle.LoadAsset (path);

                if (asset is T) {
                    return asset as T;
                } else {
                    throw new System.Exception (string.Format ("Asset is not a '{0}' type!", typeof (T).Name));
                }
            }

            return default (T);
        }

        /// <summary>
        /// Gets an item with a matching id
        /// </summary>
        /// <param name="id">The id to query</param>
        /// <returns>An empty ElementData if none are found</returns>
        public ElementData FindElementData (uint id) {
            for (var i = 0; i < elements.Length; ++i) {
                var element = elements[i];

                if (element.id == id) {
                    return element;
                }
            }

            return ElementData.Empty;
        }

        /// <summary>
        /// Finds an item by name
        /// </summary>
        /// <param name="name">The name of the item</param>
        public Item FindItem (string name) {
            uint id = 0;
            uint.TryParse (name, System.Globalization.NumberStyles.HexNumber, null, out id);

            for (var i = 0; i < elements.Length; ++i) {
                var element = elements[i];

                if (element.id == id || element.name == name) {
                    return new Item {
                        id = element.id,
                        name = element.name
                    };
                }
            }

            return default (Item);
        }

        /// <summary>
        /// Loads assets from a remote server and internally populates the list of 
        /// items in the game
        /// </summary>
        public void LoadAssets () {
            downloadQueue.Clear ();
            downloadStatus = DownloadStatus.Downloading;

            var manifestRequest = UnityWebRequest.Get (assetListUrl);
            var manifestWWW = manifestRequest.SendWebRequest ();

            Debug.Log ("[Item Catalog] Starting download...");

            manifestWWW.completed += c => {
                var json = DownloadHandlerBuffer.GetContent (manifestRequest);
                var data = ProcessBundleJsonData (json);

                for (var i = 0; i < data.Length; ++i) {
                    downloadQueue.Enqueue (data[i]);
                }

                DownloadFromQueue (downloadQueue);

                manifestRequest.Dispose ();
            };
        }

        /// <summary>
        /// Adds a collection of elements into an existing collection
        /// </summary>
        /// <param name="array">The elements to add</param>
        private void AddToElementCollection (ElementData[] array) {
            lock (lockObj) {
                var extendedArray = new ElementData[elements.Length + array.Length];
                elements.CopyTo (extendedArray, 0);
                array.CopyTo (extendedArray, elements.Length);

                elements = extendedArray;
            }
        }

        /// <summary>
        /// Processes the downloads, one by one on a queue
        /// </summary>
        /// <param name="queue">The download queue</param>
        private void DownloadFromQueue (Queue<BundleData> queue) {
            if (queue.Count > 0) {
                LoadAssetBundle (queue.Peek (),
                    () => {
                        queue.Dequeue ();
                        DownloadFromQueue (queue);
                    },
                    () => {
                        Debug.LogWarning ("[Item Catalog] Skipping bundle - this might break saves");
                        queue.Dequeue ();
                        DownloadFromQueue (queue);
                    });
            } else {
                downloadStatus = DownloadStatus.Completed;

                Debug.LogWarning ("[Item Catalog] <color=green>Download complete!</color>");
            }
        }

        /// <summary>
        /// Extracts metadata from a JSON data that describes any external properties
        /// of an asset
        /// </summary>
        private Tuple<string, string>[] ExtractMetadata (JsonData metadata) {
            var meta = new Tuple<string, string>[metadata.Count];

            for (var i = 0; i < metadata.Count; ++i) {
                var current = metadata[i];
                var key = current.Keys.GetEnumerator ();
                key.MoveNext ();

                var value = current[key.Current].GetString ();

                meta[i] = new Tuple<string, string> (key.Current, value);
            }

            return meta;
        }

        /// <summary>
        /// Extracts tags for a given element
        /// </summary>
        /// <param name="tagData">The tag data to extract froms</param>
        private string[] ExtractTags (JsonData tagData) {
            var tags = new string[tagData.Count];

            for (var k = 0; k < tagData.Count; ++k) {
                tags[k] = tagData[k].GetString ();
            }

            return tags;
        }



        /// <summary>
        /// Loads an asset bundle from a given bundle information
        /// </summary>
        /// <param name="bundleData">The AssetBundle information to load from</param>
        private void LoadAssetBundle (BundleData bundleData, Action onSuccess, Action onFail) {
            var bundleWWW = new UnityWebRequest (bundleData.path);
            var handler = new DownloadHandlerAssetBundle (bundleData.path, 0);
            bundleWWW.downloadHandler = handler;

            Debug.LogFormat ("[Item Catalog] Attempting to download '{0}'...", bundleData.name);
            var op = bundleWWW.SendWebRequest ();

            op.completed += (c) => {
                if (bundleWWW.isNetworkError || bundleWWW.isNetworkError) {
                    Debug.LogFormat ("[Item Catalog] <color=red>Error: Cannot download '{0}'!\n</color>", bundleData.name, bundleWWW.error);
                    onFail?.Invoke ();
                } else {
                    var bundle = handler.assetBundle;
                    assetBundles.Add ((uint)bundleData.id, bundle);

                    var manifest = bundle.LoadAsset<TextAsset> (string.Format ("{0}-manifest.json", bundle.name));
                    var data = JsonMapper.ToObject (manifest.text);
                    var assetList = data["assets"];

                    // Create a new array of elements, and loop through the json data
                    // to populate element properties
                    var elements = new ElementData[assetList.Count];
                    for (var i = 0; i < assetList.Count; ++i) {
                        var asset = assetList[i];
                        var id = asset["id"].GetString ();

                        var element = new ElementData {
                            name = asset["name"].GetString (),
                            id = id.ToUInt (),
                            bundleId = (uint)bundleData.id,
                            description = asset["description"].GetString (),
                            tags = ExtractTags (asset["tags"]),
                            metadata = ExtractMetadata (asset["metadata"])
                        };

                        // load the thumbnail
                        var thumbnail = bundle.LoadAsset<Texture2D> (id);
                        element.thumbnail = Sprite.Create (thumbnail, new Rect (0, 0, 128f, 128f), new Vector2 (0.5f, 0.5f), 100f);

                        assetPaths.Add (element.id, asset["prefab"].GetString ().Replace (" (UnityEngine.GameObject)", ""));
                        elements[i] = element;
                    }

                    AddToElementCollection (elements);

                    Debug.LogFormat ("[Item Catalog] Downloaded '{0}'", bundleData.name);

                    onSuccess?.Invoke ();
                }
            };
        }

        /// <summary>
        /// Parses and populates AssetBundle information for future loading
        /// </summary>
        /// <param name="json">The json string to parse from</param>
        private BundleData[] ProcessBundleJsonData (string json) {
            var data = JsonMapper.ToObject (json);
            var bundles = new BundleData[data.Count];

            for (var i = 0; i < data.Count; ++i) {
                var bundleElement = data[i];

                bundles[i] = new BundleData {
                    name = bundleElement["name"].GetString (),
                    id = bundleElement["id"].GetString ().ToUInt (),
                    bundleName = bundleElement["bundle-name"].GetString (),
                    path = string.Format ("{0}{1}", urlPrefix, bundleElement["path"].GetString ())
                };
            }

            return bundles;
        }
    }
}