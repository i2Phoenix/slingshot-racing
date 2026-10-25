Shader "Custom/GlossMaskedScaled" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _GlossTex ("Gloss (RGB)", 2D) = "white" {}
 _SpecPower ("Specular Power", Range(0,128)) = 9
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