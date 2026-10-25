Shader "Custom/PipeGUI_Streamer" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _SpecColor ("Specular Colour", Color) = (1,1,1,1)
 _SpecPower ("Specular Power", Range(0,128)) = 10
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  Cull Off
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float _SpecPower;
uniform float4 _SpecColor;

static float4 gl_Position;
static float3 _glesNormal;
static float4 _glesVertex;
static float4 _glesColor;
static float2 xlv_TEXCOORD0;
static float4 _glesMultiTexCoord0;
static float3 xlv_TEXCOORD1;
static float3 xlv_TEXCOORD2;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float3 _glesNormal : NORMAL;
    float4 _glesColor : COLOR;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float3 xlv_TEXCOORD2 : TEXCOORD0;
    float3 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD2;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float3 tmpvar_1 = normalize(_glesNormal);
    float4 tmpvar_4;
    tmpvar_4.w = 1.0f;
    tmpvar_4.x = _glesVertex.xyz.x;
    tmpvar_4.y = _glesVertex.xyz.y;
    tmpvar_4.z = _glesVertex.xyz.z;
    float4 worldPos = tmpvar_4;
    worldPos.z = _glesVertex.z * 0.0199999995529651641845703125f;
    float4 tmpvar_5 = mul(worldPos, transpose(UNITY_MATRIX_MVP));
    float tmpvar_6 = dot(float3(-0.577367007732391357421875f, 0.577367007732391357421875f, -0.577367007732391357421875f), tmpvar_1);
    float3 tmpvar_7 = (_glesColor * (abs(tmpvar_6) + 0.4000000059604644775390625f)).xyz;
    float3 tmpvar_2 = tmpvar_7;
    float tmpvar_8 = dot(normalize(float3(-0.577367007732391357421875f, 0.577367007732391357421875f, -0.577367007732391357421875f) + normalize(_WorldSpaceCameraPos - worldPos.xyz)), tmpvar_1);
    float tmpvar_9;
    if (tmpvar_6 > 0.0f)
    {
        tmpvar_9 = tmpvar_8;
    }
    else
    {
        tmpvar_9 = -tmpvar_8;
    }
    float3 tmpvar_10 = (_SpecColor.xyz * 4.0f) * pow(max(0.0f, tmpvar_9), _SpecPower);
    float3 tmpvar_3 = tmpvar_10;
    gl_Position = tmpvar_5;
    xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
    xlv_TEXCOORD1 = tmpvar_2;
    xlv_TEXCOORD2 = tmpvar_3;
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesNormal = stage_input._glesNormal;
    _glesVertex = stage_input._glesVertex;
    _glesColor = stage_input._glesColor;
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.xlv_TEXCOORD0 = xlv_TEXCOORD0;
    stage_output.xlv_TEXCOORD1 = xlv_TEXCOORD1;
    stage_output.xlv_TEXCOORD2 = xlv_TEXCOORD2;
    return stage_output;
}
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float3 xlv_TEXCOORD2 : TEXCOORD0;
    float3 xlv_TEXCOORD1 : TEXCOORD1;
    float2 xlv_TEXCOORD0 : TEXCOORD2;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float4 tmpvar_1 = tex2D(_MainTex, xlv_TEXCOORD0);
    float4 c = tmpvar_1;
    float3 _37 = (tmpvar_1.xyz + (xlv_TEXCOORD2 * tmpvar_1.w)) * xlv_TEXCOORD1;
    c.x = _37.x;
    c.y = _37.y;
    c.z = _37.z;
    c.w = 1.0f;
    fragmentColor = c;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD0 = stage_input.xlv_TEXCOORD0;
    xlv_TEXCOORD2 = stage_input.xlv_TEXCOORD2;
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
