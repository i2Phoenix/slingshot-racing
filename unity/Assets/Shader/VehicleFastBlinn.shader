Shader "Custom/VehicleFastBlinn" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _GlossTex ("Gloss (G)", 2D) = "white" {}
 _Lightmap ("Lightmap (RGB)", 2D) = "lightmap" { }
 _SpecPower ("Specular Power", Range(0,128)) = 48
 _UOffset ("UOffset", Float) = 0
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
#pragma multi_compile DISABLE_CONSTANT_EXTRACTION ENABLE_CONSTANT_EXTRACTION
#if defined(DISABLE_CONSTANT_EXTRACTION)
uniform float4 g_highlightDirection;
uniform float4 p_lmMatR1;
uniform float4 p_lmMatR2;
uniform float _UOffset;
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
    gl_Position.z -= 0.004999999888241291046142578125f;
    i_mainUV.x = _glesMultiTexCoord0.x + ((_glesMultiTexCoord0.y < 0.75f) ? _UOffset : 0.0f);
    i_mainUV.y = _glesMultiTexCoord0.y;
    i_lightmapUV.x = dot(lmMatR1, float4(_glesVertex.xyz, 1.0f));
    i_lightmapUV.y = dot(lmMatR2, float4(_glesVertex.xyz, 1.0f));
    float3 _133 = max(dot(-lightDirOS, _glesNormal), 0.0f).xxx;
    i_colour.x = _133.x;
    i_colour.y = _133.y;
    i_colour.z = _133.z;
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
#elif defined(ENABLE_CONSTANT_EXTRACTION)
uniform float3 p_lightDirOS;
uniform float3 p_halfVecOS;
uniform float4 p_lmMatR1;
uniform float4 p_lmMatR2;
uniform float _UOffset;
uniform float _SpecPower;
uniform float4 g_highlightDirection;

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
    float3 lightDirOS = p_lightDirOS;
    float3 halfVecOS = p_halfVecOS;
    float4 lmMatR1 = p_lmMatR1;
    float4 lmMatR2 = p_lmMatR2;
    gl_Position = mul(float4(_glesVertex.xyz, 1.0f), transpose(UNITY_MATRIX_MVP));
    gl_Position.z -= 0.004999999888241291046142578125f;
    i_mainUV.x = _glesMultiTexCoord0.x + ((_glesMultiTexCoord0.y < 0.75f) ? _UOffset : 0.0f);
    i_mainUV.y = _glesMultiTexCoord0.y;
    i_lightmapUV.x = dot(lmMatR1, float4(_glesVertex.xyz, 1.0f));
    i_lightmapUV.y = dot(lmMatR2, float4(_glesVertex.xyz, 1.0f));
    float3 _106 = max(dot(-lightDirOS, _glesNormal), 0.0f).xxx;
    i_colour.x = _106.x;
    i_colour.y = _106.y;
    i_colour.z = _106.z;
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
#endif
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
#pragma multi_compile DISABLE_CONSTANT_EXTRACTION ENABLE_CONSTANT_EXTRACTION
#if defined(DISABLE_CONSTANT_EXTRACTION)
uniform float4 g_highlightDirection;
uniform float4 p_lmMatR1;
uniform float4 p_lmMatR2;
uniform float _UOffset;
uniform float _SpecPower;

static float4 gl_Position;
static float3 _glesNormal;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float2 xlv_TEXCOORD0;
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
    float3x3 tmpvar_6;
    tmpvar_6[0] = transpose(unity_WorldToObject)[0].xyz;
    tmpvar_6[1] = transpose(unity_WorldToObject)[1].xyz;
    tmpvar_6[2] = transpose(unity_WorldToObject)[2].xyz;
    float3 tmpvar_7 = mul(g_highlightDirection.xyz, tmpvar_6);
    float4 tmpvar_8;
    tmpvar_8.w = 1.0f;
    tmpvar_8.x = _WorldSpaceCameraPos.x;
    tmpvar_8.y = _WorldSpaceCameraPos.y;
    tmpvar_8.z = _WorldSpaceCameraPos.z;
    float3 tmpvar_9 = normalize(normalize(mul(tmpvar_8, transpose(unity_WorldToObject)).xyz) - tmpvar_7);
    float4 tmpvar_10 = mul(transpose(unity_ObjectToWorld), p_lmMatR1);
    float4 tmpvar_11 = mul(transpose(unity_ObjectToWorld), p_lmMatR2);
    float4 tmpvar_12;
    tmpvar_12.w = 1.0f;
    tmpvar_12.x = _glesVertex.xyz.x;
    tmpvar_12.y = _glesVertex.xyz.y;
    tmpvar_12.z = _glesVertex.xyz.z;
    float4 tmpvar_13 = mul(tmpvar_12, transpose(UNITY_MATRIX_MVP));
    float4 tmpvar_2 = tmpvar_13;
    tmpvar_2.z = tmpvar_13.z - 0.004999999888241291046142578125f;
    float tmpvar_14;
    if (_glesMultiTexCoord0.y < 0.75f)
    {
        tmpvar_14 = _UOffset;
    }
    else
    {
        tmpvar_14 = 0.0f;
    }
    float tmpvar_15 = _glesMultiTexCoord0.x + tmpvar_14;
    float2 tmpvar_3;
    tmpvar_3.x = tmpvar_15;
    float tmpvar_16 = _glesMultiTexCoord0.y;
    tmpvar_3.y = tmpvar_16;
    float4 tmpvar_17;
    tmpvar_17.w = 1.0f;
    tmpvar_17.x = _glesVertex.xyz.x;
    tmpvar_17.y = _glesVertex.xyz.y;
    tmpvar_17.z = _glesVertex.xyz.z;
    float tmpvar_18 = dot(tmpvar_10, tmpvar_17);
    float2 tmpvar_4;
    tmpvar_4.x = tmpvar_18;
    float4 tmpvar_19;
    tmpvar_19.w = 1.0f;
    tmpvar_19.x = _glesVertex.xyz.x;
    tmpvar_19.y = _glesVertex.xyz.y;
    tmpvar_19.z = _glesVertex.xyz.z;
    float tmpvar_20 = dot(tmpvar_11, tmpvar_19);
    tmpvar_4.y = tmpvar_20;
    float tmpvar_21 = max(dot(-tmpvar_7, tmpvar_1), 0.0f);
    float3 _178 = tmpvar_21.xxx;
    float4 tmpvar_5;
    tmpvar_5.x = _178.x;
    tmpvar_5.y = _178.y;
    tmpvar_5.z = _178.z;
    float tmpvar_22 = pow(max(dot(tmpvar_9, tmpvar_1), 0.0f), _SpecPower);
    tmpvar_5.w = tmpvar_22;
    gl_Position = tmpvar_2;
    xlv_TEXCOORD0 = tmpvar_3;
    xlv_TEXCOORD1 = tmpvar_4;
    xlv_COLOR0 = tmpvar_5;
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
#elif defined(ENABLE_CONSTANT_EXTRACTION)
uniform float _UOffset;
uniform float4 p_lmMatR1;
uniform float4 p_lmMatR2;
uniform float3 p_lightDirOS;
uniform float3 p_halfVecOS;
uniform float _SpecPower;

static float4 gl_Position;
static float3 _glesNormal;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float2 xlv_TEXCOORD0;
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
    float4 tmpvar_6;
    tmpvar_6.w = 1.0f;
    tmpvar_6.x = _glesVertex.xyz.x;
    tmpvar_6.y = _glesVertex.xyz.y;
    tmpvar_6.z = _glesVertex.xyz.z;
    float4 tmpvar_7 = mul(tmpvar_6, transpose(UNITY_MATRIX_MVP));
    float4 tmpvar_2 = tmpvar_7;
    tmpvar_2.z = tmpvar_7.z - 0.004999999888241291046142578125f;
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
    float2 tmpvar_3;
    tmpvar_3.x = tmpvar_9;
    float tmpvar_10 = _glesMultiTexCoord0.y;
    tmpvar_3.y = tmpvar_10;
    float4 tmpvar_11;
    tmpvar_11.w = 1.0f;
    tmpvar_11.x = _glesVertex.xyz.x;
    tmpvar_11.y = _glesVertex.xyz.y;
    tmpvar_11.z = _glesVertex.xyz.z;
    float tmpvar_12 = dot(p_lmMatR1, tmpvar_11);
    float2 tmpvar_4;
    tmpvar_4.x = tmpvar_12;
    float4 tmpvar_13;
    tmpvar_13.w = 1.0f;
    tmpvar_13.x = _glesVertex.xyz.x;
    tmpvar_13.y = _glesVertex.xyz.y;
    tmpvar_13.z = _glesVertex.xyz.z;
    float tmpvar_14 = dot(p_lmMatR2, tmpvar_13);
    tmpvar_4.y = tmpvar_14;
    float tmpvar_15 = max(dot(-p_lightDirOS, tmpvar_1), 0.0f);
    float3 _125 = tmpvar_15.xxx;
    float4 tmpvar_5;
    tmpvar_5.x = _125.x;
    tmpvar_5.y = _125.y;
    tmpvar_5.z = _125.z;
    float tmpvar_16 = pow(max(dot(p_halfVecOS, tmpvar_1), 0.0f), _SpecPower);
    tmpvar_5.w = tmpvar_16;
    gl_Position = tmpvar_2;
    xlv_TEXCOORD0 = tmpvar_3;
    xlv_TEXCOORD1 = tmpvar_4;
    xlv_COLOR0 = tmpvar_5;
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
uniform float4 g_highlightDirection;
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
#endif
ENDCG

 }
}
}
