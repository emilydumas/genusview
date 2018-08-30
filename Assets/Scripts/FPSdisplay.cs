// Display FPS in lower right of screen
// Based on FPSDisplay by Dave Hampson (http://wiki.unity3d.com/index.php?title=FramesPerSecond)
using UnityEngine;
using System.Collections;
 
public class FPSdisplay : MonoBehaviour
{
    public Color indicatorColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
	private float deltaTime = 0.0f;
 
	void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}
 
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
 
		GUIStyle style = new GUIStyle();
 
		Rect rect = new Rect(w/2,h/2,w/2,h/2);
		style.alignment = TextAnchor.LowerRight;
		style.fontSize = 12;
		style.normal.textColor = indicatorColor;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.} fps", fps);
		GUI.Label(rect, text, style);
	}
}