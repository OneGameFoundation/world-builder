using UnityEngine;
using OneGame.Lua;
using UnityEngine.UI;

namespace OneGame.UI {
    /// <summary>
    /// A UI script that displays a quick-access inventory on the screen
    /// </summary>
    public class InventoryBar : MonoBehaviour {

        [SerializeField]
        private RectTransform[] slots;
        [SerializeField]
        private Sprite placeholderSprite;

        [SerializeField]
        private RectTransform indicator;

        [SerializeField, Space]
        private GameEventTable eventTable;
        [SerializeField]
        private ItemCatalog itemCatalog;

        private Image[] spriteImages;
        private ElementData[] elements;
        private Inventory inventory;

        private void Start () {
            spriteImages = new Image[slots.Length];
            elements = new ElementData[slots.Length];
            GenerateImageReferences (slots, ref spriteImages);

            indicator.gameObject.SetActive (false);
        }

        private void OnEnable () {
            if (eventTable != null) {
                eventTable.Register<WorldMode> ("OnWorldModeChange", HandleWorldModeChange);
                eventTable.Register<WorldMode, WorldMode> ("OnWorldModeTransition", HandleWorldModeTransition);
            }
        }

        private void OnDisable () {
            if (eventTable != null) {
                eventTable.Unregister<WorldMode> ("OnWorldModeChange", HandleWorldModeChange);
                eventTable.Unregister<WorldMode, WorldMode> ("OnWorldModeTransition", HandleWorldModeTransition);
            }
        }


        /// <summary>
        /// Populates the image references to later referencing
        /// </summary>
        /// <param name="slots">The slots to reference</param>
        /// <param name="images">The image references to populate</param>
        private void GenerateImageReferences (RectTransform[] slots, ref Image[] images) {
            var length = slots.Length;

            for (var i = 0; i < length; ++i) {
                var child = slots[i].GetChild (0);
                images[i] = child.GetComponent<Image> ();
            }
        }

        /// <summary>
        /// Delegate called when the player has attached an equipment
        /// </summary>
        private void HandleEquipEvent (Entity entity, BodyPart bodyPart, EquipType type) {
            var id = entity.assetId.ToUInt ();
            for (var i = 0; i < elements.Length; ++i) {
                if (elements[i].id == id) {
                    TweenIndicator (indicator, slots[i].anchoredPosition, 0.2f);
                    break;
                }
            }
        }

        /// <summary>
        /// Delegate called when the player adds or removes an item from the inventory
        /// </summary>
        /// <param name="entity">The entity that was added or removed</param>
        /// <param name="amount">The amount to add or remove</param>
        private void HandleInventoryChangeEvent (Entity entity, int amount) {
            var entities = inventory.StoredEntities;
            elements = new ElementData[spriteImages.Length];

            var length = Mathf.Min (entities.Length, elements.Length);
            for (var i = 0; i < length; ++i) {
                elements[i] = itemCatalog.FindElementData (entities[i].assetId.ToUInt ());
            }

            if (length == entities.Length) {
                for (var i = length; i < elements.Length; ++i) {
                    elements[i] = ElementData.Empty;
                }
            }

            PopulateSprites (elements, spriteImages, placeholderSprite);
        }

        /// <summary>
        /// Delegate called when the world mode changes
        /// </summary>
        /// <param name="mode">The current world mode</param>
        private void HandleWorldModeChange (WorldMode mode) {
            if (mode == WorldMode.Play) {
                inventory = Player.Instance.inventory;
                if (inventory != null) {
                    inventory.OnItemAdd += HandleInventoryChangeEvent;
                    inventory.OnItemRemove += HandleInventoryChangeEvent;
                    inventory.OnItemEquip += HandleEquipEvent;
                }
            }
        }

        /// <summary>
        /// Delegate called whent the world mode begins its transiton
        /// </summary>
        /// <param name="start">The starting phase of the transition</param>
        /// <param name="end">The ending phase of the transition</param>
        private void HandleWorldModeTransition (WorldMode start, WorldMode end) {
            if (start == WorldMode.Play && end == WorldMode.Build) {
                if (inventory != null) {
                    inventory.OnItemAdd -= HandleInventoryChangeEvent;
                    inventory.OnItemRemove -= HandleInventoryChangeEvent;
                    inventory.OnItemEquip -= HandleEquipEvent;
                }
            }
        }

        /// <summary>
        /// Populates the bars with sprites
        /// </summary>
        private void PopulateSprites (ElementData[] data, Image[] images, Sprite placeholder) {
            var length = data.Length;

            for (var i = 0; i < length; ++i) {
                var current = data[i];
                images[i].sprite = current.id > 0 ? current.thumbnail : placeholder;
            }
        }

        /// <summary>
        /// Tweens the indicator to the targeted slot
        /// </summary>
        /// <param name="indicator">The indicator to tween</param>
        /// <param name="anchoredTarget">The target position</param>
        /// <param name="duration">The duration of the tween</param>
        private void TweenIndicator (RectTransform indicator, Vector2 anchoredTarget, float duration) {
            indicator.gameObject.SetActive (true);

            LeanTween.cancel (indicator);
            LeanTween.move (indicator, anchoredTarget, duration).setEaseOutBack ();
        }
    }
}