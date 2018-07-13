using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace OneGame {
	public class ScriptableObjectGenerator : EditorWindow {

		private string searchString;
		private List<Tuple<string, MonoScript>> availableScripts;
		private List<Tuple<string, MonoScript>> results;
		private Vector2 scroll;

		[MenuItem ("Tools/Scriptable Object Generator")]
		public static void Open () {
			GetWindow<ScriptableObjectGenerator> (true, "Scriptable Generator");
		}

		private void OnEnable () {
			availableScripts = new List<Tuple<string, MonoScript>> ();
			results = new List<Tuple<string, MonoScript>> ();
			FetchAvailableScriptables ();

			var root = this.GetRootVisualContainer ();
			root.style.flex = 1;

			root.Add (new IMGUIContainer (DrawGUI) {
				style = { flex = 1 }
			});
		}

		private void DrawGUI () {
			// Draw the search bar
			using (var vertical = new GUILayout.VerticalScope ()) {
				using (var searchScope = new GUILayout.HorizontalScope ()) {
					EditorGUI.BeginChangeCheck ();
					searchString = EditorGUILayout.TextField ("Search:", searchString);
					if (EditorGUI.EndChangeCheck ()) {
						QueryScripts (searchString);
					}

					if (GUILayout.Button ("x", GUILayout.Width (20f))) {
						searchString = string.Empty;
						QueryScripts (searchString);
					}
				}
			}

			// Draw the results window
			EditorGUILayout.Separator ();

			using (var scrollScope = new EditorGUILayout.ScrollViewScope (scroll)) {
				for (var i = 0; i < results.Count; ++i) {
					var current = results[i];

					if (GUILayout.Button (current.item1)) {
						var obj = CreateInstance (current.item2.GetClass ());
						var path = EditorUtility.SaveFilePanelInProject ("Create Scriptable Object", current.item1, "asset", "Where to save?");

						if (!string.IsNullOrEmpty (path)) {
							AssetDatabase.CreateAsset (obj, path);
							AssetDatabase.SaveAssets ();

							EditorGUIUtility.PingObject (obj);
						}
					}
				}

				scroll = scrollScope.scrollPosition;
			}
		}

		private void FetchAvailableScriptables () {
			var guids = AssetDatabase.FindAssets ("t:Monoscript");

			foreach (var guid in guids) {
				var path = AssetDatabase.GUIDToAssetPath (guid);
				var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript> (path);

				if (monoScript != null && monoScript.GetClass () != null) {
					var t = monoScript.GetClass ();

					if (t.IsSubclassOf (typeof (ScriptableObject)) && !t.IsAbstract) {
						availableScripts.Add (new Tuple<string, MonoScript> (t.Name, monoScript));
					}
				}
			}

			availableScripts.Sort ((a, b) => { return EditorUtility.NaturalCompare (a.item1, b.item1); });
			results.AddRange (availableScripts);
		}

		private void QueryScripts (string query) {
			results.Clear ();

			var normalizedQuerty = query.ToLower ();
			var currentResults = availableScripts.FindAll (a => a.item1.ToLower ().Contains (normalizedQuerty));
			results.AddRange (currentResults);
		}
	}
}