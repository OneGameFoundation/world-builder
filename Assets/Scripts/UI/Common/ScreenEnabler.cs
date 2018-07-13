using UnityEngine;

namespace OneGame.UI {
	public class ScreenEnabler : MonoBehaviour {
		[SerializeField]
		private WorldMode mode;
		[SerializeField]
		private GameEventTable table;

		private CanvasGroup group;

		private void Awake () {
			group = GetComponent<CanvasGroup> ();
		}

		private void OnEnable () {
			table.Register<WorldMode> ("OnWorldModeChange", HandleWorldModeChangeEvent);
		}

		private void OnDisable () {
			table.Unregister<WorldMode> ("OnWorldModeChange", HandleWorldModeChangeEvent);
		}

		private void HandleWorldModeChangeEvent (WorldMode mode) {
			var shouldDisplay = this.mode == mode;

			group.alpha = shouldDisplay ? 1f : 0f;
			group.interactable = shouldDisplay;
			group.blocksRaycasts = shouldDisplay;
		}
	}
}