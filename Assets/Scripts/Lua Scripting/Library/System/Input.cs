using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua {
	/// <summary>
	/// A c# bridge for detecting input
	/// </summary>
	[MoonSharpUserData]
	public static class Input {
		/// <summary>
		/// Checks if the key is pressed or held down
		/// </summary>
		/// <param name="key">The key to check</param>
		public static bool GetKey (string key) {
			return UnityEngine.Input.GetKey (key);
		}

		/// <summary>
		/// Checks if the key is held down
		/// </summary>
		/// <param name="key">The key to check</param>
		public static bool GetKeyDown (string key) {
			return UnityEngine.Input.GetKeyDown (key);
		}

		/// <summary>
		/// Checks if the key was released
		/// </summary>
		/// <param name="key">The key to check</param>
		public static bool GetKeyUp (string key) {
			return UnityEngine.Input.GetKeyUp (key);
		}

		/// <summary>
		/// Checks if the mouse button was pressed or held down
		/// </summary>
		/// <param name="button">The button to check</param>
		public static bool GetMouse (int button) {
			return UnityEngine.Input.GetMouseButton (button);
		}

		/// <summary>
		/// Checks if the mouse button was held down
		/// </summary>
		/// <param name="button">The button to check</param>
		public static bool GetMouseDown (int button) {
			return UnityEngine.Input.GetMouseButtonDown (button);
		}

		/// <summary>
		/// Checks if the mouse button was recently released
		/// </summary>
		/// <param name="button">The button to check</param>
		public static bool GetMouseUp (int button) {
			return UnityEngine.Input.GetMouseButtonUp (button);
		}
	}
}