Shader "Custom/FrontEndSpecular" {
Properties {
 _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
 _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Shininess ("Shininess", Range(1,128)) = 48
}
SubShader { 
 Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float3 _LightColor0;
uniform float3 _Color;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float3 _glesNormal;
static float2 xlv_TEXCOORD;
static float3 xlv_TEXCOORD1;
static float xlv_TEXCOORD2;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float xlv_TEXCOORD2 : TEXCOORD0;
    float3 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD : TEXCOORD2;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float4 tmpvar_4;
    tmpvar_4.w = 1.0f;
    tmpvar_4.x = _glesVertex.xyz.x;
    tmpvar_4.y = _glesVertex.xyz.y;
    tmpvar_4.z = _glesVertex.xyz.z;
    float2 tmpvar_5 = _glesMultiTexCoord0.xy;
    float2 tmpvar_1 = tmpvar_5;
    float4 tmpvar_6;
    tmpvar_6.w = 1.0f;
    tmpvar_6.x = _glesVertex.xyz.x;
    tmpvar_6.y = _glesVertex.xyz.y;
    tmpvar_6.z = _glesVertex.xyz.z;
    float4 tmpvar_7;
    tmpvar_7.w = 0.0f;
    float3 _53 = normalize(_glesNormal);
    tmpvar_7.x = _53.x;
    tmpvar_7.y = _53.y;
    tmpvar_7.z = _53.z;
    float3 tmpvar_8 = normalize(mul(tmpvar_7, transpose(unity_ObjectToWorld)).xyz);
    float tmpvar_9 = max(dot(normalize(_WorldSpaceLightPos0.xyz + normalize(_WorldSpaceCameraPos - mul(tmpvar_6, transpose(unity_ObjectToWorld)).xyz)), tmpvar_8), 0.0f);
    float tmpvar_3 = tmpvar_9;
    float4 tmpvar_10;
    tmpvar_10.w = 1.0f;
    tmpvar_10.x = tmpvar_8.x;
    tmpvar_10.y = tmpvar_8.y;
    tmpvar_10.z = tmpvar_8.z;
    float4 normal_i0 = tmpvar_10;
    float tmpvar_12 = dot(unity_SHAr, normal_i0);
    float3 x1;
    x1.x = tmpvar_12;
    float tmpvar_13 = dot(unity_SHAg, normal_i0);
    x1.y = tmpvar_13;
    float tmpvar_14 = dot(unity_SHAb, normal_i0);
    x1.z = tmpvar_14;
    float4 tmpvar_15 = normal_i0.xyzz * normal_i0.yzzx;
    float tmpvar_16 = dot(unity_SHBr, tmpvar_15);
    float3 x2;
    x2.x = tmpvar_16;
    float tmpvar_17 = dot(unity_SHBg, tmpvar_15);
    x2.y = tmpvar_17;
    float tmpvar_18 = dot(unity_SHBb, tmpvar_15);
    x2.z = tmpvar_18;
    float tmpvar_19 = (normal_i0.x * normal_i0.x) - (normal_i0.y * normal_i0.y);
    float vC = tmpvar_19;
    float3 tmpvar_20 = unity_SHC.xyz * vC;
    float3 x3 = tmpvar_20;
    float3 tmpvar_11 = ShadeSH9(normal_i0);
    float3 tmpvar_21 = (((_LightColor0 * max(dot(tmpvar_8, _WorldSpaceLightPos0.xyz), 0.0f)) * 2.0f) + tmpvar_11) * _Color;
    float3 tmpvar_2 = tmpvar_21;
    gl_Position = mul(tmpvar_4, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD = tmpvar_1;
    xlv_TEXCOORD1 = tmpvar_2;
    xlv_TEXCOORD2 = tmpvar_3;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    _glesNormal = stage_input._glesNormal;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.xlv_TEXCOORD = xlv_TEXCOORD;
    stage_output.xlv_TEXCOORD1 = xlv_TEXCOORD1;
    stage_output.xlv_TEXCOORD2 = xlv_TEXCOORD2;
    return stage_output;
}
uniform float3 _SpecColor;
uniform float _Shininess;
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float xlv_TEXCOORD2 : TEXCOORD0;
    float3 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD : TEXCOORD2;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float3 tmpvar_1 = (_LightColor0 * _SpecColor) * 2.0f;
    float3 specularLightColour = tmpvar_1;
    float tmpvar_2 = pow(xlv_TEXCOORD2, _Shininess);
    float specDot = tmpvar_2;
    float4 tmpvar_3;
    tmpvar_3.w = 1.0f;
    float3 _56 = (tex2D(_MainTex, xlv_TEXCOORD).xyz * xlv_TEXCOORD1) + (specularLightColour * specDot);
    tmpvar_3.x = _56.x;
    tmpvar_3.y = _56.y;
    tmpvar_3.z = _56.z;
    fragmentColor = tmpvar_3;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD2 = stage_input.xlv_TEXCOORD2;
    xlv_TEXCOORD = stage_input.xlv_TEXCOORD;
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
