//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class CharacterAnimation : MonoBehaviour {

	private Animator animator;
	public Transform upperSpine;
	public Transform headCamera;
	public Quaternion CameraRotation;
	private CharacterSystem character;
	
	// *************************
	// For legacy animation to rotation upper part along with camera.
	
	void Start () {
		animator = this.GetComponent<Animator>();
		character = this.GetComponent<CharacterSystem>();
		if(headCamera == null){
			FPSCamera fpscam = this.GetComponentInChildren<FPSCamera>();
			headCamera = fpscam.gameObject.transform;
		}
	}

	void Update () {
		if(animator == null || character == null)
			return;
		
		// this is for legacy animation
		// if you using Mecanim in unity Pro, 
		//if(headCamera)
			// you can use animator.SetLookAtPosition (headCamera.transform.forward * 10) instead.
		
		if (upperSpine) {
				// get rotation from Upper Spin
			CameraRotation = upperSpine.localRotation;
			CameraRotation.eulerAngles = new Vector3(upperSpine.localRotation.eulerAngles.x,upperSpine.localRotation.eulerAngles.y,-headCamera.transform.rotation.eulerAngles.x);
			
			// rotation Upper spin along with camera angle
			upperSpine.transform.localRotation = CameraRotation;
			// update animation transform
			if(animator.GetComponent<Animation>() && animator.GetComponent<Animation>()[animator.GetComponent<Animation>().clip.name])
				animator.GetComponent<Animation>()[animator.GetComponent<Animation>().clip.name].AddMixingTransform(upperSpine);

		}
	}
}
