using System;
using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua {
    /// <summary>
    /// A C# representation of the player
    /// </summary>
    public class Player {
        /// <summary>
        /// The current player instance
        /// </summary>
        [MoonSharpHidden]
        public static Player Instance { get; private set; }

        /// <summary>
        /// The player's health component
        /// </summary>
        public Health health { get; private set; }

        /// <summary>
        /// The player's inventory
        /// </summary>
        public Inventory inventory { get; private set; }

        /// <summary>
        /// Is the player alive? (Health > 0)
        /// </summary>
        public bool isAlive { get { return health != null ? health.health > 0 : true; } }

        /// <summary>
        /// Is the player currently moving?
        /// </summary>
        public bool isMoving {
            get {
                if (GetMovingFunction != null) {
                    return GetMovingFunction ();
                }

                return false;
            }
            set { SetMovingFunction?.Invoke (value); }
        }

        /// <summary>
        /// The player's current position
        /// </summary>
        public Vec3 position {
            get { return new Vec3 (transform.position); }
            set { transform.position = value.ToVector3 (); }
        }

        /// <summary>
        /// The player's current rotation
        /// </summary>
        public Vec3 rotation {
            get { return new Vec3 (transform.eulerAngles); }
            set { transform.eulerAngles = value.ToVector3 (); }
        }

        /// <summary>
        /// The player's current scale
        /// </summary>
        public Vec3 scale {
            get { return new Vec3 (transform.localScale); }
            set { transform.localScale = value.ToVector3 (); }
        }

        [MoonSharpHidden]
        public Func<bool> GetMovingFunction;
        [MoonSharpHidden]
        public Action<bool> SetMovingFunction;

        private Transform transform;

        public Player (GameObject obj) {
            health = obj.GetComponent<Health> ();
            transform = obj.transform;
            inventory = new Inventory ();

            Instance = this;
        }
    }
}