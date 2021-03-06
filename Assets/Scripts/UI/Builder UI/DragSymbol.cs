﻿using UnityEngine;
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
        private Sprite icon;
        private RectTransform rect;

        private void Start () {
            rect = GetComponent<RectTransform> ();
            gameObject.SetActive (false);
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
            var normalizedMouse = new Vector3 (Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            rect.anchoredPosition = new Vector2 (normalizedMouse.x * 1920f - 960f, normalizedMouse.y * 1080f - 540f);
        }

        /// <summary>
        /// Hide the DragSymbol when the drag is ended
        /// </summary>
        /// <param name="id"></param>
        private void EndDrag (uint id) {
            gameObject.SetActive (false);
        }

        /// <summary>
        /// Sync the dragged icon to the DragSymbol, and show the Drag Symbol when the drag started
        /// </summary>
        /// <param name="icon">the icon sprite to switch to</param>
        private void StartDrag (Sprite icon) {
            rect.anchoredPosition = Input.mousePosition;
            img.sprite = icon;
            gameObject.SetActive (true);
        }
    }
}