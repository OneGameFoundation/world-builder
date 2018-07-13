using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;
using OneGame.Lua;
using UnityEngine;

namespace OneGame {
    /// <summary>
    /// An enum describing where the save file should save to
    /// </summary>
    public enum JsonSaveType {
        /// <summary>
        /// The save file will be saved to a temporary buffer
        /// </summary>
        Buffer,

        /// <summary>
        /// The save file will be saved to a local file
        /// </summary>
        File
    }

    /// <summary>
    /// Handles saving and loading the current state of the world 
    /// </summary>
    [CreateAssetMenu (menuName = "Scriptable Objects/Managers/Save Manager")]
    public class SaveManager : ScriptableObject {

        public string ActiveSaveId { get; set; }
        public string[] WorldIds {
            get {
                var ids = new string[saves.Count];
                saves.Keys.CopyTo (ids, 0);
                return ids;
            }
        }

        public SaveData this [string name, JsonSaveType type] {
            get {
                string[] buffers;

                if (saves.TryGetValue (name, out buffers)) {
                    return CreateFromSaveString (buffers[(int) type]);
                }

                return SaveData.Empty;
            }
            set {
                string[] buffers;

                if (!saves.TryGetValue (name, out buffers)) {
                    buffers = new string[2];
                    saves.Add (name, buffers);
                }

                buffers[(int) type] = JsonMapper.ToJson (value);
            }
        }

        private Dictionary<string, string[]> saves;

        private void OnEnable () {
            saves = new Dictionary<string, string[]> ();
            PopulateSaves ();
        }

        public void SaveWorld (string name, JsonSaveType type) {
            string[] buffers;

            if (saves.TryGetValue (name, out buffers)) {
                Save (buffers[(int) type], string.Format ("save_{0}.json", name));
            }
        }

        /// <summary>
        /// Creates a new save data from the current json save string
        /// </summary>
        /// <param name="jsonString">The json-style save string</param>
        private SaveData CreateFromSaveString (string jsonString) {
            var saveList = new EntitySaveData[0];
            var playerPosition = new Vector3Double ();
            var stringNames = new string[0];
            var stringValues = new string[0];
            var numberNames = new string[0];
            var numberValues = new double[0];
            var boolNames = new string[0];
            var boolValues = new bool[0];

            if (!string.IsNullOrEmpty (jsonString)) {
                var json = LitJson.JsonMapper.ToObject (jsonString);
                var entityData = json["entities"];
                saveList = new EntitySaveData[entityData.Count];

                for (var i = 0; i < entityData.Count; ++i) {
                    var datum = entityData[i];

                    // Fetch the scripts from the save data
                    var scriptJson = datum["script"];
                    var scriptData = new LuaScript {
                        id = (uint) scriptJson["id"],
                        code = scriptJson["code"].GetString (),
                        properties = scriptJson.Keys.Contains ("properties") ?
                        ExtractScriptProperties (scriptJson["properties"]) : new ScriptProperty[0]
                    };

                    var saveData = new EntitySaveData {
                        id = (uint) datum["id"],
                        uniqueId = datum["uniqueId"].GetString (),
                        type = (int) datum["type"],
                        scale = new Vector3Double ((float) datum["scale"]["x"], (float) datum["scale"]["y"], (float) datum["scale"]["z"]),
                        position = new Vector3Double ((float) datum["position"]["x"], (float) datum["position"]["y"], (float) datum["position"]["z"]),
                        rotation = new QuaternionDouble ((float) datum["rotation"]["x"], (float) datum["rotation"]["y"], (float) datum["rotation"]["z"], (float) datum["rotation"]["w"]),
                        script = scriptData
                    };

                    saveList[i] = saveData;
                }

                var keys = json.Keys;

                // Load the platyer position, if possible
                if (keys.Contains ("playerPosition")) {
                    playerPosition.x = (double) json["playerPosition"]["x"];
                    playerPosition.y = (double) json["playerPosition"]["y"];
                    playerPosition.z = (double) json["playerPosition"]["z"];
                }

                // Load the metadata, if possible
                if (keys.Contains ("stringNames")) {
                    stringNames = ExtractStrings (json["stringNames"]);
                }
                if (keys.Contains ("numberNames")) {
                    numberNames = ExtractStrings (json["numberNames"]);
                }
                if (keys.Contains ("boolNames")) {
                    boolNames = ExtractStrings (json["boolNames"]);
                }
                if (keys.Contains ("stringValues")) {
                    stringValues = ExtractStrings (json["stringValues"]);
                }
                if (keys.Contains ("numberValues")) {
                    numberValues = ExtractNumbers (json["numberValues"]);
                }
                if (keys.Contains ("boolValues")) {
                    boolValues = ExtractBooleans (json["boolValues"]);
                }
            }

            return new SaveData {
                entities = saveList,
                    playerPosition = playerPosition,
                    stringNames = stringNames,
                    stringValues = stringValues,
                    numberNames = numberNames,
                    numberValues = numberValues,
                    boolNames = boolNames,
                    boolValues = boolValues
            };
        }

        /// <summary>
        /// Extracts all boolean values from the json file
        /// </summary>
        private bool[] ExtractBooleans (JsonData data) {
            var length = data.Count;
            var bools = new bool[length];

            for (var i = 0; i < length; ++i) {
                bools[i] = data[i].GetBoolean ();
            }

            return bools;
        }

        /// <summary>
        /// Extracts all numerical values from the json file
        /// </summary>
        private double[] ExtractNumbers (JsonData data) {
            var length = data.Count;
            var numbers = new double[length];

            for (var i = 0; i < length; ++i) {
                numbers[i] = data[i].GetReal ();
            }

            return numbers;
        }

        /// <summary>
        /// Extracts script properties from json data
        /// </summary>
        private ScriptProperty[] ExtractScriptProperties (JsonData data) {
            var props = new ScriptProperty[data.Count];

            for (var i = 0; i < props.Length; ++i) {
                var datum = data[i];

                var prop = new ScriptProperty {
                    name = datum["name"].GetString (),
                    value = datum["value"].GetString (),
                    type = (PropertyType) (int) datum["type"]
                };

                props[i] = prop;
            }

            return props;
        }

        /// <summary>
        /// Extracts a string array from the json data
        /// </summary>
        private string[] ExtractStrings (JsonData data) {
            var length = data.Count;
            var strings = new string[length];

            for (var i = 0; i < length; ++i) {
                strings[i] = data[i].GetString ();
            }

            return strings;
        }

        /// <summary>
        /// Fetches save file paths from a remote location
        /// </summary>
        /// <param name="path">The path to look in</param>
        private string[] FetchSaveFiles (string path) {
            var files = Directory.GetFiles (path);
            var paths = new List<string> ();

            for (var i = 0; i < files.Length; ++i) {
                var filePath = files[i];

                if (filePath.Contains ("save_") && filePath.Contains (".json") && !filePath.Contains (".meta")) {
                    paths.Add (filePath);
                }
            }

            return paths.ToArray ();
        }

        /// <summary>
        /// Populates the buffers with previously stored save files
        /// </summary>
        private void PopulateSaves () {
            var filePaths = FetchSaveFiles (Application.dataPath);
            var split = new char[] { '/', '\\', '.', '_' };

            for (var i = 0; i < filePaths.Length; ++i) {
                var name = filePaths[i].Split (split, System.StringSplitOptions.RemoveEmptyEntries);
                var saveText = File.ReadAllText (filePaths[i]);
                saves.Add (name[name.Length - 2], new string[] { saveText, saveText });
            }
        }

        /// <summary>
        /// Saves a json string into a file
        /// </summary>
        /// <param name="json">The json representation of the save file</param>
        /// <param name="filename">The name of the save</param>
        private void Save (string json, string filename) {
            var bytes = Encoding.UTF8.GetBytes (json);
            File.WriteAllBytes (Application.dataPath + "/" + filename, bytes);

            Debug.LogFormat ("File saved at {0}", Application.dataPath + "/" + filename);
        }

        /// <summary>
        /// Loads a json string from a file
        /// </summary>
        /// <param name="filename">The name of the file</param>
        private string Load (string filename) {
            var text = string.Empty;
            if (File.Exists (Application.dataPath + "/" + filename)) {
                text = File.ReadAllText (Application.dataPath + "/" + filename);
            }

            return text;
        }

    }
}