Shader "Sprite/Vertex Colored Alpha8" {
Properties {
 _AlphaTex ("Alpha Texture", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZTest Always
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha OneMinusSrcAlpha, Zero One
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

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float4 _glesColor : COLOR;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float2 xlv_TEXCOORD0 : TEXCOORD0;
    float4 xlv_COLOR0 : TEXCOORD1;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    gl_Position = mul(_glesVertex, transpose(UNITY_MATRIX_MVP));
    xlv_COLOR0 = _glesColor;
    xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    _glesColor = stage_input._glesColor;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.xlv_COLOR0 = xlv_COLOR0;
    stage_output.xlv_TEXCOORD0 = xlv_TEXCOORD0;
    return stage_output;
}
uniform sampler2D _AlphaTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float2 xlv_TEXCOORD0 : TEXCOORD0;
    float4 xlv_COLOR0 : TEXCOORD1;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1;
    tmpvar_1.x = xlv_COLOR0.xyz.x;
    tmpvar_1.y = xlv_COLOR0.xyz.y;
    tmpvar_1.z = xlv_COLOR0.xyz.z;
    tmpvar_1.w = xlv_COLOR0.w * tex2D(_AlphaTex, xlv_TEXCOORD0).w;
    fragmentColor = tmpvar_1;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_COLOR0 = stage_input.xlv_COLOR0;
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
