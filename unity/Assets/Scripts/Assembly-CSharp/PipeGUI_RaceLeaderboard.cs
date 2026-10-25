using System;
using System.Collections;
using UnityEngine;

public class PipeGUI_RaceLeaderboard : MonoBehaviour
{
	private struct PointsResult
	{
		public string m_name;

		public int m_colourIndex;

		public int m_points;
	}

	private RaceManager m_raceManager;

	private RaceManager.DisplayResults m_currentDisplayType;

	private PointsResult[] m_results;

	public PipeGUI_NameTimeLapPoints[] m_entries;

	public UIBistateInteractivePanel m_icons;

	public UIBistateInteractivePanel m_eventNumber;

	private IEnumerator Start()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnDisplayResults += OnDisplayResults;
			m_raceManager.OnClearResults += ClearResults;
			m_raceManager.OnAddRaceResult += AddResult;
		}
		yield return null;
		yield return null;
		base.gameObject.SetActiveRecursively(false);
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnDisplayResults -= OnDisplayResults;
			m_raceManager.OnClearResults -= ClearResults;
			m_raceManager.OnAddRaceResult -= AddResult;
		}
	}

	private void OnDisplayResults(RaceManager.DisplayResults _display)
	{
		m_currentDisplayType = _display;
		if (_display == RaceManager.DisplayResults.Times)
		{
			// Unity 3 allowed active descendants below an inactive parent.
			// Modern Unity needs the leaderboard root active before its panels.
			gameObject.SetActive(true);
			m_icons.BringIn();
			PipeGUI_NameTimeLapPoints[] entries = m_entries;
			foreach (PipeGUI_NameTimeLapPoints pipeGUI_NameTimeLapPoints in entries)
			{
				if (pipeGUI_NameTimeLapPoints.IsValid)
				{
					pipeGUI_NameTimeLapPoints.ShowTimes();
				}
			}
		}
		switch (_display)
		{
		case RaceManager.DisplayResults.Points:
			m_icons.Dismiss();
			base.gameObject.active = true;
			StartCoroutine(ShowPoints());
			break;
		case RaceManager.DisplayResults.Hide:
		{
			StopAllCoroutines();
			m_icons.Dismiss();
			m_eventNumber.Dismiss();
			PipeGUI_NameTimeLapPoints[] entries2 = m_entries;
			foreach (PipeGUI_NameTimeLapPoints pipeGUI_NameTimeLapPoints2 in entries2)
			{
				pipeGUI_NameTimeLapPoints2.Hide();
			}
			break;
		}
		}
	}

	private IEnumerator ShowPoints()
	{
		PipeGUI_NameTimeLapPoints[] entries = m_entries;
		foreach (PipeGUI_NameTimeLapPoints entry in entries)
		{
			if (entry.IsValid)
			{
				entry.Hide();
			}
		}
		yield return YieldCache.WaitForSeconds(0.75f);
		Array.Sort(m_results, PointsSort);
		m_eventNumber.BringIn();
		for (int index = 0; index < m_results.Length; index++)
		{
			PipeGUI_NameTimeLapPoints entry2 = m_entries[index];
			PointsResult points = m_results[index];
			entry2.SetData(points.m_name, points.m_colourIndex, points.m_points);
			if (entry2.IsValid)
			{
				entry2.ShowPoints();
			}
		}
	}

	private void ClearResults(int _numEntries)
	{
		PipeGUI_NameTimeLapPoints[] entries = m_entries;
		foreach (PipeGUI_NameTimeLapPoints pipeGUI_NameTimeLapPoints in entries)
		{
			pipeGUI_NameTimeLapPoints.Invalidate();
		}
		m_results = new PointsResult[_numEntries];
	}

	private void AddResult(int _position, string _name, int _playerIndex, int _colourIndex, int _lapsBehind, float _raceTime, float _bestLap, int _points)
	{
		PipeGUI_NameTimeLapPoints pipeGUI_NameTimeLapPoints = m_entries[_position - 1];
		pipeGUI_NameTimeLapPoints.SetData(_name, _colourIndex, _lapsBehind, _raceTime, _bestLap, _points);
		if (m_currentDisplayType == RaceManager.DisplayResults.Times)
		{
			pipeGUI_NameTimeLapPoints.ShowTimes();
		}
		m_results[_playerIndex].m_name = _name;
		m_results[_playerIndex].m_points = _points;
		m_results[_playerIndex].m_colourIndex = _colourIndex;
	}

	private int PointsSort(PointsResult _lhs, PointsResult _rhs)
	{
		if (_lhs.m_points < _rhs.m_points)
		{
			return 1;
		}
		if (_lhs.m_points > _rhs.m_points)
		{
			return -1;
		}
		return 0;
	}
}
