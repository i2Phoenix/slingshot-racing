Shader "Custom/BakedShadowProjection1Pass" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Geometry+25" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Geometry+25" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Blend Zero OneMinusSrcColor, Zero One
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"

static float4 gl_Position;
static float4 _glesVertex;
static float4 xlv_COLOR0;
static float4 _glesColor;
static float2 xlv_TEXCOORD0;
static float4 _glesMultiTexCoord0;
static float2 xlv_TEXCOORD1;
static float4 _glesMultiTexCoord1;

struct SRVertexInput
{
    float4 _glesMultiTexCoord1 : TEXCOORD1;
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float4 _glesColor : COLOR;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float2 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD0 : TEXCOORD1;
    float4 xlv_COLOR0 : TEXCOORD2;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float4 tmpvar_2;
    tmpvar_2.w = 1.0f;
    tmpvar_2.x = _glesVertex.xyz.x;
    tmpvar_2.y = _glesVertex.xyz.y;
    tmpvar_2.z = _glesVertex.xyz.z;
    float4 tmpvar_3 = mul(tmpvar_2, transpose(UNITY_MATRIX_MVP));
    float4 tmpvar_1 = tmpvar_3;
    tmpvar_1.z = tmpvar_3.z - 0.039999999105930328369140625f;
    gl_Position = tmpvar_1;
    xlv_COLOR0 = _glesColor;
    xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
    xlv_TEXCOORD1 = _glesMultiTexCoord1.xy;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    _glesColor = stage_input._glesColor;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    _glesMultiTexCoord1 = stage_input._glesMultiTexCoord1;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.xlv_COLOR0 = xlv_COLOR0;
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
    float4 xlv_COLOR0 : TEXCOORD2;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 1.0f;
    float3 _40 = xlv_COLOR0.xyz * lerp(tex2D(_MainTex, xlv_TEXCOORD0).w, tex2D(_MainTex, xlv_TEXCOORD1).w, xlv_COLOR0.w);
    tmpvar_1.x = _40.x;
    tmpvar_1.y = _40.y;
    tmpvar_1.z = _40.z;
    fragmentColor = tmpvar_1;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD0 = stage_input.xlv_TEXCOORD0;
    xlv_TEXCOORD1 = stage_input.xlv_TEXCOORD1;
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
