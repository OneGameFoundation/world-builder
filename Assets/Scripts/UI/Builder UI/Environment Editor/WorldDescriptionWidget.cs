using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace OneGame {
	/// <summary>
	/// A small utility UI script that allows the player to edit the current
	/// world descriptions
	/// </summary>
	public class WorldDescriptionWidget : MonoBehaviour {
		[SerializeField]
		private SaveManager saveManager;
		[SerializeField]
		private GameEventTable eventTable;

		[SerializeField, Space]
		private TMP_InputField nameField;
		[SerializeField]
		private TMP_InputField descriptionField;

		private void Start () {
			var save = saveManager[saveManager.ActiveSaveId, JsonSaveType.File];

			nameField.text = save.GetStringValue ("world-name", string.Empty);
			descriptionField.text = save.GetStringValue ("world-description", string.Empty);
		}

		private void OnEnable () {
			if (nameField != null) {
				nameField.onEndEdit.AddListener (HandleNameChangeEvent);
			}

			if (descriptionField != null) {
				descriptionField.onEndEdit.AddListener (HandleDescriptionChangeEvent);
			}
		}

		private void OnDisable () {
			if (nameField != null) {
				nameField.onEndEdit.RemoveListener (HandleNameChangeEvent);
			}

			if (descriptionField != null) {
				descriptionField.onEndEdit.RemoveListener (HandleDescriptionChangeEvent);
			}
		}

		/// <summary>
		/// Delegate calle when the player finishes typing the description
		/// </summary>
		/// <param name="description">The new description</param>
		private void HandleDescriptionChangeEvent (string description) {
			eventTable?.Invoke<string> ("OnWorldDescriptionChange", description);
		}

		/// <summary>
		/// Delegate called when the player finishes changing the name
		/// </summary>
		/// <param name="name">The new name</param>
		private void HandleNameChangeEvent (string name) {
			eventTable?.Invoke<string> ("OnWorldNameChange", name);
		}
	}
}