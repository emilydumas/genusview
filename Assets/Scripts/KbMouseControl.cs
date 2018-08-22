using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseMode { Look, Stick };

public class KbMouseControl : MonoBehaviour {
	public GameObject camera;
	public GameObject stickHolder;
	private StickBehavior sb;
	private MouseMode mouseMode = MouseMode.Look;

	public float speed = 10.0f;
	public float mouseSpeed = 5.0f;

	void Start() {
		sb = stickHolder.GetComponent<StickBehavior>();
	}

	void Update () {
		float horiz = Input.GetAxis ("Horizontal") * speed;
		float depth = Input.GetAxis ("Vertical") * speed;

		float lookh = Input.GetAxis("Mouse X") * mouseSpeed;
		float lookv = Input.GetAxis("Mouse Y") * mouseSpeed;

		float dt = Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Space)) {
			if (mouseMode == MouseMode.Look) {
				mouseMode = MouseMode.Stick;
				sb.makeVisible();
				sb.startDrawing();
			} else {
				sb.stopDrawing();
				sb.makeInvisible();
				mouseMode = MouseMode.Look;
			}
		}

		camera.transform.Translate (horiz * dt, 0f, depth * dt);
		if (mouseMode == MouseMode.Look) {
			camera.transform.Rotate(0,lookh,0);
		}
	}
}
