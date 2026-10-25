Shader "Custom/PipeGUI_MeshParticle" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _GlossTex ("Gloss (RGB)", 2D) = "white" {}
 _SpecPower ("Specular Power", Range(0,128)) = 10
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
uniform float _SpecPower;

static float4 gl_Position;
static float3 _glesNormal;
static float4 _glesVertex;
static float2 xlv_TEXCOORD0;
static float4 _glesMultiTexCoord0;
static float4 xlv_TEXCOORD1;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float4 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD0 : TEXCOORD1;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 0.0f;
    float3 _19 = normalize(_glesNormal);
    tmpvar_1.x = _19.x;
    tmpvar_1.y = _19.y;
    tmpvar_1.z = _19.z;
    float4 tmpvar_2 = _glesVertex;
    tmpvar_2.z = (-3.7999999523162841796875f) + (_glesVertex.z * 0.0500000007450580596923828125f);
    float4 tmpvar_4;
    tmpvar_4.w = 1.0f;
    tmpvar_4.x = tmpvar_2.xyz.x;
    tmpvar_4.y = tmpvar_2.xyz.y;
    tmpvar_4.z = tmpvar_2.xyz.z;
    float3 tmpvar_5 = tmpvar_1.xyz;
    float3 tmpvar_6 = (max(0.0f, dot(float3(-0.577367007732391357421875f, 0.577367007732391357421875f, -0.577367007732391357421875f), tmpvar_5)) + 0.4000000059604644775390625f).xxx;
    float4 tmpvar_3;
    tmpvar_3.x = tmpvar_6.x;
    tmpvar_3.y = tmpvar_6.y;
    tmpvar_3.z = tmpvar_6.z;
    float tmpvar_7 = pow(dot(normalize(float3(-0.577367007732391357421875f, 0.577367007732391357421875f, -0.577367007732391357421875f) + normalize(_WorldSpaceCameraPos - tmpvar_2.xyz)), tmpvar_5), _SpecPower);
    tmpvar_3.w = tmpvar_7;
    gl_Position = mul(tmpvar_4, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
    xlv_TEXCOORD1 = tmpvar_3;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesNormal = stage_input._glesNormal;
    _glesVertex = stage_input._glesVertex;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.xlv_TEXCOORD0 = xlv_TEXCOORD0;
    stage_output.xlv_TEXCOORD1 = xlv_TEXCOORD1;
    return stage_output;
}
uniform sampler2D _MainTex;
uniform sampler2D _GlossTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float4 xlv_TEXCOORD1 : TEXCOORD0;
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
    float3 _41 = (tex2D(_MainTex, xlv_TEXCOORD0).xyz + (tex2D(_GlossTex, xlv_TEXCOORD0).xyz * xlv_TEXCOORD1.w)) * xlv_TEXCOORD1.xyz;
    tmpvar_1.x = _41.x;
    tmpvar_1.y = _41.y;
    tmpvar_1.z = _41.z;
    fragmentColor = tmpvar_1;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD0 = stage_input.xlv_TEXCOORD0;
    xlv_TEXCOORD1 = stage_input.xlv_TEXCOORD1;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
ENDCG

 }
}
}
