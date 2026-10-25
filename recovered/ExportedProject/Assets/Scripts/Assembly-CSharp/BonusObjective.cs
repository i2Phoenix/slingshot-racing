using System;

[Serializable]
public class BonusObjective
{
	public enum Type
	{
		None = 0,
		SingleLapTime = 1,
		TotalRaceTime = 2,
		UseNumBoostsLEq = 3,
		UseNumBoostsGEq = 4,
		MaxSpeed = 5,
		NumGrapplesFiredLEq = 6,
		NumVehicleCollisionsLEq = 7,
		NumLapsLess = 8,
		NumLapsLEq = 9,
		NumLapsGEq = 10,
		TutorialPerfectTraining = 11,
		TutorialPerfectPractice = 12
	}

	public Type m_type;

	public float m_data1;

	public bool m_mustWin;

	private int IntData1
	{
		get
		{
			return (int)m_data1;
		}
	}

	private float FloatData1
	{
		get
		{
			return m_data1;
		}
	}

	public bool HasPassed(RaceStats _stats)
	{
		return HasPassedImpl(_stats) && WinCondition(_stats);
	}

	private bool HasPassedImpl(RaceStats _stats)
	{
		switch (m_type)
		{
		case Type.None:
		case Type.TutorialPerfectTraining:
		case Type.TutorialPerfectPractice:
			return false;
		case Type.SingleLapTime:
			return _stats.m_bestLapTime <= FloatData1;
		case Type.TotalRaceTime:
			return _stats.m_numLapsRemaining == 0 && _stats.m_raceTime <= FloatData1;
		case Type.UseNumBoostsLEq:
			return _stats.m_stats.NumBoostsHit <= IntData1;
		case Type.UseNumBoostsGEq:
			return _stats.m_stats.NumBoostsHit >= IntData1;
		case Type.NumGrapplesFiredLEq:
			return _stats.m_stats.NumGrapplesFired <= IntData1;
		case Type.NumVehicleCollisionsLEq:
			return _stats.m_stats.NumVehicleCollisions <= IntData1;
		case Type.MaxSpeed:
			return _stats.m_stats.MaxSpeed >= FloatData1;
		case Type.NumLapsLess:
			return _stats.m_numLapsCompleted < IntData1;
		case Type.NumLapsLEq:
			return _stats.m_numLapsCompleted <= IntData1;
		case Type.NumLapsGEq:
			return _stats.m_numLapsCompleted >= IntData1;
		default:
			Debug.LogError("Unrecognised type: " + m_type);
			return false;
		}
	}

	public string GetDescription()
	{
		string text = Language.Get("ID_Objective" + m_type);
		switch (m_type)
		{
		case Type.SingleLapTime:
		case Type.TotalRaceTime:
		case Type.MaxSpeed:
			text = string.Format(text, FloatData1);
			break;
		case Type.UseNumBoostsLEq:
		case Type.UseNumBoostsGEq:
		case Type.NumGrapplesFiredLEq:
		case Type.NumVehicleCollisionsLEq:
		case Type.NumLapsLess:
		case Type.NumLapsLEq:
		case Type.NumLapsGEq:
			text = string.Format(text, IntData1);
			break;
		}
		if (m_mustWin)
		{
			text += Language.Get("ID_ObjectiveAndWin");
		}
		return text;
	}

	private bool WinCondition(RaceStats _stats)
	{
		return !m_mustWin || _stats.m_finishPosition == 1;
	}
}
