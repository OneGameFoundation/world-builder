using UnityEngine;

namespace OneGame {
    /// <summary>
    /// Handles placement interactions of entities in the world builder
    /// </summary>
    [RequireComponent (typeof (ICameraController))]
    public class BuilderCameraController : MonoBehaviour {
        [SerializeField]
        private GameEventTable eventTable;
        [SerializeField]
        private PanMode panMode = PanMode.RTSMouse;

        public enum PanMode {
            RTSMouse,
            WSAD
        }

        private ICameraController cameraController;

        private void Awake () {
            cameraController = GetComponent<ICameraController> ();
        }

        private void OnEnable () {
            if (eventTable != null) {
                eventTable.Register ("OnSaveLoaded", cameraController.ResetCameraInstant);
            }
        }

        private void OnDisable () {
            if (eventTable != null) {
                eventTable.Unregister ("OnSaveLoaded", cameraController.ResetCameraInstant);
            }
        }

        private void LateUpdate () {

            // Reset the camera if both mouse buttons are clicked OR the escape key is held
            if (Input.GetMouseButton (0) && Input.GetMouseButton (1) || Input.GetKeyDown (KeyCode.Escape)) {
                cameraController.ResetCamera ();
            }
            // Rotate the camera, if possible
            else if (Input.GetMouseButton (1) && !Input.GetMouseButton (0)) {
                cameraController.RotateCamera (Input.GetAxis ("Mouse X"));
            } else {
                // Pan the camera, if possible
                if (panMode == PanMode.RTSMouse) {
                    var mousePosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
                    cameraController.PanCamera (mousePosition);
                }

                // Apply zoom
                var scroll = Input.GetAxis ("Mouse ScrollWheel");
                cameraController.ZoomCamera (scroll);

            }

            if (panMode == PanMode.WSAD) {
                cameraController.PanCameraAxis (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
            }

        }
    }
}