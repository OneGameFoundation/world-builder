using UnityEngine;

namespace OneGame {
	/// <summary>
	/// A utility script that autoloads scenes at start
	/// </summary>
	public class SceneBootstrap : MonoBehaviour {
		[SerializeField]
		private bool autoloadScenes;
		[SerializeField]
		private string[] scenesToLoad;
		[SerializeField]
		private SceneLoader sceneLoader;

		private void Start () {
			if (autoloadScenes) {
				sceneLoader.BootstrapScenes (scenesToLoad);
			}
		}

		public void Load () {
			sceneLoader.LoadScenes (scenesToLoad);
		}
	}
}