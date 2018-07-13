using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using OneGame.Lua;
using TMPro;
using UnityEngine;

namespace OneGame.UI {
	public class ScriptEditor : MonoBehaviour {

		[SerializeField]
		private GameEventTable eventTable;

		[SerializeField]
		private MicroGameEngine engine;
		[SerializeField]
		private GameObject errorMessage;

		private CanvasGroup group;
		private TMP_InputField input;

		private IEntityContainer entity;
		private LuaScript script;

		private void Awake () {
			group = GetComponent<CanvasGroup> ();
			input = GetComponentInChildren<TMP_InputField> ();
			Close ();
		}

		private void OnEnable () {
			errorMessage.SetActive (false);
			if (eventTable != null) {
				eventTable.Register<IEntityContainer, LuaScript> ("OnScriptEditorOpen", HandleEditScriptEvent);
				eventTable.Register ("OnScriptEditorClose", Close);
				eventTable.Register ("OnEntityDeselect", Close);
				eventTable.Register<LuaScript> ("OnGameManagerOpen", HandleEditGameManagerEvent);
			}
		}

		private void OnDisable () {
			if (eventTable != null) {
				eventTable.Unregister<IEntityContainer, LuaScript> ("OnScriptEditorOpen", HandleEditScriptEvent);
				eventTable.Unregister ("OnScriptEditorClose", Close);
				eventTable.Unregister ("OnEntityDeselect", Close);
				eventTable.Unregister<LuaScript> ("OnGameManagerOpen", HandleEditGameManagerEvent);
			}
		}

		public void ApplyScript () {
			var isScriptValid = false;

			try {
				var table = script.script.Globals;
				var dynValue = new List<DynValue> ();

				foreach (var key in table.Keys) {
					var value = table.Get (key);
					var dataType = value.Type;
					if (dataType == DataType.String || dataType == DataType.Boolean || dataType == DataType.Number) {
						dynValue.Add (key);
					}
				}

				foreach (var thing in dynValue) {
					table.Remove (thing);
				}

				script.script.DoString (input.text);
				errorMessage.SetActive (false);
				isScriptValid = true;
			} catch {
				Debug.LogWarning ("Error on script!  Will not apply");
				errorMessage.SetActive (true);
				errorMessage.GetComponent<Animator> ().Play ("Error Msg Shake");
				isScriptValid = false;
			}

			if (isScriptValid) {
				var candidate = script.CreateCopy ();
				candidate.code = input.text;
				candidate.properties = ScriptUtility.ExtractProperties (script.script);
				entity.Script = candidate;
				eventTable.Invoke<IEntityContainer> ("OnScriptApply", entity);
			}
		}

		public void CheckScriptViability () {
			try {
				script.script.DoString (input.text);
				errorMessage.SetActive (false);
			} catch {
				Debug.LogWarning ("Error on script!  Will not apply");
				errorMessage.SetActive (true);
				errorMessage.GetComponent<Animator> ().Play ("Error Msg Shake");
			}
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
			CheckScriptViability ();
		}

		private void HandleEditScriptEvent (IEntityContainer entity, LuaScript script) {
			this.entity = entity;
			this.script = script;

			input.text = script.code;
			Open ();
		}

		private void HandleEditGameManagerEvent (LuaScript script) {
			//this.entity = entity;
			this.script = script;

			input.text = script.code;
			Open ();
		}

		public void EditGameManager () {
			entity = engine;
			eventTable.Invoke<LuaScript> ("OnGameManagerOpen", engine.Script);
		}

	}
}