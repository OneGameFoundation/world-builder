using MoonSharp.Interpreter;
using OneGame.Lua;
using OneGame.TPC;
using UnityEngine;
using System;

namespace OneGame {
    using Type = Lua.Type;

    public class InteractivePlayer : MonoBehaviour, IEntityContainer {
        public Entity Entity { get; private set; }
        public GameObject GameObject { get; private set; }
        public Transform Transform { get; private set; }

        public LuaScript Script { get; set; }
        public NativeComponent[] ActiveComponents { get; private set; }
        public string AssetId { get; private set; }
        public string UniqueId { get; private set; }


        [SerializeField]
        private BodyMap[] bodyMappings;

        private Animator animator;
        private Player player;
        private PlayerMotor motor;

        /// <summary>
        /// A one-to-one relationship between an anchor type and a transform
        /// </summary>
        [Serializable]
        public struct BodyMap {
            public BodyPart type;
            public Transform transform;
        }

        private void Awake () {
            animator = GetComponent<Animator> ();
            motor = GetComponent<PlayerMotor> ();
            Entity = new Entity (gameObject);
            Entity.SetId ("000000");
            player = new Player (gameObject);
            AssetId = "00000000";
            UniqueId = "00000000";

            GameObject = gameObject;
            Transform = transform;

            var script = new Script ();
            LuaUtilities.ApplyDefaultValues (script);
            Script = new LuaScript {
                code = string.Empty,
                script = script
            };

            ActiveComponents = GetComponents<NativeComponent> ();
        }

        private void OnEnable () {
            if (player != null) {
                player.health.OnDeath += HandleDeath;
                player.inventory.OnItemEquip += HandleItemEquipEvent;
                player.inventory.OnUnequip += HandleUnequipEvent;
                player.GetMovingFunction += HandleGetMovingFunction;
                player.SetMovingFunction += HandleSetMovingFunction;
            }
        }

        private void OnDisable () {
            if (player != null) {
                player.health.OnDeath -= HandleDeath;
                player.inventory.OnItemEquip -= HandleItemEquipEvent;
                player.inventory.OnUnequip -= HandleUnequipEvent;
                player.GetMovingFunction -= HandleGetMovingFunction;
                player.SetMovingFunction -= HandleSetMovingFunction;
            }
        }

        public void AddComponent (Type type, string name) { }
        public void InvokeLuaMethod (string methodName) { }
        public void InvokeLuaMethod (string methodName, DynValue value) { }

        private Transform FindChild (Transform parent, string targetName) {
            if (parent.gameObject.name == targetName) {
                return parent;
            }

            for (var i = 0; i < parent.childCount; ++i) {
                var child = parent.GetChild (i);

                var candidate = FindChild (child, targetName);
                if (candidate != null) {
                    return candidate;
                }
            }

            return default (Transform);
        }

        private void HandleDeath () {
            Game.FireEvent ("onPlayerDeath");
        }

        /// <summary>
        /// Delegate called when the player equips an item from the inventory
        /// </summary>
        /// <param name="entity">The entity to equip</param>
        /// <param name="bodyPart">Where the equip will be on the body</param>
        /// <param name="type">The equipment type</param>
        private void HandleItemEquipEvent (Entity entity, BodyPart bodyPart, EquipType type) {
            Transform anchor;

            if (LocateBodyPart (bodyPart, bodyMappings, out anchor)) {
                var transform = entity.transform;
                transform.SetParent (anchor);

                var equipment = transform.GetComponent<IEquippable> ();
                if (equipment != null) {
                    equipment.Anchor = anchor;
                }

                animator.SetInteger ("Equip Type", (int)type);

                var container = transform.GetComponent<IEntityContainer> ();
                container?.InvokeLuaMethod ("onEquip", UserData.Create (entity));
            }
        }

        private bool HandleGetMovingFunction () {
            return motor.CanMove;
        }

        private void HandleSetMovingFunction (bool value) {
            motor.CanMove = false;
        }

        private void HandleUnequipEvent (Entity entity) {
            if (entity.transform == null) { return; }

            entity.transform.parent = null;

            var equippables = entity.transform.GetComponents<IEquippable> ();
            foreach (var equip in equippables) {
                equip.Unequip ();
            }

            animator.SetInteger ("Equip Type", 0);
            animator.SetTrigger ("Equip");

            var container = entity.transform.GetComponent<IEntityContainer> ();
            container?.InvokeLuaMethod ("onUnequip");
        }

        /// <summary>
        /// Finds a body part with a matching anchor
        /// </summary>
        /// <returns>True if the body part exists</returns>
        private bool LocateBodyPart (BodyPart anchor, BodyMap[] mappings, out Transform transform) {
            for (var i = 0; i < mappings.Length; ++i) {
                var map = mappings[i];

                if (map.type == anchor) {
                    transform = map.transform;
                    return true;
                }
            }

            transform = default (Transform);
            return false;
        }
    }
}