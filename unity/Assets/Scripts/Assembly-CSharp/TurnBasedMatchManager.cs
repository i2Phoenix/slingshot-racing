using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SocialManager))]
[AddComponentMenu("Gameplay/Turn Based Match Manager")]
public class TurnBasedMatchManager : MonoBehaviour
{
	private class ResultComparer : IComparer<TurnBasedMatchData.PlayerResultsData>
	{
		public int Compare(TurnBasedMatchData.PlayerResultsData _lhs, TurnBasedMatchData.PlayerResultsData _rhs)
		{
			return _lhs.RaceTime.CompareTo(_rhs.RaceTime);
		}
	}

	private struct MatchState
	{
		private readonly TurnBasedMatchData m_matchData;

		private readonly string m_currentPlayerID;

		private readonly bool m_localPlayerInMatch;

		public TurnBasedMatchData MatchData
		{
			get
			{
				return m_matchData;
			}
		}

		public string CurrentPlayerID
		{
			get
			{
				return m_currentPlayerID;
			}
		}

		public bool LocalPlayerInMatch
		{
			get
			{
				return m_localPlayerInMatch;
			}
		}

		public MatchState(TurnBasedMatchData _matchData, string _currentPlayerID, bool _localPlayerInMatch)
		{
			m_matchData = _matchData;
			m_currentPlayerID = _currentPlayerID;
			m_localPlayerInMatch = _localPlayerInMatch;
		}
	}

	private struct GhostRecordedData
	{
		private readonly byte[] m_ghostData;

		private readonly float m_totalTime;

		private readonly float m_bestLapTime;

		public byte[] GhostData
		{
			get
			{
				return m_ghostData;
			}
		}

		public float TotalTime
		{
			get
			{
				return m_totalTime;
			}
		}

		public float BestLapTime
		{
			get
			{
				return m_bestLapTime;
			}
		}

		public GhostRecordedData(byte[] _ghostData, float _totalTime, float _bestLapTime)
		{
			m_ghostData = _ghostData;
			m_totalTime = _totalTime;
			m_bestLapTime = _bestLapTime;
		}
	}

	private const int c_retryCount = 1;

	public static readonly string[] s_playerIDs = new string[2] { "00000001", "00000002" };

	public static readonly string[] s_playerNames = new string[2] { "Player1", "Player2" };

	private RaceManager m_raceManager;

	private MatchState m_currentMatchState;

	private bool m_isSupported;

	public bool IsSupported
	{
		get
		{
			return m_isSupported;
		}
	}

	public TurnBasedMatchData CurrentMatchData
	{
		get
		{
			return m_currentMatchState.MatchData;
		}
	}

	public string CurrentPlayerID
	{
		get
		{
			return m_currentMatchState.CurrentPlayerID;
		}
	}

	public bool CurrentLocalPlayerInMatch
	{
		get
		{
			return m_currentMatchState.LocalPlayerInMatch;
		}
	}

	public event Action OnGotoMatchEvent;

	public event Action OnGotoMatchFailedEvent;

	public event Action OnMatchDataUpdatedEvent;

	public event Action OnDisconnectedEvent;

	public event Action<string> OnFailureEvent;

	public event Action<string> OnPlayerTurnInOtherMatch;

	public event Action<string> OnOtherMatchEnded;

	public event Action<int> OnCountedAvailableTurns;

	private void Awake()
	{
		m_raceManager = GetComponent<RaceManager>();
	}

	private void Start()
	{
		m_isSupported = true;
		if (m_raceManager != null)
		{
			m_raceManager.OnGhostRecorded += OnGhostRecorded;
		}
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnGhostRecorded -= OnGhostRecorded;
		}
	}

	private void OnApplicationPause(bool _pause)
	{
		if (IsSupported && !_pause && CurrentMatchData != null)
		{
			ClearCurrentMatch();
			if (OnDisconnectedEvent != null)
			{
				OnDisconnectedEvent();
			}
		}
	}

	private void OnLevelWasLoaded()
	{
		if (Application.loadedLevelName == "TitleScreen")
		{
			ClearCurrentMatch();
		}
	}

	public void CountAvailableTurns()
	{
		if (IsSupported && OnCountedAvailableTurns == null)
		{
			Debug.LogError("Async: OnCountedAvailableTurns callback not set");
		}
	}

	public void GotoMatch(string _matchID)
	{
		if (IsSupported)
		{
			ClearCurrentMatch();
			Debug.LogError("Async: GotoMatch only supported with game center");
		}
	}

	public void FindMatch()
	{
		if (IsSupported)
		{
			ClearCurrentMatch();
			m_currentMatchState = new MatchState(new TurnBasedMatchData(this, null), s_playerIDs[0], true);
			for (int i = 0; i < s_playerIDs.Length; i++)
			{
				TurnBasedMatchData.PlayerData playerData = m_currentMatchState.MatchData.AddPlayerData(s_playerIDs[i]);
				playerData.Name = s_playerNames[i];
				playerData.TBPlayerIndex = i;
			}
			if (OnGotoMatchEvent != null)
			{
				OnGotoMatchEvent();
			}
		}
	}

	public bool IsCurrentPlayersTurn()
	{
		return true;
	}

	public bool IsWaitingForServer()
	{
		return false;
	}

	public bool QuitLocalPlayer()
	{
		return QuitLocalPlayerInternal();
	}

	private bool QuitLocalPlayerInternal()
	{
		if (!IsSupported)
		{
			return false;
		}
		if (CurrentMatchData == null)
		{
			return false;
		}
		return true;
	}

	public bool RemoveCurrentMatch()
	{
		if (!IsSupported)
		{
			return false;
		}
		return true;
	}

	public void ClearCurrentMatch()
	{
		m_currentMatchState = default(MatchState);
	}

	public bool SetRaceSequence(ITBRaceSequence _raceSequence)
	{
		if (!IsSupported)
		{
			return false;
		}
		if (CurrentMatchData == null)
		{
			return false;
		}
		if (!IsCurrentPlayersTurn())
		{
			return false;
		}
		if (CurrentMatchData.NumRaces != 0)
		{
			return false;
		}
		for (int i = 0; i < _raceSequence.NumEvents; i++)
		{
			ITBRaceEvent iTBRaceEvent = _raceSequence.GetEvent(i);
			if (CurrentMatchData.HasDataForRaceEvent(iTBRaceEvent.RaceEventName))
			{
				Debug.LogError("Race event already exists");
				continue;
			}
			TurnBasedMatchData.RaceEventData raceEventData = CurrentMatchData.AddRaceEventData(iTBRaceEvent.RaceEventName);
			raceEventData.TBRaceState = ((i == 0) ? TurnBasedMatchData.TBRaceState.Waiting : TurnBasedMatchData.TBRaceState.InActive);
			raceEventData.NumLaps = iTBRaceEvent.NumLaps;
			raceEventData.Reversed = iTBRaceEvent.Reversed;
			raceEventData.Variant = iTBRaceEvent.Variant;
		}
		return true;
	}

	private void OnGhostRecorded(Vehicle _vehicle, RaceCampaignSave.RaceEventSaveData _saveData, byte[] _ghostData)
	{
		GhostRecordedData ghostRecordedData = new GhostRecordedData(_ghostData, _saveData.BestTotalTime, _saveData.BestLapTime);
		GhostRecordedInternal(ghostRecordedData);
	}

	private bool GhostRecordedInternal(GhostRecordedData _ghostRecordedData)
	{
		if (CurrentMatchData == null)
		{
			Debug.LogError("Async: Match data missing");
			return false;
		}
		if (!IsCurrentPlayersTurn())
		{
			Debug.LogError("Async: Ghost data received out of turn");
			return false;
		}
		TurnBasedMatchData turnBasedMatchData = CurrentMatchData.Clone();
		TurnBasedMatchData.RaceEventData raceEventData = turnBasedMatchData.CurrentRace;
		if (raceEventData == null)
		{
			Debug.LogError("Async: No race to add ghost data");
			return false;
		}
		string currentPlayerID = CurrentPlayerID;
		if (currentPlayerID == null || currentPlayerID.Length == 0)
		{
			Debug.LogError("Async: No local player");
			return false;
		}
		if (raceEventData.HasResultsForPlayer(currentPlayerID))
		{
			Debug.LogError("Async: Race already has data for player");
			return false;
		}
		if (!turnBasedMatchData.HasDataForPlayer(currentPlayerID))
		{
			Debug.LogError("Async: No data for local player");
			return false;
		}
		TurnBasedMatchData.PlayerResultsData playerResultsData = raceEventData.AddPlayerResultsData(currentPlayerID);
		playerResultsData.GhostData = _ghostRecordedData.GhostData;
		playerResultsData.Position = 0;
		playerResultsData.RaceTime = _ghostRecordedData.TotalTime;
		playerResultsData.BestLap = _ghostRecordedData.BestLapTime;
		bool flag = true;
		bool canContinueMatch = true;
		for (int i = 0; i < s_playerIDs.Length; i++)
		{
			if (!raceEventData.HasResultsForPlayer(s_playerIDs[i]))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			raceEventData = DoRaceComplete(turnBasedMatchData, raceEventData, canContinueMatch);
		}
		turnBasedMatchData.IncrementSequence();
		string currentPlayerID2 = s_playerIDs[0];
		if (raceEventData != null)
		{
			for (int j = 0; j < s_playerIDs.Length; j++)
			{
				if (!raceEventData.HasResultsForPlayer(s_playerIDs[j]))
				{
					currentPlayerID2 = s_playerIDs[j];
					break;
				}
			}
		}
		m_currentMatchState = new MatchState(turnBasedMatchData, currentPlayerID2, raceEventData != null);
		if (OnMatchDataUpdatedEvent != null)
		{
			OnMatchDataUpdatedEvent();
		}
		return true;
	}

	private void LogMatchDataStats(TurnBasedMatchData _matchData, int _dataSize)
	{
		int totalGhostDataSize = _matchData.TotalGhostDataSize;
		Debug.Log(string.Format("Async: Match data size: {0} bytes ({1} ghost, {2} other)", _dataSize, totalGhostDataSize, _dataSize - totalGhostDataSize));
	}

	private TurnBasedMatchData.RaceEventData DoRaceComplete(TurnBasedMatchData _matchData, TurnBasedMatchData.RaceEventData _raceData, bool _canContinueMatch)
	{
		TurnBasedMatchData.PlayerResultsData[] array = new TurnBasedMatchData.PlayerResultsData[_raceData.NumResultDatas];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = _raceData.GetResultData(i);
		}
		Array.Sort(array, new ResultComparer());
		for (int j = 0; j < array.Length; j++)
		{
			array[j].Position = j + 1;
		}
		_raceData.TBRaceState = TurnBasedMatchData.TBRaceState.Completed;
		_raceData = _matchData.CurrentRace;
		if (_raceData != null && _canContinueMatch)
		{
			_raceData.TBRaceState = TurnBasedMatchData.TBRaceState.Waiting;
		}
		return _raceData;
	}
}
