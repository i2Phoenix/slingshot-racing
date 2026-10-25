Shader "Custom/WindSnow" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Geometry+100" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Geometry+100" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha, Zero One
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float3 g_windSnowData;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float4 _glesColor;
static float2 xlv_TEXCOORD0;
static float xlv_COLOR;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float4 _glesColor : COLOR;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float xlv_COLOR : TEXCOORD0;
    float2 xlv_TEXCOORD0 : TEXCOORD1;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float4 tmpvar_3;
    tmpvar_3.w = 1.0f;
    tmpvar_3.x = _glesVertex.xyz.x;
    tmpvar_3.y = _glesVertex.xyz.y;
    tmpvar_3.z = _glesVertex.xyz.z;
    float tmpvar_4 = _glesMultiTexCoord0.x - dot(g_windSnowData, _glesColor.xyz);
    float2 tmpvar_1;
    tmpvar_1.x = tmpvar_4;
    float tmpvar_5 = _glesMultiTexCoord0.y;
    tmpvar_1.y = tmpvar_5;
    float tmpvar_6 = _glesColor.w;
    float tmpvar_2 = tmpvar_6;
    gl_Position = mul(tmpvar_3, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD0 = tmpvar_1;
    xlv_COLOR = tmpvar_2;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    _glesColor = stage_input._glesColor;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.xlv_TEXCOORD0 = xlv_TEXCOORD0;
    stage_output.xlv_COLOR = xlv_COLOR;
    return stage_output;
}
uniform float3 g_windSnowColour;
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float xlv_COLOR : TEXCOORD0;
    float2 xlv_TEXCOORD0 : TEXCOORD1;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 colour;
    colour.x = g_windSnowColour.x;
    colour.y = g_windSnowColour.y;
    colour.z = g_windSnowColour.z;
    colour.w = (tex2D(_MainTex, xlv_TEXCOORD0) * xlv_COLOR).x;
    fragmentColor = colour;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD0 = stage_input.xlv_TEXCOORD0;
    xlv_COLOR = stage_input.xlv_COLOR;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
ENDCG

 }
}
}
