using System;
using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua {
    /// <summary>
    /// A C# representation of an entity object
    /// </summary>
    [MoonSharpUserData]
    public class Entity {
        /// <summary>
        /// The entity's reference asset id
        /// </summary>
        public string assetId { get; private set; }

        /// <summary>
        /// Is the entity active in the scene?
        /// </summary>
        public bool active { get { return gameObject.activeInHierarchy; } set { gameObject.SetActive (value); } }

        /// <summary>
        /// The entity's forward vector
        /// </summary>
        public Vec3 forward { get { return new Vec3 (transform.forward); } }

        /// <summary>
        /// The unique id of the entity
        /// </summary>
        public string id { get; private set; }

        /// <summary>
        /// The entity's position in local space
        /// </summary>
        public Vec3 localPosition {
            get { return new Vec3 (transform.localPosition); }
            set { transform.localPosition = value.ToVector3 (); }
        }

        /// <summary>
        /// The entity's local rotation
        /// </summary>
        public Vec3 localRotation {
            get { return new Vec3 (transform.localEulerAngles); }
            set { transform.localEulerAngles = value.ToVector3 (); }
        }

        /// <summary>
        /// The name of the entity
        /// </summary>
        public string name { get { return gameObject.name; } }

        /// <summary>
        /// The entity's current position
        /// </summary>
        public Vec3 position {
            get { return new Vec3 (transform.position); }
            set { transform.position = value.ToVector3 (); }
        }

        /// <summary>
        /// The entity's rotation
        /// </summary>
        public Vec3 rotation {
            get { return new Vec3 (transform.localEulerAngles); }
            set { transform.localEulerAngles = value.ToVector3 (); }
        }

        /// <summary>
        /// The entity's current scale
        /// </summary>
        public Vec3 scale {
            get { return new Vec3 (transform.localScale); }
            set { transform.localScale = value.ToVector3 (); }
        }

        [MoonSharpHidden]
        public Transform transform { get; private set; }

        [MoonSharpHidden]
        public GameObject gameObject { get; private set; }

        [MoonSharpHidden]
        public event Action<string> OnEntityCall;
        [MoonSharpHidden]
        public event Action<string> OnScriptAdd;
        [MoonSharpHidden]
        public event Action<Type, string> OnComponentAdd;
        [MoonSharpHidden]
        public event Action<Type, string, Item> OnComponentAddWithData;

        [MoonSharpHidden]
        public Entity (GameObject source) {
            gameObject = source;
            transform = source.transform;
        }

        /// <summary>
        /// Adds a component to the entity
        /// </summary>
        /// <param name="componentType">The type of component</param>
        /// <param name="name">The name to call the component</param>
        public void AddComponent (Type componentType, string name) {
            OnComponentAdd?.Invoke (componentType, name);
        }

        /// <summary>
        /// Adds a component to the entity, and applies metadata to the component.
        /// Useful for creating weapons
        /// </summary>
        /// <param name="componentType">The type of component</param>
        /// <param name="name">The name to call the component</param>
        /// <param name="itemData">The metadata to pass</param>
        public void AddComponent (Type componentType, string name, Item itemData) {
            OnComponentAddWithData?.Invoke (componentType, name, itemData);
        }

        /// <summary>
        /// Adds a script to run to the entity
        /// </summary>
        public void AddScript (string script) {
            if (OnScriptAdd != null) {
                OnScriptAdd (script);
            }
        }

        /// <summary>
        /// Calls a function on the entity
        /// </summary>
        /// <param name="function">The function to call</param>
        public void Call (string function) {
            OnEntityCall?.Invoke (function);
        }

        /// <summary>
        /// Sets the unique id of the entity
        /// </summary>
        [MoonSharpHidden]
        public void SetId (string id) {
            this.id = id;
        }

        /// <summary>
        /// Sets the asset id of the entity
        /// </summary>
        /// <param name="id">The id of the asset</param>
        [MoonSharpHidden]
        public void SetAssetId (string id) {
            this.assetId = id;
        }
    }
}