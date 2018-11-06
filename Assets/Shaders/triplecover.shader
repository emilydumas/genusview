Shader "triplecover" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert
      struct Input {
          float2 uv_MainTex;
      };
      sampler2D _MainTex;
      void surf (Input IN, inout SurfaceOutput o) {
          float3 col0, col1, col2;
          if (IN.uv_MainTex.x < 0.3333333) {
              col0 = tex2D (_MainTex, IN.uv_MainTex).rgb;
              col1 = tex2D (_MainTex, IN.uv_MainTex + float2(1/3.0,1/3.0));
              col2 = tex2D (_MainTex, IN.uv_MainTex + float2(2/3.0,0));
          } else {
              col0 = tex2D (_MainTex, IN.uv_MainTex).rgb;
              col1 = tex2D (_MainTex, IN.uv_MainTex + float2(-1/3.0,1/3.0));
              col2 = tex2D (_MainTex, IN.uv_MainTex + float2(1/3.0,1/3.0));
          }
          o.Albedo = (1/3.0)*col0 + (1/3.0)*col1 + (1/3.0)*col2;
      }
      ENDCG
    } 
    Fallback "Diffuse"
}