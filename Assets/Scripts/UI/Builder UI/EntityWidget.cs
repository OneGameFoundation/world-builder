using OneGame.Lua;
using UnityEngine;

namespace OneGame.UI {
	/// <summary>
	/// A widget that appears when the player clicks on an entity
	/// </summary>
	public class EntityWidget : MonoBehaviour {
		/// <summary>
		/// The currently selected entity
		/// </summary>
		public IEntityContainer Entity { get; private set; }

		[SerializeField]
		private GameEventTable eventTable;

		[SerializeField, Space]
		private GameObject scaleWidget;
		[SerializeField]
		private Vector2 widgetXPositions;
		[SerializeField]
		private float scaleTweenTime = 0.2f;

		[SerializeField]
		private float sizeAttribute = 10f;
		[SerializeField]
		private float offsetAttribute = 0.3f;
		[SerializeField]
		private Vector2 widgetSizeRange = new Vector2 (0.8f, 4f);

		private RectTransform rectTransform;
		private RectTransform root;
		private CanvasGroup group;

		private void Awake () {
			group = GetComponent<CanvasGroup> ();
			root = transform.root.GetComponent<RectTransform> ();
			rectTransform = GetComponent<RectTransform> ();
		}

		private void Start () {
			TweenWidget (scaleWidget, widgetXPositions.x, Vector3.zero, scaleTweenTime);
			Close ();

		}

		private void OnEnable () {
			if (eventTable != null) {
				eventTable.Register<IEntityContainer> ("OnEntitySelect", HandleEntitySelectionEvent);
				eventTable.Register ("OnEntityDeselect", Close);
				eventTable.Register ("OnDeleteButtonClicked", Close);
			}
		}

		private void OnDisable () {
			if (eventTable != null) {
				eventTable.Unregister<IEntityContainer> ("OnEntitySelect", HandleEntitySelectionEvent);
				eventTable.Unregister ("OnEntityDeselect", Close);
				eventTable.Unregister ("OnDeleteButtonClicked", Close);
			}
		}

		private void LateUpdate () {
			if (group.interactable && Entity != null) {
				Transform transform = Entity.Transform;

				if (transform != null) {
					var screenPos = Camera.main.WorldToViewportPoint (Entity.Transform.position);

					var size = root.sizeDelta;

					var targetPos = new Vector2 (
						(screenPos.x * size.x) - (size.x * offsetAttribute),
						(screenPos.y * size.y) - (size.y * offsetAttribute)
					);

					rectTransform.anchoredPosition = targetPos;
					rectTransform.localScale = Mathf.Clamp (sizeAttribute / Camera.allCameras[0].fieldOfView, widgetSizeRange.x, widgetSizeRange.y) * Vector3.one;

				} else {
					Close ();
				}
			}
		}

		/// <summary>
		/// Hide the widget
		/// </summary>
		public void Close () {
			group.alpha = 0f;
			group.interactable = false;
			group.blocksRaycasts = false;
		}

		/// <summary>
		/// Show the widget
		/// </summary>
		public void Open () {
			group.alpha = 1f;
			group.interactable = true;
			group.blocksRaycasts = true;
		}

		/// <summary>
		/// Show the plus minus sign with animation
		/// </summary>
		public void ToggleScaleUtility () {
			if (scaleWidget.transform.localScale.magnitude < 0.5f) {
				TweenWidget (scaleWidget, widgetXPositions.y, Vector3.one, scaleTweenTime);
			} else {
				TweenWidget (scaleWidget, widgetXPositions.x, Vector3.zero, scaleTweenTime);
			}
		}

		/// <summary>
		/// Open widget when an entity is selected
		/// </summary>
		/// <param name="entity"></param>
		private void HandleEntitySelectionEvent (IEntityContainer entity) {
			Entity = entity;
			Open ();
		}

		/// <summary>
		/// Animated object movement to certain destination
		/// </summary>
		/// <param name="widget">the object to move</param>
		/// <param name="xPos">the destinate X position to go</param>
		/// <param name="scale">the destinate scale</param>
		/// <param name="time">the animation time</param>
		private void TweenWidget (GameObject widget, float xPos, Vector3 scale, float time) {
			LeanTween.cancel (widget);

			var rectTransform = widget.GetComponent<RectTransform> ();
			LeanTween.value (widget, rectTransform.anchoredPosition.x, xPos, time)
				.setOnUpdate ((float f) => {
					var pos = rectTransform.anchoredPosition;
					pos.x = f;
					rectTransform.anchoredPosition = pos;
				});
			LeanTween.scale (widget, scale, time).setEaseOutBack ();
		}
	}
}