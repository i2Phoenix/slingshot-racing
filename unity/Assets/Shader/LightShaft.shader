Shader "Custom/LightShaft" {
SubShader { 
 Tags { "QUEUE"="Geometry+100" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Geometry+100" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Blend One One
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesColor;
static float4 xlv_TEXCOORD;

struct SRVertexInput
{
    float4 _glesColor : COLOR;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float4 xlv_TEXCOORD : TEXCOORD0;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float4 tmpvar_2;
    tmpvar_2.w = 1.0f;
    tmpvar_2.x = _glesVertex.xyz.x;
    tmpvar_2.y = _glesVertex.xyz.y;
    tmpvar_2.z = _glesVertex.xyz.z;
    float4 tmpvar_3 = _glesColor + _glesColor;
    float4 tmpvar_1 = tmpvar_3;
    gl_Position = mul(tmpvar_2, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD = tmpvar_1;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    _glesColor = stage_input._glesColor;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.xlv_TEXCOORD = xlv_TEXCOORD;
    return stage_output;
}
static float4 fragmentColor;

struct SRFragmentInput
{
    float4 xlv_TEXCOORD : TEXCOORD0;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    fragmentColor = xlv_TEXCOORD;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD = stage_input.xlv_TEXCOORD;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
ENDCG

 }
}
}
