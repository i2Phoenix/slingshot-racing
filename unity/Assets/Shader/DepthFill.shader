Shader "Custom/DepthFill" {
SubShader { 
 Tags { "QUEUE"="Geometry+40" "RenderType"="Opaque" }
 Pass {
  Name "BASE"
  Tags { "QUEUE"="Geometry+40" "RenderType"="Opaque" }
  ColorMask 0
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
    float4 tmpvar_2;
    tmpvar_2.w = 1.0f;
    tmpvar_2.x = _glesVertex.xyz.x;
    tmpvar_2.y = _glesVertex.xyz.y;
    tmpvar_2.z = _glesVertex.xyz.z;
    float4 tmpvar_3 = mul(tmpvar_2, transpose(UNITY_MATRIX_MVP));
    float4 tmpvar_1 = tmpvar_3;
    tmpvar_1.z = tmpvar_3.z - 0.00449999980628490447998046875f;
    gl_Position = tmpvar_1;
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
    fragmentColor = 1.0f.xxxx;
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
