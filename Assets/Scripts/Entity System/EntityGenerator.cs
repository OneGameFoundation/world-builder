using System.Collections.Generic;
using OneGame.Lua;
using UnityEngine;

namespace OneGame {
    [CreateAssetMenu (menuName = "Scriptable Objects/Entity System/Entity Generator")]
    public class EntityGenerator : ScriptableObject {

        /// <summary>
        /// A collection of all spawned entities in the game
        /// </summary>
        public IEntityContainer[] SpawnedEntities { get { return spawnedEntities.ToArray (); } }

        [SerializeField]
        private ItemCatalog itemDatabase;
        [SerializeField]
        private ScriptManager scriptManager;

        private List<IEntityContainer> spawnedEntities;

        private void OnEnable () {
            spawnedEntities = new List<IEntityContainer> ();

            SpawnManager.OnIemSpawn += SpawnEntity;
            SpawnManager.FindEntityFunction = FindEntity;
            SpawnManager.CreateEmptyDelegate += SpawnEmptyEntity;
            SpawnManager.OnEntityDestroy += DestroyEntity;

        }

        private void OnDisable () {
            SpawnManager.OnIemSpawn -= SpawnEntity;
            SpawnManager.FindEntityFunction -= FindEntity;
            SpawnManager.CreateEmptyDelegate -= SpawnEmptyEntity;
            SpawnManager.OnEntityDestroy -= DestroyEntity;
        }

        /// <summary>
        /// Deletes all spawned entities
        /// </summary>
        public void Clear () {
            foreach (var entity in spawnedEntities) {
                Destroy (entity.GameObject);
            }

            spawnedEntities.Clear ();
        }

        /// <summary>
        /// Deletes an entity from the scene
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        public void DeleteEntity (EntityGameObject entity) {
            spawnedEntities.Remove (entity);

            Destroy (entity.GameObject);
        }

        /// <summary>
        /// Spawns an entity of a given element
        /// </summary>
        public EntityGameObject SpawnEntity (ElementData data, Vector3 position) {
            var asset = itemDatabase.GetAsset<GameObject> (data.id);
            var clone = Instantiate (asset, position, Quaternion.identity);
            clone.name = data.name;

            AddMeshCollider (clone);

            var entity = clone.AddComponent<EntityGameObject> ();
            entity.ScriptManager = scriptManager;
            entity.ApplyElementData (data);
            scriptManager.AddRunner (entity);

            spawnedEntities.Add (entity);

            return entity;
        }

        /// <summary>
        /// Spawns an entity of a given element
        /// </summary>
        public EntityGameObject SpawnEntity (ElementData data, string id, Vector3 position, Vector3 scale, Quaternion rotation) {
            var asset = itemDatabase.GetAsset<GameObject> (data.id);
            var clone = Instantiate (asset, position, rotation);
            clone.transform.localScale = scale;
            AddMeshCollider (clone);

            var entity = clone.AddComponent<EntityGameObject> ();
            entity.ScriptManager = scriptManager;
            entity.ApplyElementData (data, id);
            scriptManager.AddRunner (entity);

            spawnedEntities.Add (entity);

            return entity;
        }

        /// <summary>
        /// Destroys a virtually-spawned entity
        /// </summary>
        /// <param name="obj">The object refernce of the entity</param>
        private void DestroyEntity (Entity obj) {
            var entity = spawnedEntities.Find (e => e.Entity == obj);

            if (entity != null) {
                spawnedEntities.Remove (entity);
                Destroy (entity.GameObject);
            }
        }

        /// <summary>
        /// Finds an entity object refernce by name
        /// </summary>
        /// <param name="name">The name of the entity</param>
        private Entity FindEntity (string name) {
            var index = spawnedEntities.FindIndex (s => s.GameObject.name == name || s.UniqueId == name);

            if (index > -1) {
                return spawnedEntities[index].Entity;
            }

            return default (Entity);
        }

        /// <summary>
        /// Spawns a placeholder entity with the given name
        /// </summary>
        private Entity SpawnEmptyEntity (string name) {
            var go = new GameObject (name);

            var entity = go.AddComponent<EntityGameObject> ();
            entity.ScriptManager = scriptManager;
            entity.ApplyElementData (
                new ElementData {
                    name = name,
                    id = 0
                },
                EntityUtilities.GetUniqueId ()
            );

            spawnedEntities.Add (entity);

            return entity.Entity;
        }

        /// <summary>
        /// Spawns an entity in the virtual pool
        /// </summary>
        private Entity SpawnEntity (Item item, Vector3 position, Vector3 scale, Vector3 rotation) {
            var data = itemDatabase.FindElementData (item.id);

            var asset = itemDatabase.GetAsset<GameObject> (data.id);
            var clone = Instantiate (asset, position, Quaternion.Euler (rotation));
            clone.transform.localScale = scale;
            AddMeshCollider (clone);

            var entity = clone.AddComponent<EntityGameObject> ();
            entity.ScriptManager = scriptManager;
            entity.ApplyElementData (data);

            scriptManager.AddRunner (entity);
            entity.InvokeLuaMethod ("start");

            spawnedEntities.Add (entity);

            return entity.Entity;
        }

        /// <summary>
        /// Adds a collider for later selection by the player
        /// </summary>
        private void AddMeshCollider (GameObject gameObj) {
            var filters = gameObj.GetComponentsInChildren<MeshFilter> ();

            if (filters.Length > 0) {
                foreach (var filter in filters) {
                    var collider = filter.GetComponent<Collider> ();
                    if (collider == null) {
                        collider = filter.gameObject.AddComponent<MeshCollider> ();
                        (collider as MeshCollider).sharedMesh = filter.sharedMesh;
                    }
                }
            }
        }

    }
}