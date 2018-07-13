using System.Collections;
using OneGame.TPC;
using UnityEngine;

namespace OneGame {
	/// <summary>
	/// Describes the current mode of the world
	/// </summary>
	public enum WorldMode { Build, Play }
	public class WorldManager : MonoBehaviour {
		/// <summary>
		/// The current world mode
		/// </summary>
		public WorldMode CurrentMode {
			get { return currentMode; }
			set { SetCurrentMode (value); }
		}

		[SerializeField, Space]
		private WorldMode initialWorldMode;

		[SerializeField, Space]
		private CameraFollower cameraFollower;
		[SerializeField]
		private PlayerMotor motor;

		[SerializeField, Header ("Managers")]
		private ScriptManager scriptManager;
		[SerializeField]
		private EntityGenerator entityGenerator;
		[SerializeField]
		private SceneLoader sceneLoader;

		[SerializeField, Space]
		private GameEventTable eventTable;

		private WorldMode currentMode;
		private bool isInit = false;

		private void OnEnable () {
			if (eventTable != null) {
				eventTable.Register ("OnPlayButtonClicked", ActivatePlayMode);
				eventTable.Register ("OnEditorModeButtonClicked", ActivateBuildMode);
				eventTable.Register ("OnPauseMenuClosed", HandlePauseMenuCloseEvent);
				eventTable.Register ("OnQuitButtonClicked", HandleQuitEvent);
			}

			if (sceneLoader != null) {
				sceneLoader.OnSceneLoadComplete += ActivateBuildModeDelayed;
			}
		}

		private void OnDisable () {
			if (eventTable != null) {
				eventTable.Unregister ("OnPlayButtonClicked", ActivatePlayMode);
				eventTable.Unregister ("OnEditorModeButtonClicked", ActivateBuildMode);
				eventTable.Unregister ("OnPauseMenuClosed", HandlePauseMenuCloseEvent);
				eventTable.Unregister ("OnQuitButtonClicked", HandleQuitEvent);
			}

			if (sceneLoader != null) {
				sceneLoader.OnSceneLoadComplete -= ActivateBuildModeDelayed;
			}
		}

		private void Update () {
			// Debug feature: open the editor menu panel
			// if the player presses escape
			if (currentMode == WorldMode.Play) {
				if (Input.GetKeyDown (KeyCode.Escape)) {
					eventTable?.Invoke ("OnPauseMenuOpen");
					SetMouseVisibilityState (true);
				}
			}
		}

		public void ActivateBuildMode () {
			SetCurrentMode (WorldMode.Build);
		}

		public void ActivatePlayMode () {
			SetCurrentMode (WorldMode.Play);

			var scriptRunners = scriptManager.Runners;
			foreach (var runner in scriptRunners) {
				runner.InvokeLuaMethod ("start");
			}
		}

		/// <summary>
		/// Sets the current world mode
		/// </summary>
		public void SetCurrentMode (WorldMode mode) {
			// Invoke the event to notify other scripts that the world mode is changing
			eventTable?.Invoke<WorldMode, WorldMode> ("OnWorldModeTransition", currentMode, mode);

			currentMode = mode;

			motor.CanMove = mode == WorldMode.Play;
			cameraFollower.CanFollowTarget = mode == WorldMode.Play;
			scriptManager.AllowScriptRunning = mode == WorldMode.Play;
			SetMouseVisibilityState (mode == WorldMode.Build);

			// Invoke the event to notify other scripts that the world mode has changed
			eventTable?.Invoke<WorldMode> ("OnWorldModeChange", mode);
		}

		private void ActivateBuildModeDelayed () {
			if (!isInit) {
				isInit = true;
				CancelInvoke ();
				Invoke ("ActivateBuildMode", 0.05f);
			}
		}

		/// <summary>
		/// Delegate used to handle the event where the player closes the pause menu
		/// </summary>
		private void HandlePauseMenuCloseEvent () {
			SetMouseVisibilityState (false);
		}

		/// <summary>
		/// Delegate used to handle the event where the player quits the game
		/// </summary>
		private void HandleQuitEvent () {
			entityGenerator.Clear ();
		}

		/// <summary>
		/// Controls whether the mouse should stay hidden and locked
		/// </summary>
		/// <param name="visible"></param>
		private void SetMouseVisibilityState (bool visible) {
			Cursor.visible = visible;
			Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
		}

	}
}