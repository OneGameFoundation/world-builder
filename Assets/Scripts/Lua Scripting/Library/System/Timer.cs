using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua
{
    /// <summary>
    /// A c# bridge for keeping track of time
    /// </summary>
    [MoonSharpUserData]
	public class Timer {

		private static Dictionary<string, float> timeBank = new Dictionary<string, float> ();

		/// <summary>
		/// Clears all timers
		/// </summary>
		public static void Clear () {
			timeBank.Clear ();
		}

		/// <summary>
		/// Gets the elapsed time since the timer was started
		/// </summary>
		/// <param name="name">The name of the timer</param>
		/// <returns>The elapsed time, 0 if the timer does not exist</returns>
		public static float GetElapsedTime (string name) {
			var time = 0f;

			if (timeBank.TryGetValue (name, out time)) {
				return Time.timeSinceLevelLoad - time;
			}

			return time;
		}

		/// <summary>
		/// Is a timer currently running?
		/// </summary>
		/// <param name="name">The name of the timer</param>
		public static bool IsTimerRunning (string name) {
			return timeBank.ContainsKey (name);
		}

		/// <summary>
		/// Starts the timer with a specified name. If the name already exists, 
		/// then the timer will reset.
		/// </summary>
		/// <param name="name">The name of the timer</param>
		public static void StartTimer (string name) {
			if (!timeBank.ContainsKey (name)) {
				timeBank.Add (name, Time.timeSinceLevelLoad);
			} else {
				timeBank[name] = Time.timeSinceLevelLoad;
			}
		}

		/// <summary>
		/// Stops the timer and returns the elapsed time since it was first started.
		/// </summary>
		/// <param name="name">The name of the timer</param>
		/// <returns>An elapsed time since it was started. 0 if no such timer exists</returns>
		public static float StopTimer (string name) {
			var time = 0f;

			if (timeBank.TryGetValue (name, out time)) {
				timeBank.Remove (name);
				return Time.timeSinceLevelLoad - time;
			}

			return time;
		}
	}
}