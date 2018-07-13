using UnityEngine;

namespace OneGame.Lua {
    public interface IEquippable {
        Transform Anchor { get; set; }

        void Unequip ();
    }
}