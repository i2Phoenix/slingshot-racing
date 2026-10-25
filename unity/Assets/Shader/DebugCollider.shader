Shader "Editor/RedAlpha" {
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha, Zero One
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"

static float4 gl_Position;
static float4 _glesVertex;

struct SRVertexInput
{
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float4 tmpvar_1;
    tmpvar_1.w = 1.0f;
    tmpvar_1.x = _glesVertex.xyz.x;
    tmpvar_1.y = _glesVertex.xyz.y;
    tmpvar_1.z = _glesVertex.xyz.z;
    gl_Position = mul(tmpvar_1, transpose(UNITY_MATRIX_MVP));
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    return stage_output;
}
static float4 fragmentColor;

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    fragmentColor = float4(1.0f, 0.0f, 0.0f, 0.25f);
}

SRFragmentOutput SRFragment()
{
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
ENDCG

 }
}
}
