using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour {
	public float speed = 10.0f;
	public float mouseSpeed = 5.0f;

	void Update () {
		float horiz = Input.GetAxis ("Horizontal") * speed;
		float depth = Input.GetAxis ("Vertical") * speed;

		float lookh = Input.GetAxis("Mouse X") * mouseSpeed;
		float lookv = Input.GetAxis("Mouse Y") * mouseSpeed;

		float dt = Time.deltaTime;

		transform.Translate (horiz * dt, 0f, depth * dt);
		if (!Input.GetMouseButton(0)) {
			transform.Rotate(0,lookh,0);
		}
	}
}
