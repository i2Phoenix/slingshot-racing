Shader "Custom/FrontEndDiffuse" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
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
#pragma multi_compile DIRECTIONAL
#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_ON
#pragma multi_compile SHADOWS_OFF SHADOWS_SCREEN
#pragma multi_compile __ VERTEXLIGHT_ON
#if defined(DIRECTIONAL) && defined(SHADOWS_OFF) && defined(LIGHTMAP_OFF) && defined(DIRLIGHTMAP_OFF)
uniform float3 _LightColor0;
uniform float3 _Color;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float3 _glesNormal;
static float2 xlv_TEXCOORD;
static float3 xlv_TEXCOORD1;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
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
    float4 tmpvar_5;
    tmpvar_5.w = 0.0f;
    float3 _43 = normalize(_glesNormal);
    tmpvar_5.x = _43.x;
    tmpvar_5.y = _43.y;
    tmpvar_5.z = _43.z;
    float3 tmpvar_6 = normalize(mul(tmpvar_5, transpose(unity_ObjectToWorld)).xyz);
    float4 tmpvar_7;
    tmpvar_7.w = 1.0f;
    tmpvar_7.x = tmpvar_6.x;
    tmpvar_7.y = tmpvar_6.y;
    tmpvar_7.z = tmpvar_6.z;
    float4 normal_i0 = tmpvar_7;
    float tmpvar_9 = dot(unity_SHAr, normal_i0);
    float3 x1;
    x1.x = tmpvar_9;
    float tmpvar_10 = dot(unity_SHAg, normal_i0);
    x1.y = tmpvar_10;
    float tmpvar_11 = dot(unity_SHAb, normal_i0);
    x1.z = tmpvar_11;
    float4 tmpvar_12 = normal_i0.xyzz * normal_i0.yzzx;
    float tmpvar_13 = dot(unity_SHBr, tmpvar_12);
    float3 x2;
    x2.x = tmpvar_13;
    float tmpvar_14 = dot(unity_SHBg, tmpvar_12);
    x2.y = tmpvar_14;
    float tmpvar_15 = dot(unity_SHBb, tmpvar_12);
    x2.z = tmpvar_15;
    float tmpvar_16 = (normal_i0.x * normal_i0.x) - (normal_i0.y * normal_i0.y);
    float vC = tmpvar_16;
    float3 tmpvar_17 = unity_SHC.xyz * vC;
    float3 x3 = tmpvar_17;
    float3 tmpvar_8 = ShadeSH9(normal_i0);
    float3 tmpvar_18 = (((_LightColor0 * 2.0f) * max(dot(tmpvar_6, _WorldSpaceLightPos0.xyz), 0.0f)) + tmpvar_8) * _Color;
    float3 tmpvar_2 = tmpvar_18;
    gl_Position = mul(tmpvar_3, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD = tmpvar_1;
    xlv_TEXCOORD1 = tmpvar_2;
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
    return stage_output;
}
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 1.0f;
    float3 _30 = tex2D(_MainTex, xlv_TEXCOORD).xyz * xlv_TEXCOORD1;
    tmpvar_1.x = _30.x;
    tmpvar_1.y = _30.y;
    tmpvar_1.z = _30.z;
    fragmentColor = tmpvar_1;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD = stage_input.xlv_TEXCOORD;
    xlv_TEXCOORD1 = stage_input.xlv_TEXCOORD1;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
#elif defined(DIRECTIONAL) && defined(SHADOWS_OFF) && defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_OFF)
uniform float3 _LightColor0;
uniform float3 _Color;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float3 _glesNormal;
static float2 xlv_TEXCOORD;
static float3 xlv_TEXCOORD1;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
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
    float4 tmpvar_5;
    tmpvar_5.w = 0.0f;
    float3 _43 = normalize(_glesNormal);
    tmpvar_5.x = _43.x;
    tmpvar_5.y = _43.y;
    tmpvar_5.z = _43.z;
    float3 tmpvar_6 = normalize(mul(tmpvar_5, transpose(unity_ObjectToWorld)).xyz);
    float4 tmpvar_7;
    tmpvar_7.w = 1.0f;
    tmpvar_7.x = tmpvar_6.x;
    tmpvar_7.y = tmpvar_6.y;
    tmpvar_7.z = tmpvar_6.z;
    float4 normal_i0 = tmpvar_7;
    float tmpvar_9 = dot(unity_SHAr, normal_i0);
    float3 x1;
    x1.x = tmpvar_9;
    float tmpvar_10 = dot(unity_SHAg, normal_i0);
    x1.y = tmpvar_10;
    float tmpvar_11 = dot(unity_SHAb, normal_i0);
    x1.z = tmpvar_11;
    float4 tmpvar_12 = normal_i0.xyzz * normal_i0.yzzx;
    float tmpvar_13 = dot(unity_SHBr, tmpvar_12);
    float3 x2;
    x2.x = tmpvar_13;
    float tmpvar_14 = dot(unity_SHBg, tmpvar_12);
    x2.y = tmpvar_14;
    float tmpvar_15 = dot(unity_SHBb, tmpvar_12);
    x2.z = tmpvar_15;
    float tmpvar_16 = (normal_i0.x * normal_i0.x) - (normal_i0.y * normal_i0.y);
    float vC = tmpvar_16;
    float3 tmpvar_17 = unity_SHC.xyz * vC;
    float3 x3 = tmpvar_17;
    float3 tmpvar_8 = ShadeSH9(normal_i0);
    float3 tmpvar_18 = (((_LightColor0 * 2.0f) * max(dot(tmpvar_6, _WorldSpaceLightPos0.xyz), 0.0f)) + tmpvar_8) * _Color;
    float3 tmpvar_2 = tmpvar_18;
    gl_Position = mul(tmpvar_3, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD = tmpvar_1;
    xlv_TEXCOORD1 = tmpvar_2;
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
    return stage_output;
}
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 1.0f;
    float3 _30 = tex2D(_MainTex, xlv_TEXCOORD).xyz * xlv_TEXCOORD1;
    tmpvar_1.x = _30.x;
    tmpvar_1.y = _30.y;
    tmpvar_1.z = _30.z;
    fragmentColor = tmpvar_1;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD = stage_input.xlv_TEXCOORD;
    xlv_TEXCOORD1 = stage_input.xlv_TEXCOORD1;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
#elif defined(DIRECTIONAL) && defined(SHADOWS_OFF) && defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_ON)
uniform float3 _LightColor0;
uniform float3 _Color;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float3 _glesNormal;
static float2 xlv_TEXCOORD;
static float3 xlv_TEXCOORD1;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
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
    float4 tmpvar_5;
    tmpvar_5.w = 0.0f;
    float3 _43 = normalize(_glesNormal);
    tmpvar_5.x = _43.x;
    tmpvar_5.y = _43.y;
    tmpvar_5.z = _43.z;
    float3 tmpvar_6 = normalize(mul(tmpvar_5, transpose(unity_ObjectToWorld)).xyz);
    float4 tmpvar_7;
    tmpvar_7.w = 1.0f;
    tmpvar_7.x = tmpvar_6.x;
    tmpvar_7.y = tmpvar_6.y;
    tmpvar_7.z = tmpvar_6.z;
    float4 normal_i0 = tmpvar_7;
    float tmpvar_9 = dot(unity_SHAr, normal_i0);
    float3 x1;
    x1.x = tmpvar_9;
    float tmpvar_10 = dot(unity_SHAg, normal_i0);
    x1.y = tmpvar_10;
    float tmpvar_11 = dot(unity_SHAb, normal_i0);
    x1.z = tmpvar_11;
    float4 tmpvar_12 = normal_i0.xyzz * normal_i0.yzzx;
    float tmpvar_13 = dot(unity_SHBr, tmpvar_12);
    float3 x2;
    x2.x = tmpvar_13;
    float tmpvar_14 = dot(unity_SHBg, tmpvar_12);
    x2.y = tmpvar_14;
    float tmpvar_15 = dot(unity_SHBb, tmpvar_12);
    x2.z = tmpvar_15;
    float tmpvar_16 = (normal_i0.x * normal_i0.x) - (normal_i0.y * normal_i0.y);
    float vC = tmpvar_16;
    float3 tmpvar_17 = unity_SHC.xyz * vC;
    float3 x3 = tmpvar_17;
    float3 tmpvar_8 = ShadeSH9(normal_i0);
    float3 tmpvar_18 = (((_LightColor0 * 2.0f) * max(dot(tmpvar_6, _WorldSpaceLightPos0.xyz), 0.0f)) + tmpvar_8) * _Color;
    float3 tmpvar_2 = tmpvar_18;
    gl_Position = mul(tmpvar_3, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD = tmpvar_1;
    xlv_TEXCOORD1 = tmpvar_2;
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
    return stage_output;
}
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 1.0f;
    float3 _30 = tex2D(_MainTex, xlv_TEXCOORD).xyz * xlv_TEXCOORD1;
    tmpvar_1.x = _30.x;
    tmpvar_1.y = _30.y;
    tmpvar_1.z = _30.z;
    fragmentColor = tmpvar_1;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD = stage_input.xlv_TEXCOORD;
    xlv_TEXCOORD1 = stage_input.xlv_TEXCOORD1;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
#elif defined(DIRECTIONAL) && defined(SHADOWS_SCREEN) && defined(LIGHTMAP_OFF) && defined(DIRLIGHTMAP_OFF)
uniform float3 _LightColor0;
uniform float3 _Color;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float3 _glesNormal;
static float2 xlv_TEXCOORD;
static float3 xlv_TEXCOORD1;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
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
    float4 tmpvar_5;
    tmpvar_5.w = 0.0f;
    float3 _43 = normalize(_glesNormal);
    tmpvar_5.x = _43.x;
    tmpvar_5.y = _43.y;
    tmpvar_5.z = _43.z;
    float3 tmpvar_6 = normalize(mul(tmpvar_5, transpose(unity_ObjectToWorld)).xyz);
    float4 tmpvar_7;
    tmpvar_7.w = 1.0f;
    tmpvar_7.x = tmpvar_6.x;
    tmpvar_7.y = tmpvar_6.y;
    tmpvar_7.z = tmpvar_6.z;
    float4 normal_i0 = tmpvar_7;
    float tmpvar_9 = dot(unity_SHAr, normal_i0);
    float3 x1;
    x1.x = tmpvar_9;
    float tmpvar_10 = dot(unity_SHAg, normal_i0);
    x1.y = tmpvar_10;
    float tmpvar_11 = dot(unity_SHAb, normal_i0);
    x1.z = tmpvar_11;
    float4 tmpvar_12 = normal_i0.xyzz * normal_i0.yzzx;
    float tmpvar_13 = dot(unity_SHBr, tmpvar_12);
    float3 x2;
    x2.x = tmpvar_13;
    float tmpvar_14 = dot(unity_SHBg, tmpvar_12);
    x2.y = tmpvar_14;
    float tmpvar_15 = dot(unity_SHBb, tmpvar_12);
    x2.z = tmpvar_15;
    float tmpvar_16 = (normal_i0.x * normal_i0.x) - (normal_i0.y * normal_i0.y);
    float vC = tmpvar_16;
    float3 tmpvar_17 = unity_SHC.xyz * vC;
    float3 x3 = tmpvar_17;
    float3 tmpvar_8 = ShadeSH9(normal_i0);
    float3 tmpvar_18 = (((_LightColor0 * 2.0f) * max(dot(tmpvar_6, _WorldSpaceLightPos0.xyz), 0.0f)) + tmpvar_8) * _Color;
    float3 tmpvar_2 = tmpvar_18;
    gl_Position = mul(tmpvar_3, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD = tmpvar_1;
    xlv_TEXCOORD1 = tmpvar_2;
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
    return stage_output;
}
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 1.0f;
    float3 _30 = tex2D(_MainTex, xlv_TEXCOORD).xyz * xlv_TEXCOORD1;
    tmpvar_1.x = _30.x;
    tmpvar_1.y = _30.y;
    tmpvar_1.z = _30.z;
    fragmentColor = tmpvar_1;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD = stage_input.xlv_TEXCOORD;
    xlv_TEXCOORD1 = stage_input.xlv_TEXCOORD1;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
#elif defined(DIRECTIONAL) && defined(SHADOWS_SCREEN) && defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_OFF)
uniform float3 _LightColor0;
uniform float3 _Color;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float3 _glesNormal;
static float2 xlv_TEXCOORD;
static float3 xlv_TEXCOORD1;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
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
    float4 tmpvar_5;
    tmpvar_5.w = 0.0f;
    float3 _43 = normalize(_glesNormal);
    tmpvar_5.x = _43.x;
    tmpvar_5.y = _43.y;
    tmpvar_5.z = _43.z;
    float3 tmpvar_6 = normalize(mul(tmpvar_5, transpose(unity_ObjectToWorld)).xyz);
    float4 tmpvar_7;
    tmpvar_7.w = 1.0f;
    tmpvar_7.x = tmpvar_6.x;
    tmpvar_7.y = tmpvar_6.y;
    tmpvar_7.z = tmpvar_6.z;
    float4 normal_i0 = tmpvar_7;
    float tmpvar_9 = dot(unity_SHAr, normal_i0);
    float3 x1;
    x1.x = tmpvar_9;
    float tmpvar_10 = dot(unity_SHAg, normal_i0);
    x1.y = tmpvar_10;
    float tmpvar_11 = dot(unity_SHAb, normal_i0);
    x1.z = tmpvar_11;
    float4 tmpvar_12 = normal_i0.xyzz * normal_i0.yzzx;
    float tmpvar_13 = dot(unity_SHBr, tmpvar_12);
    float3 x2;
    x2.x = tmpvar_13;
    float tmpvar_14 = dot(unity_SHBg, tmpvar_12);
    x2.y = tmpvar_14;
    float tmpvar_15 = dot(unity_SHBb, tmpvar_12);
    x2.z = tmpvar_15;
    float tmpvar_16 = (normal_i0.x * normal_i0.x) - (normal_i0.y * normal_i0.y);
    float vC = tmpvar_16;
    float3 tmpvar_17 = unity_SHC.xyz * vC;
    float3 x3 = tmpvar_17;
    float3 tmpvar_8 = ShadeSH9(normal_i0);
    float3 tmpvar_18 = (((_LightColor0 * 2.0f) * max(dot(tmpvar_6, _WorldSpaceLightPos0.xyz), 0.0f)) + tmpvar_8) * _Color;
    float3 tmpvar_2 = tmpvar_18;
    gl_Position = mul(tmpvar_3, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD = tmpvar_1;
    xlv_TEXCOORD1 = tmpvar_2;
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
    return stage_output;
}
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 1.0f;
    float3 _30 = tex2D(_MainTex, xlv_TEXCOORD).xyz * xlv_TEXCOORD1;
    tmpvar_1.x = _30.x;
    tmpvar_1.y = _30.y;
    tmpvar_1.z = _30.z;
    fragmentColor = tmpvar_1;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD = stage_input.xlv_TEXCOORD;
    xlv_TEXCOORD1 = stage_input.xlv_TEXCOORD1;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
#elif defined(DIRECTIONAL) && defined(SHADOWS_SCREEN) && defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_ON)
uniform float3 _LightColor0;
uniform float3 _Color;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float3 _glesNormal;
static float2 xlv_TEXCOORD;
static float3 xlv_TEXCOORD1;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
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
    float4 tmpvar_5;
    tmpvar_5.w = 0.0f;
    float3 _43 = normalize(_glesNormal);
    tmpvar_5.x = _43.x;
    tmpvar_5.y = _43.y;
    tmpvar_5.z = _43.z;
    float3 tmpvar_6 = normalize(mul(tmpvar_5, transpose(unity_ObjectToWorld)).xyz);
    float4 tmpvar_7;
    tmpvar_7.w = 1.0f;
    tmpvar_7.x = tmpvar_6.x;
    tmpvar_7.y = tmpvar_6.y;
    tmpvar_7.z = tmpvar_6.z;
    float4 normal_i0 = tmpvar_7;
    float tmpvar_9 = dot(unity_SHAr, normal_i0);
    float3 x1;
    x1.x = tmpvar_9;
    float tmpvar_10 = dot(unity_SHAg, normal_i0);
    x1.y = tmpvar_10;
    float tmpvar_11 = dot(unity_SHAb, normal_i0);
    x1.z = tmpvar_11;
    float4 tmpvar_12 = normal_i0.xyzz * normal_i0.yzzx;
    float tmpvar_13 = dot(unity_SHBr, tmpvar_12);
    float3 x2;
    x2.x = tmpvar_13;
    float tmpvar_14 = dot(unity_SHBg, tmpvar_12);
    x2.y = tmpvar_14;
    float tmpvar_15 = dot(unity_SHBb, tmpvar_12);
    x2.z = tmpvar_15;
    float tmpvar_16 = (normal_i0.x * normal_i0.x) - (normal_i0.y * normal_i0.y);
    float vC = tmpvar_16;
    float3 tmpvar_17 = unity_SHC.xyz * vC;
    float3 x3 = tmpvar_17;
    float3 tmpvar_8 = ShadeSH9(normal_i0);
    float3 tmpvar_18 = (((_LightColor0 * 2.0f) * max(dot(tmpvar_6, _WorldSpaceLightPos0.xyz), 0.0f)) + tmpvar_8) * _Color;
    float3 tmpvar_2 = tmpvar_18;
    gl_Position = mul(tmpvar_3, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD = tmpvar_1;
    xlv_TEXCOORD1 = tmpvar_2;
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
    return stage_output;
}
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 1.0f;
    float3 _30 = tex2D(_MainTex, xlv_TEXCOORD).xyz * xlv_TEXCOORD1;
    tmpvar_1.x = _30.x;
    tmpvar_1.y = _30.y;
    tmpvar_1.z = _30.z;
    fragmentColor = tmpvar_1;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD = stage_input.xlv_TEXCOORD;
    xlv_TEXCOORD1 = stage_input.xlv_TEXCOORD1;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
#elif defined(DIRECTIONAL) && defined(SHADOWS_OFF) && defined(LIGHTMAP_OFF) && defined(DIRLIGHTMAP_OFF) && defined(VERTEXLIGHT_ON)
uniform float3 _LightColor0;
uniform float3 _Color;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float3 _glesNormal;
static float2 xlv_TEXCOORD;
static float3 xlv_TEXCOORD1;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
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
    float4 tmpvar_5;
    tmpvar_5.w = 0.0f;
    float3 _43 = normalize(_glesNormal);
    tmpvar_5.x = _43.x;
    tmpvar_5.y = _43.y;
    tmpvar_5.z = _43.z;
    float3 tmpvar_6 = normalize(mul(tmpvar_5, transpose(unity_ObjectToWorld)).xyz);
    float4 tmpvar_7;
    tmpvar_7.w = 1.0f;
    tmpvar_7.x = tmpvar_6.x;
    tmpvar_7.y = tmpvar_6.y;
    tmpvar_7.z = tmpvar_6.z;
    float4 normal_i0 = tmpvar_7;
    float tmpvar_9 = dot(unity_SHAr, normal_i0);
    float3 x1;
    x1.x = tmpvar_9;
    float tmpvar_10 = dot(unity_SHAg, normal_i0);
    x1.y = tmpvar_10;
    float tmpvar_11 = dot(unity_SHAb, normal_i0);
    x1.z = tmpvar_11;
    float4 tmpvar_12 = normal_i0.xyzz * normal_i0.yzzx;
    float tmpvar_13 = dot(unity_SHBr, tmpvar_12);
    float3 x2;
    x2.x = tmpvar_13;
    float tmpvar_14 = dot(unity_SHBg, tmpvar_12);
    x2.y = tmpvar_14;
    float tmpvar_15 = dot(unity_SHBb, tmpvar_12);
    x2.z = tmpvar_15;
    float tmpvar_16 = (normal_i0.x * normal_i0.x) - (normal_i0.y * normal_i0.y);
    float vC = tmpvar_16;
    float3 tmpvar_17 = unity_SHC.xyz * vC;
    float3 x3 = tmpvar_17;
    float3 tmpvar_8 = ShadeSH9(normal_i0);
    float3 tmpvar_18 = (((_LightColor0 * 2.0f) * max(dot(tmpvar_6, _WorldSpaceLightPos0.xyz), 0.0f)) + tmpvar_8) * _Color;
    float3 tmpvar_2 = tmpvar_18;
    gl_Position = mul(tmpvar_3, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD = tmpvar_1;
    xlv_TEXCOORD1 = tmpvar_2;
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
    return stage_output;
}
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 1.0f;
    float3 _30 = tex2D(_MainTex, xlv_TEXCOORD).xyz * xlv_TEXCOORD1;
    tmpvar_1.x = _30.x;
    tmpvar_1.y = _30.y;
    tmpvar_1.z = _30.z;
    fragmentColor = tmpvar_1;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD = stage_input.xlv_TEXCOORD;
    xlv_TEXCOORD1 = stage_input.xlv_TEXCOORD1;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
#elif defined(DIRECTIONAL) && defined(SHADOWS_SCREEN) && defined(LIGHTMAP_OFF) && defined(DIRLIGHTMAP_OFF) && defined(VERTEXLIGHT_ON)
uniform float3 _LightColor0;
uniform float3 _Color;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float3 _glesNormal;
static float2 xlv_TEXCOORD;
static float3 xlv_TEXCOORD1;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
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
    float4 tmpvar_5;
    tmpvar_5.w = 0.0f;
    float3 _43 = normalize(_glesNormal);
    tmpvar_5.x = _43.x;
    tmpvar_5.y = _43.y;
    tmpvar_5.z = _43.z;
    float3 tmpvar_6 = normalize(mul(tmpvar_5, transpose(unity_ObjectToWorld)).xyz);
    float4 tmpvar_7;
    tmpvar_7.w = 1.0f;
    tmpvar_7.x = tmpvar_6.x;
    tmpvar_7.y = tmpvar_6.y;
    tmpvar_7.z = tmpvar_6.z;
    float4 normal_i0 = tmpvar_7;
    float tmpvar_9 = dot(unity_SHAr, normal_i0);
    float3 x1;
    x1.x = tmpvar_9;
    float tmpvar_10 = dot(unity_SHAg, normal_i0);
    x1.y = tmpvar_10;
    float tmpvar_11 = dot(unity_SHAb, normal_i0);
    x1.z = tmpvar_11;
    float4 tmpvar_12 = normal_i0.xyzz * normal_i0.yzzx;
    float tmpvar_13 = dot(unity_SHBr, tmpvar_12);
    float3 x2;
    x2.x = tmpvar_13;
    float tmpvar_14 = dot(unity_SHBg, tmpvar_12);
    x2.y = tmpvar_14;
    float tmpvar_15 = dot(unity_SHBb, tmpvar_12);
    x2.z = tmpvar_15;
    float tmpvar_16 = (normal_i0.x * normal_i0.x) - (normal_i0.y * normal_i0.y);
    float vC = tmpvar_16;
    float3 tmpvar_17 = unity_SHC.xyz * vC;
    float3 x3 = tmpvar_17;
    float3 tmpvar_8 = ShadeSH9(normal_i0);
    float3 tmpvar_18 = (((_LightColor0 * 2.0f) * max(dot(tmpvar_6, _WorldSpaceLightPos0.xyz), 0.0f)) + tmpvar_8) * _Color;
    float3 tmpvar_2 = tmpvar_18;
    gl_Position = mul(tmpvar_3, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD = tmpvar_1;
    xlv_TEXCOORD1 = tmpvar_2;
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
    return stage_output;
}
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD : TEXCOORD1;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 1.0f;
    float3 _30 = tex2D(_MainTex, xlv_TEXCOORD).xyz * xlv_TEXCOORD1;
    tmpvar_1.x = _30.x;
    tmpvar_1.y = _30.y;
    tmpvar_1.z = _30.z;
    fragmentColor = tmpvar_1;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD = stage_input.xlv_TEXCOORD;
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
