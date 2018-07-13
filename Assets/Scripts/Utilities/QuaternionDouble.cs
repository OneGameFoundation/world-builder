using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {
	[System.Serializable]
	public struct QuaternionDouble {
		public double x;
		public double y;
		public double z;
		public double w;

		public QuaternionDouble (double x, double y, double z, double w) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public QuaternionDouble (Quaternion q) {
			this.x = q.x;
			this.y = q.y;
			this.z = q.z;
			this.w = q.w;
		}

		public Quaternion ToQuaternion () {
			return new Quaternion ((float) x, (float) y, (float) z, (float) w);
		}

	}
}