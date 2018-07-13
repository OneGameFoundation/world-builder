using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame.Lua {
	/// <summary>
	/// An enum describing an editable weapon property
	/// </summary>
	public enum WeaponProperty : int {
		/// <summary>
		/// How often should the weapon perform an attack (per second)
		/// </summary>
		AttackRate = 0,

		/// <summary>
		/// How much damage can the weapon do
		/// </summary>
		Damage = 1,

		/// <summary>
		/// [Melee Only] How wide should the attack be
		/// </summary>
		HorizontalArc = 2,

		/// <summary>
		/// The current magazine size of the weapon
		/// </summary>
		MagazineSize = 3,

		/// <summary>
		/// [Range Only] How many enemies can the bullet pass through before 
		/// it does zero damage
		/// </summary>
		Penetration = 4,

		/// <summary>
		/// The range of the weapon
		/// </summary>
		Range = 5,

		/// <summary>
		/// [Range Only] How long should the wepaon reload
		/// </summary>
		ReloadTime = 6,

		/// <summary>
		/// [Range Only] How much deviation should the bullets fly
		/// </summary>
		Spread = 7,

		/// <summary>
		/// [Range Only] How many bullets should the weapon fire on attack
		/// </summary>
		PelletCount = 8,
	}
}