Shader "Custom/AdditiveTrack" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Geometry+30" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 UsePass "Custom/Additive/BASE"
}
}
