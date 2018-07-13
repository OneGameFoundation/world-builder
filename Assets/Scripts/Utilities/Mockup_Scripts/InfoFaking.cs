using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InfoFaking : MonoBehaviour {

	public string[] IDList;
	public string[] AreaList;

	public string[] DescriptionList;
	public string[] NameList;

	public Text IdField;
	public Text AreaField;
	public Text DescriptionField;
	public Text NameField;

	public CanvasGroup window;

	private int currentIndex = 0;

	private void Start () {
		IdField.text = IDList[0];
		AreaField.text = AreaList[0];
		DescriptionField.text = DescriptionList[0];
		NameField.text = NameList[0];
	}
	public void SwitchInfo (int index) {
		IdField.text = IDList[index];
		AreaField.text = AreaList[index];
		DescriptionField.text = DescriptionList[index];
		NameField.text = NameList[index];

		if (index != currentIndex) {
			LeanTween.value (window.gameObject, 1f, 0f, 0.3f).setOnUpdate ((float value) => {
				window.alpha = value;
			}).setLoopPingPong (1);
		}

		currentIndex = index;
	}
}