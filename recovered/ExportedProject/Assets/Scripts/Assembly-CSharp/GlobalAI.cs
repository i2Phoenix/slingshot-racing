using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("AI/Global AI")]
public class GlobalAI : MonoBehaviour
{
	private const string c_globalAIName = "__GlobalAI";

	private const string c_globalAIFind = "/__GlobalAI";

	private const string c_globalAIPrefab = "Prefabs/AI/GlobalAI";

	private const float c_interval = 2f;

	private const float c_segmentSize = 60f;

	private TrackLocation m_finishLocation;

	private RaceManager m_raceManager;

	private RubberBanding m_globalRubberBanding;

	private float m_segmentsPerLap;

	private List<Vehicle> m_ais;

	private List<Vehicle> m_humans;

	private void Awake()
	{
		m_ais = new List<Vehicle>();
		m_humans = new List<Vehicle>();
		m_globalRubberBanding = RubberBanding.None;
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
	}

	private void Start()
	{
		m_finishLocation = FinishLine.Get().Location;
		float num = TrackUtils.TrackLength(m_finishLocation);
		if (float.IsNaN(num))
		{
			m_segmentsPerLap = 1f;
		}
		else
		{
			m_segmentsPerLap = num / 60f;
		}
	}

	private void OnDestroy()
	{
		m_ais = null;
		m_humans = null;
	}

	private void BeginInfrequentUpdate()
	{
		m_globalRubberBanding = RubberBanding.None;
		InvokeRepeating("InfrequentUpdate", 2f, 2f);
	}

	private void InfrequentUpdate()
	{
		if (m_humans.Count > 0)
		{
			float num = float.MaxValue;
			float num2 = 0f;
			for (int i = 0; i < m_humans.Count; i++)
			{
				TrackLocation location = m_humans[i].Location;
				float num3 = m_humans[i].NumLapsDone;
				float num4 = TrackUtils.ForwardDistance(m_finishLocation, location);
				float num5 = TrackUtils.ForwardDistance(location, m_finishLocation);
				if (!float.IsNaN(num4) && !float.IsNaN(num5))
				{
					float num6 = num4 + num5;
					if (num6 > float.Epsilon)
					{
						num3 += num4 / num6;
					}
				}
				num3 *= m_segmentsPerLap;
				num = Mathf.Min(num, num3);
				num2 = Mathf.Max(num2, num3);
			}
			float num7 = float.MaxValue;
			float num8 = 0f;
			for (int j = 0; j < m_ais.Count; j++)
			{
				TrackLocation location2 = m_ais[j].Location;
				float num9 = m_ais[j].NumLapsDone;
				float num10 = TrackUtils.ForwardDistance(m_finishLocation, location2);
				float num11 = TrackUtils.ForwardDistance(location2, m_finishLocation);
				if (!float.IsNaN(num10) && !float.IsNaN(num11))
				{
					float num12 = num10 + num11;
					if (num12 > float.Epsilon)
					{
						num9 += num10 / num12;
					}
				}
				num9 *= m_segmentsPerLap;
				num7 = Mathf.Min(num7, num9);
				num8 = Mathf.Max(num8, num9);
			}
			int o_globalAIBias;
			int o_bestAIBias;
			int o_worstAIBias;
			CalculateAIBias(num2, num8, out o_globalAIBias, out o_bestAIBias, out o_worstAIBias);
			VehicleAIController vehicleAIController = null;
			VehicleAIController vehicleAIController2 = null;
			AIDifficulty aIDifficulty = AIDifficulty.Easy;
			AIDifficulty aIDifficulty2 = AIDifficulty.Optimal;
			for (int k = 0; k < m_ais.Count; k++)
			{
				VehicleAIController component = m_ais[k].GetComponent<VehicleAIController>();
				if (vehicleAIController == null || component.m_difficulty > aIDifficulty)
				{
					vehicleAIController = component;
					aIDifficulty = component.m_difficulty;
				}
				if (vehicleAIController2 == null || component.m_difficulty < aIDifficulty2)
				{
					vehicleAIController2 = component;
					aIDifficulty2 = component.m_difficulty;
				}
			}
			int num13 = Mathf.RoundToInt(num7 - num2);
			if (m_globalRubberBanding < RubberBanding.Slow)
			{
				num13++;
			}
			int num14 = Mathf.RoundToInt(num - num8);
			if (m_globalRubberBanding > RubberBanding.Fast)
			{
				num14++;
			}
			if (num13 > 0)
			{
				m_globalRubberBanding = BiasRubberBanding((num13 <= 1) ? RubberBanding.Slow : RubberBanding.VerySlow, o_globalAIBias);
			}
			else if (num14 > 0)
			{
				m_globalRubberBanding = BiasRubberBanding((num14 <= 1) ? RubberBanding.Fast : RubberBanding.VeryFast, o_globalAIBias);
			}
			else
			{
				m_globalRubberBanding = BiasRubberBanding(RubberBanding.None, o_globalAIBias);
				o_bestAIBias += Math.Max(0, Mathf.RoundToInt(num2 - num8));
				o_worstAIBias += Math.Min(0, Mathf.RoundToInt(num - num7));
			}
			RubberBanding rubberBanding = BiasRubberBanding(m_globalRubberBanding, o_bestAIBias);
			RubberBanding rubberBanding2 = BiasRubberBanding(m_globalRubberBanding, o_worstAIBias);
			for (int l = 0; l < m_ais.Count; l++)
			{
				VehicleAIController component2 = m_ais[l].GetComponent<VehicleAIController>();
				if (component2 == vehicleAIController)
				{
					component2.UpdateRubberBanding(rubberBanding);
				}
				else if (component2 == vehicleAIController2)
				{
					component2.UpdateRubberBanding(rubberBanding2);
				}
				else
				{
					component2.UpdateRubberBanding(m_globalRubberBanding);
				}
			}
		}
		else
		{
			for (int m = 0; m < m_ais.Count; m++)
			{
				VehicleAIController component3 = m_ais[m].GetComponent<VehicleAIController>();
				component3.UpdateRubberBanding(RubberBanding.None);
			}
		}
	}

	private RubberBanding BiasRubberBanding(RubberBanding _rubberBanding, int _bias)
	{
		int i = (int)(_rubberBanding + _bias);
		return (RubberBanding)MathUtils.Clamp(i, 0, 4);
	}

	private void CalculateAIBias(float _maxHumanSegment, float _maxAISegment, out int o_globalAIBias, out int o_bestAIBias, out int o_worstAIBias)
	{
		o_globalAIBias = 0;
		o_bestAIBias = 0;
		o_worstAIBias = 0;
		if (!(m_raceManager != null))
		{
			return;
		}
		RaceAISchedule aISchedule = m_raceManager.AISchedule;
		if (!(aISchedule != null))
		{
			return;
		}
		bool flag = _maxHumanSegment > _maxAISegment;
		bool flag2 = _maxAISegment > _maxHumanSegment;
		bool flag3 = Mathf.Abs(_maxHumanSegment - _maxAISegment) < 1f;
		float num = (float)m_raceManager.NumLaps * m_segmentsPerLap;
		float num2 = _maxHumanSegment / num;
		RaceAISchedule.KeyFrame keyFrame = null;
		for (int i = 0; i < aISchedule.m_keyFrames.Length; i++)
		{
			RaceAISchedule.KeyFrame keyFrame2 = aISchedule.m_keyFrames[i];
			if (keyFrame2.m_propThroughRace > num2)
			{
				break;
			}
			bool flag4 = false;
			switch (keyFrame2.m_condition)
			{
			case RaceAISchedule.Condition.None:
				flag4 = true;
				break;
			case RaceAISchedule.Condition.HumanInFront:
				flag4 = flag;
				break;
			case RaceAISchedule.Condition.AIInFront:
				flag4 = flag2;
				break;
			case RaceAISchedule.Condition.Tie:
				flag4 = flag3;
				break;
			case RaceAISchedule.Condition.TieOrHumanInFront:
				flag4 = flag3 || flag;
				break;
			case RaceAISchedule.Condition.TieOrAIInFront:
				flag4 = flag3 || flag2;
				break;
			}
			if (flag4)
			{
				keyFrame = keyFrame2;
			}
		}
		if (keyFrame != null)
		{
			o_globalAIBias = keyFrame.m_globalAIBias;
			o_bestAIBias = keyFrame.m_bestAIBias;
			o_worstAIBias = keyFrame.m_worstAIBias;
		}
	}

	public void RegisterAI(Vehicle _vehicle)
	{
		if (!m_ais.Contains(_vehicle))
		{
			m_ais.Add(_vehicle);
			if (m_ais.Count == 1)
			{
				BeginInfrequentUpdate();
			}
		}
	}

	public void UnregisterAI(Vehicle _vehicle)
	{
		if (m_ais.Contains(_vehicle))
		{
			m_ais.Remove(_vehicle);
			if (m_ais.Count == 0)
			{
				CancelInvoke();
			}
		}
	}

	public void RegisterHuman(Vehicle _vehicle)
	{
		if (!m_humans.Contains(_vehicle))
		{
			m_humans.Add(_vehicle);
		}
	}

	public void UnregisterHuman(Vehicle _vehicle)
	{
		if (m_humans.Contains(_vehicle))
		{
			m_humans.Remove(_vehicle);
		}
	}

	public static GlobalAI Get()
	{
		GameObject gameObject = GameObject.Find("/__GlobalAI");
		if (gameObject == null)
		{
			GameObject original = (GameObject)Resources.Load("Prefabs/AI/GlobalAI", typeof(GameObject));
			gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
			gameObject.name = "__GlobalAI";
		}
		return gameObject.GetComponent<GlobalAI>();
	}
}
