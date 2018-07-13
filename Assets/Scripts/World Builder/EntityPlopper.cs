using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {
    /// <summary>
    /// An interaction tool that allows the player to place entities into the scene
    /// </summary>
    public class EntityPlopper : MonoBehaviour {

        public ElementData SelectedElement { get; private set; }

        [SerializeField]
        private EntityGenerator generator;
        [SerializeField]
        private ItemCatalog catalog;
        [SerializeField]
        private GameEventTable eventTable;

        private GameObject indicator;

        private void OnEnable () {
            if (eventTable != null) {
                eventTable.Register<ElementData> ("OnElementButtonClicked", StartPlacement);
            }
        }

        private void OnDisable () {
            if (eventTable != null) {
                eventTable.Unregister<ElementData> ("OnElementButtonClicked", StartPlacement);
            }
        }

        private void Update () {
            if (indicator != null) {
                var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
                    if (hit.collider.CompareTag ("Ground")) {
                        indicator.transform.position = hit.point;
                    }
                }

                if (Input.GetMouseButtonDown (1)) {
                    StopPlacement ();
                } else if (Input.GetMouseButtonDown (0)) {
                    PlaceElement (SelectedElement);
                }
            }
        }

        private void PlaceElement (ElementData data) {
            if (data.id != 0) {
                generator.SpawnEntity (data, indicator.transform.position);
                StopPlacement ();
            }
        }

        private void StartPlacement (ElementData data) {
            var asset = catalog.GetAsset<GameObject> (data.id);
            indicator = Instantiate (asset) ?? null;

            if (indicator != null) {
                indicator.SetActive (true);
            }

            SelectedElement = data;
        }

        private void StopPlacement () {
            if (indicator != null) {
                Destroy (indicator);
            }

            SelectedElement = default (ElementData);
        }
    }
}