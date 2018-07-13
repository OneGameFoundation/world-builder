using System;
using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua {
    using Random = UnityEngine.Random;

    public enum WeaponType { Melee, Range }

    /// <summary>
    /// A native component that can attack other entities
    /// </summary>
    [MoonSharpUserData]
    public class Weapon : NativeComponent, IEquippable {
        /// <summary>
        /// The name of the ammo that this weapon relies on
        /// </summary>
        public string ammoName { get; set; }

        /// <summary>
        /// The type of weapon - determines whether the weapon is a hitscan or a melee weapon
        /// </summary>
        public WeaponType weaponType { get; set; }

        /// <summary>
        /// Gets or sets the float properties by name
        /// </summary>
        public float this[string name] {
            get {
                var values = Enum.GetNames (typeof (WeaponProperty));
                for (var i = 0; i < values.Length; ++i) {
                    if (values[i] == name) {
                        return weaponProperties[i];
                    }
                }

                return 0;
            }
            set {
                var values = Enum.GetNames (typeof (WeaponProperty));
                for (var i = 0; i < values.Length; ++i) {
                    if (values[i] == name) {
                        weaponProperties[i] = value;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// The weapon's transform parent
        /// </summary>
        [MoonSharpHidden]
        public Transform Anchor {
            get { return anchor; }
            set {
                anchor = value;
                root = anchor.root;
                transform.SetParent (anchor);
                transform.localPosition = MetadataUtilities.GetVector3 ("weapon-position", metadata);
                transform.localEulerAngles = MetadataUtilities.GetVector3 ("weapon-rotation", metadata);
                transform.localScale = MetadataUtilities.GetVector3 ("weapon-scale", metadata);

                animator = GetComponentInParent<Animator> ();
                rigidbody.isKinematic = value != null;
            }
        }

        /// <summary>
        /// Can this weapon attack?
        /// </summary>
        private bool CanAttack { get { return !isReloading && Time.time - lastAttackTimestamp >= 1f / weaponProperties[(int)WeaponProperty.AttackRate]; } }

        [SerializeField]
        private KeyCode attackKey = KeyCode.Mouse0;

        private Transform root;
        private Transform anchor;
        private Animator animator;
        private new AudioSource audio;
        private new Rigidbody rigidbody;
        private LineRenderer[] lineRenderers;
        private WeaponAssetDefaults assetDefaults;
        private ObjectPool pool;

        private bool isReloading;
        private float currentMagazine;
        private Vector3 barrelExhaustPosition;

        private float lastAttackTimestamp;
        private IMetadata metadata;

        private float[] weaponProperties;

        private RaycastHit[] raycastResults = new RaycastHit[8];

        protected override void Awake () {
            rigidbody = gameObject.AddComponent<Rigidbody> ();
            rigidbody.isKinematic = true;

            PrepareWeapon ();

            assetDefaults = WeaponAssetDefaults.Instance;
            pool = ObjectPool.Instance;

            weaponProperties = new float[Enum.GetValues (typeof (WeaponProperty)).Length];
            isReloading = false;
        }

        private void Start () {
            lastAttackTimestamp = Mathf.NegativeInfinity;

            var colliders = GetComponentsInChildren<MeshCollider> ();
            foreach (var collider in colliders) {
                collider.convex = true;
            }
        }

        private void Update () {
            if (Player.Instance.health.health > 0) {
                if (anchor != null && UnityEngine.Input.GetKey (attackKey)) {
                    Attack ();
                }
            }
        }

        /// <summary>
        /// Processes the metadata on the weapon
        /// </summary>
        /// <param name="data">The weapon data</param>
        public override void ProcessMetadata (IMetadata data) {
            metadata = data;

            weaponProperties[(int)WeaponProperty.AttackRate] = MetadataUtilities.GetFloat ("weapon-fire-rate", data);
            weaponProperties[(int)WeaponProperty.Damage] = MetadataUtilities.GetFloat ("weapon-damage", metadata);
            weaponProperties[(int)WeaponProperty.Range] = MetadataUtilities.GetFloat ("weapon-range", data);
            weaponProperties[(int)WeaponProperty.MagazineSize] = MetadataUtilities.GetFloat ("weapon-mag", metadata);
            weaponProperties[(int)WeaponProperty.ReloadTime] = MetadataUtilities.GetFloat ("weapon-reload", metadata);
            weaponProperties[(int)WeaponProperty.Spread] = MetadataUtilities.GetFloat ("weapon-spread", metadata);
            weaponProperties[(int)WeaponProperty.Penetration] = MetadataUtilities.GetFloat ("weapon-hits", metadata);
            weaponProperties[(int)WeaponProperty.PelletCount] = MetadataUtilities.GetFloat ("weapon-hits", metadata);
            weaponType = MetadataUtilities.GetString ("weapon-type", data) == "Range" ? WeaponType.Range : WeaponType.Melee;
            barrelExhaustPosition = MetadataUtilities.GetVector3 ("weapon-barrel-position", data);
        }

        /// <summary>
        /// Performs an attack based on thr weapon type
        /// </summary>
        public void Attack () {
            if (CanAttack) {
                lastAttackTimestamp = Time.time;
                animator?.SetTrigger ("Attack");

                var range = weaponProperties[(int)WeaponProperty.Range];
                var damage = weaponProperties[(int)WeaponProperty.Damage];

                switch (weaponType) {
                    case WeaponType.Range:
                        if (currentMagazine > 0) {
                            PlayClip (assetDefaults.GetAsset<AudioClip> ("shoot-clip"));
                            SpawnMuzzleFlash (assetDefaults.GetAsset<GameObject> ("muzzle-flash-prefab"));

                            var pelletCount = weaponProperties[(int)WeaponProperty.PelletCount];
                            for (var i = 0; i < pelletCount; ++i) {
                                CastAttackHitscan (range, damage);
                            }
                            currentMagazine--;
                        } else {
                            Reload ();
                        }
                        break;

                    case WeaponType.Melee:
                        CastAttackMelee (range, damage);
                        break;
                }
            }
        }

        /// <summary>
        /// Reloads the weapon
        /// </summary>
        public void Reload () {
            if (!isReloading) {
                isReloading = true;
                PlayClip (assetDefaults.GetAsset<AudioClip> ("reload-clip"));

                StopAllCoroutines ();
                var reloadTime = weaponProperties[(int)WeaponProperty.ReloadTime];
                StartCoroutine (PlayReloadEffect (reloadTime));
            }
        }

        /// <summary>
        /// Unequips the weapon
        /// </summary>
        public void Unequip () {
            transform.SetParent (null);
            anchor = null;

            // TODO: Make this logic more generic
            rigidbody.isKinematic = false;

            StopAllCoroutines ();
            isReloading = false;
        }

        /// <summary>
        /// Casts a hitscan attack towards the middle of the screen
        /// </summary>
        private void CastAttackHitscan (float range, float damage) {
            // Fire a raycast at the center of the screen
            var ray = Camera.main.ScreenPointToRay (new Vector2 (Screen.width * 0.5f, Screen.height * 0.5f));
            var spread = weaponProperties[(int)WeaponProperty.Spread];
            var direction = ray.direction;
            direction += Random.insideUnitSphere * spread;
            ray.direction = direction;

            var weaponRange = range + Vector3.Distance (transform.position, ray.origin);
            var count = Physics.RaycastNonAlloc (ray, raycastResults, weaponRange, 1 << 0 | 1 << 8, QueryTriggerInteraction.Ignore);

            var penetration = weaponProperties[(int)WeaponProperty.Penetration];

            // For every element up to the number of enemies hit by penetration count, 
            // deal damage equal to the weapon's damage
            var hitCount = 0;
            for (var i = 0; i < count && hitCount <= penetration; ++i) {
                var hit = raycastResults[i];

                if (hit.transform.root != root) {
                    // Spawn a hit prefab at the raycast point
                    var hitClone = pool.Spawn (assetDefaults.GetAsset<GameObject> ("hit-prefab"), hit.point, Quaternion.LookRotation (hit.normal));
                    pool.PutBack (hitClone, 5f);

                    // Inflict damage, if possible
                    var health = hit.collider.GetComponentInParent<Health> ();
                    health?.Damage (damage);

                    hitCount++;
                }
            }

            // Spawn a line renderer to show where the bullet landed
            var lineClone = pool.Spawn (assetDefaults.GetAsset<GameObject> ("bullet-trail"), Vector3.zero, Vector2.zero);
            var lineRenderer = lineClone.GetComponent<LineRenderer> ();
            lineRenderer?.SetPositions (new Vector3[] { transform.TransformPoint (barrelExhaustPosition), ray.origin + ray.direction * range });
        }

        /// <summary>
        /// Casts an attack in a wide arc
        /// </summary>
        private void CastAttackMelee (float range, float damage) {
            var results = Physics.OverlapSphere (root.position, range, 1 << 0, QueryTriggerInteraction.Ignore);
            var current = transform.position;
            var fwd = root.forward;
            var arc = weaponProperties[(int)WeaponProperty.HorizontalArc] * 0.5f;

            for (var i = 0; i < results.Length; ++i) {
                var result = results[i].transform;

                if (result.root != root) {
                    var diff = result.position - current;
                    diff.y = 0f;

                    if (Vector3.Angle (diff, fwd) < arc) {
                        var health = result.GetComponentInParent<Health> ();
                        health?.Damage (damage);
                    }
                }
            }
        }

        /// <summary>
        /// Plays a clip on the weapon
        /// </summary>
        /// <param name="clip">The clip to play</param>
        private void PlayClip (AudioClip clip) {
            audio.clip = clip;
            audio.Play ();
        }

        private IEnumerator PlayReloadEffect (float duration) {
            animator?.ResetTrigger ("Attack");
            animator?.SetTrigger ("Reload");

            yield return new WaitForSeconds (duration);

            currentMagazine = weaponProperties[(int)WeaponProperty.MagazineSize];
            isReloading = false;
        }

        /// <summary>
        /// Prepares the weapon's audio components
        /// </summary>
        private void PrepareWeapon () {
            audio = gameObject.AddComponent<AudioSource> ();
            audio.volume = 0.65f;
            audio.playOnAwake = false;
            audio.spatialBlend = 1f;
        }

        /// <summary>
        /// Spawns a muzzle flash at the end of the weapon barrel
        /// </summary>
        /// <param name="prefab">The prefab to spawn</param>
        private void SpawnMuzzleFlash (GameObject prefab) {
            var pool = ObjectPool.Instance;
            var clone = pool.Spawn (prefab, transform).transform;
            clone.localPosition = barrelExhaustPosition;
            clone.localRotation = Quaternion.identity;

            pool.PutBack (clone.gameObject, 2f);
        }
    }
}