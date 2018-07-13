using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {
	/// <summary>
	/// A common worker that processes coroutines, useful for running coroutines from 
	/// ScriptableObjects
	/// </summary>
	public class CoroutineWorker : MonoBehaviour {
		/// <summary>
		/// The current coroutine worker instance
		/// </summary>
		public static CoroutineWorker Instance {
			get {
				lock (lockObj) {
					if (instance == null) {
						instance = FindObjectOfType<CoroutineWorker> ();

						if (instance == null) {
							var go = new GameObject ("Coroutine Worker (Generated)");
							instance = go.AddComponent<CoroutineWorker> ();
						}
					}
					return instance;
				}
			}
		}

		private static CoroutineWorker instance;
		private static object lockObj = new Object ();

		private Dictionary<int, List<Coroutine>> routines;

		private void Awake () {
			routines = new Dictionary<int, List<Coroutine>> ();
		}

		public Coroutine RunCoroutine (int id, IEnumerator routine) {
			List<Coroutine> runningRoutines;

			if (routines == null) {
				routines = new Dictionary<int, List<Coroutine>> ();
			}

			if (!routines.TryGetValue (id, out runningRoutines)) {
				runningRoutines = new List<Coroutine> ();
				routines.Add (id, runningRoutines);
			}

			var coroutine = StartCoroutine (routine);
			return coroutine;
		}
	}
}