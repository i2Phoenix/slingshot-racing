using UnityEngine;

[AddComponentMenu("Track/Track Lighting")]
public class TrackLighting : MonoBehaviour
{
	private bool m_reversed;

	public Color32[] m_coarseSamples;

	public int m_numCoarseSamplesX;

	public int m_numCoarseSamplesY;

	public int lightmapIndex;

	public Vector4 lightmapTilingOffset;

	public Vector4 trackUVScaleOffset;

	public float m_lightmapPadding;

	public bool HasCoarseSamples
	{
		get
		{
			return m_coarseSamples != null && m_coarseSamples.Length > 0;
		}
	}

	public bool Reversed
	{
		get
		{
			return m_reversed;
		}
	}

	public Color32 GetCoarseSample(int _x, int _y)
	{
		return m_coarseSamples[_x + _y * m_numCoarseSamplesX];
	}

	public void SetCoarseSample(int _x, int _y, Color32 _colour)
	{
		m_coarseSamples[_x + _y * m_numCoarseSamplesX] = _colour;
	}

	public Color CoarseSample(float _u, float _v)
	{
		float num = m_numCoarseSamplesX - 1;
		float num2 = m_numCoarseSamplesY - 1;
		float num3 = _u * num;
		float num4 = _v * num2;
		int max = m_numCoarseSamplesX - 2;
		int max2 = m_numCoarseSamplesY - 2;
		int num5 = MathUtils.Clamp((int)num3, 0, max);
		int num6 = MathUtils.Clamp((int)num4, 0, max2);
		int num7 = num5 + 1;
		int num8 = num6 + 1;
		float lerp = Mathf.Clamp01(num3 - (float)num5);
		float lerp2 = Mathf.Clamp01(num4 - (float)num6);
		int num9 = num6 * m_numCoarseSamplesX;
		int num10 = num8 * m_numCoarseSamplesX;
		Color a = m_coarseSamples[num5 + num9];
		Color b = m_coarseSamples[num7 + num9];
		Color a2 = m_coarseSamples[num5 + num10];
		Color b2 = m_coarseSamples[num7 + num10];
		Color a3 = ColourUtils.LerpNoClamp(a, b, lerp);
		Color b3 = ColourUtils.LerpNoClamp(a2, b2, lerp);
		Color color = ColourUtils.LerpNoClamp(a3, b3, lerp2);
		float num11 = color.a * 4f;
		Color result = default(Color);
		result.r = color.r * num11;
		result.g = color.g * num11;
		result.b = color.b * num11;
		result.a = 1f;
		return result;
	}

	public void OnReverseTrack()
	{
		m_reversed = true;
	}
}
