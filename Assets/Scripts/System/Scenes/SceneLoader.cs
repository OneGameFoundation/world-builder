using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneGame {
	/// <summary>
	/// A manager that handles loading groups of scenes
	/// </summary>
	[CreateAssetMenu (menuName = "Scriptable Objects/Managers/Scene Loader")]
	public class SceneLoader : ScriptableObject {
		/// <summary>
		/// Is the scene loader ready to load?
		/// </summary>
		public bool IsReadyToLoad {
			get {
				lock (lockObj) {
					return isReadyToLoad;
				}
			}
			private set {
				lock (lockObj) {
					isReadyToLoad = value;
				}
			}
		}

		/// <summary>
		/// Invoked when the scene loader begins loading the scenes
		/// </summary>
		public Action<string[]> OnSceneLoadStart;

		/// <summary>
		/// Invoked when the scene loading process is complete
		/// </summary>
		public Action OnSceneLoadComplete;

		private Stack<string[]> loadedScenes;
		private bool isReadyToLoad;

		private static object lockObj = new object ();

		private void OnEnable () {
			loadedScenes = new Stack<string[]> ();
			IsReadyToLoad = true;
		}

		/// <summary>
		/// Loads a new scene
		/// </summary>
		/// <param name="sceneName"></param>
		public void LoadScene (string sceneName) {
			LoadScenes (new string[] { sceneName });
		}

		/// <summary>
		/// Loads a set of new scenes
		/// </summary>
		/// <param name="scenesToLoad">The scenes to add</param>
		public void LoadScenes (string[] scenesToLoad) {
			if (IsReadyToLoad) {
				IsReadyToLoad = false;
				var loadedCount = 0;

				OnSceneLoadStart?.Invoke (scenesToLoad);
				PopLastSceneGroup ();

				for (var i = 0; i < scenesToLoad.Length; ++i) {
					var sceneName = scenesToLoad[i];

					var op = SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);
					op.completed += (a) => {
						loadedCount++;

						IsReadyToLoad = (loadedCount == scenesToLoad.Length);
						if (IsReadyToLoad) {
							OnSceneLoadComplete?.Invoke ();
						}
					};
				}

				loadedScenes.Push (scenesToLoad);
			}
		}

		/// <summary>
		/// Loads scenes that are not previously loaded into the game
		/// </summary>
		/// <param name="scenes">The scenes to bootstrap</param>
		internal void BootstrapScenes (string[] scenes) {
			if (IsReadyToLoad) {
				IsReadyToLoad = false;
				var loadedCount = 0;

				OnSceneLoadStart?.Invoke (scenes);

				for (var i = 0; i < scenes.Length; ++i) {
					var sceneName = scenes[i];

					if (!IsSceneLoaded (sceneName)) {
						var op = SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);
						op.completed += (a) => {
							loadedCount++;

							IsReadyToLoad = (loadedCount == scenes.Length);

							if (IsReadyToLoad) {
								OnSceneLoadComplete?.Invoke ();
							}
						};
					} else {
						loadedCount++;

						IsReadyToLoad = (loadedCount == scenes.Length);

						if (IsReadyToLoad) {
							OnSceneLoadComplete?.Invoke ();
						}
					}
				}

				loadedScenes.Push (scenes);
			}
		}

		/// <summary>
		/// Check if particular scene is already loaded
		/// </summary>
		/// <param name="scene">the name of the scene</param>
		/// <returns></returns>
		private bool IsSceneLoaded (string scene) {
			var count = SceneManager.sceneCount;

			for (var i = 0; i < count; ++i) {
				var loadedScene = SceneManager.GetSceneAt (i);

				if (loadedScene.name == scene) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Unload scenes in the loadedScenes stack
		/// </summary>
		/// <param name="popAmount">the amount of scene want to unload</param>
		private void PopLastSceneGroup (int popAmount = 1) {
			for (var i = 0; i < popAmount; ++i) {
				if (loadedScenes.Count > 0) {
					var scenes = loadedScenes.Pop ();

					foreach (var scene in scenes) {
						SceneManager.UnloadSceneAsync (scene);
					}
				}
			}
		}
	}
}