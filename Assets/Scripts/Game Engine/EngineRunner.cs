using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {
	public class EngineRunner : MonoBehaviour {
		[SerializeField]
		private MicroGameEngine engine;
		[SerializeField]
		private ScriptManager scriptManager;
		[SerializeField]
		private GameEventTable eventTable;

		private void Start () {
			scriptManager.AddRunner (engine);
		}

		private void OnEnable () {
			if (eventTable != null) {
				eventTable.Register<WorldMode> ("OnWorldModeChange", HandleGameModeChange);
			}
		}

		private void OnDisable () {
			if (eventTable != null) {
				eventTable.Unregister<WorldMode> ("OnWorldModeChange", HandleGameModeChange);
			}
		}

		private void OnDestroy () {
			scriptManager.RemoveRunner (engine);
		}

		private void HandleGameModeChange (WorldMode mode) {
			if (mode == WorldMode.Play) {
				engine.InitializeGame ();
			}
		}
	}
}