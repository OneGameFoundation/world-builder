using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OneGame {
	/// <summary>
	/// A camera controller that pans the camera, much like a real-time strategy game
	/// </summary>
	public class RTSCameraController : MonoBehaviour, ICameraController {
		[SerializeField]
		private Transform cameraTransform;
		[SerializeField]
		private Transform centerTarget;
		[SerializeField]
		private float initialTargetDistance = 15f;

		[SerializeField, Header ("Camera Panning")]
		private float panningBoundary = 50f;
		[SerializeField]
		private float panSpeed = 2f;
		[SerializeField]
		private float maxSpeedScale = 1.5f;

		[SerializeField, Header ("Zooming")]
		private float defaultFov = 60f;
		[SerializeField]
		private Vector2 fovRange = new Vector2 (5f, 65f);
		[SerializeField]
		private float zoomSensitivity = 10f;

		[SerializeField, Header ("Rotation")]
		private float maxRotationSpeed = 10f;
		[SerializeField]
		private float rotationSensitivity = 1f;
		[SerializeField]
		private Vector2 cameraMaxRange = new Vector2 (3000f, 3000f);

		public bool IsResetting { get { return LeanTween.isTweening (cameraTransform.gameObject); } }

		private Rect ignorePanningRect;
		private Quaternion initialCameraRotation;
		private Vector3 initialForward;
		private float fov;
		private Vector3 cameraPosHolder;
		private float panTimer;

		private void Start () {
			ignorePanningRect = new Rect ();
			ignorePanningRect.xMin = panningBoundary;
			ignorePanningRect.xMax = Screen.width - panningBoundary;
			ignorePanningRect.yMin = panningBoundary;
			ignorePanningRect.yMax = Screen.height - panningBoundary;

			initialCameraRotation = cameraTransform.rotation;
			initialForward = cameraTransform.forward;
			fov = Camera.main.fieldOfView;
		}

		/// <summary>
		/// Moves the camera on the x and z axis, as long as the mouse is near the edges of the screen
		/// </summary>
		/// <param name="mousePosition">The position of the player mouse</param>
		public void PanCamera (Vector2 mousePosition) {
			if (!ignorePanningRect.Contains (mousePosition)) {
				var delta = (mousePosition - ignorePanningRect.center).normalized;
				var d = mousePosition - ignorePanningRect.center;

				//calculate how much the cursor is outside of the screen
				float into;
				var absX = Mathf.Abs (d.x);
				var absY = Mathf.Abs (d.y);
				if (absX > absY) {
					into = absX - ignorePanningRect.width / 2f;
				} else {
					into = absY - ignorePanningRect.height / 2f;
				}

				//scale up the speedScale regarding how long the cursor stops inside the 'pan area'
				float speedScale = 0f;
				panTimer += Time.deltaTime;
				speedScale = Mathf.Clamp (panTimer * 10f, 0f, maxSpeedScale * 10f);

				//if cursor is outside to a certain value, apply the calcularted result to overwrite the speedScale
				if (into > 10) {
					speedScale = into / 100f * maxSpeedScale;
				}

				//apply the calculation to the camera's transform
				var worldDelta = new Vector3 (
					delta.x * panSpeed * speedScale * Time.deltaTime * fov / 50,
					0,
					delta.y * panSpeed * speedScale * Time.deltaTime * fov / 50);
				cameraPosHolder = cameraTransform.position + Quaternion.Euler (0f, cameraTransform.eulerAngles.y, 0f) * worldDelta;

				//when camera reached the boundary, it stops
				cameraPosHolder.x = Mathf.Clamp (cameraPosHolder.x, -cameraMaxRange.x, cameraMaxRange.x);
				cameraPosHolder.z = Mathf.Clamp (cameraPosHolder.z, -cameraMaxRange.y, cameraMaxRange.y);
				cameraTransform.position = cameraPosHolder;

			} else {
				//reset the timer if cursor is outside of 'pan area'
				panTimer = 0f;
			}
		}

		/// <summary>
		/// use Unity's input axis to move the camera
		/// </summary>
		/// <param name="x">the amount to move on x axis</param>
		/// <param name="y">the amount to move on y axis</param>
		public void PanCameraAxis (float x, float y) {

			var worldDelta = new Vector3 (
				x * panSpeed * Time.deltaTime * fov / 50,
				0,
				y * panSpeed * Time.deltaTime * fov / 50);

			//when camera reached the boundary, it stops
			cameraPosHolder = cameraTransform.position + Quaternion.Euler (0f, cameraTransform.eulerAngles.y, 0f) * worldDelta;
			cameraPosHolder.x = Mathf.Clamp (cameraPosHolder.x, -cameraMaxRange.x, cameraMaxRange.x);
			cameraPosHolder.z = Mathf.Clamp (cameraPosHolder.z, -cameraMaxRange.y, cameraMaxRange.y);
			cameraTransform.position = cameraPosHolder;

		}

		/// <summary>
		/// Resets the camera's transform values
		/// </summary>
		public void ResetCamera () {
			var cameraObj = cameraTransform.gameObject;
			var camera = Camera.main;

			if (LeanTween.isTweening (cameraObj)) {
				LeanTween.cancel (cameraObj);
			}

			var targetPosition = centerTarget.position - initialForward * initialTargetDistance;

			LeanTween.move (cameraObj, targetPosition, 0.1f).setEaseInCirc ().setEaseOutCirc ();
			LeanTween.rotate (cameraObj, initialCameraRotation.eulerAngles, 0.1f).setEaseInCirc ().setEaseOutElastic ();
			LeanTween.value (camera.fieldOfView, defaultFov, 0.1f)
				.setEaseInCirc ()
				.setEaseOutElastic ()
				.setOnUpdate (f => camera.fieldOfView = f);
		}

		/// <summary>
		/// Resets the camera's transform values instantly
		/// </summary>
		public void ResetCameraInstant () {
			var cameraObj = cameraTransform.gameObject;
			var camera = Camera.main;

			if (LeanTween.isTweening (cameraObj)) {
				LeanTween.cancel (cameraObj);
			}

			var targetPosition = centerTarget.position - initialForward * initialTargetDistance;

			LeanTween.move (cameraObj, targetPosition, 0.01f).setEaseInCirc ().setEaseOutCirc ();
			LeanTween.rotate (cameraObj, initialCameraRotation.eulerAngles, 0.01f).setEaseInCirc ().setEaseOutElastic ();
			LeanTween.value (camera.fieldOfView, defaultFov, 0.01f)
				.setEaseInCirc ()
				.setEaseOutElastic ()
				.setOnUpdate (f => camera.fieldOfView = f);
		}

		/// <summary>
		/// Rotates the camera by a certain angle
		/// </summary>
		/// <param name="delta">THe amount to rotate by</param>
		public void RotateCamera (float delta) {
			var rotationAmount = Mathf.Clamp (delta * rotationSensitivity, -maxRotationSpeed, maxRotationSpeed);

			var camera = Camera.main;

			RaycastHit hit;
			var pivot = Vector3.zero;

			if (LeanTween.isTweening (gameObject)) {
				LeanTween.cancel (gameObject);
			}
			if (Physics.Raycast (camera.transform.position, camera.transform.forward, out hit, Mathf.Infinity, 1 << 8)) {
				pivot = hit.point;
				//Debug.LogWarningFormat ("raycasted! hit point is: {0}", hit.point);
			}

			cameraTransform.RotateAround (pivot, Vector3.up, rotationAmount);
		}

		/// <summary>
		/// Zooms the camera, based on the delta value
		/// </summary>
		public void ZoomCamera (float delta) {
			if (!EventSystem.current.IsPointerOverGameObject (-1)) {
				fov = Camera.main.fieldOfView;
				fov = Mathf.Clamp (fov + delta * zoomSensitivity, fovRange.x, fovRange.y);

				Camera.main.fieldOfView = fov;
			}

		}

	}
}