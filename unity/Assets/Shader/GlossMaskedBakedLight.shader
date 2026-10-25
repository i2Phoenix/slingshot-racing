Shader "Custom/GlossMaskedBakedLight" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _GlossTex ("Gloss (RGB)", 2D) = "white" {}
 _SpecPower ("Specular Power", Range(0,128)) = 9
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float3 g_highlightDirection;
uniform float _SpecPower;

static float4 gl_Position;
static float4 _glesVertex;
static float3 _glesNormal;
static float4 _glesColor;
static float2 xlv_TEXCOORD0;
static float4 _glesMultiTexCoord0;
static float4 xlv_COLOR0;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesColor : COLOR;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float4 xlv_COLOR0 : TEXCOORD0;
    float2 xlv_TEXCOORD0 : TEXCOORD1;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float4 tmpvar_2;
    tmpvar_2.w = 1.0f;
    tmpvar_2.x = _glesVertex.xyz.x;
    tmpvar_2.y = _glesVertex.xyz.y;
    tmpvar_2.z = _glesVertex.xyz.z;
    float4 tmpvar_3;
    tmpvar_3.w = 1.0f;
    tmpvar_3.x = _WorldSpaceCameraPos.x;
    tmpvar_3.y = _WorldSpaceCameraPos.y;
    tmpvar_3.z = _WorldSpaceCameraPos.z;
    float4 tmpvar_4;
    tmpvar_4.w = 0.0f;
    float3 _45 = -g_highlightDirection;
    tmpvar_4.x = _45.x;
    tmpvar_4.y = _45.y;
    tmpvar_4.z = _45.z;
    float tmpvar_5 = pow(max(dot(normalize(normalize(mul(tmpvar_3, transpose(unity_WorldToObject)).xyz - _glesVertex.xyz) + mul(tmpvar_4, transpose(unity_WorldToObject)).xyz), normalize(_glesNormal)), 0.0f), _SpecPower);
    float4 tmpvar_1;
    tmpvar_1.x = _glesColor.xyz.x;
    tmpvar_1.y = _glesColor.xyz.y;
    tmpvar_1.z = _glesColor.xyz.z;
    tmpvar_1.w = tmpvar_5;
    gl_Position = mul(tmpvar_2, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
    xlv_COLOR0 = tmpvar_1;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    _glesNormal = stage_input._glesNormal;
    _glesColor = stage_input._glesColor;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.xlv_TEXCOORD0 = xlv_TEXCOORD0;
    stage_output.xlv_COLOR0 = xlv_COLOR0;
    return stage_output;
}
uniform sampler2D _MainTex;
uniform sampler2D _GlossTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float4 xlv_COLOR0 : TEXCOORD0;
    float2 xlv_TEXCOORD0 : TEXCOORD1;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 1.0f;
    float3 _41 = (tex2D(_MainTex, xlv_TEXCOORD0).xyz + (tex2D(_GlossTex, xlv_TEXCOORD0).xyz * xlv_COLOR0.w)) * xlv_COLOR0.xyz;
    tmpvar_1.x = _41.x;
    tmpvar_1.y = _41.y;
    tmpvar_1.z = _41.z;
    fragmentColor = tmpvar_1;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD0 = stage_input.xlv_TEXCOORD0;
    xlv_COLOR0 = stage_input.xlv_COLOR0;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
ENDCG

 }
}
}
