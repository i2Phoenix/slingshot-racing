using UnityEngine;

public class PipeGUI_Awards : MonoBehaviour
{
	private RaceManager m_raceManager;

	private RaceCampaignSave.AwardType m_award;

	private bool m_improved;

	public PackedSprite[] m_awards;

	public GameObject m_improvedResult;

	private void Start()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject != null)
		{
			m_raceManager = gameObject.GetComponent<RaceManager>();
			m_raceManager.OnSaveUpdatedEvent += OnSaveDataUpdated;
			m_raceManager.OnFinalisedRaceStats += OnFinalisedRaceStats;
		}
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnSaveUpdatedEvent -= OnSaveDataUpdated;
		}
	}

	private void OnFinalisedRaceStats(RaceStats _raceStats)
	{
		m_award = _raceStats.m_award;
	}

	private void OnSaveDataUpdated(object _sender, RaceManager.SaveUpdatedArgs _args)
	{
		RaceCampaignSave.RaceStatsImproved improvements = _args.Improvements;
		m_improved = false;
		if (improvements.ImprovedPosition || improvements.ImprovedBestLap || improvements.ImprovedTotalTime)
		{
			m_improved = true;
		}
	}

	private void OnEnable()
	{
		Refresh();
	}

	private void Refresh()
	{
		if (m_awards != null)
		{
			int award = (int)m_award;
			for (int i = 0; i < m_awards.Length; i++)
			{
				if (m_awards[i] != null)
				{
					m_awards[i].gameObject.active = award > i;
				}
			}
		}
		if (m_improvedResult != null)
		{
			m_improvedResult.active = m_improved;
		}
	}
}
