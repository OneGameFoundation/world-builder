using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OneGame.Lua;
using UnityEngine;
using UnityEngine.UI;

namespace OneGame.UI {
	public class EntityComponentSubMenu : MonoBehaviour, IEntityWindowComponent {
		public IEntityContainer Entity {
			get { return entity; }
			set {
				entity = value;
				ClearButtons ();
				GenerateButtons (entity.ActiveComponents);
			}
		}
		public EntityWindowComponentType Type { get { return EntityWindowComponentType.Component; } }

		[SerializeField]
		private GameEventTable eventTable;
		[SerializeField]
		private RectTransform componentParent;
		[SerializeField]
		private GameObject componentPrefab;
		[SerializeField]
		private EntityComponentMap map;

		private IEntityContainer entity;
		private CanvasGroup group;

		private List<GameObject> spawnedButtons;

		private void Awake () {
			spawnedButtons = new List<GameObject> ();
			group = GetComponent<CanvasGroup> ();
		}

		public void Close () {
			group.alpha = 0f;
			group.interactable = false;
			group.blocksRaycasts = false;
		}

		public void Open () {
			group.alpha = 1f;
			group.interactable = true;
			group.blocksRaycasts = true;

			// eventTable?.Invoke<Property[]> ("OnPropertiesInspect", new Property[0]);
			eventTable?.Invoke<Tuple<ScriptProperty, Action<string>>[] > (
				"OnPropertiesInspect", new Tuple<ScriptProperty, Action<string>>[0]);
		}

		private void ClearButtons () {
			for (var i = 0; i < spawnedButtons.Count; ++i) {
				Destroy (spawnedButtons[i]);
			}

			spawnedButtons.Clear ();
		}

		private void GenerateButtons (NativeComponent[] components) {
			for (var i = 0; i < components.Length; ++i) {
				var component = components[i];
				var clone = Instantiate (componentPrefab, componentParent);

				var sprite = map.GetSprite (component.GetType ().Name.ToLower ());
				SetSprite (sprite, clone);

				var button = clone.GetComponent<Button> ();
				if (button != null) {
					button.onClick.AddListener (() => {
						// eventTable?.Invoke<Property[]> ("OnPropertiesInspect", GetProperties (component));
						eventTable?.Invoke<Tuple<ScriptProperty, Action<string>>[] > (
							"OnPropertiesInspect", GetScriptProperties (component));
					});
				}

				spawnedButtons.Add (clone);
			}
		}

		private void SetSprite (Sprite sprite, GameObject button) {
			var images = button.GetComponentsInChildren<Image> ();

			if (images.Length > 0) {
				var image = images[images.Length - 1];
				image.sprite = sprite;
			}
		}

		private Tuple<ScriptProperty, Action<string>>[] GetScriptProperties (NativeComponent component) {
			var type = component.GetType ();
			var properties = type.GetProperties ().Where (p => System.Attribute.IsDefined (p, typeof (EditableFieldAttribute)));

			var propArray = new Tuple<ScriptProperty, Action<string>>[properties.Count ()];
			var index = 0;

			foreach (var prop in properties) {
				var current = prop;
				var p = new ScriptProperty () { name = current.Name };
				Action<string> callback = null;

				if (current.PropertyType == typeof (bool)) {
					p.type = PropertyType.Boolean;
					p.value = current.GetValue (component).ToString ();
					callback = s => {
						current.SetValue (component, s == "true");
					};
				} else if (current.PropertyType == typeof (string)) {
					p.type = PropertyType.String;
					p.value = current.GetValue (component).ToString ();
					callback = s => {
						current.SetValue (component, s);
					};
				} else if (current.PropertyType == typeof (float)) {
					p.type = PropertyType.Number;
					p.value = current.GetValue (component).ToString ();
					callback = s => {
						var floatNum = default (float);
						if (float.TryParse (s, System.Globalization.NumberStyles.Float, null, out floatNum)) {
							current.SetValue (component, floatNum);
						}
					};
				} else if (current.PropertyType == typeof (int)) {
					p.type = PropertyType.Number;
					p.value = current.GetValue (component).ToString ();
					callback = s => {
						var intNum = default (int);
						if (int.TryParse (s, System.Globalization.NumberStyles.Integer, null, out intNum)) {
							current.SetValue (component, intNum);
						}
					};
				} else if (current.PropertyType == typeof (double)) {
					p.type = PropertyType.Number;
					p.value = current.GetValue (component).ToString ();
					callback = s => {
						var doubleNum = default (double);
						if (double.TryParse (s, System.Globalization.NumberStyles.None, null, out doubleNum)) {
							current.SetValue (component, doubleNum);
						}
					};
				}

				propArray[index++] = new Tuple<ScriptProperty, Action<string>> (p, callback);
			}

			return propArray;
		}

		private Property[] GetProperties (NativeComponent component) {
			var propList = new List<Property> ();

			var type = component.GetType ();
			var properties = type.GetProperties ().Where (p => System.Attribute.IsDefined (p, typeof (EditableFieldAttribute)));

			foreach (var prop in properties) {
				var current = prop;
				var p = new Property {
					name = current.Name
				};

				//TODO: Simplify this large block of code
				if (current.PropertyType == typeof (bool)) {
					p.type = PropertyType.Boolean;
					p.value = current.GetValue (component).ToString ();
					p.onPropertyEdit = s => {
						current.SetValue (component, s == "true");
					};
					propList.Add (p);
				} else if (current.PropertyType == typeof (string)) {
					p.type = PropertyType.String;
					p.value = current.GetValue (component).ToString ();
					p.onPropertyEdit = s => {
						current.SetValue (component, s);
					};
					propList.Add (p);
				} else if (current.PropertyType == typeof (float)) {
					p.type = PropertyType.Number;
					p.value = current.GetValue (component).ToString ();
					p.onPropertyEdit = s => {
						var floatNum = default (float);
						if (float.TryParse (s, System.Globalization.NumberStyles.Float, null, out floatNum)) {
							current.SetValue (component, floatNum);
						}
					};
					propList.Add (p);
				} else if (current.PropertyType == typeof (int)) {
					p.type = PropertyType.Number;
					p.value = current.GetValue (component).ToString ();
					p.onPropertyEdit = s => {
						var intNum = default (int);
						if (int.TryParse (s, System.Globalization.NumberStyles.Integer, null, out intNum)) {
							current.SetValue (component, intNum);
						}
					};
					propList.Add (p);
				} else if (current.PropertyType == typeof (double)) {
					p.type = PropertyType.Number;
					p.value = current.GetValue (component).ToString ();
					p.onPropertyEdit = s => {
						var doubleNum = default (double);
						if (double.TryParse (s, System.Globalization.NumberStyles.None, null, out doubleNum)) {
							current.SetValue (component, doubleNum);
						}
					};
					propList.Add (p);
				}

			}

			return propList.ToArray ();
		}
	}
}