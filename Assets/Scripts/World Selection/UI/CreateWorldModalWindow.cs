using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace OneGame {
	/// <summary>
	/// A modal window that opens when the player wants to create a new world
	/// </summary>
	public class CreateWorldModalWindow : MonoBehaviour {

		[SerializeField]
		private SaveManager saveManager;
		[SerializeField]
		private GameEventTable eventTable;

		[SerializeField]
		private TMP_InputField nameField;
		[SerializeField]
		private TMP_InputField descriptionField;

		private CanvasGroup group;

		private void Start () {
			group = GetComponent<CanvasGroup> ();
			Close ();
		}

		private void OnEnable () {
			if (eventTable != null) {
				eventTable.Register ("OnWorldCreateButtonClicked", Open);
			}
		}

		private void OnDisable () {
			if (eventTable != null) {
				eventTable.Unregister ("OnWorldCreateButtonClicked", Open);
			}
		}

		/// <summary>
		/// Closes the window
		/// </summary>
		public void Close () {
			TweenWindow (0f);
		}

		/// <summary>
		/// Commits the initial world values into a new world save
		/// </summary>
		public void CommitWorldCreation () {
			var uniqueId = EntityUtilities.GetUniqueId ();
			var name = nameField.text;
			var description = descriptionField.text;
			var save = saveManager[uniqueId, JsonSaveType.File];

			SaveExtensions.SetStringValue (ref save, "world-name", name);
			SaveExtensions.SetStringValue (ref save, "world-description", description);
			SaveExtensions.SetStringValue (ref save, "world-id", uniqueId);

			saveManager[uniqueId, JsonSaveType.File] = save;
			saveManager.SaveWorld (uniqueId, JsonSaveType.File);

			// Broadcast the world creation to other scripts
			eventTable?.Invoke<string> ("OnWorldCreate", uniqueId);

			Close ();
		}

		/// <summary>
		/// Opens the window
		/// </summary>
		public void Open () {
			nameField.text = string.Empty;
			descriptionField.text = string.Empty;

			TweenWindow (1f);
		}

		/// <summary>
		/// Tweens the window's alpha value
		/// </summary>
		private void TweenWindow (float targetAlpha) {
			LeanTween.cancel (gameObject);
			LeanTween.alphaCanvas (group, targetAlpha, 0.2f)
				.setEaseOutCubic ()
				.setOnComplete (() => {
					group.interactable = targetAlpha > 0.5f;
					group.blocksRaycasts = targetAlpha > 0.5f;
				});
		}
	}
}