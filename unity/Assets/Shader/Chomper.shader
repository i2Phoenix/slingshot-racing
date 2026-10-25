Shader "Custom/Chomper" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _GlossTex ("Gloss (G)", 2D) = "white" {}
 _Lightmap ("Lightmap (RGB)", 2D) = "lightmap" { }
 _SpecPower ("Specular Power", Range(0,128)) = 48
}
SubShader { 
 Tags { "QUEUE"="Geometry+40" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Geometry+40" "RenderType"="Opaque" }
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float4 g_highlightDirection;
uniform float4 p_lmMatR1;
uniform float4 p_lmMatR2;
uniform float _SpecPower;

static float4 gl_Position;
static float4 _glesVertex;
static float2 i_mainUV;
static float4 _glesMultiTexCoord0;
static float2 i_lightmapUV;
static float4 i_colour;
static float3 _glesNormal;

struct SRVertexInput
{
    float4 _glesVertex : POSITION;
    float3 _glesNormal : NORMAL;
    float4 _glesMultiTexCoord0 : TEXCOORD0;
};

struct SRVertexOutput
{
    float4 i_colour : TEXCOORD0;
    float2 i_mainUV : TEXCOORD1;
    float2 i_lightmapUV : TEXCOORD2;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float3 lightDirOS = mul(float4(g_highlightDirection.xyz, 0.0f), transpose(unity_WorldToObject)).xyz;
    float3 camPosOS = mul(float4(_WorldSpaceCameraPos, 1.0f), transpose(unity_WorldToObject)).xyz;
    float3 halfVecOS = normalize(normalize(camPosOS) - lightDirOS);
    float4 lmMatR1 = mul(transpose(unity_ObjectToWorld), p_lmMatR1);
    float4 lmMatR2 = mul(transpose(unity_ObjectToWorld), p_lmMatR2);
    gl_Position = mul(float4(_glesVertex.xyz, 1.0f), transpose(UNITY_MATRIX_MVP));
    i_mainUV = _glesMultiTexCoord0.xy;
    i_lightmapUV.x = dot(lmMatR1, float4(_glesVertex.xyz, 1.0f));
    i_lightmapUV.y = dot(lmMatR2, float4(_glesVertex.xyz, 1.0f));
    float3 _112 = max(dot(-lightDirOS, _glesNormal), 0.0f).xxx;
    i_colour.x = _112.x;
    i_colour.y = _112.y;
    i_colour.z = _112.z;
    float specDot = max(dot(halfVecOS, _glesNormal), 0.0f);
    i_colour.w = pow(specDot, _SpecPower);
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    _glesNormal = stage_input._glesNormal;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.i_mainUV = i_mainUV;
    stage_output.i_lightmapUV = i_lightmapUV;
    stage_output.i_colour = i_colour;
    return stage_output;
}
uniform sampler2D _MainTex;
uniform sampler2D _GlossTex;
uniform sampler2D _Lightmap;

static float4 fragmentColor;

struct SRFragmentInput
{
    float4 i_colour : TEXCOORD0;
    float2 i_mainUV : TEXCOORD1;
    float2 i_lightmapUV : TEXCOORD2;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

float3 DecodeLightmapHalf(float4 color)
{
    return color.xyz;
}

void frag_main()
{
    float3 diffuseTex = tex2D(_MainTex, i_mainUV).xyz;
    float3 specularTex = tex2D(_GlossTex, i_mainUV).xyz;
    float lightmapScale = g_highlightDirection.w;
    float4 param = tex2D(_Lightmap, i_lightmapUV);
    float3 lightmap = DecodeLightmapHalf(param) * lightmapScale;
    float3 _71 = ((diffuseTex * i_colour.xyz) + (specularTex * i_colour.w)) * lightmap;
    fragmentColor.x = _71.x;
    fragmentColor.y = _71.y;
    fragmentColor.z = _71.z;
    fragmentColor.w = 1.0f;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    i_mainUV = stage_input.i_mainUV;
    i_lightmapUV = stage_input.i_lightmapUV;
    i_colour = stage_input.i_colour;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
ENDCG
 }
}
SubShader { 
 Tags { "QUEUE"="Geometry+40" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Geometry+40" "RenderType"="Opaque" }
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float4 g_highlightDirection;
uniform float4 p_lmMatR1;
uniform float4 p_lmMatR2;
uniform float _SpecPower;

static float4 gl_Position;
static float3 _glesNormal;
static float4 _glesVertex;
static float2 xlv_TEXCOORD0;
static float4 _glesMultiTexCoord0;
static float2 xlv_TEXCOORD1;
static float4 xlv_COLOR0;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float4 xlv_COLOR0 : TEXCOORD0;
    float2 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD2;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float3 tmpvar_1 = normalize(_glesNormal);
    float3x3 tmpvar_4;
    tmpvar_4[0] = transpose(unity_WorldToObject)[0].xyz;
    tmpvar_4[1] = transpose(unity_WorldToObject)[1].xyz;
    tmpvar_4[2] = transpose(unity_WorldToObject)[2].xyz;
    float3 tmpvar_5 = mul(g_highlightDirection.xyz, tmpvar_4);
    float4 tmpvar_6;
    tmpvar_6.w = 1.0f;
    tmpvar_6.x = _WorldSpaceCameraPos.x;
    tmpvar_6.y = _WorldSpaceCameraPos.y;
    tmpvar_6.z = _WorldSpaceCameraPos.z;
    float4 tmpvar_7;
    tmpvar_7.w = 1.0f;
    tmpvar_7.x = _glesVertex.xyz.x;
    tmpvar_7.y = _glesVertex.xyz.y;
    tmpvar_7.z = _glesVertex.xyz.z;
    float4 tmpvar_8;
    tmpvar_8.w = 1.0f;
    tmpvar_8.x = _glesVertex.xyz.x;
    tmpvar_8.y = _glesVertex.xyz.y;
    tmpvar_8.z = _glesVertex.xyz.z;
    float tmpvar_9 = dot(mul(transpose(unity_ObjectToWorld), p_lmMatR1), tmpvar_8);
    float2 tmpvar_2;
    tmpvar_2.x = tmpvar_9;
    float4 tmpvar_10;
    tmpvar_10.w = 1.0f;
    tmpvar_10.x = _glesVertex.xyz.x;
    tmpvar_10.y = _glesVertex.xyz.y;
    tmpvar_10.z = _glesVertex.xyz.z;
    float tmpvar_11 = dot(mul(transpose(unity_ObjectToWorld), p_lmMatR2), tmpvar_10);
    tmpvar_2.y = tmpvar_11;
    float tmpvar_12 = max(dot(-tmpvar_5, tmpvar_1), 0.0f);
    float3 _126 = tmpvar_12.xxx;
    float4 tmpvar_3;
    tmpvar_3.x = _126.x;
    tmpvar_3.y = _126.y;
    tmpvar_3.z = _126.z;
    float tmpvar_13 = pow(max(dot(normalize(normalize(mul(tmpvar_6, transpose(unity_WorldToObject)).xyz) - tmpvar_5), tmpvar_1), 0.0f), _SpecPower);
    tmpvar_3.w = tmpvar_13;
    gl_Position = mul(tmpvar_7, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
    xlv_TEXCOORD1 = tmpvar_2;
    xlv_COLOR0 = tmpvar_3;
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
    stage_output.xlv_COLOR0 = xlv_COLOR0;
    return stage_output;
}
uniform sampler2D _MainTex;
uniform sampler2D _GlossTex;
uniform sampler2D _Lightmap;

static float4 fragmentColor;

struct SRFragmentInput
{
    float4 xlv_COLOR0 : TEXCOORD0;
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
    float4 tmpvar_2;
    tmpvar_2.w = 1.0f;
    float3 _58 = ((tex2D(_MainTex, xlv_TEXCOORD0).xyz * xlv_COLOR0.xyz) + (tex2D(_GlossTex, xlv_TEXCOORD0).xyz * xlv_COLOR0.w)) * (tex2D(_Lightmap, xlv_TEXCOORD1).xyz * lightmapScale);
    tmpvar_2.x = _58.x;
    tmpvar_2.y = _58.y;
    tmpvar_2.z = _58.z;
    fragmentColor = tmpvar_2;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD0 = stage_input.xlv_TEXCOORD0;
    xlv_COLOR0 = stage_input.xlv_COLOR0;
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
