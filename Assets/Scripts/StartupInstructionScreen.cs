// Help screen support

// Attach this script to an object that has a "Panel" child and a 
// "Text" descendent. Then this script will show/hide the Panel when
// requested, and will load the help text into the contents of the
// Text descendent.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartupInstructionScreen : MonoBehaviour {
    public float holdTime = 0.5f;
    public float fadeTime = 0.25f;
    private Image[] images;
    private Dictionary<Image,Color> imageInitColors = new Dictionary<Image,Color>();
    private Text[] texts;
    private Dictionary<Text, Color> textInitColors = new Dictionary<Text, Color>(); 

	void Awake () {
        texts = GetComponentsInChildren<Text>();
        images = GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            imageInitColors[img] = img.color;
        }
        foreach (Text t in texts)
        {
            textInitColors[t] = t.color;
        }
	}
	
    Color _toTrans(Color c)
    {
        return new Color(c.r, c.g, c.b, 0f);
    }

    void Update()
    {
        if (Time.time >  (holdTime + fadeTime))
        {
            gameObject.SetActive(false);
            return;
        }
        if (Time.time > holdTime)
        {
            float s = (Time.time - holdTime) / fadeTime;
            foreach (Text t in texts)
            {
                Color c = textInitColors[t];
                t.color = Color.Lerp(c,_toTrans(c),s);
            }
            foreach (Image img in images)
            {
                Color c = imageInitColors[img];
                img.color = Color.Lerp(c, _toTrans(c), s);
            }
        }
    }
}

