using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua {
    /// <summary>
    /// A native component that fires an event when an entity is near
    /// </summary>
    [MoonSharpUserData]
    public class Trigger : NativeComponent {

        /// <summary>
        /// Is this trigger active?
        /// </summary>
        public bool active {
            get { return enabled; }
            set {
                enabled = value;
                triggerCollider.enabled = value;
            }
        }

        /// <summary>
        /// The radius of the trigger
        /// </summary>
        public float size {
            get { return triggerCollider.radius; }
            set { triggerCollider.radius = value; }
        }

        private SphereCollider triggerCollider;

        protected override void Awake () {
            base.Awake ();

            triggerCollider = gameObject.AddComponent<SphereCollider> ();
            triggerCollider.isTrigger = true;
        }

        private void OnTriggerEnter (Collider other) {
            var targetEntity = other.GetComponentInParent<IEntityContainer> ();
            if (targetEntity != null) {
                entity.InvokeLuaMethod ("onTriggerEnter", UserData.Create (targetEntity.Entity));
            }
        }

        private void OnTriggerExit (Collider other) {
            var targetEntity = other.GetComponentInParent<IEntityContainer> ();
            if (targetEntity != null) {
                entity.InvokeLuaMethod ("onTriggerExit", UserData.Create (targetEntity.Entity));
            }
        }
    }
}