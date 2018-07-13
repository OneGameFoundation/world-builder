using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {
	/// <summary>
	/// An effect that causes the bullet trail to "disappear" over time
	/// </summary>
	[RequireComponent (typeof (LineRenderer))]
	public class BulletTrailEffect : MonoBehaviour {
		[SerializeField]
		private float timeToLive = 0.1f;
		[SerializeField]
		private float fadeDuration = 0.1f;

		private LineRenderer lineRenderer;

		private void Awake () {
			lineRenderer = GetComponent<LineRenderer> ();
		}

		private void OnEnable () {
			Invoke ("PlayTrail", 0f);
		}

		private void OnDisable () {
			LeanTween.cancel (gameObject);
			CancelInvoke ();
		}

		/// <summary>
		/// Plays a "trail render" effect to give the illusion that the bullet is flying
		/// </summary>
		private void PlayTrail () {
			var start = lineRenderer.GetPosition (0);
			var end = lineRenderer.GetPosition (1);
			LeanTween.value (gameObject, start, end, fadeDuration)
				.setOnUpdate ((Vector3 v) => { lineRenderer.SetPosition (0, v); })
				.setEaseInOutCubic ()
				.setDelay (timeToLive);
		}

	}
}