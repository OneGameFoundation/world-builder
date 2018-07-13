using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {
    /// <summary>
    /// A manager that creates a pool of clones to spawn from
    /// </summary>
    [CreateAssetMenu (fileName = "ObjectPool", menuName = "Scriptable Objects/Managers/Object Pool", order = 0)]
    public class ObjectPool : ScriptableObject {
        /// <summary>
        /// The current ObjectPool instance
        /// </summary>
        public static ObjectPool Instance { get; private set; }

        [SerializeField]
        private int poolSize = 10;

        private Dictionary<GameObject, List<GameObject>> objectPools;

        private void OnEnable () {
            objectPools = new Dictionary<GameObject, List<GameObject>> ();
            Instance = this;
        }

        /// <summary>
        /// Clears the pool
        /// </summary>
        public void Clear () {
            foreach (var pool in objectPools) {
                var list = pool.Value;
                for (var i = 0; i < list.Count; ++i) {
                    Destroy (list[i]);
                }
                list.Clear ();
            }

            objectPools.Clear ();
        }

        /// <summary>
        /// Clears a particular pool
        /// </summary>
        /// <param name="key">The pool to clear</param>
        public void ClearPool (GameObject key) {
            List<GameObject> pool;

            if (objectPools.TryGetValue (key, out pool)) {
                for (var i = 0; i < pool.Count; ++i) {
                    Destroy (pool[i]);
                }

                pool.Clear ();
                objectPools.Remove (key);
            }
        }

        /// <summary>
        /// Spawns a gameobject with a given prefab
        /// </summary>
        /// <param name="prefab">The prefab to clone</param>
        /// <param name="pos">The position of the clone</param>
        /// <param name="rot">The rotation of the clone</param>
        public GameObject Spawn (GameObject prefab, Vector3 pos, Vector3 rot) {
            var instance = GetObject (prefab);
            instance.SetActive (true);
            instance.transform.position = pos;
            instance.transform.eulerAngles = rot;
            return instance;
        }

        /// <summary>
        /// Spaawns a gameobject with a given prefab
        /// </summary>
        /// <param name="prefab">The prefab to clone</param>
        /// <param name="pos">The position of the clone</param>
        /// <param name="rot">The rotation of the clone</param>
        public GameObject Spawn (GameObject prefab, Vector3 pos, Quaternion rot) {
            var instance = GetObject (prefab);
            instance.SetActive (true);
            instance.transform.position = pos;
            instance.transform.rotation = rot;
            return instance;
        }

        /// <summary>
        /// Spawns a gameobject and parents it to a transform instance
        /// </summary>
        /// <param name="prefab">The prefab to clone</param>
        /// <param name="transform">The clone's new parent</param>
        public GameObject Spawn (GameObject prefab, Transform transform) {
            var instance = GetObject (prefab);
            instance.SetActive (true);
            instance.transform.SetParent (transform);
            return instance;
        }

        /// <summary>
        /// Despawns the cloned gameobject
        /// </summary>
        /// <param name="obj">The object to despawn</param>
        public void PutBack (GameObject obj) {
            if (obj != null) {
                obj.SetActive (false);
                obj.transform.position = Vector3.zero;
                obj.transform.SetParent (null);
            }
        }

        /// <summary>
        /// Despawns the cloned gameobject with a delay
        /// </summary>
        /// <param name="obj">The object to despawn</param>
        /// <param name="delay">How long should the despawn wait?</param>
        public void PutBack (GameObject obj, float delay) {
            ScriptableWorker.RunCoroutine (DelayPutBack (obj, delay));
        }

        /// <summary>
        /// Disables all spawned objects
        /// </summary>
        public void PutBackAll () {
            foreach (var item in objectPools) {
                var list = item.Value;
                foreach (GameObject obj in list) {
                    obj.SetActive (false);
                }
            }
        }

        /// <summary>
        /// Coroutine to add a delayed despawn
        /// </summary>
        /// <param name="obj">The object to despawn</param>
        /// <param name="delay">How long should the routine wait to despawn</param>
        private IEnumerator DelayPutBack (GameObject obj, float delay) {
            yield return new WaitForSeconds (delay);
            PutBack (obj);
        }

        //get the gameObject instance given the gameObject key
        private GameObject GetObject (GameObject key) {
            List<GameObject> list;
            if (objectPools.TryGetValue (key, out list)) {
                //find one instance not in use
                for (var i = 0; i < list.Count; ++i) {
                    var gameObject = list[i];

                    if (!gameObject.activeInHierarchy) {
                        return gameObject;
                    }
                }

                // Extend the pool and return a fresh clone if the pool if full
                return ExtendExistingPool (key);
            }
            return GenerateNewPool (key);
        }

        /// <summary>
        /// Extends an existing pool
        /// </summary>
        /// <param name="key">The prefab to extend the pool on</param>
        private GameObject ExtendExistingPool (GameObject key) {
            List<GameObject> list;

            if (objectPools.TryGetValue (key, out list)) {
                var instanceIndex = list.Count;

                for (var i = 0; i < poolSize; i++) {
                    var clone = Instantiate (key, Vector3.zero, Quaternion.identity);
                    clone.SetActive (false);
                    list.Add (clone);
                }

                return list[instanceIndex];
            }

            return GenerateNewPool (key);
        }

        /// <summary>
        /// Creates a new pool for a prefab
        /// </summary>
        /// <param name="key">The prefab to create a pool with</param>
        private GameObject GenerateNewPool (GameObject key) {
            var list = new List<GameObject> (poolSize);
            objectPools.Add (key, list);
            return ExtendExistingPool (key);
        }
    }
}