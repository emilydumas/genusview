using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickControl : MonoBehaviour {
	public float maxRotation = 90f;
	public float maxDist = Mathf.Infinity;
	private Quaternion initQ;
	private Renderer[] rends;

	void Start () {
		Cursor.visible = false;
		rends = gameObject.GetComponentsInChildren<Renderer>();
		initQ = transform.localRotation;
		makeInvisible();
	}

	private Vector2 RelMousePos() {
		return new Vector2 (2 * Input.mousePosition.x / Screen.width - 1, 2 * Input.mousePosition.y / Screen.height - 1);
	}

	void setVisibility(bool b)
	{	
		foreach (Renderer r in rends) {
            r.enabled = b;
        }
    }

	void makeInvisible()
	{
		setVisibility(false);
    }

	void makeVisible() {
		setVisibility(true);
	}

	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			makeVisible();
		}
		if (Input.GetMouseButtonUp(0)) {
			makeInvisible();
		}
		if (Input.GetMouseButton(0)) {
			Vector2 mp = RelMousePos ();
			transform.localRotation = initQ * Quaternion.Euler(-0.5f*maxRotation*mp.y,0,-0.5f*maxRotation*mp.x);
			PaintFirstHit();
		}
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
