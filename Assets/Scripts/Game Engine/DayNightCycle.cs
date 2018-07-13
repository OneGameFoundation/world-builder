using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace OneGame {
	/// <summary>
	/// Handles the positioning and color of the sun and moon
	/// </summary>
	public class DayNightCycle : MonoBehaviour {
		public float CurrentTime { get { return currentTime; } }
		public float StartTime { get { return time; } set { time = value; } }
		public float TimeScale { get; set; }
		public bool UseDayNightCycle { get; set; }

		[SerializeField, Range (0, 24)]
		private float time;

		[SerializeField]
		private Light sun;
		[SerializeField]
		private Transform celestialAnchor;

		[SerializeField, Header ("Effects")]
		private Gradient lightGradient;
		[SerializeField]
		private Gradient domeGradient;
		[SerializeField]
		private AnimationCurve sunIntensity;
		[SerializeField, Range (0f, 1f)]
		private float ambientIntensity;

		[SerializeField, Space]
		private Renderer skyDomeRenderer;
		[SerializeField]
		private GameEventTable table;

		private Material domeMaterial;
		private float currentTime;

		private Light oldSun;
		private AmbientMode oldMode;
		private Color oldAmbientColor;

		private void Start () {
			StartTime = time;

			domeMaterial = Instantiate (skyDomeRenderer.material);
			skyDomeRenderer.material = domeMaterial;

			oldSun = RenderSettings.sun;
			oldMode = RenderSettings.ambientMode;
			oldAmbientColor = RenderSettings.ambientLight;

			RenderSettings.sun = sun;
			RenderSettings.ambientMode = AmbientMode.Flat;
		}

		private void OnEnable () {
			if (table != null) {
				table.Register<float> ("OnSunPositionSet", ApplyTime);
				table.Register<float> ("OnSunPositionSet", ApplyDomeColor);
				table.Register<float> ("OnSunPositionSet", SetStartTime);
				table.Register ("ToggleDayNightCycle", ToggleDayNightCycle);
				table.Register<WorldMode> ("OnWorldModeChange", HandleWorldModeChangeEvent);
				table.Register<float> ("OnDayNightSpeedChange", SetCycleTimeScale);
				table.Register<float, float, bool> ("OnDaySettingsInit", HandleSettingsInitEvent);
			}
		}

		private void OnDisable () {
			if (table != null) {
				table.Unregister<float> ("OnSunPositionSet", ApplyTime);
				table.Unregister<float> ("OnSunPositionSet", ApplyDomeColor);
				table.Unregister<float> ("OnSunPositionSet", SetStartTime);
				table.Unregister ("ToggleDayNightCycle", ToggleDayNightCycle);
				table.Unregister<WorldMode> ("OnWorldModeChange", HandleWorldModeChangeEvent);
				table.Unregister<float> ("OnDayNightSpeedChange", SetCycleTimeScale);
				table.Unregister<float, float, bool> ("OnDaySettingsInit", HandleSettingsInitEvent);
			}
		}

		private void OnDestroy () {
			Destroy (domeMaterial);

			// Restore previous render settings
			RenderSettings.sun = oldSun;
			RenderSettings.ambientMode = oldMode;
			RenderSettings.ambientLight = oldAmbientColor;
		}

		/// <summary>
		/// Sets the time scale
		/// </summary>
		public void SetCycleTimeScale (float scale) {
			TimeScale = scale;

			table?.Invoke<float, float, bool> ("OnDaySettingsChanged", time, TimeScale, UseDayNightCycle);
		}

		/// <summary>
		/// Toggles the progression between day and night
		/// </summary>
		public void ToggleDayNightCycle () {
			UseDayNightCycle = !UseDayNightCycle;

			table?.Invoke<float, float, bool> ("OnDaySettingsChanged", time, TimeScale, UseDayNightCycle);
		}

		/// <summary>
		/// Applies lighting effects based on the current time
		/// </summary>
		private void ApplyTime (float time) {
			var normalizedTime = Mathf.Repeat (time, 24f) / 24f;

			// Apply the rotation
			celestialAnchor.localEulerAngles = new Vector3 (0f, 0f, normalizedTime * 360f);

			// Apply lights
			sun.intensity = sunIntensity.Evaluate (normalizedTime);

			var color = lightGradient.Evaluate (normalizedTime);
			sun.color = color;

			// Apply global illumination
			RenderSettings.ambientLight = Color.Lerp (Color.black, color, ambientIntensity);
		}

		private void ApplyDomeColor (float time) {
			domeMaterial.color = domeGradient.Evaluate (time / 24f);
		}

		private void HandleSettingsInitEvent (float time, float scale, bool cycle) {
			StartTime = time;
			TimeScale = scale;
			UseDayNightCycle = cycle;

			ApplyTime (StartTime);
			ApplyDomeColor (StartTime);
		}

		private void HandleWorldModeChangeEvent (WorldMode mode) {
			switch (mode) {
				case WorldMode.Play:
					if (UseDayNightCycle) {
						StopAllCoroutines ();

						currentTime = StartTime;
						StartCoroutine (PlayDayNightCycle ());
					}
					break;

				case WorldMode.Build:
					StopAllCoroutines ();
					ApplyTime (time);
					ApplyDomeColor (time);
					break;
			}
		}

		private IEnumerator PlayDayNightCycle () {
			while (true) {
				var tick = Time.timeScale * TimeScale / 8400f;
				currentTime = Mathf.Repeat (currentTime + tick, 24f);
				ApplyTime (currentTime);
				ApplyDomeColor (currentTime);
				yield return null;
			}
		}

		private void SetStartTime (float time) {
			StartTime = time;

			table?.Invoke<float, float, bool> ("OnDaySettingsChanged", time, TimeScale, UseDayNightCycle);
		}

	}
}