public class RaceStats
{
	private RaceEvent m_race;

	public VehicleStats m_stats;

	public float m_raceTime;

	public float m_bestLapTime;

	public int m_finishPosition;

	public int m_numLapsCompleted;

	public int m_numLapsRemaining;

	public RaceCampaignSave.AwardType m_award;

	public bool[] m_objectivesPassedOverride;

	public RaceCampaignSave.VehicleRaceSummary SaveSummary
	{
		get
		{
			RaceCampaignSave.VehicleRaceSummary result = new RaceCampaignSave.VehicleRaceSummary
			{
				FinishPosition = m_finishPosition,
				BestLap = m_bestLapTime,
				TotalTime = ((m_numLapsRemaining != 0) ? 0f : m_raceTime)
			};
			if (m_race.m_bonusObjectives != null)
			{
				for (int i = 0; i < m_race.m_bonusObjectives.Length; i++)
				{
					BonusObjective bonusObjective = m_race.m_bonusObjectives[i];
					bool completed = m_objectivesPassedOverride[i] || bonusObjective.HasPassed(this);
					result.SetBonusObjectiveCompleted(i, completed);
				}
			}
			return result;
		}
	}

	public RaceEvent Race
	{
		get
		{
			return m_race;
		}
	}

	public RaceStats(RaceEvent _race)
	{
		m_race = _race;
		if (_race.HasBonusObjectives)
		{
			m_objectivesPassedOverride = new bool[_race.m_bonusObjectives.Length];
		}
	}

	public void Log()
	{
	}
}
