//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public static class MouseLock
{
	private static bool mouseLocked;

	
	public static bool MouseLocked {
		get {
			return mouseLocked;
		}
		set {
			mouseLocked = value;
			
			#if !UNITY_5_4_OR_NEWER
				Screen.lockCursor = value;
			#else
				Cursor.visible = !value;
				if (Cursor.visible) {	
					Cursor.lockState = CursorLockMode.None;
				} else {
					Cursor.lockState = CursorLockMode.Locked;
			
				}
			#endif
		}
	}
	

}

