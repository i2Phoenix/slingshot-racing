Shader "PipeGUI/Steam" {
Properties {
 _MainTex ("Base Texture", 2D) = "white" {}
 _MainTex2_ST ("Second Texture Tiling/Offset", Vector) = (1,1,0,0)
 _GradientTex ("Gradient Texture", 2D) = "white" {}
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