Shader "Custom/BakedBlobShadow" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Geometry+20" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Geometry+20" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Blend One OneMinusSrcAlpha, Zero One
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float4 g_blobShadowGlow;
uniform float4 g_blobShadowAlpha;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesColor;
static float2 xlv_TEXCOORD0;
static float4 _glesMultiTexCoord0;
static float4 xlv_TEXCOORD1;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float4 _glesColor : COLOR;
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
    float4 tmpvar_3;
    tmpvar_3.w = 1.0f;
    tmpvar_3.x = _glesVertex.xyz.x;
    tmpvar_3.y = _glesVertex.xyz.y;
    tmpvar_3.z = _glesVertex.xyz.z;
    float4 tmpvar_4 = mul(tmpvar_3, transpose(UNITY_MATRIX_MVP));
    float4 tmpvar_1 = tmpvar_4;
    tmpvar_1.z = tmpvar_4.z - 0.039999999105930328369140625f;
    float3 tmpvar_5 = float3(1.0f, 0.89999997615814208984375f, 0.800000011920928955078125f) * dot(_glesColor, g_blobShadowGlow);
    float4 tmpvar_2;
    tmpvar_2.x = tmpvar_5.x;
    tmpvar_2.y = tmpvar_5.y;
    tmpvar_2.z = tmpvar_5.z;
    float tmpvar_6 = dot(_glesColor, g_blobShadowAlpha);
    tmpvar_2.w = tmpvar_6;
    gl_Position = tmpvar_1;
    xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
    xlv_TEXCOORD1 = tmpvar_2;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    _glesColor = stage_input._glesColor;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
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
    float4 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD0 : TEXCOORD1;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    fragmentColor = xlv_TEXCOORD1 * tex2D(_MainTex, xlv_TEXCOORD0).w;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD1 = stage_input.xlv_TEXCOORD1;
    xlv_TEXCOORD0 = stage_input.xlv_TEXCOORD0;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
ENDCG

 }
}
}
