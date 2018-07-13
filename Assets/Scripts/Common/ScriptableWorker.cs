using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {
	/// <summary>
	/// A worker script that can run update loops, as well as coroutines
	/// </summary>
	public class ScriptableWorker : MonoBehaviour {
		/// <summary>
		/// An event that is triggered every update loop
		/// </summary>
		public static event Action OnUpdateEvent;

		private static ScriptableWorker Instance { get; set; }

		private void Awake () {
			Instance = this;
		}

		private void Update () {
			OnUpdateEvent?.Invoke ();
		}

		/// <summary>
		/// Creates a simple worker in the hierarchy
		/// </summary>
		/// <param name="name">The name of the worker</param>
		[Obsolete ("Register onto the event instead")]
		public static ScriptableWorker CreateWorker (string name) {
			var go = new GameObject (name);
			var worker = go.AddComponent<ScriptableWorker> ();
			DontDestroyOnLoad (go);

			return worker;
		}

		/// <summary>
		/// Registers an update call to the update event
		/// </summary>
		/// <param name="updateHandler">The update handler</param>
		public static void Register (Action updateHandler) {
			OnUpdateEvent += updateHandler;
		}

		public static void RunCoroutine (IEnumerator routine) {
			Instance?.StartCoroutine (routine);
		}

		/// <summary>
		/// Unregisters an update call from the update event
		/// </summary>
		/// <param name="updateHandler">The update handler</param>
		public static void Unregister (Action updateHandler) {
			OnUpdateEvent -= updateHandler;
		}
	}
}