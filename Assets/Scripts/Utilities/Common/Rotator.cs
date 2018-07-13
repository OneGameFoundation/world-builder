using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {
	/// <summary>
	/// A simple utility class that rotates an object along an axis
	/// </summary>
	public class Rotator : MonoBehaviour {

		[SerializeField]
		private Vector3 axis;
		[SerializeField]
		private float speed = 60f;

		private void LateUpdate () {
			var rotation = transform.eulerAngles;
			var amount = speed * Time.deltaTime;

			rotation.x += axis.x * amount;
			rotation.y += axis.y * amount;
			rotation.z += axis.z * amount;

			transform.eulerAngles = rotation;
		}
	}
}