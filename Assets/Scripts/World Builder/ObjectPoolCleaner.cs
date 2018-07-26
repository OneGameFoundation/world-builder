using UnityEngine;

namespace OneGame {
    /// <summary>
    /// An event reciever that cleans the pool when the mode begins to change
    /// </summary>
    public class ObjectPoolCleaner : MonoBehaviour {
        [SerializeField]
        private ObjectPool pool;
        [SerializeField]
        private GameEventTable eventTable;


        private void OnEnable () {
            eventTable?.Register<WorldMode, WorldMode> ("OnWorldModeTransition", HandleModeTransition);
        }

        private void OnDisable () {
            eventTable?.Unregister<WorldMode, WorldMode> ("OnWorldModeTransition", HandleModeTransition);
        }

        private void HandleModeTransition (WorldMode start, WorldMode end) {
            pool.Clear ();
        }
    }
}