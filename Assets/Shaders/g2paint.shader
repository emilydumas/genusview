// Shader for drawing a spot on a 6-panel v1 texture map 
// Inspired by "InkPainter", https://github.com/EsProgram/InkPainter
// David Dumas <david@dumas.io>

Shader "g2paint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_PaintUV("Position to paint",VECTOR) = (0,0,0,0)
		_SpotColor("Color of spot to paint",VECTOR) = (1,1,1,1)
		_SpotSize("Size of spot (as cosh(hyperbolic radius)-1)",FLOAT) = 0.1
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			uniform sampler2D _MainTex;
			uniform float4 _PaintUV;
			uniform float4 _SpotColor;
			uniform float _SpotSize;
			
			#include "UnityCG.cginc"
			#include "hyputil.cginc"
			
			float4 frag (v2f_img i) : COLOR
			{
				vect_in_fund vif = six_panel_uv_to_vif(i.uv);
				vect_in_fund vifspot = six_panel_uv_to_vif(_PaintUV.xy);

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				if (vif.coset != vifspot.coset) 
					return col;
				if (vif.coset == 255)
					return col;
	
				float d = hyp_quasi_dist(vif.v, vifspot.v);
				if (d > _SpotSize)
					return col;
				else
					return _SpotColor;
			}
			ENDCG
		}
	}
}
