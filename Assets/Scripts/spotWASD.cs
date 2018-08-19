using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spotWASD : MonoBehaviour {
	public float speed = 10.0f;
	public float spotSize = 0.001f;
	public Material paintMaterial;
	private Vector2 location = new Vector2(0,0);
	private Material m;
	private Texture baseTexture;
	private int mainTexturePropertyID;
	private int paintXYPropertyID;
	private RenderTexture rt;

	void Start () {
        Material sm = gameObject.GetComponent<Renderer>().sharedMaterial;

		m = gameObject.GetComponent<Renderer>().material; // This will generate and return a copy!
		baseTexture = m.mainTexture;
		mainTexturePropertyID = Shader.PropertyToID("_MainTex");

		paintXYPropertyID = Shader.PropertyToID ("_PaintXY");
		paintMaterial.SetFloat ("_SpotSize", spotSize);

		// Create a new rendertexture that is initially a copy of the current texture
		rt = new RenderTexture (baseTexture.width, baseTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		rt.filterMode = baseTexture.filterMode;
		Graphics.Blit(baseTexture, rt);
		m.SetTexture(mainTexturePropertyID, rt);

        // Set all objects that previously used the shared material to use the copy
        GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject go in gos)
        {
            Renderer rend = go.GetComponent<Renderer>();
            if (rend == null)
            {
                continue;
            }
            if (rend.sharedMaterial == sm)
            {
                Debug.Log("Setting material for " + go.name);
                rend.sharedMaterial = m;
            }
        }
	}


	void Update () {
		float horiz = Input.GetAxis ("Horizontal") * speed;
		float vert = Input.GetAxis ("Vertical") * speed;
		float dt = Time.deltaTime;

		location = location - new Vector2(horiz * dt, vert * dt);
		PaintXY(location);
	}

	public void PaintXY(Vector2 xy) {
		RenderTexture buffer = RenderTexture.GetTemporary(rt.width, rt.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		paintMaterial.SetVector (paintXYPropertyID, new Vector4(xy.x,xy.y,0,0));
		Graphics.Blit(rt, buffer, paintMaterial);
		Graphics.Blit(buffer, rt);
		RenderTexture.ReleaseTemporary(buffer);
	}
}
