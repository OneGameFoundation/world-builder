using System.Collections;
using System.Collections.Generic;
using OneGame.Lua;
using TMPro;
using UnityEngine;

namespace OneGame.UI {
	public class MessageLogger : MonoBehaviour {

		[SerializeField]
		private TMP_Text messageText;

		[SerializeField, Header ("Effects")]
		private float fadeInDuration = 0.2f;
		[SerializeField]
		private float fadeOutDuration = 0.4f;
		[SerializeField]
		private float messageHoldDuration = 5f;

		private CanvasGroup group;

		private void Awake () {
			group = GetComponent<CanvasGroup> ();
		}

		private void OnEnable () {
			//Game.OnAlert += HandleMessageEvent;
			Game.OnMessage += HandleMessageEvent;
		}

		private void OnDisable () {
			//Game.OnAlert -= HandleMessageEvent;
			Game.OnMessage -= HandleMessageEvent;
		}

		private void HandleMessageEvent (string message) {
			messageText.text += message + "\n";

			LeanTween.cancel (gameObject);
			LeanTween.value (gameObject, group.alpha, 1f, fadeInDuration)
				.setOnUpdate (f => group.alpha = f)
				.setOnComplete (() => {
					LeanTween.value (gameObject, group.alpha, 0f, fadeOutDuration)
						.setOnUpdate (f => group.alpha = f).setDelay (messageHoldDuration);
				});
		}
	}
}