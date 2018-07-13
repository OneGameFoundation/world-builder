using OneGame.Lua;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OneGame {
	using Input = UnityEngine.Input;
	public class EntityManipulator : MonoBehaviour {
		[SerializeField, Header ("Tool Options")]
		private LayerMask toolMask;
		[SerializeField]
		private float minScale = 0.5f;
		[SerializeField]
		private float maxScale = 3f;
		[SerializeField]
		private float scaleStep = 0.25f;

		[SerializeField, Space]
		private GameObject circleSelectionPrefab;
		[SerializeField]
		private GameEventTable eventTable;
		[SerializeField]
		private EntityGenerator generator;

		/// <summary>
		/// Enum describing what tool state the manipulator is in
		/// </summary>
		public enum ToolType { None, Move, Rotate }

		private SelectedCircle circle;
		private IEntityContainer currentEntity;
		private ToolType currentTool;

		private void Start () {
			currentTool = ToolType.None;

			var clone = Instantiate (circleSelectionPrefab, transform);
			circle = clone.GetComponent<SelectedCircle> ();
		}

		private void OnEnable () {
			if (eventTable != null) {
				eventTable.Register ("OnMoveButtonClicked", ActivateMoveTool);
				eventTable.Register ("OnRotateButtonClicked", ActiveRotateTool);
				eventTable.Register ("OnScaleUpButtonClicked", ScaleSelectedEntityUp);
				eventTable.Register ("OnScaleDownButtonClicked", ScaleSelectedEntityDown);
				eventTable.Register ("OnDeleteButtonClicked", DeleteEntity);
			}
		}

		private void OnDisable () {
			if (eventTable != null) {
				eventTable.Unregister ("OnMoveButtonClicked", ActivateMoveTool);
				eventTable.Unregister ("OnRotateButtonClicked", ActiveRotateTool);
				eventTable.Unregister ("OnScaleUpButtonClicked", ScaleSelectedEntityUp);
				eventTable.Unregister ("OnScaleDownButtonClicked", ScaleSelectedEntityDown);
				eventTable.Unregister ("OnDeleteButtonClicked", DeleteEntity);
			}
		}

		private void Update () {
			// Check if the player has clicked on an entity
			switch (currentTool) {
				case ToolType.Move:
					MoveSelectedEntity ();
					break;

				case ToolType.Rotate:
					RotateSelectedEntity ();
					break;

				default:
					if (!EventSystem.current.IsPointerOverGameObject (-1) && Input.GetMouseButtonDown (0)) {
						RaycastHit hit;

						if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit)) {
							var entity = hit.transform.GetComponentInParent<IEntityContainer> ();

							if (entity != null) {
								// Select the entity
								SelectEntity (entity);
							}
						}
					}

					if (currentEntity != null && (Input.GetMouseButtonDown (1) || Input.GetKeyDown (KeyCode.Escape))) {
						UnselectEntity ();
					}
					break;
			}
		}

		/// <summary>
		/// Selects an active entity
		/// </summary>
		/// <param name="entity">The entity to select</param>
		public void SelectEntity (IEntityContainer entity) {
			currentEntity = entity;

			circle.Activate (entity.Transform.position, GetBounds (entity.Transform));

			// Throw an event
			eventTable?.Invoke<IEntityContainer> ("OnEntitySelect", entity);
		}

		/// <summary>
		/// Deselects the current entity choice
		/// </summary>
		public void UnselectEntity () {
			currentEntity = null;
			circle.Deactivate ();
			ActivateTool (ToolType.None);

			eventTable?.Invoke ("OnEntityDeselect");
		}

		/// <summary>
		/// Activates the move tool
		/// </summary>
		private void ActivateMoveTool () {
			ActivateTool (ToolType.Move);
		}

		/// <summary>
		/// Activates the rotate tool
		/// </summary>
		private void ActiveRotateTool () {
			ActivateTool (ToolType.Rotate);
		}

		/// <summary>
		/// Attempts to activate a tool for the player to use
		/// </summary>
		/// <param name="tool">The tool to use</param>
		private void ActivateTool (ToolType tool) {
			if (currentEntity != null || tool == ToolType.None) {
				currentTool = tool;
			}
		}

		private void DeleteEntity () {
			generator.DeleteEntity (currentEntity as EntityGameObject);
			circle.Deactivate ();
		}

		/// <summary>
		/// Moves the current entity to the area underneath the mouse position
		/// </summary>
		private void MoveSelectedEntity () {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (!EventSystem.current.IsPointerOverGameObject (-1) && Physics.Raycast (ray, out hit, Mathf.Infinity, toolMask)) {
				if (hit.collider.CompareTag ("Ground")) {
					currentEntity.Transform.position = hit.point;
					circle.transform.position = hit.point;
				}
			}

			if (Input.GetMouseButtonDown (0)) {
				ActivateTool (ToolType.None);
			}
		}

		/// <summary>
		/// Rotates the current entity towards the mouse position
		/// </summary>
		private void RotateSelectedEntity () {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (!EventSystem.current.IsPointerOverGameObject (-1) && Physics.Raycast (ray, out hit, Mathf.Infinity, toolMask)) {
				if (hit.collider.CompareTag ("Ground")) {
					var diff = hit.point - currentEntity.Transform.position;
					diff.y = 0;
					currentEntity.Transform.rotation = Quaternion.LookRotation (diff);
				}
			}

			if (Input.GetMouseButtonDown (0)) {
				ActivateTool (ToolType.None);
			}
		}

		/// <summary>
		/// Scales the selected entity downwards
		/// </summary>
		private void ScaleSelectedEntityDown () {
			if (currentEntity != null) {
				var transform = currentEntity.Transform;
				var scale = transform.localScale;

				scale.x = Mathf.Clamp (scale.x - scaleStep, minScale, maxScale);
				scale.y = Mathf.Clamp (scale.y - scaleStep, minScale, maxScale);
				scale.z = Mathf.Clamp (scale.z - scaleStep, minScale, maxScale);

				transform.localScale = scale;
			}
		}

		/// <summary>
		/// Enlargens the selected entity
		/// </summary>
		private void ScaleSelectedEntityUp () {
			if (currentEntity != null) {
				var transform = currentEntity.Transform;
				var scale = transform.localScale;

				scale.x = Mathf.Clamp (scale.x + scaleStep, minScale, maxScale);
				scale.y = Mathf.Clamp (scale.y + scaleStep, minScale, maxScale);
				scale.z = Mathf.Clamp (scale.z + scaleStep, minScale, maxScale);

				transform.localScale = scale;
			}
		}

		/// <summary>
		/// Finds the bounds of a given transform
		/// </summary>
		private Bounds GetBounds (Transform transform) {
			var bounds = new Bounds (transform.position, Vector3.zero);
			var renderers = transform.GetComponentsInChildren<Renderer> ();

			for (var i = 0; i < renderers.Length; ++i) {
				bounds.Encapsulate (renderers[i].bounds);
			}

			return bounds;
		}
	}
}