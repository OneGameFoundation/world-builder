using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OneGame.UI {
    /// <summary>
    /// A button element in the library that represents a loaded element
    /// </summary>
    public class LibraryElement : MonoBehaviour, ILibraryElement, IPointerEnterHandler, IPointerExitHandler {

        /// <summary>
        /// The current element
        /// </summary>
        public ElementData Element {
            get { return element; }
            set {
                element = value;

                if (element.thumbnail != null) {
                    buttonImage.sprite = element.thumbnail;
                }
            }
        }

        public RectTransform Transform { get; private set; }

        [SerializeField]
        private Image buttonImage;
        [SerializeField]
        private GameEventTable eventTable;

        private ElementData element;

        private void Awake () {
            Transform = GetComponent<RectTransform> ();

            var button = GetComponent<Button> ();
            if (button != null) {
                button.onClick.AddListener (() => { eventTable?.Invoke<ElementData> ("OnElementButtonClicked", Element); });
            }

            //add BeginDrag & EndDrag listener to eventTrigger
            var eventTrigger = gameObject.GetComponent<EventTrigger> ();
            if (eventTrigger) {
                EventTrigger.Entry entry = new EventTrigger.Entry ();
                entry.eventID = EventTriggerType.BeginDrag;
                entry.callback.AddListener ((data) => { eventTable?.Invoke<Sprite> ("OnElementButtonDragged", element.thumbnail); });
                eventTrigger.triggers.Add (entry);

                EventTrigger.Entry entry1 = new EventTrigger.Entry ();
                entry1.eventID = EventTriggerType.EndDrag;
                entry1.callback.AddListener ((data) => { eventTable?.Invoke<uint> ("OnElementButtonDropped", element.id); });
                eventTrigger.triggers.Add (entry1);
            }
        }

        public void OnPointerEnter (PointerEventData eventData) {
            eventTable?.Invoke<ILibraryElement> ("OnLibraryButtonEnter", this);
        }

        public void OnPointerExit (PointerEventData eventData) {
            eventTable?.Invoke<ILibraryElement> ("OnLibraryButtonExit", this);
        }
    }
}