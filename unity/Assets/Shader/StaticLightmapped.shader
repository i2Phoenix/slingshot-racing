// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Custom/StaticLightmapped" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Pass {
  Name "GLSL"
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"

static float4 gl_Position;
static float4 _glesVertex;
static float2 i_mainUV;
static float4 _glesMultiTexCoord0;
static float2 i_lightmapUV;
static float4 _glesMultiTexCoord1;

struct SRVertexInput
{
    float4 _glesVertex : POSITION;
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float4 _glesMultiTexCoord1 : TEXCOORD1;
};

struct SRVertexOutput
{
    float2 i_mainUV : TEXCOORD0;
    float2 i_lightmapUV : TEXCOORD1;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    gl_Position = mul(float4(_glesVertex.xyz, 1.0f), transpose(UNITY_MATRIX_MVP));
    i_mainUV = _glesMultiTexCoord0.xy;
    i_lightmapUV = (_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    _glesMultiTexCoord1 = stage_input._glesMultiTexCoord1;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.i_mainUV = i_mainUV;
    stage_output.i_lightmapUV = i_lightmapUV;
    return stage_output;
}
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float2 i_mainUV : TEXCOORD0;
    float2 i_lightmapUV : TEXCOORD1;
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
    float3 colour = diffuse * lightmap;
    fragmentColor.x = colour.x;
    fragmentColor.y = colour.y;
    fragmentColor.z = colour.z;
    fragmentColor.w = 1.0f;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    i_lightmapUV = stage_input.i_lightmapUV;
    i_mainUV = stage_input.i_mainUV;
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
  Name "CG"
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float4 _glesMultiTexCoord1;
static float2 xlv_TEXCOORD0;
static float2 xlv_TEXCOORD1;

struct SRVertexInput
{
    float4 _glesMultiTexCoord1 : TEXCOORD1;
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float2 xlv_TEXCOORD1 : TEXCOORD0;
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
    float2 tmpvar_4 = _glesMultiTexCoord0.xy;
    float2 tmpvar_1 = tmpvar_4;
    float2 tmpvar_5 = (_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw;
    float2 tmpvar_2 = tmpvar_5;
    gl_Position = mul(tmpvar_3, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD0 = tmpvar_1;
    xlv_TEXCOORD1 = tmpvar_2;
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
    return stage_output;
}
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float2 xlv_TEXCOORD1 : TEXCOORD0;
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
    float3 _37 = (tex2D(_MainTex, xlv_TEXCOORD0).xyz * 0.58823502063751220703125f) * (UNITY_SAMPLE_TEX2D(unity_Lightmap, xlv_TEXCOORD1).xyz * 2.0f);
    tmpvar_1.x = _37.x;
    tmpvar_1.y = _37.y;
    tmpvar_1.z = _37.z;
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
