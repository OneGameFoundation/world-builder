using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OneGame {
	/// <summary>
	/// A window that displays the player's current worlds
	/// </summary>
	public class LobbySelectionWindow : MonoBehaviour {
		[SerializeField]
		private SaveManager saveManager;
		[SerializeField]
		private GameEventTable eventTable;

		[SerializeField]
		private RectTransform parent;
		[SerializeField]
		private GameObject selectionPrefab;
		[SerializeField]
		private Color selectedTabColor;
		[SerializeField]
		private Color unselectedColor;

		private Dictionary<string, GameObject> worldButtons;

		private IEnumerator Start () {
			var ids = saveManager.WorldIds;
			var saves = new SaveData[ids.Length];
			worldButtons = new Dictionary<string, GameObject> ();

			for (var i = 0; i < saves.Length; ++i) {
				saves[i] = saveManager[ids[i], JsonSaveType.File];
			}

			PopulateWorldButtons (ids, saves);

			yield return null;
			// Select the first one one the list
			saveManager.ActiveSaveId = ids[0];
			eventTable?.Invoke<string> ("OnWorldClick", ids[0]);
		}

		private void OnEnable () {
			if (eventTable != null) {
				eventTable.Register<string> ("OnWorldCreate", HandleWorldCreationEvent);
				eventTable.Register<string> ("OnWorldClick", HighLightTab);
			}
		}

		private void OnDisable () {
			if (eventTable != null) {
				eventTable.Unregister<string> ("OnWorldCreate", HandleWorldCreationEvent);
				eventTable.Unregister<string> ("OnWorldClick", HighLightTab);
			}
		}

		/// <summary>
		/// Creates a single clickable button that displays information
		/// when the players click
		/// </summary>
		/// <param name="id">The id of the world</param>
		/// <param name="save">The associated save file</param>
		private void GenerateWorldButton (string id, SaveData save) {
			var clone = Instantiate (selectionPrefab, parent);

			var text = clone.GetComponentInChildren<TMP_Text> ();
			if (text != null) {
				text.text = save.GetStringValue ("world-name", "World");
			}

			worldButtons.Add (id, clone);

			// Assign an event invokation to the button, if any
			var button = clone.GetComponentInChildren<Button> ();
			if (button != null) {
				button.onClick.AddListener (() => {
					saveManager.ActiveSaveId = id;
					eventTable?.Invoke<string> ("OnWorldClick", id);
				});
			}
		}

		/// <summary>
		/// Create a new world's save file and button
		/// </summary>
		/// <param name="id"></param>
		private void HandleWorldCreationEvent (string id) {
			var save = saveManager[id, JsonSaveType.File];

			GenerateWorldButton (id, save);
			saveManager.ActiveSaveId = id;
			HighLightTab (id);
		}

		/// <summary>
		///	Generates the UI with buttons for the player to select world with
		/// </summary>
		/// <param name="ids">The world ids</param>
		/// <param name="worldSaves">The save files related to the worlds</param>
		private void PopulateWorldButtons (string[] ids, SaveData[] worldSaves) {
			for (var i = 0; i < worldSaves.Length; ++i) {
				GenerateWorldButton (ids[i], worldSaves[i]);
			}
		}

		/// <summary>
		/// Highlight the selected tab and unhighlight other tabs
		/// </summary>
		/// <param name="id">The world ids</param>
		private void HighLightTab (string id) {
			foreach (var obj in worldButtons) {
				if (obj.Key == id) {
					obj.Value.GetComponent<Image> ().color = selectedTabColor;
				} else {
					obj.Value.GetComponent<Image> ().color = unselectedColor;
				}
			}
		}
	}
}