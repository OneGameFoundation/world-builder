using System;
using LitJson;
using OneGame.Lua;

namespace OneGame {
    /// <summary>
    /// A collection of variables describing the state of the game when it was saved
    /// </summary>
    [Serializable, JsonIgnoreMember (new string[] { "Empty" })]
    public struct SaveData {
        /// <summary>
        /// A collection of spawned entities in the game
        /// </summary>
        public EntitySaveData[] entities;

        /// <summary>
        /// The position of the player
        /// </summary>
        public Vector3Double playerPosition;

        /// <summary>
        /// Keys pertaining to the number variables
        /// </summary>
        public string[] numberNames;

        /// <summary>
        /// Number-related save values
        /// </summary>
        public double[] numberValues;

        /// <summary>
        /// String-related save names
        /// </summary>
        public string[] stringNames;

        /// <summary>
        /// String-related save values
        /// </summary>
        public string[] stringValues;

        /// <summary>
        /// Boolean-related save names
        /// </summary>
        public string[] boolNames;

        /// <summary>
        /// Boolean-related save values
        /// </summary>
        public bool[] boolValues;

        /// <summary>
        /// An empty representation of a save
        /// </summary>
        public static SaveData Empty {
            get {
                return new SaveData {
                    entities = new EntitySaveData[0],
                        numberNames = new string[0],
                        numberValues = new double[0],
                        stringNames = new string[0],
                        stringValues = new string[0],
                        boolNames = new string[0],
                        boolValues = new bool[0]
                };
            }
        }

        /// <summary>
        /// Creates a deep copy of the current save file
        /// </summary>
        public SaveData CreateCopy () {
            var save = new SaveData {
                entities = new EntitySaveData[this.entities.Length],
                playerPosition = this.playerPosition,
                numberNames = new string[this.numberNames.Length],
                numberValues = new double[this.numberValues.Length],
                stringNames = new string[this.stringNames.Length],
                stringValues = new string[this.stringValues.Length],
                boolNames = new string[this.boolNames.Length],
                boolValues = new bool[this.boolValues.Length]
            };

            entities.CopyTo (save.entities, 0);
            numberNames.CopyTo (save.numberNames, 0);
            numberValues.CopyTo (save.numberValues, 0);
            stringNames.CopyTo (save.stringNames, 0);
            stringValues.CopyTo (save.stringValues, 0);
            boolNames.CopyTo (save.boolNames, 0);
            boolValues.CopyTo (save.boolValues, 0);
            return save;
        }
    }

    /// <summary>
    /// The current state of the entity when it was saved
    /// </summary>
    [Serializable]
    public class EntitySaveData {
        /// <summary>
        /// The asset id of the entity
        /// </summary>
        public uint id;

        /// <summary>
        /// The entity's unique id
        /// </summary>
        public string uniqueId;

        /// <summary>
        /// The type of entity
        /// </summary>
        public int type;

        /// <summary>
        /// The entity's scale
        /// </summary>
        public Vector3Double scale;

        /// <summary>
        /// The entity's position
        /// </summary>
        public Vector3Double position;

        /// <summary>
        /// The entity's rotation
        /// </summary>
        public QuaternionDouble rotation;

        /// <summary>
        /// The custom script that the entity runs
        /// </summary>
        public LuaScript script;
    }
}