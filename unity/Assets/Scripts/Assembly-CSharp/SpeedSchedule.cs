using System;
using UnityEngine;

public class SpeedSchedule : ScriptableObject
{
	[Serializable]
	public class SplineData
	{
		public string m_name;

		public float[] m_normalisedSpeeds;

		public float m_segmentLength;
	}

	public SplineData[] m_splineData;
}
