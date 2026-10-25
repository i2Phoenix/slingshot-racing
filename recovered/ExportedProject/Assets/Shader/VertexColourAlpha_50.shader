Shader "Custom/VertexColourAlpha_50" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Geometry+50" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 UsePass "Custom/VertexColourAlpha_10/BASE"
}
}