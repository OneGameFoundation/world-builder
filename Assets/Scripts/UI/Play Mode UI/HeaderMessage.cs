using System.Collections;
using System.Collections.Generic;
using OneGame.Lua;
using TMPro;
using UnityEngine;

namespace OneGame.UI {
    public class HeaderMessage : MonoBehaviour {

        [SerializeField]
        private float fadeInDuration = 0.2f;
        [SerializeField]
        private float fadeOutDuration = 0.2f;
        [SerializeField]
        private float messageHoldDuration = 5f;

        private TMP_Text text;
        private CanvasGroup group;

        private void Awake () {
            text = GetComponent<TMP_Text> ();
            group = GetComponent<CanvasGroup> ();
        }

        private void OnEnable () {
            Game.OnAlert += HandleAlertEvent;
        }

        private void OnDisable () {
            Game.OnAlert -= HandleAlertEvent;
        }

        private void HandleAlertEvent (string message, float duration) {
            text.text = message;

            TweenMessage (gameObject, duration);
        }

        /// <summary>
        /// Tweens a message to full opacity and back again after a delay
        /// </summary>
        /// <param name="gameObject">The object to tween</param>
        /// <param name="delay">The delay to tween back</param>
        private void TweenMessage (GameObject gameObject, float delay) {
            LeanTween.cancel (gameObject);

            LeanTween.value (gameObject, 0f, 1f, fadeInDuration)
                .setOnUpdate (f => group.alpha = f)
                .setOnComplete (() => {
                    LeanTween.value (gameObject, 1f, 0f, fadeOutDuration)
                        .setOnUpdate (f => group.alpha = f)
                        .setDelay (delay > Mathf.Epsilon ? delay : messageHoldDuration);
                });
        }
    }
}