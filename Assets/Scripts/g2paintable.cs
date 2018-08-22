using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class g2paintable : MonoBehaviour {
	public float spotSize = 0.001f;
	public Material paintMaterial;
	private Material m;
	private Texture baseTexture;
	private int mainTexturePropertyID;
	private int paintUVPropertyID;
	private RenderTexture rt;

	void Start () {
        Material sm = gameObject.GetComponent<Renderer>().sharedMaterial;

		m = gameObject.GetComponent<Renderer>().material; // This will generate and return a copy!
		baseTexture = m.mainTexture;
		mainTexturePropertyID = Shader.PropertyToID("_MainTex");

		paintUVPropertyID = Shader.PropertyToID ("_PaintUV");
		paintMaterial.SetFloat ("_SpotSize", spotSize);

		// Create a new rendertexture that is initially a copy of the current texture
		rt = new RenderTexture (baseTexture.width, baseTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		rt.filterMode = baseTexture.filterMode;
		Graphics.Blit(baseTexture, rt);
		m.SetTexture(mainTexturePropertyID, rt);

        // Find all objects that used the baseTexture, and set them to use the copy instead.
        GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject go in gos)
        {
            Renderer rend = go.GetComponent<Renderer>(); 
            if (rend == null)
            {
                continue;
            }

	
            if (rend.sharedMaterial.mainTexture == baseTexture)
            {
                Debug.Log("Setting " + go.name + " to use shared rendermaterial");
				Material otherm = rend.material;  // Generates a copy.
				otherm.SetTexture(mainTexturePropertyID, rt);
            }
        }
	}

	public void PaintUV(Vector2 uv) {
		RenderTexture buffer = RenderTexture.GetTemporary(rt.width, rt.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		paintMaterial.SetVector (paintUVPropertyID, new Vector4(uv.x,uv.y,0,0));
		Graphics.Blit(rt, buffer, paintMaterial);
		Graphics.Blit(buffer, rt);
		RenderTexture.ReleaseTemporary(buffer);
	}
}
