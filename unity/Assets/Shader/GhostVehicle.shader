Shader "Custom/GhostVehicle" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Lightmap ("Lightmap (RGB)", 2D) = "lightmap" { }
 _UOffset ("UOffset", Float) = 0
}
SubShader { 
 Tags { "QUEUE"="Geometry+40" "RenderType"="Opaque" }
 UsePass "Custom/DepthFill/BASE"
 Pass {
  Tags { "QUEUE"="Geometry+40" "RenderType"="Opaque" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha, Zero One
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float _UOffset;
uniform float4 p_lmMatR1;
uniform float4 p_lmMatR2;
uniform float4 g_highlightDirection;
uniform float _Alpha;

static float4 gl_Position;
static float3 _glesNormal;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float2 xlv_TEXCOORD0;
static float2 xlv_TEXCOORD1;
static float4 xlv_TEXCOORD2;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float4 xlv_TEXCOORD2 : TEXCOORD0;
    float2 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD2;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float3 tmpvar_1 = normalize(_glesNormal);
    float4 tmpvar_6;
    tmpvar_6.w = 1.0f;
    tmpvar_6.x = _glesVertex.xyz.x;
    tmpvar_6.y = _glesVertex.xyz.y;
    tmpvar_6.z = _glesVertex.xyz.z;
    float4 tmpvar_7 = mul(tmpvar_6, transpose(UNITY_MATRIX_MVP));
    float4 tmpvar_2 = tmpvar_7;
    tmpvar_2.z = tmpvar_7.z - 0.004999999888241291046142578125f;
    float2 mainUV = _glesMultiTexCoord0.xy;
    float tmpvar_8;
    if (_glesMultiTexCoord0.y < 0.75f)
    {
        tmpvar_8 = _UOffset;
    }
    else
    {
        tmpvar_8 = 0.0f;
    }
    mainUV.x = _glesMultiTexCoord0.x + tmpvar_8;
    float2 tmpvar_3 = mainUV;
    float4 tmpvar_9;
    tmpvar_9.w = 1.0f;
    tmpvar_9.x = _glesVertex.xyz.x;
    tmpvar_9.y = _glesVertex.xyz.y;
    tmpvar_9.z = _glesVertex.xyz.z;
    float tmpvar_10 = dot(mul(transpose(unity_ObjectToWorld), p_lmMatR1), tmpvar_9);
    float2 tmpvar_4;
    tmpvar_4.x = tmpvar_10;
    float4 tmpvar_11;
    tmpvar_11.w = 1.0f;
    tmpvar_11.x = _glesVertex.xyz.x;
    tmpvar_11.y = _glesVertex.xyz.y;
    tmpvar_11.z = _glesVertex.xyz.z;
    float tmpvar_12 = dot(mul(transpose(unity_ObjectToWorld), p_lmMatR2), tmpvar_11);
    tmpvar_4.y = tmpvar_12;
    float3x3 tmpvar_13;
    tmpvar_13[0] = transpose(unity_WorldToObject)[0].xyz;
    tmpvar_13[1] = transpose(unity_WorldToObject)[1].xyz;
    tmpvar_13[2] = transpose(unity_WorldToObject)[2].xyz;
    float4 tmpvar_14;
    tmpvar_14.w = 1.0f;
    tmpvar_14.x = _WorldSpaceCameraPos.x;
    tmpvar_14.y = _WorldSpaceCameraPos.y;
    tmpvar_14.z = _WorldSpaceCameraPos.z;
    float tmpvar_15 = min(1.0f, 1.75f - (1.75f * dot(normalize(mul(tmpvar_14, transpose(unity_WorldToObject)).xyz), tmpvar_1)));
    float tmpvar_16 = (max(dot(-mul(g_highlightDirection.xyz, tmpvar_13), tmpvar_1), 0.0f) * 0.60000002384185791015625f) + 0.4000000059604644775390625f;
    float tmpvar_17 = tmpvar_16 + (tmpvar_16 * tmpvar_15);
    float tmpvar_18 = ((tmpvar_15 * 0.60000002384185791015625f) + 0.4000000059604644775390625f) * _Alpha;
    float3 _189 = tmpvar_17.xxx;
    float4 tmpvar_5;
    tmpvar_5.x = _189.x;
    tmpvar_5.y = _189.y;
    tmpvar_5.z = _189.z;
    tmpvar_5.w = tmpvar_18;
    gl_Position = tmpvar_2;
    xlv_TEXCOORD0 = tmpvar_3;
    xlv_TEXCOORD1 = tmpvar_4;
    xlv_TEXCOORD2 = tmpvar_5;
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
    stage_output.xlv_TEXCOORD2 = xlv_TEXCOORD2;
    return stage_output;
}
uniform sampler2D _MainTex;
uniform sampler2D _Lightmap;

static float4 fragmentColor;

struct SRFragmentInput
{
    float4 xlv_TEXCOORD2 : TEXCOORD0;
    float2 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD2;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float tmpvar_1 = g_highlightDirection.w;
    float lightmapScale = tmpvar_1;
    float3 _46 = (tex2D(_MainTex, xlv_TEXCOORD0).xyz * xlv_TEXCOORD2.xyz) * (tex2D(_Lightmap, xlv_TEXCOORD1).xyz * lightmapScale);
    float4 tmpvar_2;
    tmpvar_2.x = _46.x;
    tmpvar_2.y = _46.y;
    tmpvar_2.z = _46.z;
    tmpvar_2.w = xlv_TEXCOORD2.w;
    fragmentColor = tmpvar_2;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD0 = stage_input.xlv_TEXCOORD0;
    xlv_TEXCOORD2 = stage_input.xlv_TEXCOORD2;
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
