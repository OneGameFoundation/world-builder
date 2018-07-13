using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {
	[System.Serializable]
	public struct Vector3Double {
		public double x;
		public double y;
		public double z;

		public Vector3Double (float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3Double (Vector3 vector) {
			x = vector.x;
			y = vector.y;
			z = vector.z;
		}

		public Vector3 ToVector3 () {
			return new Vector3 ((float) x, (float) y, (float) z);
		}
	}
}