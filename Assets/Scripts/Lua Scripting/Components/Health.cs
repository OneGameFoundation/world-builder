using System;
using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua {
    /// <summary>
    /// A native component for health
    /// </summary>
    [MoonSharpUserData]
    public class Health : NativeComponent {
        [EditableField]
        public float health { get; set; }
        public bool isAlive { get { return health > 0f; } }

        [EditableField]
        public float maxHealth { get; set; }

        [SerializeField]
        private float startingHealth = 100;

        public Action OnDeath;
        public Action<float> OnDamage;

        protected override void Awake () {
            base.Awake ();

            maxHealth = startingHealth;
            health = startingHealth;
        }

        public void Damage (float amount) {
            if (health > 0) {
                health -= amount;

                if (health <= 0) {
                    try {
                        entity?.InvokeLuaMethod ("onDeath");
                        Game.FireEvent ("onEntityDeath", UserData.Create (entity.Entity));
                    } catch { }
                    OnDeath?.Invoke ();
                } else {
                    OnDamage?.Invoke (amount);
                    entity?.InvokeLuaMethod ("onDamage", DynValue.NewNumber (amount));
                }
            }
        }

        public void Heal (float amount) {
            health = Mathf.Clamp (health + amount, 0, maxHealth);
        }
    }
}