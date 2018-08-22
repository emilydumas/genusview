// NOT CURRENTLY USED.
// This functionality has been moved into "StickControl"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintIntersection : MonoBehaviour {
	public float maxDist = Mathf.Infinity;

    void Update() {
        if (Input.GetMouseButton(0))
            PaintFirstHit();
    }

    void PaintFirstHit() {
        var raydir = transform.TransformDirection(Vector3.up);
        RaycastHit hit;

        if (Physics.Raycast (transform.position, raydir, out hit, maxDist)) {
			GameObject g = hit.transform.gameObject;
			g2paintable p = g.GetComponent<g2paintable> ();
			if (p != null) {
				p.PaintUV (hit.textureCoord);
			}
		}
	}
}
