using System.Collections;
using System.Collections.Generic;
using OneGame.Lua;
using UnityEngine;
using UnityEngine.UI;

namespace OneGame.UI {
    [RequireComponent (typeof (CanvasGroup))]
    public class EntityWindow : MonoBehaviour {

        [SerializeField, Header ("Description")]
        private Text nameText;
        [SerializeField]
        private Text idText;
        [SerializeField]
        private Text descriptionText;
        [SerializeField]
        private Image thumbnail;
        [SerializeField]
        private Sprite loadingImage;
        [SerializeField]
        private RectTransform tagParent;
        [SerializeField]
        private GameObject tagPrefab;

        [SerializeField, Header ("Effects")]
        private float fadeInDuration = 0.2f;
        [SerializeField]
        private float fadeOutDuration = 0.2f;

        [SerializeField, Space]
        private GameEventTable eventTable;
        [SerializeField]
        private ItemCatalog itemDatabase;
        [SerializeField, Header ("Tabs")]
        private GameObject defaultTabButton;
        [SerializeField]
        private GameObject scriptTabButton;
        [SerializeField]
        private Sprite defaultTabSelected;
        [SerializeField]
        private Sprite defaultTabUnselected;
        [SerializeField]
        private Sprite scriptTabSelected;
        [SerializeField]
        private Sprite scriptTabUnselected;

        private CanvasGroup windowGroup;
        private List<GameObject> generatedTags;
        private IEntityWindowComponent[] windowComponents;

        private void Start () {
            windowComponents = GetComponentsInChildren<IEntityWindowComponent> ();
            windowGroup = GetComponent<CanvasGroup> ();
            windowGroup.alpha = 0f;
            windowGroup.interactable = false;
            windowGroup.blocksRaycasts = false;
            generatedTags = new List<GameObject> ();

            OpenSubMenu (EntityWindowComponentType.Component);
        }

        private void OnEnable () {
            if (eventTable != null) {
                eventTable.Register<IEntityContainer> ("OnEntitySelect", HandleEntitySelectionEvent);
                eventTable.Register ("OnEntityDeselect", HandleEntityUnselectEvent);
                eventTable.Register ("OnEntityComponentTabClick", HandleComponentButtonClick);
                eventTable.Register ("OnEntityScriptTabClick", HanldleScriptButtonClick);
            }
        }

        private void OnDisable () {
            if (eventTable != null) {
                eventTable.Unregister<IEntityContainer> ("OnEntitySelect", HandleEntitySelectionEvent);
                eventTable.Unregister ("OnEntityDeselect", HandleEntityUnselectEvent);
                eventTable.Unregister ("OnEntityComponentTabClick", HandleComponentButtonClick);
                eventTable.Unregister ("OnEntityScriptTabClick", HanldleScriptButtonClick);
            }
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
        /// recieve selected entity and update the info in the entity window
        /// </summary>
        /// <param name="entity"></param>
        private void HandleEntitySelectionEvent (IEntityContainer entity) {
            var id = default (uint);
            uint.TryParse (entity.AssetId, System.Globalization.NumberStyles.HexNumber, null, out id);
            var element = itemDatabase.FindElementData (id);

            // Set the description values
            nameText.text = entity.GameObject.name;
            idText.text = string.Format ("ID: {0}", entity.Entity.id);
            descriptionText.text = element.description;

            // Set the thumbnail
            if (element.thumbnail != null) {
                thumbnail.sprite = element.thumbnail;
            } else {
                thumbnail.sprite = loadingImage;
                StopAllCoroutines ();
                StartCoroutine (SetThumbnailAsync (element));
            }

            // Set the tags
            ClearTags ();
            GenerateTags (element.tags);

            // Update the entity values in the subwindows
            for (var i = 0; i < windowComponents.Length; ++i) {
                windowComponents[i].Entity = entity;
            }

            if (entity.ActiveComponents.Length == 0) {
                eventTable.Invoke ("OnEntityScriptTabClick");
            }

            TweenWindow (1f, fadeInDuration);
        }

        /// <summary>
        /// fade out the window when entity is unselected
        /// </summary>
        private void HandleEntityUnselectEvent () {
            TweenWindow (0f, fadeOutDuration);
        }

        /// <summary>
        /// handles component button click event
        /// </summary>
        private void HandleComponentButtonClick () {
            defaultTabButton.GetComponent<Image> ().sprite = defaultTabSelected;
            scriptTabButton.GetComponent<Image> ().sprite = scriptTabUnselected;
            OpenSubMenu (EntityWindowComponentType.Component);
        }

        /// <summary>
        /// handles script button click event
        /// </summary>
        private void HanldleScriptButtonClick () {
            defaultTabButton.GetComponent<Image> ().sprite = defaultTabUnselected;
            scriptTabButton.GetComponent<Image> ().sprite = scriptTabSelected;
            OpenSubMenu (EntityWindowComponentType.Script);
        }

        /// <summary>
        /// opens particular panel
        /// </summary>
        /// <param name="type"></param>
        private void OpenSubMenu (EntityWindowComponentType type) {
            for (var i = 0; i < windowComponents.Length; ++i) {
                var component = windowComponents[i];

                if (component.Type == type) {
                    component.Open ();
                } else {
                    component.Close ();
                }
            }
        }

        /// <summary>
        /// load the entity's thumbnail
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerator SetThumbnailAsync (ElementData data) {
            while (data.thumbnail == null) {
                yield return null;
            }

            thumbnail.sprite = data.thumbnail;
        }

        /// <summary>
        /// fade in/out entity window
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="duration"></param>
        private void TweenWindow (float alpha, float duration) {
            LeanTween.cancel (gameObject);

            LeanTween.alphaCanvas (windowGroup, alpha, duration)
                .setEaseOutBack ()
                .setOnComplete (() => {
                    windowGroup.interactable = alpha == 1f;
                    windowGroup.blocksRaycasts = alpha == 1f;
                });
        }
    }
}