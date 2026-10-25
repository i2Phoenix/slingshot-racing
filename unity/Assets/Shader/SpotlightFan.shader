Shader "Custom/SpotlightFan" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Geometry+90" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Name "CG"
  Tags { "QUEUE"="Geometry+90" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Blend One One
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float4 g_cameraPos;

static float4 gl_Position;
static float4 _glesTANGENT;
static float4 _glesVertex;
static float4 _glesMultiTexCoord1;
static float4 _glesColor;
static float2 xlv_TEXCOORD0;
static float4 _glesMultiTexCoord0;
static float3 xlv_TEXCOORD1;

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
    float3 xlv_TEXCOORD1 : TEXCOORD0;
    float2 xlv_TEXCOORD0 : TEXCOORD1;
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
    float4 tmpvar_3;
    tmpvar_3.w = 1.0f;
    tmpvar_3.x = _glesVertex.xyz.x;
    tmpvar_3.y = _glesVertex.xyz.y;
    tmpvar_3.z = _glesVertex.xyz.z;
    float4 tmpvar_4 = mul(tmpvar_3, transpose(unity_ObjectToWorld));
    float4 worldPos = tmpvar_4;
    float tmpvar_5;
    if (_glesMultiTexCoord1.x < 0.0f)
    {
        tmpvar_5 = 0.0f;
    }
    else
    {
        tmpvar_5 = _glesMultiTexCoord1.x;
    }
    float normalY = tmpvar_5;
    float tmpvar_6 = sqrt(1.0f - (_glesMultiTexCoord1.y * _glesMultiTexCoord1.y));
    float3 tmpvar_7;
    tmpvar_7.x = tmpvar_6;
    tmpvar_7.y = normalY;
    tmpvar_7.z = _glesMultiTexCoord1.y;
    float3 worldNormal = tmpvar_7;
    float3x3 tmpvar_8;
    tmpvar_8[0] = transpose(unity_ObjectToWorld)[0].xyz;
    tmpvar_8[1] = transpose(unity_ObjectToWorld)[1].xyz;
    tmpvar_8[2] = transpose(unity_ObjectToWorld)[2].xyz;
    float3 tmpvar_9 = mul((tmpvar_1 * float4(1, 1, 1, 1).w).xyz, tmpvar_8);
    float3 worldTangent = tmpvar_9;
    float3 tmpvar_10 = normalize(g_cameraPos.xyz - tmpvar_4.xyz);
    float3 tmpvar_11;
    if (_glesMultiTexCoord1.x < 0.0f)
    {
        tmpvar_11 = float3(0.0f, 1.0f, 0.0f);
    }
    else
    {
        tmpvar_11 = tmpvar_10;
    }
    float3 tmpvar_12 = normalize(cross(tmpvar_11, worldTangent));
    float tmpvar_13 = dot(worldTangent, tmpvar_10);
    float tmpvar_14 = _glesColor.w * 32.0f;
    float dist = tmpvar_14;
    float3 _176 = tmpvar_4.xyz + ((((worldTangent * worldNormal.x) + ((cross(worldTangent, tmpvar_12) * clamp(-tmpvar_13, 0.0f, 1.0f)) * worldNormal.y)) + (tmpvar_12 * worldNormal.z)) * dist);
    worldPos.x = _176.x;
    worldPos.y = _176.y;
    worldPos.z = _176.z;
    float4 _183 = worldPos;
    float3 _188 = _183.xyz + (tmpvar_10 * 0.20000000298023223876953125f);
    worldPos.x = _188.x;
    worldPos.y = _188.y;
    worldPos.z = _188.z;
    float4 tmpvar_15;
    tmpvar_15.w = 1.0f;
    float3 _201 = mul(worldPos, transpose(unity_WorldToObject)).xyz;
    tmpvar_15.x = _201.x;
    tmpvar_15.y = _201.y;
    tmpvar_15.z = _201.z;
    float3 tmpvar_16 = _glesColor.xyz * ((1.0f - pow(abs(tmpvar_13), 4.0f)) * 2.0f);
    float3 tmpvar_2 = tmpvar_16;
    gl_Position = mul(tmpvar_15, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
    xlv_TEXCOORD1 = tmpvar_2;
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
