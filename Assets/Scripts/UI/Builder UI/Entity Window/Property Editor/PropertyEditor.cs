using System;
using System.Collections;
using System.Collections.Generic;
using OneGame.Lua;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OneGame.UI {
	/// <summary>
	/// An editor that allows the player to visually edit properties
	/// </summary>
	public class PropertyEditor : MonoBehaviour {

		[SerializeField]
		private GameEventTable eventTable;

		[SerializeField, Space]
		private RectTransform content;
		[SerializeField]
		private GameObject booleanPrefab;
		[SerializeField]
		private GameObject stringPrefab;
		[SerializeField]
		private GameObject numberPrefab;
		[SerializeField]
		private GameObject referencePrefab;

		private List<GameObject> generatedContent;
		private CanvasGroup group;

		private void Awake () {
			generatedContent = new List<GameObject> ();
			group = GetComponent<CanvasGroup> ();
		}

		private void OnEnable () {
			if (eventTable != null) {
				eventTable.Register<Tuple<ScriptProperty, Action<string>>[] > ("OnPropertiesInspect", EditProperties);
				eventTable.Register<IEntityContainer> ("OnEntitySelect", HandleEnitySelectionEvent);
				eventTable.Register ("OnEntityDeselect", Close);
			}
		}

		private void OnDisable () {
			if (eventTable != null) {
				eventTable.Unregister<Tuple<ScriptProperty, Action<string>>[] > ("OnPropertiesInspect", EditProperties);
				eventTable.Unregister<IEntityContainer> ("OnEntitySelect", HandleEnitySelectionEvent);
				eventTable.Unregister ("OnEntityDeselect", Close);
			}
		}

		public void Open () {
			group.alpha = 1f;
			group.interactable = true;
			group.blocksRaycasts = true;
		}

		public void ClearProperties () {
			for (var i = 0; i < generatedContent.Count; ++i) {
				Destroy (generatedContent[i]);

			}

			generatedContent.Clear ();
		}

		public void Close () {
			group.alpha = 0;
			group.interactable = false;
			group.blocksRaycasts = false;
		}

		/// <summary>
		/// Create property fields on the ui as ui elements in property window
		/// </summary>
		/// <param name="properties"></param>
		public void EditProperties (Tuple<ScriptProperty, Action<string>>[] properties) {

			ClearProperties ();
			if (properties.Length < 1) {
				Close ();
				return;
			}

			Open ();

			for (var i = 0; i < properties.Length; ++i) {
				var prop = properties[i].item1;
				var callback = properties[i].item2;

				switch (prop.type) {
					case PropertyType.Boolean:
						var boolProp = AttachProperty (booleanPrefab, prop.name);
						var toggle = boolProp.GetComponentInChildren<Toggle> ();
						if (toggle != null) {
							toggle.isOn = prop.value == "true";
							toggle.onValueChanged.AddListener (b => {
								prop.value = b ? "true" : "false";
								callback?.Invoke (prop.value);
							});
						}
						break;

					case PropertyType.Number:
						var numberProp = AttachProperty (numberPrefab, prop.name);
						var numberField = numberProp.GetComponentInChildren<TMP_InputField> ();
						if (numberField != null) {
							numberField.text = prop.value;
							numberField.onEndEdit.AddListener (s => {
								prop.value = s;
								callback?.Invoke (s);
							});
						}
						break;

					case PropertyType.String:

						if (prop.name.IndexOf ("id_") == 0) {
							var refProp = AttachProperty (referencePrefab, prop.name);
							var stringField = refProp.GetComponentInChildren<TMP_InputField> ();
							if (stringField != null) {
								stringField.text = prop.value;
								stringField.onEndEdit.AddListener (s => {
									prop.value = s;
									callback?.Invoke (s);
								});
							}
						} else {
							var stringProp = AttachProperty (stringPrefab, prop.name);
							var stringField = stringProp.GetComponentInChildren<TMP_InputField> ();
							if (stringField != null) {
								stringField.text = prop.value;
								stringField.onEndEdit.AddListener (s => {
									prop.value = s;
									callback?.Invoke (s);
								});
							}
						}

						break;
				}

			}

		}

		/// <summary>
		/// Attach the property ui element and the values
		/// </summary>
		/// <param name="prefab"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private GameObject AttachProperty (GameObject prefab, string name) {
			var clone = Instantiate (prefab, content);
			generatedContent.Add (clone);

			var text = clone.GetComponentInChildren<TMP_Text> ();
			if (text != null) {
				text.text = name;
			}

			return clone;
		}

		private void HandleEnitySelectionEvent (IEntityContainer entity) {
			ClearProperties ();
			Close ();
		}
	}
}