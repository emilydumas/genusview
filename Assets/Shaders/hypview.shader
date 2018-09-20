// Take a 6-panel Klein model texture (6-fold square tiling)
// and show it in either the Klein or Poincare model
// (with infinite resolution!)

Shader "Unlit/hypview"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Poincare("Draw in Poincare disk?", INT) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Cull Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			int _Poincare;

			// Apply this transformation to the point before rendering
			// Should be an element of SO(2,1)
			// We store as 4x4 matrix since that is what Material.SetMatrix allows.
			float4x4 _PreTransformation = float4x4(1, 0, 0, 0,   0, 1, 0, 0,   0, 0, 1, 0,  0, 0, 0, 1);
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			#include "hyputil.cginc"

			fixed4 frag (v2f i) : SV_Target
			{
				float2 xy = 2.0*(i.uv - float2(0.5,0.5));
				float2 uv = float2(0,0);


				UNITY_BRANCH
				if (length(xy) < 1.0) {
					// Convert Poincare to Klein if necessary.
					// If _Poincare==1, pcoef*xy is the image of xy in the Klein model
					// If _Poincare==0, then pcoef=1 so xy in unchanged
					float pcoef = (2.0*_Poincare / (1.0 + dot(xy,xy))) + (1.0-_Poincare);
					xy = pcoef*xy;

					float3 v = fromklein(xy);

					// Promote v to 4-vector, transform, and demote
					float4 v4 = float4(v.x,v.y,v.z,0);
					v4 = mul(_PreTransformation,v4);
					v = float3(v4.x,v4.y,v4.z);

					vect_in_fund vf = tofund(v);
					if (vf.coset != 255) {
						uv = six_panel_vif_to_uv(vf);
					}
				}

				fixed4 col = tex2D(_MainTex,uv);

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
