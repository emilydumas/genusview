using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ViewMode { Poincare, Klein };

public class h2viewcontrol : MonoBehaviour {
	public ViewMode viewMode;
	private Material m = null;
	private Material pm = null;

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
