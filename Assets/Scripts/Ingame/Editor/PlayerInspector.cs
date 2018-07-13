using System.Collections.Generic;
using System.Reflection;
using OneGame.Lua;
using UnityEditor;
using UnityEngine;

namespace OneGame {
	[CustomEditor (typeof (InteractivePlayer))]
	public class PlayerInspector : Editor {

		private Dictionary<Entity, int> inventory;

		private void OnEnable () {
			var instance = Player.Instance;

			if (instance != null) {
				var playerInventory = instance.inventory;

				if (playerInventory != null) {
					var type = playerInventory.GetType ();
					var field = type.GetField ("storedEntities", BindingFlags.NonPublic | BindingFlags.Instance);

					inventory = field.GetValue (playerInventory) as Dictionary<Entity, int>;
				}
			}
		}

		public override void OnInspectorGUI () {
			base.OnInspectorGUI ();

			if (inventory != null) {
				EditorGUILayout.LabelField (string.Format ("Number of Items: {0}", inventory.Count));
				foreach (var element in inventory) {
					using (var horizontal = new EditorGUILayout.HorizontalScope ()) {
						try {
							EditorGUILayout.LabelField (element.Key.name);
							EditorGUILayout.LabelField (element.Value.ToString (), GUILayout.Width (100f));
						} catch (System.NullReferenceException) {
							continue;
						} catch (MissingReferenceException) {
							continue;
						}
					}
				}
			}
		}
	}
}