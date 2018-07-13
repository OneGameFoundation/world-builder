using System.Collections;
using System.Collections.Generic;
using OneGame;
using UnityEngine;
using UnityEngine.UI;

namespace OneGame.UI {
	/// <summary>
	/// UI that show up when start draging from an element inside Library panel
	/// </summary>
	public class DragSymbol : MonoBehaviour {
		[SerializeField]
		private GameEventTable eventTable;
		[SerializeField]
		private Image img;
		private bool isDragging;
		private Sprite icon;
		private RectTransform rect;

		private void Start () {
			gameObject.SetActive (false);
			isDragging = false;
			rect = GetComponent<RectTransform> ();
		}

		private void OnEnable () {
			eventTable?.Register<Sprite> ("OnElementButtonDragged", StartDrag);
			eventTable?.Register<uint> ("OnElementButtonDropped", EndDrag);
		}

		private void OnDestroy () {
			eventTable?.Unregister<Sprite> ("OnElementButtonDragged", StartDrag);
			eventTable?.Unregister<uint> ("OnElementButtonDropped", EndDrag);
		}

		private void Update () {
			if (isDragging) {
				rect.anchoredPosition = Input.mousePosition;
			}
		}

		/// <summary>
		/// Hide the DragSymbol when the drag is ended
		/// </summary>
		/// <param name="id"></param>
		private void EndDrag (uint id) {
			isDragging = false;
			gameObject.SetActive (false);
		}

		/// <summary>
		/// Sync the dragged icon to the DragSymbol, and show the Drag Symbol when the drag started
		/// </summary>
		/// <param name="icon">the icon sprite to switch to</param>
		private void StartDrag (Sprite icon) {
			rect.anchoredPosition = Input.mousePosition;
			isDragging = true;
			img.sprite = icon;
			gameObject.SetActive (true);

		}
	}
}