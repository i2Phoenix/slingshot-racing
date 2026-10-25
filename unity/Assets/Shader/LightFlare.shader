Shader "Custom/LightFlare" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Geometry+90" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Geometry+90" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Blend One One
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float4 g_cameraPos;
uniform float4 g_cameraForward;
uniform float4 g_cameraUp;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesMultiTexCoord0;
static float4 _glesColor;
static float2 xlv_TEXCOORD0;
static float3 xlv_TEXCOORD1;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float4 _glesColor : COLOR;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD0 : TEXCOORD1;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float4 tmpvar_2;
    tmpvar_2.w = 1.0f;
    tmpvar_2.x = _glesVertex.xyz.x;
    tmpvar_2.y = _glesVertex.xyz.y;
    tmpvar_2.z = _glesVertex.xyz.z;
    float4 tmpvar_3 = mul(tmpvar_2, transpose(unity_ObjectToWorld));
    float4 worldPos = tmpvar_3;
    float3 tmpvar_4 = normalize(g_cameraPos.xyz - tmpvar_3.xyz);
    float3 tmpvar_5 = normalize(cross(-tmpvar_4, normalize(g_cameraForward.xyz - g_cameraUp.xyz)));
    float2 tmpvar_6 = (_glesMultiTexCoord0.xy - 0.5f.xx) * 2.0f;
    float tmpvar_7 = _glesColor.w * 32.0f;
    float3 _100 = tmpvar_3.xyz + ((((cross(tmpvar_4, tmpvar_5) * tmpvar_6.x) + (tmpvar_5 * tmpvar_6.y)) * tmpvar_7) + (tmpvar_4 * (tmpvar_7 * 0.25f)));
    worldPos.x = _100.x;
    worldPos.y = _100.y;
    worldPos.z = _100.z;
    float4 tmpvar_8;
    tmpvar_8.w = 1.0f;
    float3 _113 = mul(worldPos, transpose(unity_WorldToObject)).xyz;
    tmpvar_8.x = _113.x;
    tmpvar_8.y = _113.y;
    tmpvar_8.z = _113.z;
    float3 tmpvar_9 = _glesColor.xyz + _glesColor.xyz;
    float3 tmpvar_1 = tmpvar_9;
    gl_Position = mul(tmpvar_8, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
    xlv_TEXCOORD1 = tmpvar_1;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesVertex = stage_input._glesVertex;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    _glesColor = stage_input._glesColor;
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
    float3 xlv_TEXCOORD1 : TEXCOORD0;
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
    float3 _30 = tex2D(_MainTex, xlv_TEXCOORD0).xyz * xlv_TEXCOORD1;
    tmpvar_1.x = _30.x;
    tmpvar_1.y = _30.y;
    tmpvar_1.z = _30.z;
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
