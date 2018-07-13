using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua {
	/// <summary>
	/// A C# bridge for manipulating animations in Lua
	/// </summary>
	[MoonSharpUserData]
	public class Anim {

		private Animator animator;

		public Anim (Animator animator) {
			this.animator = animator;
		}

		/// <summary>
		/// Gets a boolean value
		/// </summary>
		/// <param name="name">The name of the boolean value</param>
		/// <returns>False is there is no boolean value</returns>
		public bool GetBoolean (string name) {
			return animator != null ? animator.GetBool (name) : false;
		}

		/// <summary>
		/// Gets an animator float value
		/// </summary>
		/// <param name="name">The name of the float number</param>
		/// <returns>0 if no animator exists</returns>
		public float GetFloat (string name) {
			return animator != null ? animator.GetFloat (name) : 0f;
		}

		/// <summary>
		/// Gets an animator integer value
		/// </summary>
		/// <param name="name">The name of the integer</param>
		/// <returns>0 if no animator exists</returns>
		public float GetInt (string name) {
			return animator != null ? animator.GetInteger (name) : 0;
		}

		/// <summary>
		/// Resets an animator trigger
		/// </summary>
		/// <param name="name">The name of the trigger</param>
		public void ResetTrigger (string name) {
			animator?.ResetTrigger (name);
		}

		/// <summary>
		/// Sets an animator boolean value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <param name="value">The value to set</param>
		public void SetBoolean (string name, bool value) {
			animator?.SetBool (name, value);
		}

		/// <summary>
		/// Sets an animator float value
		/// </summary>
		/// <param name="name">The name of the float value</param>
		/// <param name="value">The float value</param>
		public void SetFloat (string name, float value) {
			animator?.SetFloat (name, value);
		}

		/// <summary>
		/// Sets an animator integer value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <param name="value">The amount to set</param>
		public void SetInt (string name, int value) {
			animator?.SetInteger (name, value);
		}

		/// <summary>
		/// Activates an animator trigger
		/// </summary>
		/// <param name="name">The name of the trigger</param>
		public void SetTrigger (string name) {
			animator?.SetTrigger (name);
		}
	}
}