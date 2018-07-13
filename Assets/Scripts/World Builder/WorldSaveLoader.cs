using System.Collections;
using System.Collections.Generic;
using OneGame.Lua;
using UnityEngine;

namespace OneGame {
    /// <summary>
    /// Handles loading and saving game state whenever the game switches between
    /// build mode and play mode
    /// </summary>
    public class WorldSaveLoader : MonoBehaviour {
        [SerializeField]
        private bool loadOnStart;
        [SerializeField]
        private EntityGenerator generator;
        [SerializeField]
        private MicroGameEngine engine;
        [SerializeField]
        private ItemCatalog database;
        [SerializeField]
        private SaveManager saveManager;
        [SerializeField, Space]
        private GameEventTable eventSystem;
        [SerializeField]
        private SceneLoader sceneLoader;

        private SaveData settingsData;

        private IEnumerator Start () {
            settingsData = new SaveData ();

            while (database.Status != ItemCatalog.LoadingStatus.Completed || !sceneLoader.IsReadyToLoad) {
                yield return null;
            }
            yield return null;

            if (loadOnStart) {
                LoadFromSave (saveManager[saveManager.ActiveSaveId, JsonSaveType.File]);
            }
        }

        private void OnEnable () {
            if (eventSystem != null) {
                eventSystem.Register<WorldMode, WorldMode> ("OnWorldModeTransition", HandleModeTransition);
                eventSystem.Register ("OnSaveButtonClicked", SaveToFile);

                eventSystem.Register<float, float, bool> ("OnDaySettingsChanged", HandleDaySettingsChangeEvent);
                eventSystem.Register<string> ("OnWorldDescriptionChange", HandleWorldDescriptionChange);
                eventSystem.Register<string> ("OnWorldNameChange", HandleWorldNameChange);
            }
        }

        private void OnDisable () {
            if (eventSystem != null) {
                eventSystem.Unregister<WorldMode, WorldMode> ("OnWorldModeTransition", HandleModeTransition);
                eventSystem.Unregister ("OnSaveButtonClicked", SaveToFile);

                eventSystem.Unregister<float, float, bool> ("OnDaySettingsChanged", HandleDaySettingsChangeEvent);
                eventSystem.Unregister<string> ("OnWorldDescriptionChange", HandleWorldDescriptionChange);
                eventSystem.Unregister<string> ("OnWorldNameChange", HandleWorldNameChange);
            }
        }

        /// <summary>
        /// Delegate called when the day-night cycle settings change
        /// </summary>
        private void HandleDaySettingsChangeEvent (float time, float scale, bool cycle) {
            SaveExtensions.SetNumberValue (ref settingsData, "day-time", time);
            SaveExtensions.SetNumberValue (ref settingsData, "time-scale", scale);
            SaveExtensions.SetBoolValue (ref settingsData, "day-night-cycle", cycle);
        }

        /// <summary>
        /// Delegate called when the world state begins transitioning
        /// </summary>
        private void HandleModeTransition (WorldMode startingMode, WorldMode targetMode) {
            var id = saveManager.ActiveSaveId;

            switch (startingMode) {
                case WorldMode.Build:
                    // From Build -> Build, just load from the save file
                    if (targetMode == WorldMode.Build) {
                        generator.Clear ();
                        LoadFromSave (saveManager[id, JsonSaveType.File]);
                    } else {
                        // Otherwise, save to buffer, and load from it
                        SaveToBuffer ();
                        generator.Clear ();
                        LoadFromSave (saveManager[id, JsonSaveType.Buffer]);
                    }
                    break;

                case WorldMode.Play:
                    // From Play -> Build, clear the generator, and load from buffer
                    generator.Clear ();
                    LoadFromSave (saveManager[id, JsonSaveType.Buffer]);
                    break;
            }
        }

        /// <summary>
        /// Delegate called when the world description changes
        /// </summary>
        /// <param name="description">The new description</param>
        private void HandleWorldDescriptionChange (string description) {
            SaveExtensions.SetStringValue (ref settingsData, "world-description", description);
        }

        /// <summary>
        /// Delegate called when the world name changes
        /// </summary>
        /// <param name="name">The new name</param>
        private void HandleWorldNameChange (string name) {
            SaveExtensions.SetStringValue (ref settingsData, "world-name", name);
        }

        /// <summary>
        /// Loads the gme from a save file
        /// </summary>
        /// <param name="saveData">The data to load</param>
        private void LoadFromSave (SaveData saveData) {
            // Load the game manager script
            var script = engine.Script;
            script.code = saveData.GetStringValue ("game-manager-script", string.Empty);
            script.script.ApplyDefaultValues ();
            script.script.DoString (script.code);
            engine.Script = script;

            // Set the player position
            Player.Instance.position = new Vec3 (saveData.playerPosition.ToVector3 ());

            generator.Clear ();

            var entityList = saveData.entities;

            foreach (var item in entityList) {
                var element = database.FindElementData (item.id);

                if (element.id != 0) {
                    try {
                        var entity = generator.SpawnEntity (element, item.uniqueId, item.position.ToVector3 (), item.scale.ToVector3 (), item.rotation.ToQuaternion ());
                        entity.AddScript (item.script);
                    } catch (System.ArgumentException) {
                        continue;
                    }
                }
            }

            // Copy the settings
            settingsData = saveData.CreateCopy ();

            // Invoke time data
            if (eventSystem != null) {
                eventSystem.Invoke<float, float, bool> ("OnDaySettingsInit",
                    (float)settingsData.GetNumberValue ("day-time", 12),
                    (float)settingsData.GetNumberValue ("time-scale", 1),
                    settingsData.GetBoolValue ("day-night-cycle", false));
            }

            eventSystem?.Invoke ("OnSaveLoaded");
        }

        /// <summary>
        /// Creates a snapshot of the current world and saves it to the save manager's buffer
        /// </summary>
        private void SaveToBuffer () {
            var name = saveManager.ActiveSaveId;
            saveManager[name, JsonSaveType.Buffer] = CreateRuntimeSaveData ();
        }

        /// <summary>
        /// Creates a snapshot of the current world and save it to the save manager's file 
        /// </summary>
        private void SaveToFile () {
            var id = saveManager.ActiveSaveId;
            saveManager[saveManager.ActiveSaveId, JsonSaveType.File] = CreateRuntimeSaveData ();
            saveManager.SaveWorld (id, JsonSaveType.File);
        }

        /// <summary>
        /// Generates a snapshot of the world
        /// </summary>
        private SaveData CreateRuntimeSaveData () {
            var entities = generator.SpawnedEntities;
            var saveList = new EntitySaveData[entities.Length];

            for (var i = 0; i < entities.Length; ++i) {
                var entity = entities[i];
                var transform = entity.Transform;

                var saveData = new EntitySaveData {
                    id = entity.AssetId.ToUInt (),
                    uniqueId = entity.UniqueId,
                    scale = new Vector3Double (transform.localScale),
                    position = new Vector3Double (transform.position),
                    rotation = new QuaternionDouble (transform.rotation),
                    script = entity.Script
                };
                saveList[i] = saveData;
            }

            var save = settingsData.CreateCopy ();
            save.playerPosition = new Vector3Double (Player.Instance.position.ToVector3 ());
            save.entities = saveList;

            SaveExtensions.SetStringValue (ref save, "game-manager-script", engine.Script.code);

            return save;
        }
    }
}