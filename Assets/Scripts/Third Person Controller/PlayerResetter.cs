using OneGame.Lua;
using UnityEngine;

namespace OneGame
{
    public class PlayerResetter : MonoBehaviour {

		[SerializeField]
		private GameEventTable eventTable;

		private void OnEnable () {
			if (eventTable != null) {
				eventTable.Register<WorldMode> ("OnWorldModeChange", HandleWorldModeChange);
			}
		}

		private void OnDisable () {
			if (eventTable != null) {
				eventTable.Unregister<WorldMode> ("OnWorldModeChange", HandleWorldModeChange);
			}
		}

		private void HandleWorldModeChange (WorldMode mode) {
			var animator = GetComponent<Animator> ();
			animator?.SetTrigger ("Reset");

			var player = Player.Instance;
			if (player != null) {
				player.inventory?.Clear ();
				player.health.health = player.health.maxHealth;
			}
		}
	}
}