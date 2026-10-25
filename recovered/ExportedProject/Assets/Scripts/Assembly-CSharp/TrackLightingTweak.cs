using System;
using UnityEngine;

[AddComponentMenu("Track/Track Lighting Tweak")]
public class TrackLightingTweak : MonoBehaviour
{
	[Serializable]
	public class Tweak
	{
		public float m_startT;

		public float m_endT;

		public float m_height;
	}

	public Tweak[] m_tweaks;

	public float HeightAt(float _t)
	{
		float num = 0f;
		if (m_tweaks != null)
		{
			for (int i = 0; i < m_tweaks.Length; i++)
			{
				Tweak tweak = m_tweaks[i];
				if (_t >= tweak.m_startT && _t <= tweak.m_endT)
				{
					float num2 = (tweak.m_endT - tweak.m_startT) * 0.5f;
					float num3 = (tweak.m_endT + tweak.m_startT) * 0.5f;
					float num4 = (_t - num3) / num2;
					num += (1f - num4 * num4) * tweak.m_height;
				}
			}
		}
		return num;
	}

	public void OnReverseTrack()
	{
		if (m_tweaks != null && m_tweaks.Length > 0)
		{
			TrackSpline component = GetComponent<TrackSpline>();
			int num = m_tweaks.Length;
			Tweak[] array = new Tweak[num];
			for (int i = 0; i < num; i++)
			{
				Tweak tweak = m_tweaks[i];
				Tweak tweak2 = new Tweak();
				tweak2.m_startT = component.MaxT - tweak.m_endT;
				tweak2.m_endT = component.MaxT - tweak.m_startT;
				tweak2.m_height = tweak.m_height;
				array[num - (i + 1)] = tweak2;
			}
			m_tweaks = array;
		}
	}
}
