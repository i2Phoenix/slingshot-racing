using System;
using UnityEngine;

[AddComponentMenu("Track/Track Edge Data")]
[RequireComponent(typeof(TrackSpline))]
public class TrackEdgeData : MonoBehaviour
{
	[Serializable]
	public class KeyFrame
	{
		public TrackEdgeType m_type;

		public float m_t;
	}

	public KeyFrame[] m_innerWallData;

	public KeyFrame[] m_outerWallData;

	public TrackEdgeType m_startInnerType;

	public TrackEdgeType m_startOuterType;

	public TrackEdgeType GetInnerWallType(float _t)
	{
		TrackEdgeType result = m_startInnerType;
		int num = 0;
		while (num < m_innerWallData.Length && m_innerWallData[num].m_t < _t)
		{
			result = m_innerWallData[num++].m_type;
		}
		return result;
	}

	public TrackEdgeType GetOuterWallType(float _t)
	{
		TrackEdgeType result = m_startOuterType;
		int num = 0;
		while (num < m_outerWallData.Length && m_outerWallData[num].m_t < _t)
		{
			result = m_outerWallData[num++].m_type;
		}
		return result;
	}

	public void OnReverseTrack()
	{
		TrackSpline component = GetComponent<TrackSpline>();
		CoreUtils.Swap(ref m_innerWallData, ref m_outerWallData);
		CoreUtils.Swap(ref m_startInnerType, ref m_startOuterType);
		if (m_innerWallData != null && m_innerWallData.Length > 0)
		{
			int num = m_innerWallData.Length;
			KeyFrame[] array = new KeyFrame[num];
			TrackEdgeType trackEdgeType = m_startInnerType;
			for (int i = 0; i < num; i++)
			{
				KeyFrame keyFrame = m_innerWallData[i];
				KeyFrame keyFrame2 = new KeyFrame();
				keyFrame2.m_type = trackEdgeType;
				keyFrame2.m_t = component.MaxT - keyFrame.m_t;
				array[num - (i + 1)] = keyFrame2;
				trackEdgeType = keyFrame.m_type;
			}
			m_innerWallData = array;
			m_startInnerType = trackEdgeType;
		}
		if (m_outerWallData != null && m_outerWallData.Length > 0)
		{
			int num2 = m_outerWallData.Length;
			KeyFrame[] array2 = new KeyFrame[num2];
			TrackEdgeType trackEdgeType2 = m_startOuterType;
			for (int j = 0; j < num2; j++)
			{
				KeyFrame keyFrame3 = m_outerWallData[j];
				KeyFrame keyFrame4 = new KeyFrame();
				keyFrame4.m_type = trackEdgeType2;
				keyFrame4.m_t = component.MaxT - keyFrame3.m_t;
				array2[num2 - (j + 1)] = keyFrame4;
				trackEdgeType2 = keyFrame3.m_type;
			}
			m_outerWallData = array2;
			m_startOuterType = trackEdgeType2;
		}
	}
}
