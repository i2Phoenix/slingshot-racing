Shader "Custom/VertexColourAlpha_30" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Geometry+30" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 UsePass "Custom/VertexColourAlpha_10/BASE"
}
}