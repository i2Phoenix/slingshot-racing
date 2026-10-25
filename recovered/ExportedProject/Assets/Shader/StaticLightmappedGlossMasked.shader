Shader "Custom/StaticLightmappedGlossMasked" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _GlossTex ("Gloss (G)", 2D) = "white" {}
 _SpecColor ("Specular Colour", Color) = (1,1,1,1)
 _Shininess2 ("Specular Power", Range(0,128)) = 10
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