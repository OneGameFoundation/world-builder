using OneGame.Lua;
using UnityEngine;

namespace OneGame {
    /// <summary>
    /// A collection of information about an remote asset
    /// </summary>
    [System.Serializable]
    public struct ElementData : IMetadata {
        /// <summary>
        /// The name of the element
        /// </summary>
        public string name;

        /// <summary>
        /// A unique reference identifier of the element
        /// </summary>
        public uint id;

        /// <summary>
        /// The bundle id that this element belongs to
        /// </summary>
        public uint bundleId;

        /// <summary>
        /// The thumbnail texture of the item
        /// </summary>
        public Sprite thumbnail;

        /// <summary>
        /// The description of the asset
        /// </summary>
        public string description;

        /// <summary>
        /// Any search tags that the element contains
        /// </summary>
        public string[] tags;

        /// <summary>
        /// The metadata involved in this elemnt
        /// </summary>
        public Tuple<string, string>[] metadata { get { return meta; } set { meta = value; } }

        private Tuple<string, string>[] meta;

        /// <summary>
        /// A blank instance of an element data
        /// </summary>
        public static ElementData Empty {
            get {
                return new ElementData {
                    name = string.Empty,
                    id = 0,
                    bundleId = 0,
                    description = string.Empty,
                    tags = new string[0],
                    meta = new Tuple<string, string>[0]
                };
            }
        }
    }
}