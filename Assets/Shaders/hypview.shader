Shader "Unlit/hypview"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
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
				fixed4 col;
				float2 xy = 2.0*(i.uv - float2(0.5,0.5));

				if (length(xy) >= 1.0) {
					col = tex2D(_MainTex,float2(0,0));
				} else {
					float3 v = fromklein(xy);
					vect_in_fund vf = tofund(v);
					if (vf.coset == 255){
						col = tex2D(_MainTex,float2(0.1,0.6));
					} else {
						float2 uv = six_panel_vif_to_uv(vf);
						col = tex2D(_MainTex,uv);
					}
				}

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
