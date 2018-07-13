using UnityEngine;

namespace OneGame.Lua {
    public static class MetadataUtilities {

        public static float GetFloat (string name, IMetadata metadata) {
            var data = metadata.metadata;
            var number = 0f;

            for (var i = 0; i < data.Length; ++i) {
                var datum = data[i];

                if (datum.item1 == name) {
                    if (float.TryParse (datum.item2, out number)) {
                        return number;
                    }
                }
            }

            return number;
        }

        public static string GetString (string name, IMetadata metadata) {
            var data = metadata.metadata;
            var output = string.Empty;

            for (var i = 0; i < data.Length; ++i) {
                var datum = data[i];

                if (datum.item1 == name) {
                    output = datum.item2;
                    break;
                }
            }

            return output;
        }

        public static Vector3 GetVector3 (string name, IMetadata metadata) {
            var data = metadata.metadata;
            Vector3 vector = new Vector3 ();

            for (var i = 0; i < data.Length; ++i) {
                var datum = data[i];

                if (datum.item1 == name) {
                    try {
                        var split = datum.item2.Split (new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                        vector.x = float.Parse (split[0]);
                        vector.y = float.Parse (split[1]);
                        vector.z = float.Parse (split[2]);
                    } catch {
                        vector = Vector3.zero;
                    }
                }
            }

            return vector;
        }
    }
}