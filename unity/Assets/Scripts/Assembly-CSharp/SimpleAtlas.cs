using System;
using UnityEngine;

public class SimpleAtlas : ScriptableObject
{
	[Serializable]
	public class Animation
	{
		public int m_beginFrame;

		public int m_endFrame;

		public int m_frameRate;
	}

	public Animation[] m_animations;

	public int m_atlasResolution;

	public int m_tileWidth;

	public int m_tileHeight;

	public int m_tilesX;

	public int m_tilesY;

	public bool m_flipY;

	public Rect GetFrameRect(int _frameIndex)
	{
		int num = _frameIndex % m_tilesX;
		int num2 = _frameIndex / m_tilesX;
		if (m_flipY)
		{
			num2 = m_tilesY - (1 + num2);
		}
		return new Rect(num * m_tileWidth, num2 * m_tileHeight, m_tileWidth, m_tileHeight);
	}

	public Rect GetFrameUVRect(int _frameIndex)
	{
		int num = _frameIndex % m_tilesX;
		int num2 = _frameIndex / m_tilesX;
		if (m_flipY)
		{
			num2 = m_tilesY - (1 + num2);
		}
		float num3 = 1f / (float)m_atlasResolution;
		float num4 = (float)m_tileWidth * num3;
		float num5 = (float)m_tileHeight * num3;
		return new Rect((float)num * num4, (float)num2 * num5, num4, num5);
	}
}
