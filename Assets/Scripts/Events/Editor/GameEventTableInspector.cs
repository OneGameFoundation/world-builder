using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OneGame {
	[CustomEditor (typeof (GameEventTable))]
	public class GameEventTableInspector : Editor {

		private Dictionary<string, Delegate> events;

		private void OnEnable () {
			var table = target as GameEventTable;
			var type = table.GetType ();
			var field = type.GetField ("eventTable", BindingFlags.Instance | BindingFlags.NonPublic);

			events = (Dictionary<string, Delegate>) field.GetValue (table);
		}

		public override void OnInspectorGUI () {
			base.OnInspectorGUI ();

			if (events != null) {
				EditorGUILayout.LabelField (string.Format ("{0} active event(s)", events.Count),
					EditorStyles.largeLabel);

				EditorGUILayout.Separator ();
				DrawTableHeader ();

				using (var vertical = new EditorGUILayout.VerticalScope ()) {
					foreach (var element in events) {
						var del = element.Value;

						if (del != null) {
							using (var horizontal = new EditorGUILayout.HorizontalScope ()) {
								EditorGUILayout.LabelField (element.Key);
								EditorGUILayout.LabelField (string.Format ("{0}", element.Value.GetInvocationList ().Length), GUILayout.Width (100f));
							}
						}
					}
				}
			}
		}

		public override bool RequiresConstantRepaint () {
			return EditorApplication.isPlaying;
		}

		private void DrawTableHeader () {
			using (var horizontal = new EditorGUILayout.HorizontalScope ()) {
				GUI.Box (horizontal.rect, GUIContent.none);

				EditorGUILayout.LabelField ("Name", EditorStyles.boldLabel);
				EditorGUILayout.LabelField ("# Registered", EditorStyles.boldLabel, GUILayout.Width (100f));
			}
		}
	}
}