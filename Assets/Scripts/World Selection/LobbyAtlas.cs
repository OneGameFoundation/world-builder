using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneGame {
	using Random = System.Random;

	/// <summary>
	/// Handles the procedural generation of the current altas of player worlds
	/// </summary>
	public class LobbyAtlas : MonoBehaviour {
		[SerializeField]
		private string seed;
		[SerializeField]
		private SaveManager saveManager;
		[SerializeField]
		private GameEventTable eventTable;
		[SerializeField]
		private Transform viewCamera;
		[SerializeField]
		private Vector3 cameraOffset;
		[SerializeField]
		private Material[] materials;

		[SerializeField, Header ("Atlas Settings")]
		private Vector3 origin;
		[SerializeField]
		private float worldSize;
		[SerializeField]
		private Vector2 sizeRange;
		[SerializeField]
		private float height;
		[SerializeField]
		private float cameraTweenTime = 0.5f;

		private Random rng;
		private Dictionary<string, GameObject> worlds;

		private void Start () {
			worlds = new Dictionary<string, GameObject> ();
			rng = new Random (seed.GetHashCode ());

			var ids = saveManager.WorldIds;
			CreateAtlas (ids);

			if (ids.Length > 0) {
				eventTable?.Invoke<string> ("OnWorldClick", ids[0]);
			}
		}

		private void OnEnable () {
			if (eventTable != null) {
				eventTable.Register<string> ("OnWorldClick", FocusCamera);
				eventTable.Register<string> ("OnWorldCreate", GenerateWorld);
				eventTable.Register<string> ("OnWorldCreate", FocusCamera);
			}
		}

		private void OnDisable () {
			if (eventTable != null) {
				eventTable.Unregister<string> ("OnWorldClick", FocusCamera);
				eventTable.Unregister<string> ("OnWorldCreate", GenerateWorld);
				eventTable.Unregister<string> ("OnWorldCreate", FocusCamera);
			}
		}

		/// <summary>
		/// Focuses the camera on a targeted world through a tween effect
		/// </summary>
		/// <param name="id">the id of the world</param>
		public void FocusCamera (string id) {
			GameObject gameObject;

			if (worlds.TryGetValue (id, out gameObject)) {
				LeanTween.cancel (viewCamera.gameObject);
				LeanTween.move (viewCamera.gameObject,
						gameObject.transform.position + cameraOffset,
						cameraTweenTime)
					.setEaseOutCubic ();
			}
		}

		/// <summary>
		/// Procedurally generates an atlas with the given world ids
		/// </summary>
		/// <param name="atlasIds">The ids of the player's worlds</param>
		private void CreateAtlas (string[] atlasIds) {
			for (var i = 0; i < atlasIds.Length; ++i) {
				GenerateWorld (atlasIds[i]);
			}
		}

		private void GenerateWorld (string id) {
			var clone = GameObject.CreatePrimitive (PrimitiveType.Cube);

			// Assign the world position
			var offset = new Vector3 (
				(float) (worldSize * rng.NextDouble ()) - worldSize * 0.5f,
				0f,
				(float) (worldSize * rng.NextDouble ()) - worldSize * 0.5f
			);
			clone.transform.position = origin + offset;

			// Assign the width and height
			var width = sizeRange.x + ((sizeRange.y - sizeRange.x) * rng.NextDouble ());
			clone.transform.localScale = new Vector3 ((float) width, height, (float) width);

			// Assign the color
			var renderer = clone.GetComponent<Renderer> ();
			renderer.sharedMaterial = materials[rng.Next (0, materials.Length)];

			// Set the layer
			clone.layer = 9;

			// Move the clone to the current scene
			SceneManager.MoveGameObjectToScene (clone, gameObject.scene);
			worlds.Add (id, clone);
		}
	}
}