using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneGame.UI {
	/// <summary>
	/// A splash screen that stalls the game until all the necesary assets have been loaded
	/// </summary>
	public class SplashScreenBuilder : MonoBehaviour {

		[SerializeField, Header ("Loading Effects")]
		private Transform loadingIconTransform;
		[SerializeField]
		private float rotateSpeed = 60f;
		[SerializeField]
		private GameEventTable eventTable;

		private void OnEnable () {
			eventTable.Register ("OnSaveLoaded", DisableSplash);
		}

		private void OnDisable () {
			eventTable.Unregister ("OnSaveLoaded", DisableSplash);
		}

		private void LateUpdate () {
			var rotation = loadingIconTransform.localEulerAngles;
			rotation.z += Time.deltaTime * rotateSpeed;
			loadingIconTransform.localEulerAngles = rotation;
		}

		/// <summary>
		/// Disable Spash screen after everything is loaded
		/// </summary>
		private void DisableSplash () {
			gameObject.SetActive (false);
		}

	}
}