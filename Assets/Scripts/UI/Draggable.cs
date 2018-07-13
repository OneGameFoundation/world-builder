using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {

	/// <summary>
	/// A simple class to make anything draggable
	/// </summary>
	public class Draggable : MonoBehaviour {

		float offsetX;
		float offsetY;

		/// <summary>
		/// calculate the offset when dragging started
		/// </summary>
		public void BeginDrag () {
			offsetX = transform.position.x - Input.mousePosition.x;
			offsetY = transform.position.y - Input.mousePosition.y;
		}

		/// <summary>
		/// during dragging, update position
		/// </summary>
		public void OnDrag () {
			transform.position = new Vector3 (offsetX + Input.mousePosition.x, offsetY + Input.mousePosition.y);
		}

	}
}