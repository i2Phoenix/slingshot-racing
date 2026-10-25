Shader "Custom/CloudsColour" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base Texture", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent+50" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent+50" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Blend One OneMinusSrcAlpha, Zero One
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"

static float4 gl_Position;
static float4 _glesVertex;
static float2 xlv_TEXCOORD0;
static float4 _glesMultiTexCoord0;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float2 xlv_TEXCOORD0 : TEXCOORD0;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 1.0f;
    tmpvar_1.x = _glesVertex.xyz.x;
    tmpvar_1.y = _glesVertex.xyz.y;
    tmpvar_1.z = _glesVertex.xyz.z;
    gl_Position = mul(tmpvar_1, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.xlv_TEXCOORD0 = xlv_TEXCOORD0;
    return stage_output;
}
uniform sampler2D _MainTex;
uniform float4 _Color;

static float4 fragmentColor;

struct SRFragmentInput
{
    float2 xlv_TEXCOORD0 : TEXCOORD0;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    fragmentColor = _Color * tex2D(_MainTex, xlv_TEXCOORD0).x;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD0 = stage_input.xlv_TEXCOORD0;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
ENDCG

 }
}
}
