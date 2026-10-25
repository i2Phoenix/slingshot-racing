Shader "Hidden/BlurEffectConeTap" {
Properties {
 _MainTex ("", any) = "" {}
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  SetTexture [_MainTex] { ConstantColor (0,0,0,0.25) combine texture * constant alpha }
  SetTexture [_MainTex] { ConstantColor (0,0,0,0.25) combine texture * constant + previous }
  SetTexture [_MainTex] { ConstantColor (0,0,0,0.25) combine texture * constant + previous }
  SetTexture [_MainTex] { ConstantColor (0,0,0,0.25) combine texture * constant + previous }
 }
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
CGPROGRAM
#pragma vertex SRVertex
#pragma fragment SRFragment
#pragma target 3.0
#include "UnityCG.cginc"
uniform float4 _BlurOffsets;
uniform float4 _MainTex_TexelSize;

static float4 gl_Position;
static float4 _glesMultiTexCoord0;
static float4 _glesVertex;
static float2 xlv_TEXCOORD0;
static float2 xlv_TEXCOORD1;
static float2 xlv_TEXCOORD1_1;
static float2 xlv_TEXCOORD1_2;
static float2 xlv_TEXCOORD1_3;

struct SRVertexInput
{
    float4 _glesMultiTexCoord0 : TEXCOORD0;
    float4 _glesVertex : POSITION;
};

struct SRVertexOutput
{
    float2 xlv_TEXCOORD1_3 : TEXCOORD0;
    float2 xlv_TEXCOORD1_2 : TEXCOORD1;
    float2 xlv_TEXCOORD1_1 : TEXCOORD2;
    float2 xlv_TEXCOORD1 : TEXCOORD3;
    float2 xlv_TEXCOORD0 : TEXCOORD4;
    float4 gl_Position : SV_POSITION;
};

void vert_main()
{
    float2 tmpvar_2 = _glesMultiTexCoord0.xy - (_BlurOffsets.xy * _MainTex_TexelSize.xy);
    float2 tmpvar_1[4];
    tmpvar_1[0] = tmpvar_2 + (_MainTex_TexelSize.xy * _BlurOffsets.xy);
    tmpvar_1[1] = tmpvar_2 - (_MainTex_TexelSize.xy * _BlurOffsets.xy);
    tmpvar_1[2] = tmpvar_2 + ((_MainTex_TexelSize.xy * _BlurOffsets.xy) * float2(1.0f, -1.0f));
    tmpvar_1[3] = tmpvar_2 - ((_MainTex_TexelSize.xy * _BlurOffsets.xy) * float2(1.0f, -1.0f));
    gl_Position = mul(_glesVertex, transpose(UNITY_MATRIX_MVP));
    xlv_TEXCOORD0 = tmpvar_2;
    xlv_TEXCOORD1 = tmpvar_1[0];
    xlv_TEXCOORD1_1 = tmpvar_1[1];
    xlv_TEXCOORD1_2 = tmpvar_1[2];
    xlv_TEXCOORD1_3 = tmpvar_1[3];
}

SRVertexOutput SRVertex(SRVertexInput stage_input)
{
    _glesMultiTexCoord0 = stage_input._glesMultiTexCoord0;
    _glesVertex = stage_input._glesVertex;
    vert_main();
    SRVertexOutput stage_output;
    stage_output.gl_Position = gl_Position;
    stage_output.xlv_TEXCOORD0 = xlv_TEXCOORD0;
    stage_output.xlv_TEXCOORD1 = xlv_TEXCOORD1;
    stage_output.xlv_TEXCOORD1_1 = xlv_TEXCOORD1_1;
    stage_output.xlv_TEXCOORD1_2 = xlv_TEXCOORD1_2;
    stage_output.xlv_TEXCOORD1_3 = xlv_TEXCOORD1_3;
    return stage_output;
}
uniform sampler2D _MainTex;

static float4 fragmentColor;

struct SRFragmentInput
{
    float2 xlv_TEXCOORD1_3 : TEXCOORD0;
    float2 xlv_TEXCOORD1_2 : TEXCOORD1;
    float2 xlv_TEXCOORD1_1 : TEXCOORD2;
    float2 xlv_TEXCOORD1 : TEXCOORD3;
};

struct SRFragmentOutput
{
    float4 fragmentColor : COLOR0;
};

void frag_main()
{
    float2 tmpvar_1[4];
    tmpvar_1[0] = xlv_TEXCOORD1;
    tmpvar_1[1] = xlv_TEXCOORD1_1;
    tmpvar_1[2] = xlv_TEXCOORD1_2;
    tmpvar_1[3] = xlv_TEXCOORD1_3;
    float4 tmpvar_2 = tex2D(_MainTex, tmpvar_1[0]);
    float4 color = tmpvar_2;
    float4 tmpvar_3 = tex2D(_MainTex, tmpvar_1[1]);
    float4 tmpvar_4 = tex2D(_MainTex, tmpvar_1[2]);
    float4 tmpvar_5 = tex2D(_MainTex, tmpvar_1[3]);
    float4 tmpvar_6 = ((color + tmpvar_3) + tmpvar_4) + tmpvar_5;
    color = tmpvar_6;
    fragmentColor = tmpvar_6 * 0.25f;
}

SRFragmentOutput SRFragment(SRFragmentInput stage_input)
{
    xlv_TEXCOORD1 = stage_input.xlv_TEXCOORD1;
    xlv_TEXCOORD1_1 = stage_input.xlv_TEXCOORD1_1;
    xlv_TEXCOORD1_2 = stage_input.xlv_TEXCOORD1_2;
    xlv_TEXCOORD1_3 = stage_input.xlv_TEXCOORD1_3;
    frag_main();
    SRFragmentOutput stage_output;
    stage_output.fragmentColor = float4(fragmentColor);
    return stage_output;
}
ENDCG

 }
}
Fallback Off
}
