// Help screen support

// Attach this script to an object that has a "Panel" child and a 
// "Text" descendent. Then this script will show/hide the Panel when
// requested, and will load the help text into the contents of the
// Text descendent.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpScreen : MonoBehaviour {
	private TextAsset helpContent;
	private Text textChild;
	private GameObject panelChild;
	private bool initOK = false;
	[HideInInspector] public bool visible = false;

	GameObject FirstChildPanel() {
		foreach (Transform child in transform) {
			GameObject go = child.gameObject;
			CanvasRenderer cr = go.GetComponent<CanvasRenderer>();
			if (cr != null) {
				return go;
			}
		}
		return null;
	}

	Text FirstChildText() {
		Text[] texts = GetComponentsInChildren<Text>(true);
		if (texts == null || texts.Length == 0) {
			return null;
		}
		return texts[0];
	}

	void Awake () {
		helpContent = Resources.Load<TextAsset>("Text/help");
		textChild = FirstChildText();
		if (helpContent != null && textChild != null) {
			textChild.text = helpContent.text;
		}
		panelChild = FirstChildPanel();
		initOK = (helpContent != null) && (textChild != null) && (panelChild != null);
		SetVisible(visible); // make actual visibility consistent with initial setting
	}
	
	public void SetVisible(bool b) {
		if (panelChild != null) {
			panelChild.SetActive(b);
		}
		visible = b;
	}

	public void Hide () {
		SetVisible(false);
	}

	public void Show () {
		SetVisible(true);
	}

	public void ToggleVisibility() {
		SetVisible(!visible);
	}
}

