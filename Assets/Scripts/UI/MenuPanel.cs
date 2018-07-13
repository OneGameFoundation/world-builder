using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {
    public class MenuPanel : MonoBehaviour {
        public float animationTime = 0.2f;
        public RectTransform mask;
        public RectTransform panel;

        [SerializeField]
        private WorldMode modeToOpenIn;
        [SerializeField]
        private string eventToListen = "OnPauseMenuOpen";

        [SerializeField]
        private GameEventTable uiEventTable;

        [HideInInspector]
        public bool isOpen = false;

        private WorldMode currentMode;

        private void Awake () {
            if (uiEventTable != null) {
                uiEventTable.Register<WorldMode> ("OnWorldModeChange", HandleWorldModeChange);
                uiEventTable.Register (eventToListen, OpenPanel);
            }
        }

        private void Start () {
            mask.localScale = Vector3.zero;
            panel.localScale = Vector3.zero;
        }

        private void OnDestroy () {
            if (uiEventTable != null) {
                uiEventTable.Register<WorldMode> ("OnWorldModeChange", HandleWorldModeChange);
                uiEventTable.Unregister (eventToListen, OpenPanel);
            }
        }

        public void OpenPanel () {
            if (modeToOpenIn == currentMode) {
                mask.localScale = Vector3.one;
                LeanTween.scale (panel, Vector3.one, 0.1f).setEaseInCirc ();
                isOpen = true;
            }
        }

        public void ClosePanel () {
            LeanTween.scale (panel, Vector3.zero, 0.1f).setEaseInCirc ().setOnComplete (HideMask);
            isOpen = false;
        }

        private void HandleWorldModeChange (WorldMode mode) {
            currentMode = mode;
        }

        private void HideMask () {
            mask.localScale = Vector3.zero;
        }
    }
}