using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneGame.UI {
	/// <summary>
	/// A splash screen that stalls the game until all the necesary assets have been loaded
	/// </summary>
	public class SplashScreen : MonoBehaviour {
		[SerializeField]
		private ScriptCatalog scriptDatabase;
		[SerializeField]
		private ItemCatalog itemDatabase;

		[SerializeField, Header ("Scene Loading")]
		private SceneLoader sceneLoader;
		[SerializeField]
		private string[] scenesToLoad;

		[SerializeField, Header ("Loading Effects")]
		private Transform loadingIconTransform;
		[SerializeField]
		private float rotateSpeed = 60f;
		[SerializeField]
		private GameEventTable eventTable;

		private IEnumerator Start () {
			while (!scriptDatabase.IsReady || itemDatabase.Status == ItemCatalog.LoadingStatus.NotLoaded) {
				yield return null;
			}

			sceneLoader.LoadScenes (scenesToLoad);
		}

		private void LateUpdate () {
			var rotation = loadingIconTransform.localEulerAngles;
			rotation.z += Time.deltaTime * rotateSpeed;
			loadingIconTransform.localEulerAngles = rotation;
		}

	}
}