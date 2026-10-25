using System;
using System.Collections.Generic;
using UnityEngine;

public class PipeGUI_LeaderboardButton : MonoBehaviour
{
	private RaceManager m_raceManager;

	private SocialManager m_socialManager;

	private LeaderboardInfo m_leaderboardInfo;

	private bool m_canDisplay;

	private bool m_hasScore;

	private void Start()
	{
		base.gameObject.SetActiveRecursively(false);
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject != null)
		{
			m_raceManager = gameObject.GetComponent<RaceManager>();
			m_socialManager = gameObject.GetComponent<SocialManager>();
			m_raceManager.OnDisplayResults += OnDisplayResults;
			m_raceManager.OnRaceCompleteEvent += OnRaceComplete;
		}
		GameCenterManager.reportScoreFinished += OnReportScore;
		GPGManager.submitScoreSucceededEvent += OnSubmitScoreSucceeded;
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnDisplayResults -= OnDisplayResults;
			m_raceManager.OnRaceCompleteEvent -= OnRaceComplete;
		}
		GameCenterManager.reportScoreFinished -= OnReportScore;
		GPGManager.submitScoreSucceededEvent -= OnSubmitScoreSucceeded;
	}

	private void OnDisable()
	{
		m_leaderboardInfo = null;
		m_canDisplay = false;
		m_hasScore = false;
	}

	private void OnRaceComplete(object _sender, EventArgs _args)
	{
		m_leaderboardInfo = null;
		m_canDisplay = false;
		m_hasScore = false;
		if (!(m_raceManager == null))
		{
			m_leaderboardInfo = m_raceManager.CurrentRace.m_leaderboard;
		}
	}

	private void OnReportScore(string _leaderboardID)
	{
		if (m_leaderboardInfo != null && _leaderboardID == m_leaderboardInfo.ID)
		{
			m_hasScore = true;
			TryShow();
		}
	}

	private void OnSubmitScoreSucceeded(string _leaderboardId, Dictionary<string, object> _scoreReport)
	{
		OnReportScore(_leaderboardId);
	}

	public void OnDisplayResults(RaceManager.DisplayResults _displayResults)
	{
		if (m_raceManager == null || m_socialManager == null)
		{
			return;
		}
		if (_displayResults == RaceManager.DisplayResults.Medals)
		{
			if (m_socialManager.IsPlayerAuthenticated && (m_socialManager.IsGameCenterSupported || m_socialManager.IsGoogleGameServicesSupported) && m_leaderboardInfo != null)
			{
				base.gameObject.SetActiveRecursively(true);
			}
		}
		else
		{
			base.gameObject.SetActiveRecursively(false);
		}
	}

	public void OnLeaderboard()
	{
		if (m_socialManager != null)
		{
			m_socialManager.FlushUpdates();
		}
		m_canDisplay = true;
		TryShow();
	}

	private void TryShow()
	{
		if (m_canDisplay && m_hasScore && m_socialManager != null)
		{
			m_socialManager.ShowLeaderboard(m_leaderboardInfo);
		}
	}
}
