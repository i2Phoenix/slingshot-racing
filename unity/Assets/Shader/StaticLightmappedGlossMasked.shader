// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Custom/StaticLightmappedGlossMasked" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _GlossTex ("Gloss (G)", 2D) = "white" {}
 _SpecColor ("Specular Colour", Color) = (1,1,1,1)
 _Shininess2 ("Specular Power", Range(0,128)) = 10
}
SubShader { 
 Pass {
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float3 g_highlightDirection;
uniform float _Shininess2;
uniform float3 _SpecColor;

static float4 gl_Position;
static float4 _glesVertex;
static float2 i_mainUV;
static float4 _glesMultiTexCoord0;
static float2 i_lightmapUV;
static float4 _glesMultiTexCoord1;
static float3 _glesNormal;
static float3 i_specular;

struct SRVertexInput
{
    float4 _glesVertex : POSITION;
    float3 _glesNormal : NORMAL;
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float4 _glesMultiTexCoord1 : TEXCOORD1;
};

struct SRVertexOutput
{
    float2 i_mainUV : TEXCOORD0;
    float2 i_lightmapUV : TEXCOORD1;
    float3 i_specular : TEXCOORD2;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    gl_Position = mul(float4(_glesVertex.xyz, 1.0f), transpose(UNITY_MATRIX_MVP));
    i_mainUV = _glesMultiTexCoord0.xy;
    i_lightmapUV = (_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw;
    float3 viewDir = normalize(_WorldSpaceCameraPos - _glesVertex.xyz);
    float3 halfVector = normalize(viewDir - g_highlightDirection);
    float specDot = max(dot(halfVector, _glesNormal), 0.0f);
    float specular = pow(specDot, _Shininess2);
    i_specular = _SpecColor * specular;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    _glesMultiTexCoord1 = stage_input._glesMultiTexCoord1;
    _glesNormal = stage_input._glesNormal;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.i_mainUV = i_mainUV;
    stage_output.i_lightmapUV = i_lightmapUV;
    stage_output.i_specular = i_specular;
    return stage_output;
}
uniform sampler2D _MainTex;
uniform sampler2D _GlossTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float2 i_mainUV : TEXCOORD0;
    float2 i_lightmapUV : TEXCOORD1;
    float3 i_specular : TEXCOORD2;
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
    float c_mainColour = 1.17647063732147216796875f;
    float4 param = UNITY_SAMPLE_TEX2D(unity_Lightmap, i_lightmapUV);
    float3 lightmap = DecodeLightmapHalf(param) * c_mainColour;
    float3 diffuse = tex2D(_MainTex, i_mainUV).xyz;
    float3 gloss = tex2D(_GlossTex, i_mainUV).xyz;
    float3 colour = (diffuse + (i_specular * gloss)) * lightmap;
    fragmentColor.x = colour.x;
    fragmentColor.y = colour.y;
    fragmentColor.z = colour.z;
    fragmentColor.w = 1.0f;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    i_lightmapUV = stage_input.i_lightmapUV;
    i_mainUV = stage_input.i_mainUV;
    i_specular = stage_input.i_specular;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
ENDCG
 }
}
SubShader { 
 Pass {
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float3 g_highlightDirection;
uniform float _Shininess2;
uniform float3 _SpecColor;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesMultiTexCoord1;
static float3 _glesNormal;
static float2 xlv_TEXCOORD0;
static float4 _glesMultiTexCoord0;
static float2 xlv_TEXCOORD1;
static float3 xlv_COLOR0;

struct SRVertexInput
{
    float4 _glesMultiTexCoord1 : TEXCOORD1;
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float3 xlv_COLOR0 : TEXCOORD0;
    float2 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD2;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float4 tmpvar_2;
    tmpvar_2.w = 1.0f;
    tmpvar_2.x = _glesVertex.xyz.x;
    tmpvar_2.y = _glesVertex.xyz.y;
    tmpvar_2.z = _glesVertex.xyz.z;
    float2 tmpvar_3 = (_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw;
    float2 tmpvar_1 = tmpvar_3;
    float tmpvar_4 = pow(max(dot(normalize((-g_highlightDirection) + normalize(_WorldSpaceCameraPos - _glesVertex.xyz)), normalize(_glesNormal)), 0.0f), _Shininess2);
    float specular = tmpvar_4;
    gl_Position = mul(tmpvar_2, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
    xlv_TEXCOORD1 = tmpvar_1;
    xlv_COLOR0 = _SpecColor * specular;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    _glesMultiTexCoord1 = stage_input._glesMultiTexCoord1;
    _glesNormal = stage_input._glesNormal;
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

static float4 fragmentColor;

struct SRFragmentInput
{
    float3 xlv_COLOR0 : TEXCOORD0;
    float2 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD2;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 1.0f;
    float3 _45 = (tex2D(_MainTex, xlv_TEXCOORD0).xyz + (xlv_COLOR0 * tex2D(_GlossTex, xlv_TEXCOORD0).xyz)) * (UNITY_SAMPLE_TEX2D(unity_Lightmap, xlv_TEXCOORD1).xyz * 1.1764700412750244140625f);
    tmpvar_1.x = _45.x;
    tmpvar_1.y = _45.y;
    tmpvar_1.z = _45.z;
    fragmentColor = tmpvar_1;
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
