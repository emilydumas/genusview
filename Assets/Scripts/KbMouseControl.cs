// Input manager for keyboard and mouse control

// Detects keypresses or mouse movements and calls methods of other scripts in
// the scene to activate the associated behavior.

// Currently this script is linked to lots of other GameObjects through public
// properties that need to be configured in the Editor.  It may be better to
// configure some of these as singletons that this script can retrieve an
// instance of at runtime.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MouseMode controls what mouse movements affect:
public enum MouseMode {
	Look,   // horizontal direction of camera
	Stick,  // position of the stick/laser
	Rotate  // orientation of the genus two surface
};

public class KbMouseControl : MonoBehaviour {
	public GameObject camera;
	public GameObject stickHolder;
	public GameObject surface;
	public GameObject helpScreenParent;
	public GameObject h2view;
	public float turnRange = 180f;  // Mouse sensitivity for horizontal turns
	public float stickRange = 90f;  // Laser/stick sensitivity
	public float h2speed = 3f;
	private StickBehavior sb;
	private h2viewcontrol h2c;
	private MouseMode mouseMode = MouseMode.Look;
	private MouseMode savedMode = MouseMode.Look;
	private Quaternion stickInitQ, cameraInitQ, surfaceInitQ;
	private PaintableTexture pt = null;
	private HelpScreen helpScreen;
	private Vector3 surfaceDelta;

	public float speed = 10.0f;
	public float mouseSpeed = 4.0f;

	private Vector2 mpos = new Vector2(0,0);

	void Start() {
		pt = PaintableTexture.Instance;
		// Todo: replace below with singletons to avoid need for linking in the editor
		sb = stickHolder.GetComponent<StickBehavior>();
		helpScreen = helpScreenParent.GetComponent<HelpScreen>();
		h2c = h2view.GetComponent<h2viewcontrol>();
		stickInitQ = stickHolder.transform.localRotation;
		cameraInitQ = camera.transform.localRotation;
		surfaceInitQ = surface.transform.localRotation;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void doQuit() {
		// Exit the application (builds) or stop the player (editor)
		#if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false;
         #else
         Application.Quit();
         #endif
	}

	void OnApplicationQuit() {
		Cursor.lockState = CursorLockMode.None;
	}

	Vector2 AbsMousePos() {
		// GetAxis mouse movement is reported as a delta by default
		// Convert to absolute position
		mpos.x += 20*Input.GetAxis("Mouse X") / Screen.width;
		mpos.y += 20*Input.GetAxis("Mouse Y") / Screen.height;
		return mpos;
	}

	void ResetMousePos() {
		// Consider the current mouse position to be (0,0) but don't change the
		// rotation of anything in the scene. Since (0,0) means "no rotation
		// relative to the stored orientation" that means we must update the
		// stored orientation.
		mpos.x = 0f;
		mpos.y = 0f;
		stickInitQ = stickHolder.transform.localRotation;
		cameraInitQ = camera.transform.localRotation;
		surfaceInitQ = surface.transform.localRotation;
	}

	void Update () {
		float dt = Time.deltaTime;

		if  ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Q)) {
			doQuit();
		}

		if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.F1)) {
			helpScreen.ToggleVisibility();
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			helpScreen.Hide();
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			ResetMousePos();
			// Space bar toggles mode (walk/draw)
			if (mouseMode == MouseMode.Look) {
				mouseMode = MouseMode.Stick;
				sb.makeVisible();
			} else if (mouseMode == MouseMode.Stick) {
				sb.makeInvisible();
				mouseMode = MouseMode.Look;
			}
		}

		if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt) || Input.GetMouseButtonDown(1)) {
			// Alt or RMB means "temporarily activate rotate mode"
			// Save current mode, make sure the stick/laser is hidden, and link
			// mouse position to object orientation.
			ResetMousePos();
			savedMode = mouseMode;
			if (mouseMode == MouseMode.Stick) {
				sb.makeInvisible();
			}
			mouseMode = MouseMode.Rotate;
			surfaceDelta = surface.transform.position - camera.transform.position; 
		}
		if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt) || Input.GetMouseButtonUp(1)) {
			// Alt or RMB release means restore previous mode
			ResetMousePos();
			mouseMode = savedMode;
			if (mouseMode == MouseMode.Stick) {
				sb.makeVisible();
			}
		}

		if (Input.GetKeyDown(KeyCode.Z)) {
			// Clear all drawing on the PaintableTexture
			pt.Clear();
		}

		if (Input.GetKeyDown(KeyCode.P)) {
			// Klein-Poincare toggle
			h2c.Toggle();
			h2c.ExportMode();
		}

		if (Input.GetKeyDown(KeyCode.O)) {
			// Reset view position
			h2c.ResetPreTransformation();
			h2c.ExportPreTransformation();
		}
		if (Input.GetKey(KeyCode.I)) {
			h2c.ComposePreTransformation(HypUtil.BoostY(-h2speed*dt));
			h2c.ExportPreTransformation();
		}

		if (Input.GetKey(KeyCode.K)) {
			h2c.ComposePreTransformation(HypUtil.BoostY(h2speed*dt));
			h2c.ExportPreTransformation();
		}
		if (Input.GetKey(KeyCode.J)) {
			h2c.ComposePreTransformation(HypUtil.BoostX(h2speed*dt));
			h2c.ExportPreTransformation();
		}
		if (Input.GetKey(KeyCode.L)) {
			h2c.ComposePreTransformation(HypUtil.BoostX(-h2speed*dt));
			h2c.ExportPreTransformation();
		}


		// Walk & strafe according to keyboard
		float horiz = Input.GetAxis ("Horizontal") * speed;
		float depth = Input.GetAxis ("Vertical") * speed;
		camera.transform.Translate (horiz * dt, 0f, depth * dt);
	    if (mouseMode == MouseMode.Rotate) {
			surface.transform.position = camera.transform.position + surfaceDelta;
		}

		if (mouseMode == MouseMode.Look) {
			Vector2 mp = AbsMousePos ();
			camera.transform.localRotation = cameraInitQ * Quaternion.Euler(0,turnRange*mp.x,0);
		}
		if (mouseMode == MouseMode.Stick) {
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(0)) {
				sb.startDrawing();
			} else {
				sb.stopDrawing();
			}
			Vector2 mp = AbsMousePos ();
			stickHolder.transform.localRotation = stickInitQ * Quaternion.Euler(-0.5f*stickRange*mp.y,0,-0.5f*stickRange*mp.x);
		}
		if (mouseMode == MouseMode.Rotate) {
			Vector2 mp = AbsMousePos ();
			// TODO: Make this a more intuitive trackball-style object rotation interface.
			surface.transform.localRotation = Quaternion.AngleAxis(turnRange*mp.y,camera.transform.right) * Quaternion.AngleAxis(-turnRange*mp.x,camera.transform.up) * surfaceInitQ;
		}
	}
}
