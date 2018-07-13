using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame.TPC {
	/// <summary>
	/// Handles following a target in a smooth manner
	/// </summary>
	public class CameraFollower : MonoBehaviour {
		/// <summary>
		/// Should this camera follow a target?
		/// </summary>
		public bool CanFollowTarget { get { return canFollowTarget; } set { canFollowTarget = value; } }

		/// <summary>
		/// The target that this camera is following
		/// </summary>
		public Transform Target {
			get { return target; }
			set {
				target = value;
				motor = value.GetComponent<PlayerMotor> ();
			}
		}

		[SerializeField]
		private bool canFollowTarget;
		[SerializeField]
		private Transform target;

		[SerializeField, Header ("Positioning")]
		private Vector3 pivotPoint;
		[SerializeField]
		private float maxZDistance = 1f;

		[SerializeField, Header ("Camera Rotations")]
		private Vector2 verticalCameraRange = new Vector2 (-60f, 60f);
		[SerializeField]
		private Vector2 lookSensitivity = new Vector2 (1f, 1f);
		[SerializeField]
		private string xCameraAxis = "Mouse X";
		[SerializeField]
		private string yCameraAxis = "Mouse Y";

		private Vector3 cameraOrbit;
		private PlayerMotor motor;

		private void Start () {
			motor = target.GetComponent<PlayerMotor> ();
		}

		private void LateUpdate () {
			if (canFollowTarget) {
				// var pivot = target.position + pivotPoint;
				var pivot = target.localToWorldMatrix.MultiplyPoint3x4 (pivotPoint);

				cameraOrbit.x = Mathf.Clamp (cameraOrbit.x + Input.GetAxis (yCameraAxis) * lookSensitivity.y,
					verticalCameraRange.x,
					verticalCameraRange.y);
				cameraOrbit.y += Input.GetAxis (xCameraAxis) * lookSensitivity.x;

				// Move the camera to the orbit position
				transform.position = pivot + (Quaternion.Euler (cameraOrbit) * Vector3.forward * maxZDistance);

				// Force the camera to look at an imaginary point
				transform.LookAt (pivot);

				// Update the target look position on the motor
				motor.TargetLookPosition = transform.position + transform.forward * 10f;
			}
		}
	}
}