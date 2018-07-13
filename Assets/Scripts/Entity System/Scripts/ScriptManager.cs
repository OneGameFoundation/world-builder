using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using OneGame.Lua;
using UnityEngine;

namespace OneGame {
	[CreateAssetMenu (menuName = "Scriptable Objects/Managers/Script Manager")]
	public class ScriptManager : ScriptableObject {

		public bool AllowScriptRunning { get; set; }
		public IScriptRunner[] Runners { get { return runners.ToArray (); } }

		private List<IScriptRunner> runners;

		private void OnEnable () {
			runners = new List<IScriptRunner> ();

			Game.OnGameStateChange += HandleGameStateChangeEvent;
			Game.OnEventCallDelayed += HandleEventPropagationDelayed;
			Game.OnEventCall += HandleEventPropagation;
			Game.OnEventCallData += HandleEventPropagation;

			ScriptableWorker.Register (RunScripts);
		}

		private void OnDisable () {
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying) {
				return;
			}
#endif

			ScriptableWorker.Unregister (RunScripts);

			Game.OnGameStateChange -= HandleGameStateChangeEvent;
			Game.OnEventCall -= HandleEventPropagation;
			Game.OnEventCallData -= HandleEventPropagation;
			Game.OnEventCallDelayed -= HandleEventPropagationDelayed;
		}

		/// <summary>
		/// Adds a runner to the manager
		/// </summary>
		public void AddRunner (IScriptRunner runner) {
			if (runners.IndexOf (runner) < 0) {
				runners.Add (runner);
			}
		}

		/// <summary>
		/// Is the runner active?
		/// </summary>
		/// <param name="runner">The runner to query</param>
		public bool IsRunnerActive (IScriptRunner runner) {
			return runners.FindIndex (r => r == runner) > -1;
		}

		/// <summary>
		/// Removes a runner from a manager
		/// </summary>
		public void RemoveRunner (IScriptRunner runner) {
			runners.Remove (runner);
		}

		private void HandleEventPropagation (string eventName) {
			for (var i = 0; i < runners.Count; ++i) {
				runners[i].InvokeLuaMethod (eventName);
			}
		}

		private void HandleEventPropagation (string eventName, DynValue value) {
			for (var i = 0; i < runners.Count; ++i) {
				runners[i].InvokeLuaMethod (eventName, value);
			}
		}

		private void HandleEventPropagationDelayed (string eventName, float delay) {
			ScriptableWorker.RunCoroutine (PropagateEventDelayed (eventName, delay));
		}

		private void HandleGameStateChangeEvent (string state) {
			var value = DynValue.NewString (state);

			for (var i = 0; i < runners.Count; ++i) {
				runners[i].InvokeLuaMethod ("onGameStateChange", value);
			}
		}

		private IEnumerator PropagateEventDelayed (string eventName, float delay) {
			yield return new WaitForSeconds (delay);

			for (var i = 0; i < runners.Count; ++i) {
				runners[i].InvokeLuaMethod (eventName);
			}
		}

		private void RunScripts () {
			if (AllowScriptRunning) {
				var delta = DynValue.NewNumber (Time.deltaTime);

				for (var i = 0; i < runners.Count; ++i) {
					runners[i].InvokeLuaMethod ("update", delta);
				}
			}
		}
	}
}