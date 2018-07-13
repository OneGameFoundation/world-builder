using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua {
	/// <summary>
	/// A C# representation of a Vector3
	/// </summary>
	[MoonSharpUserData]
	public struct Vec3 {
		public float x;
		public float y;
		public float z;

		public float magnitude { get { return Mathf.Sqrt (x * x + y * y + z * z); } }

		public Vec3 normalized {
			get {
				var magnitude = this.magnitude;
				return new Vec3 (x / magnitude, y / magnitude, z / magnitude);
			}
		}

		public Vec3 (float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vec3 (Vector3 vector) {
			x = vector.x;
			y = vector.y;
			z = vector.z;
		}

		public Vector3 ToVector3 () {
			return new Vector3 (x, y, z);
		}

		public static float Distance (Vec3 lhs, Vec3 rhs) {
			var x = lhs.x - rhs.x;
			var y = lhs.y - rhs.y;
			var z = lhs.z - rhs.z;
			return Mathf.Sqrt (x * x + y * y + z * z);
		}

		public static Vec3 operator + (Vec3 lhs, Vec3 rhs) {
			return new Vec3 (lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
		}

		public static Vec3 operator - (Vec3 lhs, Vec3 rhs) {
			return new Vec3 (lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
		}

		public static Vec3 operator * (Vec3 lhs, float rhs) {
			return new Vec3 (lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
		}

		public static Vec3 operator / (Vec3 lhs, float rhs) {
			return new Vec3 (lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
		}

		public override string ToString () {
			return string.Format ("({0}, {1}, {2})", x, y, z);
		}
	}
}