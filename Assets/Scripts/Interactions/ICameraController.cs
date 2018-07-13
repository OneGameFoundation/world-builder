using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {
	public interface ICameraController {
		/// <summary>
		/// Moves the camera on the x and z axis, as long as the mouse is near the edges of the screen
		/// </summary>
		/// <param name="mousePosition">The position of the player mouse</param>
		void PanCamera (Vector2 mousePosition);

		/// <summary>
		/// Moves the camera on x and z by Horizontal and Vertical input axis values
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		void PanCameraAxis (float x, float y);

		/// <summary>
		/// Resets the camera's transform values 
		/// </summary>
		void ResetCamera ();

		/// <summary>
		/// Resets the camera's transform values instantly
		/// </summary>
		void ResetCameraInstant ();

		/// <summary>
		/// Rotates the camera by a certain angle
		/// </summary>
		/// <param name="delta">The amount to rotate by</param>
		void RotateCamera (float delta);

		/// <summary>
		/// Zooms the camera, based on the delta value
		/// </summary>
		void ZoomCamera (float delta);
	}
}