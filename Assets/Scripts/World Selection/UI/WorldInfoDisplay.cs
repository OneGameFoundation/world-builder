using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OneGame {
	public class WorldInfoDisplay : MonoBehaviour {
		[SerializeField]
		private SaveManager saveMananager;
		[SerializeField]
		private GameEventTable eventTable;

		[SerializeField]
		private TMP_Text titleText;
		[SerializeField]
		private TMP_Text idText;
		[SerializeField]
		private TMP_Text descriptionText;

		private CanvasGroup group;

		private void Awake () {
			group = GetComponent<CanvasGroup> ();
		}

		private void Start () {
			Close ();
		}

		private void OnEnable () {
			if (eventTable != null) {
				eventTable.Register<string> ("OnWorldClick", DisplayWorldStats);
				eventTable.Register<string> ("OnWorldCreate", DisplayWorldStats);
			}
		}

		private void OnDisable () {
			if (eventTable != null) {
				eventTable.Unregister<string> ("OnWorldClick", DisplayWorldStats);
				eventTable.Unregister<string> ("OnWorldCreate", DisplayWorldStats);
			}
		}

		public void Close () {
			LeanTween.cancel (gameObject);
			LeanTween.alphaCanvas (group, 0f, 0.2f);
		}

		public void DisplayWorldStats (string id) {
			var save = saveMananager[id, JsonSaveType.File];

			titleText.text = save.GetStringValue ("world-name", string.Empty);
			idText.text = save.GetStringValue ("world-id", string.Empty);
			descriptionText.text = save.GetStringValue ("world-description", string.Empty);

			Open ();
		}

		public void Open () {
			LeanTween.cancel (gameObject);
			LeanTween.alphaCanvas (group, 1f, 0.2f);
		}

	}
}