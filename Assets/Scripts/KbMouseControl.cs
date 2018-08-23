using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseMode { Look, Stick, Rotate };

public class KbMouseControl : MonoBehaviour {
	public GameObject camera;
	public GameObject stickHolder;
	public GameObject surface;
	public float turnRange = 180f;
	public float stickRange = 90f;
	private StickBehavior sb;
	private MouseMode mouseMode = MouseMode.Look;
	private MouseMode savedMode = MouseMode.Look;
	private Quaternion stickInitQ, cameraInitQ, surfaceInitQ;
	private g2paintable[] paintables;
	private Vector3 surfaceDelta;

	public float speed = 10.0f;
	public float mouseSpeed = 5.0f;

	private Vector2 mpos = new Vector2(0,0);

	void Start() {
		paintables = FindObjectsOfType<g2paintable>();
		sb = stickHolder.GetComponent<StickBehavior>();
		stickInitQ = stickHolder.transform.localRotation;
		cameraInitQ = camera.transform.localRotation;
		surfaceInitQ = surface.transform.localRotation;
		Cursor.lockState = CursorLockMode.Locked;
	}

	void doQuit() {
		#if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false;
         #else
         Application.Quit();
         #endif
	}

	void OnApplicationQuit() {
		Cursor.lockState = CursorLockMode.None;
	}

	Vector2 RelMousePos() {
		mpos.x += 20*Input.GetAxis("Mouse X") / Screen.width;
		mpos.y += 20*Input.GetAxis("Mouse Y") / Screen.height;
		return mpos;
	}

	void ResetMousePos() {
		mpos.x = 0f;
		mpos.y = 0f;
		stickInitQ = stickHolder.transform.localRotation;
		cameraInitQ = camera.transform.localRotation;
		surfaceInitQ = surface.transform.localRotation;
	}

	void Update () {
		float dt = Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Escape)) {
			doQuit();
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			ResetMousePos();
			// Space bar toggles mode (walk/draw)
			if (mouseMode == MouseMode.Look) {
		//			stickInitQ = stickHolder.transform.localRotation;
				mouseMode = MouseMode.Stick;
				sb.makeVisible();
			} else if (mouseMode == MouseMode.Stick) {
				sb.makeInvisible();
				mouseMode = MouseMode.Look;
//				cameraInitQ = camera.transform.localRotation;
			}
		}

		if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetMouseButtonDown(1)) {
			ResetMousePos();
//			surfaceInitQ = surface.transform.localRotation;
			savedMode = mouseMode;
			if (mouseMode == MouseMode.Stick) {
				sb.makeInvisible();
			}
			mouseMode = MouseMode.Rotate;
			surfaceDelta = surface.transform.position - camera.transform.position; 
		}
		if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetMouseButtonUp(1)) {
			ResetMousePos();
			mouseMode = savedMode;
			if (mouseMode == MouseMode.Stick) {
				sb.makeVisible();
			}
		}

		if (Input.GetKeyDown(KeyCode.Z)) {
			foreach (g2paintable p in paintables) {
				p.Clear();
			}
		}

		// Walk & strafe according to keyboard
		float horiz = Input.GetAxis ("Horizontal") * speed;
		float depth = Input.GetAxis ("Vertical") * speed;
		camera.transform.Translate (horiz * dt, 0f, depth * dt);
	    if (mouseMode == MouseMode.Rotate) {
			surface.transform.position = camera.transform.position + surfaceDelta;
		}		

		if (mouseMode == MouseMode.Look) {
			Vector2 mp = RelMousePos ();
			camera.transform.localRotation = cameraInitQ * Quaternion.Euler(0,turnRange*mp.x,0);
		}
		if (mouseMode == MouseMode.Stick) {
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(0)) {
				sb.startDrawing();
			} else {
				sb.stopDrawing();
			}
			Vector2 mp = RelMousePos ();
			stickHolder.transform.localRotation = stickInitQ * Quaternion.Euler(-0.5f*stickRange*mp.y,0,-0.5f*stickRange*mp.x);
		}
		if (mouseMode == MouseMode.Rotate) {
			Vector2 mp = RelMousePos ();
			surface.transform.localRotation = Quaternion.AngleAxis(turnRange*mp.y,camera.transform.right) * Quaternion.AngleAxis(-turnRange*mp.x,camera.transform.up) * surfaceInitQ;
		}
	}
}
