using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OneGame {
	[RequireComponent (typeof (TMP_InputField))]
	public class TabAdjuster : MonoBehaviour {
		[SerializeField]
		private int tabSize;

		private TMP_InputField inputField;

		private void Awake () {
			inputField = GetComponent<TMP_InputField> ();

			var tab = string.Empty;
			for (var i = 0; i < tabSize; ++i) {
				tab += " ";
			}

			inputField.onValueChanged.AddListener ((s) => {
				inputField.text = s.Replace ("\t", tab);
			});
		}
	}
}