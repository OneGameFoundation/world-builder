using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua {
    /// <summary>
    /// A enum describing the type of equipment in relation to hand usage
    /// </summary>
    public enum EquipType : int {
        Armor = 0,
        Melee1H = 1,
        Melee2H = 2,
        Range1H = 3,
        Range2H = 4
    }

    /// <summary>
    /// Enum describing a specific body part on a humanoid
    /// </summary>
    public enum BodyPart : int {
        Chest = 0,
        LeftHand = 1,
        RightHand = 2,
        LeftFoot = 3,
        RightFoot = 4,
        Head = 5
    }

    /// <summary>
    /// A c# bridge for adding and removing items that the player currently has
    /// </summary>
    [MoonSharpUserData]
    public class Inventory {

        /// <summary>
        /// The current collection of stored entities in the inventory
        /// </summary>
        public Entity[] StoredEntities { get { return entities.ToArray (); } }

        [MoonSharpHidden]
        public event Action<Entity, BodyPart, EquipType> OnItemEquip;
        [MoonSharpHidden]
        public event Action<Entity> OnUnequip;
        [MoonSharpHidden]
        public event Action<Entity, int> OnItemAdd;
        [MoonSharpHidden]
        public event Action<Entity, int> OnItemRemove;
        [MoonSharpHidden]
        public event Action<Entity> OnItemUse;

        private List<Entity> entities;
        private List<int> quantities;
        private Entity[] activeEquipment;


        public Inventory () {
            entities = new List<Entity> ();
            quantities = new List<int> ();
            activeEquipment = new Entity[6];
        }

        /// <summary>
        /// Adds an entity to the inventory
        /// </summary>
        /// <param name="obj">The entity to add</param>
        /// <param name="amount">The amount to add</param>
        public void Add (Entity obj, int amount) {
            var index = entities.IndexOf (obj);
            if (index > -1) {
                quantities[index] += amount;
            } else {
                entities.Add (obj);
                quantities.Add (amount);
            }

            // storedEntities[obj] += amount;

            OnItemAdd?.Invoke (obj, amount);
        }

        /// <summary>
        /// Adds a quantity of an item to the inventory
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="amount">The amount to add</param>
        public void Add (string name, int amount) {
            var index = entities.FindIndex (e => e.name == name);
            if (index > -1) {
                quantities[index] += amount;
                OnItemAdd?.Invoke (entities[index], amount);
            }
        }

        /// <summary>
        /// Gets the total quantity of a particular entity
        /// </summary>
        /// <param name="obj">The entity to query</param>
        public int Count (Entity obj) {
            var index = entities.IndexOf (obj);
            return index > -1 ? quantities[index] : 0;
        }

        /// <summary>
        /// Counts the quantity of a queried item name
        /// </summary>
        /// <param name="objName">The name of the object to search</param>
        public int Count (string objName) {
            var index = entities.FindIndex (e => e.name == objName);
            return index > -1 ? quantities[index] : 0;
        }

        /// <summary>
        /// Eequips the entity to the player
        /// </summary>
        /// <param name="entity">The entity to equip</param>
        /// <param name="bodyPart">The bodypart to equip to</param>
        /// <param name="type">The equipment type</param>
        public void Equip (Entity entity, BodyPart bodyPart, EquipType type) {
            // If there is an item in the current slot, unequip it
            var currentEquip = activeEquipment[(int)bodyPart];
            if (currentEquip != null) {
                UnequipFromBodySlot (bodyPart);

                if (currentEquip == entity) {
                    return;
                }
            }

            activeEquipment[(int)bodyPart] = entity;
            OnItemEquip?.Invoke (entity, bodyPart, type);
        }

        /// <summary>
        /// Gets the equipment attached to the current body part.
        /// </summary>
        /// <param name="bodyPart">The bodypart to query</param>
        /// <returns>Null if there is no object equipped</returns>
        public Entity GetEquipment (BodyPart bodyPart) {
            return activeEquipment[(int)bodyPart];
        }

        /// <summary>
        /// Removes an entity from the inventory
        /// </summary>
        /// <param name="obj">The entity to remove</param>
        /// <param name="amount">The amount to remove</param>
        public void Remove (Entity obj, int amount) {
            var index = entities.IndexOf (obj);
            if (index > -1) {
                quantities[index] -= amount;

                if (quantities[index] < 1) {
                    quantities.RemoveAt (index);
                    entities.RemoveAt (index);
                }

                OnItemRemove?.Invoke (obj, amount);
            }
        }

        /// <summary>
        /// Removes an item from the inventory
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="amount">The amount to remove</param>
        public void Remove (string name, int amount) {
            var index = entities.FindIndex (e => e.name == name);
            if (index > -1) {
                var entity = entities[index];
                quantities[index] -= amount;

                if (quantities[index] < 1) {
                    quantities.RemoveAt (index);
                    entities.RemoveAt (index);
                }

                OnItemRemove?.Invoke (entity, amount);
            }
        }

        /// <summary>
        /// Unequips an item from the inventory
        /// </summary>
        /// <param name="obj">The object to unequip</param>
        public void Unequip (Entity entity) {
            for (var i = 0; i < activeEquipment.Length; ++i) {
                if (activeEquipment[i] == entity) {
                    OnUnequip?.Invoke (entity);
                    activeEquipment[i] = null;
                    break;
                }
            }
        }

        public void UnequipFromBodySlot (BodyPart bodyPart) {
            var equip = activeEquipment[(int)bodyPart];

            if (equip != null) {
                OnUnequip?.Invoke (equip);
                activeEquipment[(int)bodyPart] = null;
            }
        }

        /// <summary>
        /// Unequips an item from the inventory by name
        /// </summary>
        /// <param name="name">The name of the equipment</param>
        public void Unequip (string name) {
            for (var i = 0; i < activeEquipment.Length; ++i) {
                var equip = activeEquipment[i];

                if (equip != null && (equip.name == name || equip.id == name)) {
                    OnUnequip?.Invoke (equip);
                    activeEquipment[i] = null;
                    break;
                }
            }
        }

        /// <summary>
        /// Attempts to use an item on the inventory
        /// </summary>
        /// <param name="index">The index to use</param>
        public void UseItem (int index) {
            if (index >= entities.Count || index < 0) {
                throw new System.ArgumentOutOfRangeException ("Index is out of range!");
            }

            entities[index].Call ("onItemUse");
            OnItemUse?.Invoke (entities[index]);
        }

        /// <summary>
        /// Clears the inventory
        /// </summary>
        [MoonSharpHidden]
        public void Clear () {
            for (var i = 0; i < activeEquipment.Length; ++i) {
                var equip = activeEquipment[i];

                if (equip != null) {
                    OnUnequip (equip);
                }

                activeEquipment[i] = null;
            }

            entities.Clear ();
            quantities.Clear ();
        }

    }
}