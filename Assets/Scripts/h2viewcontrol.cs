// Script companion to the hypview shader which exposes the Poincare/Klein
// shader properties as public member functions.

// NOTE: Both the hypview shader (for drawing H^2 tiling) and the h2paint shader
// (for adding a spot to the H^2 tiling) need to be set to the same mode at all
// times!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ViewMode { Poincare, Klein };

public class h2viewcontrol : MonoBehaviour {
	public ViewMode viewMode;
	private Material m = null;  // Target material with hypview shader
	private Material pm = null;  // Target material with h2paint shader

	void Start () {
		Renderer r = gameObject.GetComponent<Renderer>();

		if (r != null) {
			m = r.material;
		}

		PaintData pd = gameObject.GetComponent<PaintData>();
		pm = pd.paintMaterial;

		ExportMode();
	}

	public void Toggle() {
		if (viewMode == ViewMode.Poincare) {
			viewMode = ViewMode.Klein;
		} else {
			viewMode = ViewMode.Poincare;
		}
	}

	public void ExportMode() {
		// Set the shader property to the current value of viewMode.
		int p;

		if (viewMode == ViewMode.Poincare) {
			p = 1;
		} else {
			p = 0;
		}
		if (m != null) {
			m.SetInt("_Poincare",p);
		}
		if (pm != null) {
			pm.SetInt("_Poincare",p);
		}

	}
}
