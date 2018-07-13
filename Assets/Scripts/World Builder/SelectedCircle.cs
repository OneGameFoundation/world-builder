using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCircle : MonoBehaviour {

	[SerializeField, Range (3, 256)]
	private int numSegments = 128;

	public void Activate (Vector3 position, Bounds bounds) {
		gameObject.SetActive (true);
		transform.position = position;

		DoRenderer (GetRadius (bounds));
	}

	public void Deactivate () {
		gameObject.SetActive (false);
	}

	public void DoRenderer (float radius) {
		LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer> ();
		Color c1 = new Color (0.2f, 1f, 0.2f, 1f);

		// Set the colors
		lineRenderer.startColor = c1;
		lineRenderer.endColor = c1;

		// Set the width
		lineRenderer.startWidth = 0.1f;
		lineRenderer.endWidth = 0.1f;

		lineRenderer.positionCount = numSegments + 1;

		// lineRenderer.SetColors (c1, c1);
		// lineRenderer.SetWidth (0.1f, 0.1f);
		// lineRenderer.SetVertexCount (numSegments + 1);
		lineRenderer.useWorldSpace = false;

		float deltaTheta = (float) (2.0 * Mathf.PI) / numSegments;
		float theta = 0f;

		for (int i = 0; i < numSegments + 1; i++) {
			float x = radius * Mathf.Cos (theta);
			float z = radius * Mathf.Sin (theta);
			Vector3 pos = new Vector3 (x, 0, z);
			lineRenderer.SetPosition (i, pos);
			theta += deltaTheta;
		}
	}

	private float GetRadius (Bounds bounds) {
		float x = bounds.extents.x;
		float z = bounds.extents.z;
		return Mathf.Sqrt (x * x + z * z);
	}
}