Shader "Custom/SpotlightFanSparkle" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _SparkleMap ("SparkleMap (RGB)", 2D) = "white" {}
}
SubShader { 
 LOD 250
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
uniform float4 g_spotlightSparkleData;
uniform float4 g_windSnowData;

static float4 gl_Position;
static float4 _glesTANGENT;
static float4 _glesVertex;
static float4 _glesMultiTexCoord1;
static float4 _glesColor;
static float2 xlv_TEXCOORD0;
static float4 _glesMultiTexCoord0;
static float2 xlv_TEXCOORD1;
static float3 xlv_TEXCOORD2;
static float3 xlv_TEXCOORD3;

struct SRVertexInput
{
    float4 _glesTANGENT : TEXCOORD0;
    float4 _glesMultiTexCoord1 : TEXCOORD1;
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float4 _glesColor : COLOR;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float3 xlv_TEXCOORD3 : TEXCOORD0;
    float3 xlv_TEXCOORD2 : TEXCOORD1;
    float2 xlv_TEXCOORD1 : TEXCOORD2;
    float2 xlv_TEXCOORD0 : TEXCOORD3;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float3 _15 = normalize(_glesTANGENT.xyz);
    float4 tmpvar_1;
    tmpvar_1.x = _15.x;
    tmpvar_1.y = _15.y;
    tmpvar_1.z = _15.z;
    tmpvar_1.w = _glesTANGENT.w;
    float4 tmpvar_5;
    tmpvar_5.w = 1.0f;
    tmpvar_5.x = _glesVertex.xyz.x;
    tmpvar_5.y = _glesVertex.xyz.y;
    tmpvar_5.z = _glesVertex.xyz.z;
    float4 tmpvar_6 = mul(tmpvar_5, transpose(unity_ObjectToWorld));
    float4 worldPos = tmpvar_6;
    float tmpvar_7;
    if (_glesMultiTexCoord1.x < 0.0f)
    {
        tmpvar_7 = 0.0f;
    }
    else
    {
        tmpvar_7 = _glesMultiTexCoord1.x;
    }
    float normalY = tmpvar_7;
    float tmpvar_8 = sqrt(1.0f - (_glesMultiTexCoord1.y * _glesMultiTexCoord1.y));
    float3 tmpvar_9;
    tmpvar_9.x = tmpvar_8;
    tmpvar_9.y = normalY;
    tmpvar_9.z = _glesMultiTexCoord1.y;
    float3 worldNormal = tmpvar_9;
    float3x3 tmpvar_10;
    tmpvar_10[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_10[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_10[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float3 tmpvar_11 = mul((tmpvar_1 * float4(1, 1, 1, 1).w).xyz, tmpvar_10);
    float3 worldTangent = tmpvar_11;
    float3 tmpvar_12 = normalize(g_cameraPos.xyz - tmpvar_6.xyz);
    float3 tmpvar_13;
    if (_glesMultiTexCoord1.x < 0.0f)
    {
        tmpvar_13 = float3(0.0f, 1.0f, 0.0f);
    }
    else
    {
        tmpvar_13 = tmpvar_12;
    }
    float3 tmpvar_14 = normalize(cross(tmpvar_13, worldTangent));
    float tmpvar_15 = dot(worldTangent, tmpvar_12);
    float tmpvar_16 = _glesColor.w * 32.0f;
    float dist = tmpvar_16;
    float3 tmpvar_17 = (((worldTangent * worldNormal.x) + ((cross(worldTangent, tmpvar_14) * clamp(-tmpvar_15, 0.0f, 1.0f)) * worldNormal.y)) + (tmpvar_14 * worldNormal.z)) * dist;
    float3 _178 = tmpvar_6.xyz + tmpvar_17;
    worldPos.x = _178.x;
    worldPos.y = _178.y;
    worldPos.z = _178.z;
    float4 _185 = worldPos;
    float3 _190 = _185.xyz + (tmpvar_12 * 0.20000000298023223876953125f);
    worldPos.x = _190.x;
    worldPos.y = _190.y;
    worldPos.z = _190.z;
    float4 tmpvar_18;
    tmpvar_18.w = 1.0f;
    float3 _203 = mul(worldPos, transpose(unity_WorldToObject)).xyz;
    tmpvar_18.x = _203.x;
    tmpvar_18.y = _203.y;
    tmpvar_18.z = _203.z;
    float3 tmpvar_19 = ((_glesColor * (1.0f - pow(abs(tmpvar_15), 4.0f))) * 2.0f).xyz;
    float3 tmpvar_4 = tmpvar_19;
    float2 tmpvar_20;
    tmpvar_20.x = dot(tmpvar_17, tmpvar_14);
    tmpvar_20.y = dot(tmpvar_17, worldTangent);
    float2 tmpvar_21;
    tmpvar_21.x = g_spotlightSparkleData.x;
    tmpvar_21.y = -g_spotlightSparkleData.x;
    float2 tmpvar_22 = (tmpvar_20 * 0.100000001490116119384765625f) + tmpvar_21;
    float2 tmpvar_2 = tmpvar_22;
    float tmpvar_23 = frac(g_windSnowData.y * 4.0f) * 3.0f;
    float3 tmpvar_24 = clamp(tmpvar_23.xxx - float3(0.0f, 1.0f, 2.0f), 0.0f.xxx, 1.0f.xxx);
    float3 tmpvar_25 = clamp((3.0f.xxx - tmpvar_23.xxx) - float3(1.0f, 0.0f, 2.0f), 0.0f.xxx, 1.0f.xxx);
    float2 _282 = min(tmpvar_24.xy, tmpvar_25.xy);
    float3 ramps;
    ramps.x = _282.x;
    ramps.y = _282.y;
    ramps.z = max(tmpvar_24.z, tmpvar_25.z);
    float3 tmpvar_26 = ramps * 2.0f;
    float3 tmpvar_3 = tmpvar_26;
    gl_Position = mul(tmpvar_18, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
    xlv_TEXCOORD1 = tmpvar_2;
    xlv_TEXCOORD2 = tmpvar_3;
    xlv_TEXCOORD3 = tmpvar_4;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesTANGENT = stage_input._glesTANGENT;
    _glesVertex = stage_input._glesVertex;
    _glesMultiTexCoord1 = stage_input._glesMultiTexCoord1;
    _glesColor = stage_input._glesColor;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.xlv_TEXCOORD0 = xlv_TEXCOORD0;
    stage_output.xlv_TEXCOORD1 = xlv_TEXCOORD1;
    stage_output.xlv_TEXCOORD2 = xlv_TEXCOORD2;
    stage_output.xlv_TEXCOORD3 = xlv_TEXCOORD3;
    return stage_output;
}
uniform sampler2D _MainTex;
uniform sampler2D _SparkleMap;

static float4 fragmentColor;

struct SRFragmentInput
{
    float3 xlv_TEXCOORD3 : TEXCOORD0;
    float3 xlv_TEXCOORD2 : TEXCOORD1;
    float2 xlv_TEXCOORD1 : TEXCOORD2;
    float2 xlv_TEXCOORD0 : TEXCOORD3;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1 = tex2D(_MainTex, xlv_TEXCOORD0);
    float4 tmpvar_2;
    tmpvar_2.w = 1.0f;
    float3 _45 = (tmpvar_1.xyz + (tmpvar_1.xyz * dot(tex2D(_SparkleMap, xlv_TEXCOORD1).xyz, xlv_TEXCOORD2))) * xlv_TEXCOORD3;
    tmpvar_2.x = _45.x;
    tmpvar_2.y = _45.y;
    tmpvar_2.z = _45.z;
    fragmentColor = tmpvar_2;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD0 = stage_input.xlv_TEXCOORD0;
    xlv_TEXCOORD1 = stage_input.xlv_TEXCOORD1;
    xlv_TEXCOORD2 = stage_input.xlv_TEXCOORD2;
    xlv_TEXCOORD3 = stage_input.xlv_TEXCOORD3;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
ENDCG

 }
}
SubShader { 
 Tags { "QUEUE"="Geometry+90" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 UsePass "Custom/SpotlightFan/CG"
}
}
