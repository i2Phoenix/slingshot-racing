Shader "Custom/VehicleFastBlinn" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _GlossTex ("Gloss (G)", 2D) = "white" {}
 _Lightmap ("Lightmap (RGB)", 2D) = "lightmap" { TexGen ObjectLinear }
 _SpecPower ("Specular Power", Range(0,128)) = 48
 _UOffset ("UOffset", Float) = 0
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Lambert
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutput o)
		{
			float4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}