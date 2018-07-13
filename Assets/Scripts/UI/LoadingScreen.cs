using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame.UI {
	public class LoadingScreen : MonoBehaviour {
		[SerializeField]
		private GameObject cameraObject;
		[SerializeField]
		private SceneLoader sceneLoader;

		[SerializeField, Header ("Effects")]
		private float fadeInDuration = 0.1f;
		[SerializeField]
		private float fadeOutDuration = 0.1f;

		private CanvasGroup group;

		private void Awake () {
			group = GetComponent<CanvasGroup> ();
		}

		private void OnEnable () {
			if (sceneLoader != null) {
				sceneLoader.OnSceneLoadStart += HandleStartLoadEvent;
				sceneLoader.OnSceneLoadComplete += HandleFinishLoadEvent;
			}
		}

		private void OnDisable () {
			if (sceneLoader != null) {
				sceneLoader.OnSceneLoadStart -= HandleStartLoadEvent;
				sceneLoader.OnSceneLoadComplete -= HandleFinishLoadEvent;
			}
		}

		private void HandleFinishLoadEvent () {
			LeanTween.cancel (gameObject);
			LeanTween.alphaCanvas (group, 0f, fadeOutDuration)
				.setEaseOutBack ()
				.setOnComplete (() => {
					cameraObject?.SetActive (false);
				});
		}

		private void HandleStartLoadEvent (string[] scenes) {
			LeanTween.cancel (gameObject);

			cameraObject?.SetActive (true);
			LeanTween.alphaCanvas (group, 1f, fadeInDuration).setEaseOutBack ();
		}

	}
}