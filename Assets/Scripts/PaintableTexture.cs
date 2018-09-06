// Singleton that makes a RenderTexture copy of a given texture, and then tells
// every GameObject using that texture to use the RenderTexture instead.

// Missing feature: There is no support to add new GameObjects using the original Texture
// in a script and still have them altered to replace with RenderTexture copy.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on singleton implementation from https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
public class PaintableTexture : MonoBehaviour {
    public Texture target;
    public float spotSize = 0.001f;
	public Color paintColor = new Color(0,0,0,1);

    private static PaintableTexture _instance;
    private int mainTexturePropertyID;
    private int paintUVPropertyID;
    private int spotColorPropertyID;
    private int spotSizePropertyID;
    private RenderTexture rt;
    private Dictionary<GameObject,Material> paintMaterials = new Dictionary<GameObject,Material>();

    public static PaintableTexture Instance
    {
        get {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<PaintableTexture>();
                if (_instance == null) {
                    Debug.LogError("There must be a PaintableTexture script in the scene!");
                }
            }
            return _instance;
        } 
    }
    
    private void Awake()
    {
        // Ensure single instance
        if (_instance != null && _instance != this)
        {
            Debug.LogError("Awake() called on a second PaintableTexture object.  There must be only one PaintableTexture script in the scene.  Removing: " + this.gameObject.name);
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        // Actual initialization
        mainTexturePropertyID = Shader.PropertyToID("_MainTex");
        paintUVPropertyID = Shader.PropertyToID("_PaintUV");
        spotColorPropertyID = Shader.PropertyToID("_SpotColor");
        spotSizePropertyID = Shader.PropertyToID("_SpotSize");
    }

    private void Start() {
        // Bugfix note: Previously we did this texture duplication in Awake(),
        // but it seems that Awake() is sometimes called before texture loading
        // is complete and this results in Graphics.Blit() writing black pixels
        // to the RenderTexture.  Moving this initialization to Start() fixed
        // that problem.
        ReplaceTextureWithRenderTexture(target);
    }
    private void ReplaceTextureWithRenderTexture(Texture t) {
    	// Create a new rendertexture like t
		rt = new RenderTexture (t.width, t.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		rt.filterMode = t.filterMode;
		// Copy t to it
        Graphics.Blit(t, rt);
        // Find all objects that use t, and set them to use rt instead.
        GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject go in gos)
        {
            Renderer rend = go.GetComponent<Renderer>(); 
            if (rend == null)
            {
                continue;
            }
            if (rend.sharedMaterial.mainTexture == t)
            {
                Debug.Log("Setting " + go.name + " to use shared RenderTexture");
				Material mcopy = rend.material;  // Generates a copy.
				mcopy.SetTexture(mainTexturePropertyID, rt);
                PaintData pd = go.GetComponent<PaintData>();
                if (pd != null ) {
                    paintMaterials[go] = pd.paintMaterial;
                } else {
                    paintMaterials[go] = null;
                }
            }
        }
    }
    public void PaintUV(GameObject obj, Vector2 uv) {
		// Draw a new spot on the rendertexture at (u,v)
        Material m = paintMaterials[obj];
        RenderTexture buffer = RenderTexture.GetTemporary(rt.width, rt.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                
        m.SetVector (spotColorPropertyID, paintColor);
        m.SetFloat (spotSizePropertyID, spotSize);
		m.SetVector (paintUVPropertyID, new Vector4(uv.x,uv.y,0,0));
		Graphics.Blit(rt, buffer, m);
		Graphics.Blit(buffer, rt);
		RenderTexture.ReleaseTemporary(buffer);
	}

    public void Clear() {
		Graphics.Blit(target,rt);
	}
}