using System;
using UnityEngine;

[RequireComponent(typeof(RateAppPrompt))]
public class RateAppRacePrompt : MonoBehaviour
{
	private GameManager m_gameManager;

	private RaceManager m_raceManager;

	private RateAppPrompt m_rateAppPrompt;

	private float m_startRaceTime;

	private bool m_threeBolts;

	private bool m_improved;

	public float m_minTimeBetweenPrompts;

	public float m_maxTimeBetweenPrompts;

	public int m_minEarnedBolts;

	public float m_dialogDelayTime;

	private void Start()
	{
		m_gameManager = GetComponent<GameManager>();
		m_raceManager = GetComponent<RaceManager>();
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceLaunchEvent += OnRaceLaunch;
			m_raceManager.OnDisplayEndRaceOptions += OnRaceEnd;
			m_raceManager.OnRaceAbortedEvent += OnRaceAborted;
			m_raceManager.OnFinalisedRaceStats += OnFinalisedRaceStats;
			m_raceManager.OnSaveUpdatedEvent += OnSaveUpdated;
		}
		m_rateAppPrompt = GetComponent<RateAppPrompt>();
		m_rateAppPrompt.OnRatingSelected += OnRatingSelected;
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceLaunchEvent -= OnRaceLaunch;
			m_raceManager.OnDisplayEndRaceOptions -= OnRaceEnd;
			m_raceManager.OnRaceAbortedEvent -= OnRaceAborted;
			m_raceManager.OnFinalisedRaceStats -= OnFinalisedRaceStats;
			m_raceManager.OnSaveUpdatedEvent -= OnSaveUpdated;
		}
		m_rateAppPrompt.OnRatingSelected -= OnRatingSelected;
	}

	private void OnRatingSelected(Settings.RatedType _type)
	{
		if (_type == Settings.RatedType.NotYetRated)
		{
			Settings.TimeSinceReviewPrompt = 0f;
		}
	}

	private void UpdateTime()
	{
		float num = Time.timeSinceLevelLoad - m_startRaceTime;
		Settings.TimeSinceReviewPrompt += num;
		Log("Review: Time since review: " + Settings.TimeSinceReviewPrompt);
	}

	private void OnRaceEnd(bool _display)
	{
		if (!_display || Settings.Rated != Settings.RatedType.NotYetRated)
		{
			return;
		}
		UpdateTime();
		float timeSinceReviewPrompt = Settings.TimeSinceReviewPrompt;
		if (timeSinceReviewPrompt < m_minTimeBetweenPrompts)
		{
			Log("Review: Not enough time");
			return;
		}
		RaceCampaign currentCampaign = m_gameManager.CurrentCampaign;
		if (currentCampaign == null || currentCampaign.m_type != RaceCampaign.Type.SinglePlayer)
		{
			Log("Review: No campaign or single player");
			return;
		}
		if (!m_threeBolts)
		{
			Log("Review: Three bolts were not earned");
			return;
		}
		RaceCampaignSave saveData = m_gameManager.SaveData;
		if (!(saveData != null) || saveData.TotalPointsScored < m_minEarnedBolts)
		{
			Log("Review: Too few bolts earned: " + saveData.TotalPointsScored + "/" + m_minEarnedBolts);
		}
		else if (timeSinceReviewPrompt < m_maxTimeBetweenPrompts && !m_improved)
		{
			Log("Review: player has not improved");
		}
		else if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			Log("Review: player does not have internet connectivity");
		}
		else
		{
			m_rateAppPrompt.PresentDialog(m_dialogDelayTime);
		}
	}

	private void OnRaceLaunch(object _sender, EventArgs _args)
	{
		m_startRaceTime = Time.timeSinceLevelLoad;
		m_threeBolts = false;
		m_improved = false;
	}

	private void OnRaceAborted(object _sender, EventArgs _args)
	{
		UpdateTime();
	}

	private void OnFinalisedRaceStats(RaceStats _raceStats)
	{
		m_threeBolts = _raceStats.m_award == RaceCampaignSave.AwardType.Gold;
	}

	private void OnSaveUpdated(object _sender, RaceManager.SaveUpdatedArgs _args)
	{
		m_improved = _args.Improvements.ImprovedTotalTime || _args.Improvements.ImprovedPosition;
	}

	private void Log(string _str)
	{
	}
}
