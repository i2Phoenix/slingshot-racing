using System;
using UnityEngine;

[RequireComponent(typeof(GrapplePoint))]
[AddComponentMenu("AI/AI Grapple Point")]
public class AIGrapplePoint : MonoBehaviour
{
	[Serializable]
	public class Data
	{
		public float m_innerStartT;

		public float m_outerStartT;

		public float m_innerEndT;

		public float m_outerEndT;

		public Data Clone()
		{
			Data data = new Data();
			data.m_innerStartT = m_innerStartT;
			data.m_outerStartT = m_outerStartT;
			data.m_innerEndT = m_innerEndT;
			data.m_outerEndT = m_outerEndT;
			return data;
		}
	}

	private static readonly float[] s_difficultyOffset = new float[4] { 0.75f, 0.5f, 0.25f, 0f };

	private static readonly float[] s_difficultyScale = new float[4] { 0.25f, 0.5f, 0.75f, 1f };

	[SerializeField]
	[HideInInspector]
	private Data[] m_perDifficultyData;

	[HideInInspector]
	[SerializeField]
	private Data[] m_perDifficultyDataReversed;

	private GrapplePoint m_cachedGrapplePoint;

	public float m_minT;

	public float m_maxT;

	public float m_coreZoneStartT;

	public float m_coreZoneEndT;

	public GrapplePoint TargetGrapplePoint
	{
		get
		{
			if (m_cachedGrapplePoint == null)
			{
				m_cachedGrapplePoint = GetComponent<GrapplePoint>();
			}
			return m_cachedGrapplePoint;
		}
	}

	public void EnsurePerDifficultyData()
	{
		if (m_perDifficultyData == null || m_perDifficultyData.Length != 4)
		{
			m_perDifficultyData = new Data[4];
			for (int i = 0; i < 4; i++)
			{
				m_perDifficultyData[i] = new Data();
				GenerateBasicData((AIDifficulty)i);
			}
		}
		if (m_perDifficultyDataReversed == null || m_perDifficultyDataReversed.Length != 4)
		{
			m_perDifficultyDataReversed = new Data[4];
			for (int j = 0; j < 4; j++)
			{
				m_perDifficultyDataReversed[j] = new Data();
				GenerateBasicReversedData((AIDifficulty)j);
			}
		}
	}

	private void Awake()
	{
		EnsurePerDifficultyData();
	}

	private void Start()
	{
	}

	public void GetDefaultRange(out float o_startT, out float o_endT)
	{
		o_startT = m_minT;
		o_endT = m_maxT;
		TrackSpline targetTrackSpline = TargetGrapplePoint.TargetTrackSpline;
		if (targetTrackSpline != null)
		{
			for (int i = 0; i < 4; i++)
			{
				Vector3 vector = targetTrackSpline.PointAtT(o_startT);
				Vector3 normalized = targetTrackSpline.TangentAtT(o_startT).normalized;
				float num = Vector3.Dot(TargetGrapplePoint.Position - vector, normalized);
				vector += normalized * num;
				o_startT = targetTrackSpline.ClosestTTo(vector);
				o_startT = Mathf.Clamp(o_startT, TargetGrapplePoint.startProp, TargetGrapplePoint.endProp);
				Vector3 vector2 = targetTrackSpline.PointAtT(o_endT);
				Vector3 normalized2 = targetTrackSpline.TangentAtT(o_endT).normalized;
				float num2 = Vector3.Dot(TargetGrapplePoint.Position - vector2, normalized2);
				vector2 += normalized2 * num2;
				o_endT = targetTrackSpline.ClosestTTo(vector2);
				o_endT = Mathf.Clamp(o_endT, TargetGrapplePoint.startProp, TargetGrapplePoint.endProp);
			}
		}
	}

	public void GenerateBasicData(AIDifficulty _difficulty)
	{
		EnsurePerDifficultyData();
		Data data = m_perDifficultyData[(int)_difficulty];
		TrackSpline targetTrackSpline = TargetGrapplePoint.TargetTrackSpline;
		if (targetTrackSpline != null)
		{
			float o_startT;
			float o_endT;
			GetDefaultRange(out o_startT, out o_endT);
			float num = o_endT - o_startT;
			float num2 = 0f;
			for (int i = 1; i < 5; i++)
			{
				float num3 = (float)i / 5f;
				float t = o_startT + num3 * num;
				num2 += targetTrackSpline.CurveAtT(t);
			}
			float num4 = num * 0.1f;
			float num5 = 0f;
			float num6 = 0f;
			if (num2 < -0.5f)
			{
				num5 = num4;
				num6 = 0f - num4;
			}
			if (num2 > -0.5f)
			{
				num5 = 0f - num4;
				num6 = num4;
			}
			float num7 = s_difficultyScale[(int)_difficulty];
			float num8 = s_difficultyOffset[(int)_difficulty];
			num5 = num5 * num7 + num8 * num4;
			num6 = num6 * num7 + num8 * num4;
			float startProp = TargetGrapplePoint.startProp;
			float endProp = TargetGrapplePoint.endProp;
			data.m_innerStartT = Mathf.Clamp(o_startT + num5, startProp, endProp);
			data.m_outerStartT = Mathf.Clamp(o_startT + num6, startProp, endProp);
			data.m_innerEndT = Mathf.Clamp(o_endT - num5, startProp, endProp);
			data.m_outerEndT = Mathf.Clamp(o_endT - num6, startProp, endProp);
		}
		else
		{
			data.m_innerStartT = 0f;
			data.m_outerStartT = 0f;
			data.m_innerEndT = 0f;
			data.m_outerEndT = 0f;
		}
	}

	public void GenerateBasicReversedData(AIDifficulty _difficulty)
	{
		EnsurePerDifficultyData();
		Data data = m_perDifficultyDataReversed[(int)_difficulty];
		Data data2 = m_perDifficultyData[(int)_difficulty];
		if (data != null && data2 != null)
		{
			float num = ((!(TargetGrapplePoint.TargetTrackSpline != null)) ? 0f : TargetGrapplePoint.TargetTrackSpline.MaxT);
			data.m_innerStartT = num - data2.m_innerEndT;
			data.m_outerStartT = num - data2.m_outerEndT;
			data.m_innerEndT = num - data2.m_innerStartT;
			data.m_outerEndT = num - data2.m_outerStartT;
		}
	}

	public Data GetData(AIDifficulty _difficulty)
	{
		return m_perDifficultyData[(int)_difficulty];
	}

	public void SetData(AIDifficulty _difficulty, Data _data)
	{
		m_perDifficultyData[(int)_difficulty] = _data;
	}

	public Data GetReversedData(AIDifficulty _difficulty)
	{
		return m_perDifficultyDataReversed[(int)_difficulty];
	}

	public void SetReversedData(AIDifficulty _difficulty, Data _data)
	{
		m_perDifficultyDataReversed[(int)_difficulty] = _data;
	}

	public void OnReverseTrack()
	{
		Data[] perDifficultyData = m_perDifficultyData;
		m_perDifficultyData = m_perDifficultyDataReversed;
		m_perDifficultyDataReversed = perDifficultyData;
		float num = ((!(TargetGrapplePoint.TargetTrackSpline != null)) ? 0f : TargetGrapplePoint.TargetTrackSpline.MaxT);
		float minT = m_minT;
		float maxT = m_maxT;
		float coreZoneStartT = m_coreZoneStartT;
		float coreZoneEndT = m_coreZoneEndT;
		m_minT = num - maxT;
		m_maxT = num - minT;
		m_coreZoneStartT = num - coreZoneEndT;
		m_coreZoneEndT = num - coreZoneStartT;
	}
}
