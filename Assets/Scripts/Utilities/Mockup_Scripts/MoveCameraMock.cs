using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraMock : MonoBehaviour {

	public GameObject[] CameraPosList;
	public float time = 0.3f;

	public void MoveCam (int index) {
		LeanTween.cancel (gameObject);
		var destination = CameraPosList[index].transform.position;
		LeanTween.move (gameObject, destination, time).setEaseOutSine ();
	}
}