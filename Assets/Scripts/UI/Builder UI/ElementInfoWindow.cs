using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OneGame.UI {
    /// <summary>
    /// A window that displays information on the currently selected element
    /// </summary>
    [RequireComponent (typeof (CanvasGroup))]
    public class ElementInfoWindow : MonoBehaviour {

        [SerializeField, Header ("Element Description")]
        private Text nameText;
        [SerializeField]
        private Text idText;
        [SerializeField]
        private Text descriptionText;
        [SerializeField]
        private Image thumbnailImage;
        [SerializeField]
        private Sprite loadingTexture;
        [SerializeField]
        private RectTransform tagParent;
        [SerializeField]
        private GameObject tagPrefab;

        [SerializeField, Header ("Effects")]
        private float fadeInDuration = 0.2f;
        [SerializeField]
        private float fadeOutDuration = 0.2f;

        [SerializeField]
        private GameEventTable eventTable;

        private CanvasGroup windowGroup;
        private RectTransform rectTransform;
        private List<GameObject> generatedTags;

        private void Awake () {
            windowGroup = GetComponent<CanvasGroup> ();
            windowGroup.alpha = 0f;
            rectTransform = transform as RectTransform;
            generatedTags = new List<GameObject> ();
        }

        private void OnEnable () {
            if (eventTable != null) {
                eventTable.Register<ILibraryElement> ("OnLibraryButtonEnter", HandleLibraryElementEnterEvent);
                eventTable.Register<ILibraryElement> ("OnLibraryButtonExit", HandleLibraryElementExitEvent);
            }
        }

        private void OnDisable () {
            if (eventTable != null) {
                eventTable.Unregister<ILibraryElement> ("OnLibraryButtonEnter", HandleLibraryElementEnterEvent);
                eventTable.Unregister<ILibraryElement> ("OnLibraryButtonExit", HandleLibraryElementExitEvent);
            }
        }

        private void Update () {
            var pos = EventSystem.current.currentInputModule.input.mousePosition;
            rectTransform.position = pos;

            rectTransform.pivot = new Vector2 (
                Mathf.Clamp01 (rectTransform.localPosition.x),
                Mathf.Clamp01 (rectTransform.localPosition.y)
            );
        }

        /// <summary>
        /// Clears all tags from the description
        /// </summary>
        private void ClearTags () {
            for (var i = 0; i < generatedTags.Count; ++i) {
                Destroy (generatedTags[i]);
            }

            generatedTags.Clear ();
        }

        /// <summary>
        /// Generates tags for the player to see
        /// </summary>
        private void GenerateTags (string[] tags) {
            foreach (var tag in tags) {
                var clone = Instantiate (tagPrefab, tagParent);
                var text = clone.GetComponentInChildren<Text> ();

                if (text != null) {
                    text.text = tag;
                }

                generatedTags.Add (clone);
            }
        }

        /// <summary>
        /// Sync the info from the element and show the element info window when the mouse enters an element ui button
        /// </summary>
        /// <param name="libraryElement"></param>
        private void HandleLibraryElementEnterEvent (ILibraryElement libraryElement) {
            var element = libraryElement.Element;
            nameText.text = element.name;
            idText.text = string.Format ("{0}", element.id.ToHexString ());
            descriptionText.text = element.description;

            ClearTags ();
            GenerateTags (element.tags);

            StartCoroutine (SetThumbnailAsync (element));
            TweenAppearance (1f, fadeInDuration);
        }
        /// <summary>
        /// Close element info window when the mouse moves out of the element ui button
        /// </summary>
        /// <param name="libraryElement"></param>
        private void HandleLibraryElementExitEvent (ILibraryElement libraryElement) {
            TweenAppearance (0f, fadeOutDuration);
            StopAllCoroutines ();
        }

        /// <summary>
        /// "Asynchronously" sets the thumbnail texture of the element
        /// </summary>
        private IEnumerator SetThumbnailAsync (ElementData data) {
            if (data.thumbnail == null) {
                thumbnailImage.sprite = loadingTexture;
                yield return null;
            }

            while (data.thumbnail == null) {
                yield return null;
            }

            thumbnailImage.sprite = data.thumbnail;
        }

        /// <summary>
        /// Tweens the alpha of the window
        /// </summary>
        /// <param name="alpha">The target alpha value</param>
        /// <param name="duration">How long should the effect take</param>
        private void TweenAppearance (float alpha, float duration) {
            LeanTween.cancel (windowGroup.gameObject);
            LeanTween.alphaCanvas (windowGroup, alpha, duration).setEaseOutBack ();
        }
    }
}