using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueDisplay : MonoBehaviour {

	public bool useTextMeshPro = false;
	public Slider valueOriginSlider;

	private Text textObj;
	private TextMeshProUGUI tmpObj;

	public void ChangeValue () {
		if (useTextMeshPro) {
			tmpObj = GetComponent<TextMeshProUGUI> ();
			tmpObj.text = valueOriginSlider.value.ToString ();
		} else {
			textObj = GetComponent<Text> ();
			textObj.text = valueOriginSlider.value.ToString ();
		}
	}

}