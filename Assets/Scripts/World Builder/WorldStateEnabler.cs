using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {
	public class WorldStateEnabler : MonoBehaviour {
		[SerializeField]
		private WorldMode primaryMode;
		[SerializeField]
		private GameObject[] activeObjects;
		[SerializeField]
		private GameEventTable eventTable;

		private void OnEnable () {
			if (eventTable != null) {
				eventTable.Register<WorldMode> ("OnWorldModeChange", HandleWorldChangeEvent);
			}
		}

		private void OnDisable () {
			if (eventTable != null) {
				eventTable.Unregister<WorldMode> ("OnWorldModeChange", HandleWorldChangeEvent);
			}
		}

		private void HandleWorldChangeEvent (WorldMode mode) {
			bool active = mode == primaryMode;

			foreach (var obj in activeObjects) {
				obj.SetActive (active);
			}
		}
	}
}