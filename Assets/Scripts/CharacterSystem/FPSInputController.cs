//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FPSController))]

// You can change a controller here.
public class FPSInputController : MonoBehaviour
{
	public FPSController FPSmotor;
	
	void Start ()
	{
		FPSmotor = GetComponent<FPSController> ();	
		Application.targetFrameRate = 60;
	}

	void Awake ()
	{
	}

	void Update ()
	{
		if (FPSmotor == null || FPSmotor.character == null || !MouseLock.MouseLocked)
		{
			return;
		}

		// move input
		FPSmotor.Move (new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical")));
		// jump input
		//FPSmotor.Jump (Input.GetButton ("Jump"));

		// sprint input
		if (Input.GetKey (KeyCode.LeftShift)) {
			FPSmotor.Boost (1.4f);	
		}
		
		if (MouseLock.MouseLocked) {
			// aim input work only when mouse is locked
			FPSmotor.Aim (new Vector2 (Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y")));
		}
	}
}
