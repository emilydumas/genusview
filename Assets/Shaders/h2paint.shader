// Shader for drawing a spot in the hyperbolic plane on a 6-panel v1 texture map
// Inspired by "InkPainter", https://github.com/EsProgram/InkPainter
// David Dumas <david@dumas.io>

Shader "h2paint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_PaintUV("Position to paint",VECTOR) = (0,0,0,0)
		_SpotColor("Color of spot to paint",VECTOR) = (1,1,1,1)
		_SpotSize("Size of spot (as cosh(hyperbolic radius)-1)",FLOAT) = 0.1
		_Poincare("Draw in Poincare disk?", INT) = 1
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
			uniform int _Poincare;

			// Apply this transformation to the point before rendering
			// Should be an element of SO(2,1)
			// We store as 4x4 matrix since that is what Material.SetMatrix allows.
			float4x4 _PreTransformation = float4x4(1, 0, 0, 0,   0, 1, 0, 0,   0, 0, 1, 0,  0, 0, 0, 1);

			#include "UnityCG.cginc"
			#include "hyputil.cginc"
			
			float4 frag (v2f_img i) : COLOR
			{
				vect_in_fund vif = six_panel_uv_to_vif(i.uv);

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				if (vif.coset > 5)
					return col;
				
				float2 xy = 2.0*(_PaintUV.xy - float2(0.5,0.5));
				if (length(xy) >= 1)
					return col;
				float pcoef = (2.0*_Poincare / (1.0 + dot(xy,xy))) + (1.0-_Poincare);
				xy = pcoef*xy;

				float3 raw_spot_center = fromklein(xy);

				// Promote raw_spot_center to 4-vector, transform, and demote
				float4 v4 = float4(raw_spot_center.x,raw_spot_center.y,raw_spot_center.z,0);
				v4 = mul(_PreTransformation,v4);
				raw_spot_center = float3(v4.x,v4.y,v4.z);

				vect_in_fund vifspot = tofund(raw_spot_center);
				if (vifspot.coset > 5)
					return col;

				float d = g2_quasi_dist(vif, vifspot);
				if (d > _SpotSize)
					return col;
				else
					return _SpotColor;
			}
			ENDCG
		}
	}
}
