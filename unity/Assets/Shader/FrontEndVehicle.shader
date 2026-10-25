Shader "Custom/FrontEndVehicle" {
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
uniform float _UOffset;
uniform float _UOffsetNext;
uniform float3 _LightColor0;
uniform float3 _Color;
uniform float3 _TransitionPos;
uniform float3 _TransitionDir;

static float4 gl_Position;
static float3 _glesNormal;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float2 xlv_TEXCOORD;
static float2 xlv_TEXCOORD1;
static float4 xlv_TEXCOORD2;
static float xlv_TEXCOORD3;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float xlv_TEXCOORD3 : TEXCOORD0;
    float4 xlv_TEXCOORD2 : TEXCOORD1;
    float2 xlv_TEXCOORD1 : TEXCOORD2;
    float2 xlv_TEXCOORD : TEXCOORD3;
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
    float tmpvar_8;
    if (_glesMultiTexCoord0.y < 0.75f)
    {
        tmpvar_8 = _UOffset;
    }
    else
    {
        tmpvar_8 = 0.0f;
    }
    float tmpvar_9 = _glesMultiTexCoord0.x + tmpvar_8;
    float2 tmpvar_2;
    tmpvar_2.x = tmpvar_9;
    float tmpvar_10 = _glesMultiTexCoord0.y;
    tmpvar_2.y = tmpvar_10;
    float tmpvar_11;
    if (_glesMultiTexCoord0.y < 0.75f)
    {
        tmpvar_11 = _UOffsetNext;
    }
    else
    {
        tmpvar_11 = 0.0f;
    }
    float tmpvar_12 = _glesMultiTexCoord0.x + tmpvar_11;
    float2 tmpvar_3;
    tmpvar_3.x = tmpvar_12;
    float tmpvar_13 = _glesMultiTexCoord0.y;
    tmpvar_3.y = tmpvar_13;
    float4 tmpvar_14;
    tmpvar_14.w = 1.0f;
    tmpvar_14.x = _glesVertex.xyz.x;
    tmpvar_14.y = _glesVertex.xyz.y;
    tmpvar_14.z = _glesVertex.xyz.z;
    float3 tmpvar_15 = mul(tmpvar_14, transpose(unity_ObjectToWorld)).xyz;
    float4 tmpvar_16;
    tmpvar_16.w = 0.0f;
    tmpvar_16.x = tmpvar_1.x;
    tmpvar_16.y = tmpvar_1.y;
    tmpvar_16.z = tmpvar_1.z;
    float3 tmpvar_17 = normalize(mul(tmpvar_16, transpose(unity_ObjectToWorld)).xyz);
    float tmpvar_18 = max(dot(normalize(_WorldSpaceLightPos0.xyz + normalize(_WorldSpaceCameraPos - tmpvar_15)), tmpvar_17), 0.0f);
    float tmpvar_5 = tmpvar_18;
    float4 tmpvar_19;
    tmpvar_19.w = 1.0f;
    tmpvar_19.x = tmpvar_17.x;
    tmpvar_19.y = tmpvar_17.y;
    tmpvar_19.z = tmpvar_17.z;
    float4 normal_i0 = tmpvar_19;
    float tmpvar_21 = dot(unity_SHAr, normal_i0);
    float3 x1;
    x1.x = tmpvar_21;
    float tmpvar_22 = dot(unity_SHAg, normal_i0);
    x1.y = tmpvar_22;
    float tmpvar_23 = dot(unity_SHAb, normal_i0);
    x1.z = tmpvar_23;
    float4 tmpvar_24 = normal_i0.xyzz * normal_i0.yzzx;
    float tmpvar_25 = dot(unity_SHBr, tmpvar_24);
    float3 x2;
    x2.x = tmpvar_25;
    float tmpvar_26 = dot(unity_SHBg, tmpvar_24);
    x2.y = tmpvar_26;
    float tmpvar_27 = dot(unity_SHBb, tmpvar_24);
    x2.z = tmpvar_27;
    float tmpvar_28 = (normal_i0.x * normal_i0.x) - (normal_i0.y * normal_i0.y);
    float vC = tmpvar_28;
    float3 tmpvar_29 = unity_SHC.xyz * vC;
    float3 x3 = tmpvar_29;
    float3 tmpvar_20 = ShadeSH9(normal_i0);
    float3 tmpvar_30 = (((_LightColor0 * max(dot(tmpvar_17, _WorldSpaceLightPos0.xyz), 0.0f)) * 2.0f) + tmpvar_20) * _Color;
    float4 tmpvar_4;
    tmpvar_4.x = tmpvar_30.x;
    tmpvar_4.y = tmpvar_30.y;
    tmpvar_4.z = tmpvar_30.z;
    float tmpvar_31 = dot(tmpvar_15 - _TransitionPos, _TransitionDir) * (-2.0f);
    tmpvar_4.w = tmpvar_31;
    gl_Position = tmpvar_7;
    xlv_TEXCOORD = tmpvar_2;
    xlv_TEXCOORD1 = tmpvar_3;
    xlv_TEXCOORD2 = tmpvar_4;
    xlv_TEXCOORD3 = tmpvar_5;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesNormal = stage_input._glesNormal;
    _glesVertex = stage_input._glesVertex;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.xlv_TEXCOORD = xlv_TEXCOORD;
    stage_output.xlv_TEXCOORD1 = xlv_TEXCOORD1;
    stage_output.xlv_TEXCOORD2 = xlv_TEXCOORD2;
    stage_output.xlv_TEXCOORD3 = xlv_TEXCOORD3;
    return stage_output;
}
uniform float3 _SpecColor;
uniform float _Shininess;
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float xlv_TEXCOORD3 : TEXCOORD0;
    float4 xlv_TEXCOORD2 : TEXCOORD1;
    float2 xlv_TEXCOORD1 : TEXCOORD2;
    float2 xlv_TEXCOORD : TEXCOORD3;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float3 tmpvar_1 = (_LightColor0 * _SpecColor) * 2.0f;
    float3 specularLightColour = tmpvar_1;
    float tmpvar_2 = pow(xlv_TEXCOORD3, _Shininess);
    float specDot = tmpvar_2;
    float4 tmpvar_3;
    tmpvar_3.w = 1.0f;
    float3 _68 = (lerp(tex2D(_MainTex, xlv_TEXCOORD).xyz, tex2D(_MainTex, xlv_TEXCOORD1).xyz, clamp(xlv_TEXCOORD2.w, 0.0f, 1.0f).xxx) * xlv_TEXCOORD2.xyz) + (specularLightColour * specDot);
    tmpvar_3.x = _68.x;
    tmpvar_3.y = _68.y;
    tmpvar_3.z = _68.z;
    fragmentColor = tmpvar_3;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD3 = stage_input.xlv_TEXCOORD3;
    xlv_TEXCOORD = stage_input.xlv_TEXCOORD;
    xlv_TEXCOORD1 = stage_input.xlv_TEXCOORD1;
    xlv_TEXCOORD2 = stage_input.xlv_TEXCOORD2;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
ENDCG

 }
}
}
