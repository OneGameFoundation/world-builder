using UnityEngine;
using UnityEngine.UI;

namespace OneGame {
    public class EnvironmentEditor : MonoBehaviour {
        [SerializeField]
        private GameEventTable table;

        [SerializeField]
        private Slider sunPositionSlider;
        [SerializeField]
        private Slider cycleSpeedSlider;
        [SerializeField]
        private Toggle cycleToggle;

        private void OnEnable () {
            table.Register<float, float, bool> ("OnDaySettingsInit", HandleDaySettingsEvent);
            table.Invoke ("OnBuilderUILoaded");
        }

        private void OnDisable () {
            table.Unregister<float, float, bool> ("OnDaySettingsInit", HandleDaySettingsEvent);
        }
        public void setSunCycleSpeed () {
            table.Invoke<float> ("OnDayNightSpeedChange", cycleSpeedSlider.value);
        }

        public void toggleDayNightCycle () {
            table.Invoke ("ToggleDayNightCycle");
        }

        public void setSunPosition () {
            table.Invoke<float> ("OnSunPositionSet", sunPositionSlider.value);
        }

        private void HandleDaySettingsEvent (float time, float scale, bool cycle) {
            sunPositionSlider.value = time;
            cycleSpeedSlider.value = scale;

            cycleToggle.isOn = cycle;
            cycleSpeedSlider.gameObject.SetActive (cycle);
        }

    }
}