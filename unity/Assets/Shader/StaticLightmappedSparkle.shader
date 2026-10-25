// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Custom/StaticLightmappedSparkle" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _SparkleMap ("SparkleMap (RGB)", 2D) = "white" {}
}
SubShader { 
 LOD 250
 Pass {
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float4 g_sparkleData;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float4 _glesMultiTexCoord1;
static float2 xlv_TEXCOORD0;
static float2 xlv_TEXCOORD1;
static float2 xlv_TEXCOORD2;
static float3 xlv_TEXCOORD3;

struct SRVertexInput
{
    float4 _glesMultiTexCoord1 : TEXCOORD1;
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float3 xlv_TEXCOORD3 : TEXCOORD0;
    float2 xlv_TEXCOORD2 : TEXCOORD1;
    float2 xlv_TEXCOORD1 : TEXCOORD2;
    float2 xlv_TEXCOORD0 : TEXCOORD3;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float4 tmpvar_4;
    tmpvar_4.w = 1.0f;
    tmpvar_4.x = _glesVertex.xyz.x;
    tmpvar_4.y = _glesVertex.xyz.y;
    tmpvar_4.z = _glesVertex.xyz.z;
    float2 tmpvar_5 = _glesMultiTexCoord0.xy * g_sparkleData.x;
    float2 tmpvar_1 = tmpvar_5;
    float2 tmpvar_6 = (_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw;
    float2 tmpvar_2 = tmpvar_6;
    float3 tmpvar_7 = abs(normalize(_WorldSpaceCameraPos - _glesVertex.xyz));
    float3 tmpvar_8 = max((abs(frac((tmpvar_7.xzx * float3(4.0f, 3.5f, 5.0f)) + (tmpvar_7.yyz * float3(1.0f, 1.5f, 5.0f))) - 0.5f.xxx) * 2.25f) - 0.125f.xxx, 0.0f.xxx);
    float3 tmpvar_3 = tmpvar_8;
    gl_Position = mul(tmpvar_4, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
    xlv_TEXCOORD1 = tmpvar_1;
    xlv_TEXCOORD2 = tmpvar_2;
    xlv_TEXCOORD3 = tmpvar_3;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    _glesMultiTexCoord1 = stage_input._glesMultiTexCoord1;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.xlv_TEXCOORD0 = xlv_TEXCOORD0;
    stage_output.xlv_TEXCOORD1 = xlv_TEXCOORD1;
    stage_output.xlv_TEXCOORD2 = xlv_TEXCOORD2;
    stage_output.xlv_TEXCOORD3 = xlv_TEXCOORD3;
    return stage_output;
}
uniform sampler2D _MainTex;
uniform sampler2D _SparkleMap;

static float4 fragmentColor;

struct SRFragmentInput
{
    float3 xlv_TEXCOORD3 : TEXCOORD0;
    float2 xlv_TEXCOORD2 : TEXCOORD1;
    float2 xlv_TEXCOORD1 : TEXCOORD2;
    float2 xlv_TEXCOORD0 : TEXCOORD3;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 1.0f;
    float3 _49 = (tex2D(_MainTex, xlv_TEXCOORD0).xyz + dot(tex2D(_SparkleMap, xlv_TEXCOORD1).xyz, xlv_TEXCOORD3 * xlv_TEXCOORD3).xxx) * (UNITY_SAMPLE_TEX2D(unity_Lightmap, xlv_TEXCOORD2).xyz * 1.1764700412750244140625f);
    tmpvar_1.x = _49.x;
    tmpvar_1.y = _49.y;
    tmpvar_1.z = _49.z;
    fragmentColor = tmpvar_1;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD0 = stage_input.xlv_TEXCOORD0;
    xlv_TEXCOORD1 = stage_input.xlv_TEXCOORD1;
    xlv_TEXCOORD3 = stage_input.xlv_TEXCOORD3;
    xlv_TEXCOORD2 = stage_input.xlv_TEXCOORD2;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
ENDCG

 }
}
SubShader { 
 UsePass "Custom/StaticLightmapped/GLSL"
}
SubShader { 
 UsePass "Custom/StaticLightmapped/CG"
}
}
