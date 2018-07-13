using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OneGame.UI {
    /// <summary>
    /// the ui component for reference variable
    /// </summary>
    public class ReferenceVariableUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        [SerializeField]
        private GameEventTable eventTable;
        [SerializeField]
        private TMP_InputField input;
        [SerializeField]
        private Image BG;
        [SerializeField]
        private Color bgSelectedColor;
        [SerializeField]
        private ItemCatalog itemCatalog;

        private bool isHovering = false;
        private Color originalColor;
        private bool isDragging = false;

        private void OnEnable () {
            eventTable?.Register<uint> ("OnElementButtonDropped", HandleElemntButtonDropped);
            eventTable?.Register<Sprite> ("OnElementButtonDragged", StartDrag);
        }
        private void OnDisable () {
            eventTable?.Unregister<uint> ("OnElementButtonDropped", HandleElemntButtonDropped);
            eventTable?.Unregister<Sprite> ("OnElementButtonDragged", StartDrag);
        }

        void Start () {
            isHovering = false;
            originalColor = BG.color;
            isDragging = false;

        }

        private void HandleElemntButtonDropped (uint id) {
            if (isHovering) {
                var idString = id.ToHexString ();
                var element = itemCatalog.FindElementData (id);
                input.text = element.name;
                input.onEndEdit.Invoke (idString);
            }
            isDragging = false;
        }
        private void StartDrag (Sprite sp) {
            isDragging = true;
        }

        public void OnPointerEnter (PointerEventData eventData) {
            if (isDragging) {
                isHovering = true;
                BG.color = bgSelectedColor;
            }
        }

        public void OnPointerExit (PointerEventData eventData) {

            isHovering = false;
            BG.color = originalColor;

        }
    }
}