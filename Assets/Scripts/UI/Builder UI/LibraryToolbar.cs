using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneGame.UI {
	/// <summary>
	/// A toolbar that allows for searching items
	/// </summary>
	public class LibraryToolbar : MonoBehaviour {

		/// <summary>
		/// The current toolbar open status
		/// </summary>
		public PanelMode CurrentPanelMode { get; private set; }

		[SerializeField]
		private ItemCatalog itemDatabase;

		[SerializeField, Header ("UI Positioning (yPos, height)")]
		private float scaleEffectDuration = 0.3f;
		[SerializeField]
		private Vector2 collapsedPosition;
		[SerializeField]
		private Vector2 openedPosition;
		[SerializeField]
		private Vector2 expandedPosition;

		[SerializeField, Header ("Tag Options")]
		private string[] tags;
		[SerializeField]
		private RectTransform tagParent;
		[SerializeField]
		private GameObject tagPrefab;
		[SerializeField]
		private GameObject tagHeaderPrefab;

		[SerializeField, Header ("Elements")]
		private RectTransform elementParent;
		[SerializeField]
		private GameObject elementPrefab;
		[SerializeField]
		private GridLayoutGroup elementLayout;

		[SerializeField, Header ("Search")]
		private InputField searchInput;

		[SerializeField, Space]
		private GameObject resizeButton;
		[SerializeField]
		private GameObject CollapseButton;

		public enum PanelMode { Collapsed, Open, Expanded }

		private LibraryElement[] generatedElements;
		private RectTransform rectTransform;
		private GameObject[] generatedTabs;

		private IEnumerator Start () {
			rectTransform = GetComponent<RectTransform> ();
			PrepareTags ();

			searchInput.onEndEdit.AddListener (
				s => {
					if (CurrentPanelMode == PanelMode.Collapsed) {
						Resize (PanelMode.Open);
					}
					Search (s);
				});

			Collapse ();

			// Wait until the database at least has its raw data loaded
			while (itemDatabase.Status == ItemCatalog.LoadingStatus.NotLoaded) {
				yield return null;
			}

			PrepareElements (itemDatabase.Elements);
			Search (tags.Length > 0 ? tags[0] : string.Empty);
		}

		public void Collapse () {
			Resize (PanelMode.Collapsed);
			TweenToolbar (collapsedPosition, scaleEffectDuration);
		}

		/// <summary>
		/// Resizes the toolbar based on the current mode, e.g. Collapsed -> One Line -> Full
		/// </summary>
		public void Resize () {
			switch (CurrentPanelMode) {
				case PanelMode.Collapsed:
					Resize (PanelMode.Open);
					break;

				case PanelMode.Open:
					Resize (PanelMode.Expanded);

					break;

				case PanelMode.Expanded:
					Resize (PanelMode.Open);
					break;
			}
		}

		public void Resize (PanelMode mode) {
			var icon = resizeButton.transform.GetChild (0);
			CurrentPanelMode = mode;
			switch (mode) {
				case PanelMode.Collapsed:
					TweenToolbar (collapsedPosition, scaleEffectDuration);
					CollapseButton.GetComponent<Button> ().interactable = false;
					resizeButton.GetComponent<Button> ().interactable = true;
					RotateTarget (icon.gameObject, new Vector3 (0f, 0f, 0f), 0.3f);
					break;

				case PanelMode.Open:
					TweenToolbar (openedPosition, scaleEffectDuration);
					elementLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
					elementLayout.constraintCount = 1;

					RotateTarget (icon.gameObject, new Vector3 (0f, 0f, 0f), 0.3f);
					CollapseButton.GetComponent<Button> ().interactable = true;
					break;

				case PanelMode.Expanded:
					TweenToolbar (expandedPosition, scaleEffectDuration);

					// TODO: Make this work aspect ratios other than 16:9 and 16:10
					// var layoutTransform = elementLayout.GetComponent<RectTransform> ();
					elementLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
					//Debug.LogFormat ("size delta x: {0}, cellSize.x: {1}", layoutTransform.sizeDelta.x, elementLayout.cellSize.x);
					elementLayout.constraintCount = Mathf.FloorToInt (Screen.width / elementLayout.cellSize.x);

					CollapseButton.GetComponent<Button> ().interactable = true;

					RotateTarget (icon.gameObject, new Vector3 (0f, 0f, 180f), 0.3f);
					break;
			}
		}

		public void Search (string tag) {
			// TODO: Multithread this part
			for (var i = 0; i < generatedElements.Length; ++i) {
				var element = generatedElements[i];

				element.gameObject.SetActive (ContainsTag (tag, element.Element.tags));
			}
		}

		private bool ContainsTag (string tag, string[] tags) {
			for (var i = 0; i < tags.Length; ++i) {
				if (tags[i] == tag) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Prepares the tags for quick searching
		/// </summary>
		private void PrepareTags () {
			generatedTabs = new GameObject[tags.Length];
			for (var i = 0; i < tags.Length; ++i) {
				var tag = tags[i];
				var tagButton = Instantiate (i == 0 ? tagHeaderPrefab : tagPrefab, tagParent);

				var button = tagButton.GetComponent<Button> ();
				generatedTabs[i] = tagButton;
				if (button != null) {
					button.onClick.RemoveAllListeners ();
					button.onClick.AddListener (() => {
						if (CurrentPanelMode == PanelMode.Collapsed) {
							Resize (PanelMode.Open);
						}
						SwapTabStatus (tagButton);
						Search (tag);
					});
				}

				var text = tagButton.GetComponentInChildren<Text> ();
				if (text != null) {
					text.text = tag;
				}

			}
		}
		/// <summary>
		/// Rotates the target's transform towards the specified rotation
		/// </summary>
		/// <param name="target">the subject you want to rotate</param>
		/// <param name="targetRotation">the target rotation</param>
		/// <param name="time">duration to rotate</param>
		private void RotateTarget (GameObject target, Vector3 targetRotation, float time) {
			LeanTween.cancel (target);

			var transform = target.transform;

			LeanTween.value (target, transform.localEulerAngles, targetRotation, time)
				.setEaseOutCubic ()
				.setOnUpdate ((Vector3 rot) => { transform.localEulerAngles = rot; });
		}

		private void SwapTabStatus (GameObject activateTab) {
			for (var i = 0; i < generatedTabs.Length; i++) {
				var tab = generatedTabs[i];
				var image = tab.GetComponent<Image> ();
				var button = tab.GetComponent<Button> ();

				image.sprite = tab == activateTab ?
					button.spriteState.highlightedSprite : button.spriteState.pressedSprite;
			}

		}

		/// <summary>
		/// Prepares the buttons to display on the screen
		/// </summary>
		private void PrepareElements (ElementData[] elements) {
			generatedElements = new LibraryElement[elements.Length];

			for (var i = 0; i < generatedElements.Length; ++i) {
				var element = elements[i];

				var buttonClone = Instantiate (elementPrefab, elementParent);
				var libraryElement = buttonClone.GetComponent<LibraryElement> ();

				if (libraryElement != null) {
					libraryElement.Element = element;
					generatedElements[i] = libraryElement;
				}

				buttonClone.SetActive (false);
			}
		}

		private void TweenToolbar (Vector2 positioning, float tweenTime) {
			LeanTween.cancel (gameObject);

			LeanTween.value (gameObject, rectTransform.anchoredPosition.y, positioning.x, tweenTime)
				.setOnUpdate ((float f) => {
					var anchor = rectTransform.anchoredPosition;
					anchor.y = f;
					rectTransform.anchoredPosition = anchor;
				});

			LeanTween.value (gameObject, rectTransform.sizeDelta.y, positioning.y, tweenTime)
				.setOnUpdate ((float f) => {
					var size = rectTransform.sizeDelta;
					size.y = f;
					rectTransform.sizeDelta = size;
				});
		}

	}
}