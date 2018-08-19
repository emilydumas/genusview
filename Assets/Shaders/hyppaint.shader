// Shader for drawing a spot in the hyperbolic plane on a 6-panel v1 texture map
// Inspired by "InkPainter", https://github.com/EsProgram/InkPainter
// David Dumas <david@dumas.io>

Shader "hyppaint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_PaintXY("Position to paint",VECTOR) = (0,0,0,0)
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
			uniform float4 _PaintXY;
			uniform float4 _SpotColor;
			uniform float _SpotSize;
			
			#include "UnityCG.cginc"
			#include "hyputil.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
			};
			
			float4 frag (v2f_img i) : COLOR
			{
				vect_in_fund vif = six_panel_uv_to_vif(i.uv);
				float4 baseColor = tex2D(_MainTex,i.uv);

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				if (length(_PaintXY) >= 1)
					return col;

				if (vif.coset == 255)
					return col;
				
				float3 raw_spot_center = fromklein(_PaintXY.xy);
				vect_in_fund spot = tofund(raw_spot_center);
				float d = hyp_quasi_dist(spot.v, vif.v);
				if (d > _SpotSize)
					return col;
				else
					return _SpotColor;
			}
			ENDCG
		}
	}
}
