Shader "Custom/PlayerIdentifier" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Geometry+34" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Geometry+34" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZTest Always
  ZWrite Off
  Cull Off
  Blend One OneMinusSrcAlpha, Zero One
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float4 g_playerIdentifierAlpha;
uniform float4 g_cameraRight;
uniform float4 g_cameraUp;
uniform float4 g_playerIdentifierWidth;
uniform float4 g_playerIdentifierHeight;

static float4 gl_Position;
static float4 _glesVertex;
static float4 _glesColor;
static float4 _glesMultiTexCoord0;
static float2 xlv_TEXCOORD0;
static float4 xlv_COLOR;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float4 _glesColor : COLOR;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float4 xlv_COLOR : TEXCOORD0;
    float2 xlv_TEXCOORD0 : TEXCOORD1;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float4 tmpvar_1 = _glesVertex;
    float tmpvar_3 = _glesColor.w * 3.0f;
    float ramp = tmpvar_3;
    float4 upRamp = 1.0f.xxxx;
    float3 _37 = clamp(ramp.xxx - float3(0.0f, 1.0f, 2.0f), 0.0f.xxx, 1.0f.xxx);
    upRamp.y = _37.x;
    upRamp.z = _37.y;
    upRamp.w = _37.z;
    float4 downRamp = 1.0f.xxxx;
    float3 _55 = clamp((3.0f.xxx - ramp.xxx) - float3(2.0f, 1.0f, 0.0f), 0.0f.xxx, 1.0f.xxx);
    downRamp.x = _55.x;
    downRamp.y = _55.y;
    downRamp.z = _55.z;
    float4 tmpvar_4 = min(upRamp, downRamp);
    float tmpvar_5 = dot(tmpvar_4, g_playerIdentifierAlpha);
    float4 tmpvar_6;
    tmpvar_6.w = 1.0f;
    tmpvar_6.x = _glesVertex.xyz.x;
    tmpvar_6.y = _glesVertex.xyz.y;
    tmpvar_6.z = _glesVertex.xyz.z;
    float4 tmpvar_7 = mul(tmpvar_6, transpose(unity_ObjectToWorld));
    float4 worldPos = tmpvar_7;
    float2 tmpvar_8 = (_glesMultiTexCoord0.xy - 0.5f.xx) * 2.0f;
    float2 screenOffset = tmpvar_8;
    float3 tmpvar_9 = (g_cameraRight * screenOffset.x).xyz;
    float3 tmpvar_10 = (g_cameraUp * screenOffset.y).xyz;
    float3 _128 = tmpvar_7.xyz + (tmpvar_9 * (dot(tmpvar_4, g_playerIdentifierWidth) * 0.5f));
    worldPos.x = _128.x;
    worldPos.y = _128.y;
    worldPos.z = _128.z;
    float4 _135 = worldPos;
    float3 _144 = _135.xyz + (tmpvar_10 * (dot(tmpvar_4, g_playerIdentifierHeight) * 0.5f));
    worldPos.x = _144.x;
    worldPos.y = _144.y;
    worldPos.z = _144.z;
    float4 _151 = worldPos;
    float3 _161 = _151.xyz + ((tmpvar_9 + tmpvar_10) * ((1.0f - tmpvar_5) * 1.5f));
    worldPos.x = _161.x;
    worldPos.y = _161.y;
    worldPos.z = _161.z;
    float3 _172 = mul(worldPos, transpose(unity_WorldToObject)).xyz;
    tmpvar_1.x = _172.x;
    tmpvar_1.y = _172.y;
    tmpvar_1.z = _172.z;
    float4 tmpvar_11;
    tmpvar_11.w = 1.0f;
    tmpvar_11.x = tmpvar_1.xyz.x;
    tmpvar_11.y = tmpvar_1.xyz.y;
    tmpvar_11.z = tmpvar_1.xyz.z;
    float4 tmpvar_2 = _glesColor;
    tmpvar_2.w = tmpvar_5;
    gl_Position = mul(tmpvar_11, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
    xlv_COLOR = tmpvar_2;
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
    stage_output.xlv_COLOR = xlv_COLOR;
    return stage_output;
}
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float4 xlv_COLOR : TEXCOORD0;
    float2 xlv_TEXCOORD0 : TEXCOORD1;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float tmpvar_1 = tex2D(_MainTex, xlv_TEXCOORD0).y * xlv_COLOR.w;
    float3 tmpvar_2 = xlv_COLOR.xyz * tmpvar_1;
    float4 tmpvar_3;
    tmpvar_3.x = tmpvar_2.x;
    tmpvar_3.y = tmpvar_2.y;
    tmpvar_3.z = tmpvar_2.z;
    tmpvar_3.w = tmpvar_1 * 0.75f;
    fragmentColor = tmpvar_3;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD0 = stage_input.xlv_TEXCOORD0;
    xlv_COLOR = stage_input.xlv_COLOR;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
ENDCG

 }
}
}
