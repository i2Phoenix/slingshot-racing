using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[AddComponentMenu("Gameplay/Race Manager")]
public class RaceManager : MonoBehaviour
{
	public enum DisplayResults
	{
		Hide = 0,
		Times = 1,
		Points = 2,
		Medals = 3,
		Null = 4
	}

	private enum RaceState
	{
		Running = 0,
		RunningFinishing = 1,
		HumansFinished = 2,
		AllFinished = 3
	}

	public class SaveUpdatedArgs : EventArgs
	{
		public RaceCampaignSave.RaceEventSaveData SaveData { get; private set; }

		public RaceCampaignSave.RaceStatsImproved Improvements { get; private set; }

		public SaveUpdatedArgs(RaceCampaignSave.RaceEventSaveData _saveData, RaceCampaignSave.RaceStatsImproved _improvements)
		{
			SaveData = _saveData;
			Improvements = _improvements;
		}
	}

	private class VehicleInfo
	{
		public Vector3 m_launchPosition;

		public Quaternion m_launchRotation;

		public float m_launchHeightOffset;

		private int m_lapsRemaining;

		private float m_timeForCompletedLaps;

		private float m_timeForIncompletedLaps;

		public byte[] GhostData
		{
			get
			{
				GhostRecorder component = Vehicle.GetComponent<GhostRecorder>();
				if (component != null)
				{
					component.EndRecord();
					return component.ToByteArray();
				}
				return null;
			}
		}

		public bool RecordingGhost
		{
			get
			{
				return Vehicle.GetComponent<GhostRecorder>() != null;
			}
		}

		public GameObject VehicleGO { get; private set; }

		public Vehicle Vehicle { get; private set; }

		public float BestLapTime { get; private set; }

		public float LastLapTime { get; private set; }

		public bool LapJustCompleted { get; set; }

		public float TimeForCompletedLaps
		{
			get
			{
				return m_timeForCompletedLaps;
			}
			set
			{
				m_timeForCompletedLaps = value;
			}
		}

		public float TimeSinceRaceStart
		{
			get
			{
				return m_timeForCompletedLaps + ((!IsFinished) ? Vehicle.CurrentLapTime : m_timeForIncompletedLaps);
			}
		}

		public bool IsHuman { get; set; }

		public bool IsGhost { get; set; }

		public int RacePosition { get; set; }

		public bool IsFinished { get; private set; }

		public bool IsFinishedAllLaps
		{
			get
			{
				return IsFinished && m_lapsRemaining == 0;
			}
		}

		public int LapsRemaining
		{
			get
			{
				return m_lapsRemaining;
			}
		}

		public AIDifficulty AIDifficulty { get; private set; }

		public int GridIndex { get; set; }

		public string PlayerName { get; set; }

		public bool DoLapPopups { get; set; }

		public VehicleInfo(GameObject _vehicle)
		{
			VehicleGO = _vehicle;
			Vehicle = _vehicle.GetComponent<Vehicle>();
			IsHuman = VehicleGO.GetComponent<VehicleController>();
			VehicleAIController component = VehicleGO.GetComponent<VehicleAIController>();
			if (component != null)
			{
				AIDifficulty = component.m_difficulty;
			}
		}

		public static int PartialCompare(VehicleInfo left, VehicleInfo right)
		{
			if (left.IsFinished || right.IsFinished)
			{
				return 0;
			}
			if (left.m_lapsRemaining != right.m_lapsRemaining)
			{
				return left.m_lapsRemaining - right.m_lapsRemaining;
			}
			if (left.m_lapsRemaining == 0)
			{
				return (int)Mathf.Sign(left.m_timeForCompletedLaps - right.m_timeForCompletedLaps);
			}
			Vehicle vehicle = left.Vehicle;
			Vehicle vehicle2 = right.Vehicle;
			int lastWaypoint = vehicle.LastWaypoint;
			int lastWaypoint2 = vehicle2.LastWaypoint;
			if (lastWaypoint != lastWaypoint2)
			{
				return lastWaypoint2 - lastWaypoint;
			}
			TrackSpline currentTrackSpline = vehicle.CurrentTrackSpline;
			if (currentTrackSpline != null)
			{
				TrackSpline currentTrackSpline2 = vehicle2.CurrentTrackSpline;
				if (currentTrackSpline == currentTrackSpline2)
				{
					float num = currentTrackSpline.ReParamTtoS(vehicle.CurrentT);
					float num2 = currentTrackSpline.ReParamTtoS(vehicle2.CurrentT);
					if (num > num2 + 2f)
					{
						return -1;
					}
					if (num2 > num + 2f)
					{
						return 1;
					}
				}
			}
			return 0;
		}

		public void OnLapComplete(float _lapTime)
		{
			LastLapTime = _lapTime;
			if (BestLapTime == 0f || _lapTime < BestLapTime)
			{
				BestLapTime = _lapTime;
			}
			m_lapsRemaining--;
			m_timeForCompletedLaps += _lapTime;
			LapJustCompleted = true;
		}

		public void Launch(int numLaps, RaceEvent _race)
		{
			VehicleGO.BroadcastMessage("RaceEvent_Launch", SendMessageOptions.DontRequireReceiver);
			IsFinished = false;
			m_timeForCompletedLaps = 0f;
			m_timeForIncompletedLaps = 0f;
			m_lapsRemaining = numLaps;
			RacePosition = -1;
			Vehicle.EnableEasyRewards = _race.m_enableEasyRewards;
			Vehicle.Visuals.EmitLaunchParticles(0.4f);
			GhostPlayer component = Vehicle.GetComponent<GhostPlayer>();
			if (component != null)
			{
				component.Play(_race.m_playerData[Vehicle.ColourIndex].m_ghostData);
			}
			GhostRecorder component2 = Vehicle.GetComponent<GhostRecorder>();
			if (component2 != null)
			{
				component2.BeginRecord();
			}
		}

		public void FinishNow(bool _addRemainingTime)
		{
			if (!IsFinished)
			{
				VehicleController component = VehicleGO.GetComponent<VehicleController>();
				VehicleAIController vehicleAIController = VehicleGO.GetComponent<VehicleAIController>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				GhostVehicle component2 = VehicleGO.GetComponent<GhostVehicle>();
				if (vehicleAIController == null && component2 == null)
				{
					vehicleAIController = VehicleGO.AddComponent<VehicleAIController>();
				}
				if (vehicleAIController != null)
				{
					vehicleAIController.DynamicDifficulty = false;
					vehicleAIController.m_difficulty = AIDifficulty.Hard;
				}
				if (component2 != null)
				{
					component2.StopPlayback(2.5f);
				}
				IsFinished = true;
				if (_addRemainingTime)
				{
					m_timeForIncompletedLaps = Vehicle.CurrentLapTime;
				}
				GhostRecorder component3 = Vehicle.GetComponent<GhostRecorder>();
				if (component3 != null)
				{
					component3.EndRecordIn(3f);
				}
			}
		}

		public RaceCampaignSave.RaceStatsImproved UpdateSave(RaceCampaignSave.RaceEventSaveData _saveData, int _numLaps, UpdateRaceStats _updateCallback, FinalisedRaceStats _finalisedCallback)
		{
			RaceCampaignSave.RaceStatsImproved result = null;
			if (_saveData != null && Vehicle.PlayerIndex == 0 && IsHuman)
			{
				RaceStats raceStats = new RaceStats(_saveData.Race);
				raceStats.m_stats = GameObjectUtils.GetComponentInChildren<VehicleStats>(VehicleGO);
				raceStats.m_finishPosition = RacePosition + 1;
				raceStats.m_raceTime = m_timeForCompletedLaps + m_timeForIncompletedLaps;
				raceStats.m_bestLapTime = BestLapTime;
				raceStats.m_numLapsCompleted = ((_numLaps <= 0) ? (-m_lapsRemaining) : (_numLaps - m_lapsRemaining));
				raceStats.m_numLapsRemaining = ((_numLaps > 0) ? m_lapsRemaining : 0);
				if (_updateCallback != null)
				{
					_updateCallback(raceStats);
				}
				raceStats.m_award = _saveData.GetAwardType(raceStats.m_finishPosition, raceStats.m_raceTime, raceStats.m_bestLapTime);
				if (_finalisedCallback != null)
				{
					_finalisedCallback(raceStats);
				}
				RaceCampaignSave.VehicleRaceSummary saveSummary = raceStats.SaveSummary;
				result = _saveData.UpdateSave(saveSummary);
			}
			return result;
		}
	}

	public delegate void DisplayResultsEvent(DisplayResults _display);

	public delegate void DisplayEndRaceOptionsEvent(bool _display);

	public delegate bool PostponeOption();

	public delegate void UpdateRaceStats(RaceStats _raceStats);

	public delegate void FinalisedRaceStats(RaceStats _raceStats);

	public delegate void ClearResults(int numEntries);

	public delegate void AddRaceResult(int _position, string _name, int _playerIndex, int _colourIndex, int _lapsBehind, float _raceTime, float _bestLap, int _points);

	private const float c_ghostAdditionalRecordTime = 3f;

	private const float c_ghostStopPlaybackTime = 2.5f;

	public const float c_leaderboardScoreScale = 1000f;

	private DynamicCamera m_trackingCamera;

	public TouchInputManager m_touchInputManager;

	private StartGrid m_startGrid;

	private VehicleInfo[] m_vehicles;

	private VehicleInfo[] m_vehiclesInRaceOrder;

	private RaceEvent m_currentRace;

	private RaceCampaignSave.RaceEventSaveData m_saveData;

	private GameObject[] m_instantiatedObjects;

	private GameObject m_uiController;

	private RaceState m_raceState;

	private bool m_handleLapsCompleted;

	private bool m_allowAbort;

	private bool m_allowShowReplay;

	private DisplayResults m_displayResults;

	private Coroutine m_helpCoroutine;

	private string[] m_positionStrings;

	private string m_popup_Lap;

	private string m_popup_FinalLap;

	private string m_popup_Finished;

	private int m_numHumans;

	private int m_numAIs;

	private int m_numLaps;

	private int m_raceNumber;

	private int m_numRaces;

	private int m_profileNumFrames;

	private float m_profileTime;

	private GameManager.TournamentScores[] m_tournamentScores;

	private PositionPopup[] m_lapPopups;

	private PositionPopup[] m_positionPopups;

	private AudioPlayer m_audioPlayer;

	public GameObject DynamicCamera;

	public AudioBank m_endOfRace;

	public bool AutoFinish { get; set; }

	public bool SuppressEndRaceInput { get; set; }

	private Tickerboard Countdown
	{
		get
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
			if (gameObject != null)
			{
				return gameObject.GetComponentInChildren<Tickerboard>();
			}
			return null;
		}
	}

	public bool EnableTimer
	{
		set
		{
			RaceTimer timer = Timer;
			if (timer != null)
			{
				timer.enabled = value;
			}
		}
	}

	public RaceTimer Timer
	{
		get
		{
			GameObject gameObject = GameObject.Find("PipeGUI_Timer");
			if (gameObject != null)
			{
				return gameObject.GetComponentInChildren<RaceTimer>();
			}
			return null;
		}
	}

	public int NumLaps
	{
		get
		{
			return m_numLaps;
		}
	}

	public int NumRacers
	{
		get
		{
			return m_numHumans + m_numAIs;
		}
	}

	public int NumHumans
	{
		get
		{
			return m_numHumans;
		}
	}

	public int NumAIs
	{
		get
		{
			return m_numAIs;
		}
	}

	public RaceEvent.ObjectiveType Objective
	{
		get
		{
			return m_currentRace.m_objective;
		}
	}

	public RaceEvent CurrentRace
	{
		get
		{
			return m_currentRace;
		}
	}

	public VehicleFactory.Type VehicleType
	{
		get
		{
			return m_currentRace.VehicleType;
		}
	}

	public string VehicleTypeStr
	{
		get
		{
			return (VehicleType == VehicleFactory.Type.NotSet) ? Settings.VehicleType : VehicleType.ToString();
		}
	}

	public RaceAISchedule AISchedule
	{
		get
		{
			return m_currentRace.m_aiSchedule;
		}
	}

	public GameManager.TournamentScores[] TournamentScores
	{
		set
		{
			m_tournamentScores = value;
		}
	}

	public bool IsTournament
	{
		get
		{
			return m_tournamentScores != null;
		}
	}

	public bool AllGhosts { get; private set; }

	public GameCenterChallenge CurrentChallenge { get; set; }

	public bool CompletedChallenge { get; private set; }

	public int RaceNumber
	{
		get
		{
			return m_raceNumber;
		}
	}

	public int NumRaces
	{
		get
		{
			return m_numRaces;
		}
	}

	private bool DoLapPopUps
	{
		get
		{
			switch (Objective)
			{
			case RaceEvent.ObjectiveType.Pickups:
			case RaceEvent.ObjectiveType.Overtake:
			case RaceEvent.ObjectiveType.Survival:
			case RaceEvent.ObjectiveType.LastStanding:
				return false;
			default:
				return true;
			}
		}
	}

	private bool DoPositionPopups
	{
		get
		{
			RaceEvent.ObjectiveType objective = Objective;
			if (objective == RaceEvent.ObjectiveType.RacePosition || objective == RaceEvent.ObjectiveType.HeadsUp)
			{
				return true;
			}
			return false;
		}
	}

	public event EventHandler OnTrackingCameraReadyEvent;

	public event EventHandler OnRaceStartCountdownEvent;

	public event EventHandler OnRaceLaunchEvent;

	public event EventHandler OnRaceAbortedEvent;

	public event EventHandler OnRaceReplayEvent;

	public event EventHandler OnRaceCompleteEvent;

	public event DisplayResultsEvent OnDisplayResults;

	public event DisplayEndRaceOptionsEvent OnDisplayEndRaceOptions;

	public event Action<bool> OnDisplayReplayOptions;

	public event Action<bool> OnAllowPause;

	public event Action OnRacePrepared;

	public event Action OnReplayDataReady;

	public event PostponeOption PostponeLaunch;

	public event UpdateRaceStats OnUpdateRaceStats;

	public event FinalisedRaceStats OnFinalisedRaceStats;

	public event EventHandler<SaveUpdatedArgs> OnSaveUpdatedEvent;

	public event ClearResults OnClearResults;

	public event AddRaceResult OnAddRaceResult;

	public event Action<Vehicle, RaceCampaignSave.RaceEventSaveData, byte[]> OnGhostRecorded;

	public int GetVehicleIndex(GameObject _vehicle)
	{
		for (int i = 0; i < m_vehicles.Length; i++)
		{
			if (m_vehicles[i].VehicleGO == _vehicle)
			{
				return i;
			}
		}
		return -1;
	}

	public Vehicle GetVehicle(int _playerIndex)
	{
		return m_vehicles[_playerIndex].Vehicle;
	}

	public float GetVehicleTimeSinceStart(int _playerIndex)
	{
		return m_vehicles[_playerIndex].TimeSinceRaceStart;
	}

	public Vector3 GetVehicleLaunchPosition(int _playerIndex)
	{
		return m_vehicles[_playerIndex].m_launchPosition;
	}

	public int GetVehicleRacePosition(int _playerIndex)
	{
		return m_vehicles[_playerIndex].RacePosition;
	}

	public byte[] GetGhostData(int _playerIndex)
	{
		return m_vehicles[_playerIndex].GhostData;
	}

	public int GetGridIndex(int _playerIndex)
	{
		return m_vehicles[_playerIndex].GridIndex;
	}

	public string GetPlayerName(int _playerIndex)
	{
		return m_vehicles[_playerIndex].PlayerName;
	}

	public void SetRaceCount(int _raceNumber, int _numRaces)
	{
		m_raceNumber = _raceNumber;
		m_numRaces = _numRaces;
	}

	private void Start()
	{
		CacheLocalisedText();
		m_audioPlayer = GetComponent<AudioPlayer>();
		if (m_audioPlayer == null)
		{
			m_audioPlayer = base.gameObject.AddComponent<AudioPlayer>();
		}
	}

	public void PrepareRace(RaceEvent _raceEvent, RaceCampaignSave.RaceEventSaveData _saveData, bool _quickStart)
	{
		CacheLocalisedText();
		if (OnAllowPause != null)
		{
			OnAllowPause(false);
		}
		m_currentRace = _raceEvent;
		m_saveData = _saveData;
		m_numHumans = BitUtils.BitCount(_raceEvent.m_humanMask);
		m_numAIs = BitUtils.BitCount(_raceEvent.m_aiMask);
		m_numLaps = _raceEvent.m_numLaps;
		m_displayResults = DisplayResults.Hide;
		SuppressEndRaceInput = false;
		m_handleLapsCompleted = false;
		CompletedChallenge = false;
		GameObject gameObject = FinishLine.Get().gameObject;
		m_uiController = GameObject.FindWithTag("UIController");
		m_startGrid = gameObject.GetComponent<StartGrid>();
		m_raceState = RaceState.Running;
		CreateRacers();
		CreateCamera();
		TrackRacers();
		CreateInRaceGUI();
		CreatePrefabObjects();
		CreateLapTimings();
		StartCoroutine("HoldOnStartLine");
		if (m_tournamentScores == null && !_quickStart && m_numHumans == 1 && !PersistentState.Get().m_aiControlHumans && !_raceEvent.NoHelp)
		{
			m_helpCoroutine = StartCoroutine("DisplayHelp");
		}
		else
		{
			m_helpCoroutine = null;
		}
		if (OnRacePrepared != null)
		{
			OnRacePrepared();
		}
	}

	public void VehicleFinished(int vehicleIndex, int position)
	{
		VehicleInfo vehicleInfo = m_vehicles[vehicleIndex];
		vehicleInfo.FinishNow(true);
		vehicleInfo.RacePosition = position;
		bool flag = true;
		bool flag2 = true;
		for (int i = 0; i != m_vehicles.Length; i++)
		{
			flag &= !m_vehicles[i].IsHuman | m_vehicles[i].IsFinished;
			flag2 &= m_vehicles[i].IsFinished;
		}
		if (m_raceState == RaceState.Running && ((CurrentRace.m_humanMask != 0 && flag) || (CurrentRace.m_humanMask == 0 && flag2)))
		{
			StartCoroutine(EndRaceSequence());
		}
		if (flag2)
		{
			m_raceState = RaceState.AllFinished;
		}
		else if (CurrentRace.m_humanMask != 0 && flag)
		{
			m_raceState = RaceState.HumansFinished;
		}
	}

	public void RaceEvent_StartRace(bool _quickStart)
	{
		m_allowAbort = true;
		StartCoroutine("LaunchCountdown", _quickStart);
	}

	private void RaceEvent_RaceAborted()
	{
		m_allowAbort = false;
		if (OnRaceAbortedEvent != null)
		{
			OnRaceAbortedEvent(this, EventArgs.Empty);
		}
		if (Countdown != null)
		{
			Countdown.AbortCountdown();
		}
		InRaceHelp component = m_uiController.GetComponent<InRaceHelp>();
		if (component != null)
		{
			component.HideImmediate();
		}
		StopAllCoroutines();
		DestroyScreens();
		DestroyPrefabObjects();
		DestroyVehicles(false);
		DestroyPopups();
	}

	private void UIEvent_LeaveRace()
	{
		if (m_allowAbort)
		{
			BroadcastMessage("RaceEvent_RaceAborted");
			BroadcastMessage("RaceEvent_LeaveRace");
		}
	}

	private void UIEvent_RestartRace()
	{
		if (m_allowAbort)
		{
			BroadcastMessage("RaceEvent_RaceAborted");
			BroadcastMessage("RaceEvent_Replay");
		}
	}

	private void UIEvent_GotoMainMenu()
	{
		if (m_allowAbort)
		{
			BroadcastMessage("RaceEvent_RaceAborted");
			BroadcastMessage("RaceEvent_ReturnToMainMenu");
		}
	}

	public void OnChallengeSelected(GameCenterChallenge _challenge)
	{
		if (m_allowAbort && GameManager.IsChallengeSupported(_challenge))
		{
			BroadcastMessage("RaceEvent_RaceAborted");
			BroadcastMessage("RaceEvent_GotoChallenge", _challenge);
		}
	}

	private void HideResults()
	{
		m_displayResults = DisplayResults.Hide;
		if (OnDisplayResults != null)
		{
			OnDisplayResults(m_displayResults);
		}
		if (OnDisplayEndRaceOptions != null)
		{
			OnDisplayEndRaceOptions(false);
		}
		if (OnDisplayReplayOptions != null)
		{
			m_allowShowReplay = false;
			OnDisplayReplayOptions(false);
		}
	}

	private void UIEvent_Replay()
	{
		if (!SuppressEndRaceInput)
		{
			HideResults();
			Invoke("ReplayRace", 1f);
		}
	}

	private void ReplayRace()
	{
		BroadcastMessage("RaceEvent_Replay");
	}

	private void UIEvent_NextScreen()
	{
		if (SuppressEndRaceInput)
		{
			return;
		}
		switch (m_displayResults)
		{
		case DisplayResults.Hide:
			m_displayResults = ((m_tournamentScores != null || CurrentRace.NoGrade) ? DisplayResults.Times : DisplayResults.Medals);
			break;
		case DisplayResults.Medals:
			m_displayResults = ((OnAddRaceResult != null) ? DisplayResults.Times : DisplayResults.Hide);
			break;
		case DisplayResults.Points:
			m_displayResults = DisplayResults.Hide;
			break;
		case DisplayResults.Times:
			m_displayResults = ((m_tournamentScores != null) ? DisplayResults.Points : DisplayResults.Hide);
			break;
		}
		if (OnDisplayResults != null)
		{
			OnDisplayResults(m_displayResults);
		}
		if (m_displayResults == DisplayResults.Hide)
		{
			if (OnDisplayEndRaceOptions != null)
			{
				OnDisplayEndRaceOptions(false);
			}
			if (OnDisplayReplayOptions != null)
			{
				m_allowShowReplay = false;
				OnDisplayReplayOptions(false);
			}
			Invoke("NextScreen", 1f);
		}
	}

	private void UIEvent_ShowReplay()
	{
		if (!SuppressEndRaceInput)
		{
			HideResults();
			Invoke("ShowReplay", 1f);
		}
	}

	private void ShowReplay()
	{
		BroadcastMessage("RaceEvent_ShowReplay");
	}

	private void NextScreen()
	{
		StopAllCoroutines();
		DestroyScreens();
		DestroyPrefabObjects();
		DestroyVehicles(false);
		DestroyPopups();
		BroadcastMessage("RaceEvent_NextScreen");
	}

	private void Update()
	{
		if (Time.timeScale > 0f)
		{
			m_profileNumFrames++;
			m_profileTime += Time.deltaTime / Time.timeScale;
		}
	}

	private IEnumerator LaunchCountdown(bool _quickStart)
	{
		bool helpDisplayed = false;
		if (m_helpCoroutine != null)
		{
			yield return m_helpCoroutine;
			m_helpCoroutine = null;
			helpDisplayed = true;
		}
		if (OnAllowPause != null)
		{
			OnAllowPause(true);
		}
		m_profileNumFrames = 0;
		m_profileTime = 0f;
		if (_quickStart && (bool)m_trackingCamera)
		{
			m_trackingCamera.m_startLerpRate = m_trackingCamera.m_quickStartLerpRate;
		}
		if (OnRaceStartCountdownEvent != null)
		{
			OnRaceStartCountdownEvent(this, EventArgs.Empty);
		}
		if (PostponeLaunch != null)
		{
			while (PostponeLaunch())
			{
				yield return null;
			}
		}
		InRaceIdentifiers identifiers = m_uiController.GetComponent<InRaceIdentifiers>();
		if (identifiers != null)
		{
			identifiers.Play(this, _quickStart);
		}
		yield return (!_quickStart && !helpDisplayed) ? YieldCache.WaitForTwoSeconds : null;
		Tickerboard countdown = Countdown;
		if (countdown != null)
		{
			yield return countdown.StartCoroutine(countdown.Countdown(_quickStart));
		}
		StopCoroutine("HoldOnStartLine");
		for (int i = 0; i < m_vehicles.Length; i++)
		{
			VehicleAIController vehicleAI = m_vehicles[i].VehicleGO.GetComponent<VehicleAIController>();
			if (vehicleAI != null)
			{
				vehicleAI.enabled = true;
			}
			if (m_vehicles[i].Vehicle.GetComponent<Rigidbody>() != null)
			{
				m_vehicles[i].Vehicle.GetComponent<Rigidbody>().isKinematic = false;
				m_vehicles[i].Vehicle.GetComponent<Rigidbody>().interpolation = ((!TimeManager.EnableSmoothing) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
			}
		}
		if (OnRaceLaunchEvent != null)
		{
			OnRaceLaunchEvent(this, EventArgs.Empty);
		}
		VehicleInfo[] vehicles = m_vehicles;
		foreach (VehicleInfo vehicleInfo in vehicles)
		{
			vehicleInfo.Launch(NumLaps, CurrentRace);
		}
		if (m_vehiclesInRaceOrder != null)
		{
			StartCoroutine("UpdatePositions");
		}
		if (m_trackingCamera != null)
		{
			m_trackingCamera.OnRaceLaunch();
		}
		GameObject finishLine = FinishLine.Get().gameObject;
		StartFanfare startFanfare = finishLine.GetComponent<StartFanfare>();
		if (startFanfare != null)
		{
			startFanfare.LaunchFanfare();
		}
	}

	private IEnumerator UpdatePositions()
	{
		while (m_raceState == RaceState.Running || m_raceState == RaceState.RunningFinishing)
		{
			SortVehicles();
			yield return YieldCache.Wait15Hz;
		}
	}

	private IEnumerator DisplayHelp()
	{
		InRaceHelp help = m_uiController.GetComponent<InRaceHelp>();
		if (help != null)
		{
			yield return StartCoroutine(help.DisplayHelp(CurrentRace));
		}
	}

	private IEnumerator HoldOnStartLine()
	{
		yield return YieldCache.WaitForFixedUpdate;
		float countdownTime = 0f;
		while (true)
		{
			countdownTime += Time.deltaTime;
			if (countdownTime >= 0.125f)
			{
				break;
			}
			yield return YieldCache.WaitForFixedUpdate;
		}
		for (int i = 0; i < m_vehicles.Length; i++)
		{
			VehicleAIController vehicleAI = m_vehicles[i].VehicleGO.GetComponent<VehicleAIController>();
			if (vehicleAI != null)
			{
				vehicleAI.enabled = false;
			}
			if (m_vehicles[i].Vehicle.GetComponent<Rigidbody>() != null)
			{
				m_vehicles[i].Vehicle.GetComponent<Rigidbody>().isKinematic = true;
				m_vehicles[i].Vehicle.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
			}
		}
		yield return YieldCache.WaitForFixedUpdate;
		countdownTime = 0f;
		Vector3[] positions = new Vector3[m_vehicles.Length];
		Quaternion[] rotations = new Quaternion[m_vehicles.Length];
		for (int j = 0; j < m_vehicles.Length; j++)
		{
			Rigidbody rb = m_vehicles[j].Vehicle.GetComponent<Rigidbody>();
			if (!(rb == null))
			{
				positions[j] = rb.position;
				rotations[j] = rb.rotation;
				m_vehicles[j].m_launchHeightOffset = positions[j].y - m_vehicles[j].m_launchPosition.y;
			}
		}
		while (true)
		{
			countdownTime += Time.deltaTime;
			float lerp = countdownTime / 0.125f;
			lerp = Mathf.Clamp01(lerp);
			for (int k = 0; k < m_vehicles.Length; k++)
			{
				Rigidbody rb2 = m_vehicles[k].Vehicle.GetComponent<Rigidbody>();
				if (!(rb2 == null))
				{
					Vector3 targetPos = m_vehicles[k].m_launchPosition;
					targetPos.y += m_vehicles[k].m_launchHeightOffset;
					rb2.MovePosition(Vector3.Lerp(positions[k], targetPos, lerp));
					rb2.MoveRotation(Quaternion.Slerp(rotations[k], m_vehicles[k].m_launchRotation, lerp));
				}
			}
			if (countdownTime >= 0.125f)
			{
				break;
			}
			yield return YieldCache.WaitForFixedUpdate;
		}
	}

	private void CreateCamera()
	{
		GameObject gameObject = GameObject.FindWithTag("MainCamera");
		DynamicCamera dynamicCamera = null;
		if (Settings.DynamicCamera)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(DynamicCamera) as GameObject;
			dynamicCamera = gameObject2.GetComponent<DynamicCamera>();
			Color backgroundColor = gameObject.GetComponent<Camera>().backgroundColor;
			backgroundColor.a = 1f;
			dynamicCamera.GetComponent<Camera>().backgroundColor = backgroundColor;
			UnityEngine.Object.Destroy(gameObject);
			gameObject = gameObject2;
			dynamicCamera.TouchInputManager = m_touchInputManager;
			dynamicCamera.PreRaceLaunch();
		}
		m_trackingCamera = dynamicCamera;
		if (OnTrackingCameraReadyEvent != null)
		{
			OnTrackingCameraReadyEvent(this, EventArgs.Empty);
		}
	}

	private void CreateRacers()
	{
		RaceEvent currentRace = CurrentRace;
		int humanMask = currentRace.m_humanMask;
		int aiMask = currentRace.m_aiMask;
		int num = BitUtils.BitCount(humanMask);
		int num2 = BitUtils.BitCount(aiMask);
		int num3 = num + num2;
		int[] controllers = currentRace.m_controllers;
		bool flag = true;
		m_vehicles = new VehicleInfo[num3];
		m_vehiclesInRaceOrder = null;
		if (currentRace.m_objective == RaceEvent.ObjectiveType.RacePosition || currentRace.m_objective == RaceEvent.ObjectiveType.TimeTrial || currentRace.m_objective == RaceEvent.ObjectiveType.HeadsUp)
		{
			m_vehiclesInRaceOrder = new VehicleInfo[num3];
		}
		int num4 = 0;
		int num5 = 0;
		for (int i = 0; i != 2; i++)
		{
			int num6 = 0;
			while (num5 != ((i != 0) ? num3 : num))
			{
				if ((((i != 0) ? aiMask : humanMask) & (1 << num6)) != 0)
				{
					int num7 = num5;
					int num8 = ((i != 0 || IsTournament) ? num6 : Settings.PlayerColour);
					bool flag2 = false;
					bool flag3 = false;
					if (currentRace.m_playerData != null && num8 < currentRace.m_playerData.Length && currentRace.m_playerData[num8].m_ghostData != null)
					{
						flag2 = true;
						flag3 = currentRace.m_playerData[num8].m_localPlayer;
						if (currentRace.m_playerData[num8].m_playerIndex >= 0)
						{
							num7 = currentRace.m_playerData[num8].m_playerIndex;
						}
					}
					int num9 = num3 - num7 - 1;
					if (m_tournamentScores != null && num > 1)
					{
						int lastResult = m_tournamentScores[num7].m_lastResult;
						if (lastResult != 0)
						{
							num9 = num3 - lastResult;
						}
						else
						{
							do
							{
								num9 = UnityEngine.Random.Range(0, num3);
							}
							while ((num4 & (1 << num9)) != 0);
						}
						num4 |= 1 << num9;
					}
					if (currentRace.m_playerData != null && num8 < currentRace.m_playerData.Length && currentRace.m_playerData[num8].m_gridIndex >= 0)
					{
						num9 = currentRace.m_playerData[num8].m_gridIndex;
					}
					int numPlayers = (currentRace.SameStartPosition ? 1 : num3);
					num9 = ((!currentRace.SameStartPosition) ? num9 : 0);
					Vector3 o_launchPosition;
					Quaternion o_launchRotation;
					Vector3 o_startPosition;
					Quaternion o_startRotation;
					float o_spawnHeightOffset;
					m_startGrid.GetLaunchTransform(num9, numPlayers, out o_launchPosition, out o_launchRotation, out o_startPosition, out o_startRotation, out o_spawnHeightOffset);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                    Debug.Log($"QA_GRID launch={o_launchPosition:F4} rotation={o_launchRotation} start={o_startPosition:F4}");
#endif
					Vehicle vehicle = null;
					if (flag2)
					{
						GhostVehicle ghostVehicle = VehicleFactory.CreateGhost(num7, num8, o_launchPosition, o_launchRotation);
						vehicle = ghostVehicle;
						VehicleFactory.AddGhostController(vehicle);
						vehicle.IsGhost = !flag3;
					}
					else
					{
						vehicle = VehicleFactory.Create(VehicleTypeStr, num7, num8, o_launchPosition, o_launchRotation);
						if (i == 0)
						{
							string buttonOverride = null;
							if (controllers != null)
							{
								buttonOverride = "Grapple" + controllers[vehicle.PlayerIndex];
							}
							VehicleFactory.AddHumanController(vehicle, m_touchInputManager.GetTouchRegion(num7), buttonOverride);
						}
						else
						{
							AIDifficulty difficulty = ((!m_currentRace.m_allAIsTheSame) ? AIUtils.GetAIRacerDifficulty(m_currentRace.m_aiDifficulty, num7 - num, num2) : m_currentRace.m_aiDifficulty);
							VehicleFactory.AddAIController(vehicle, difficulty, m_currentRace.m_noAIMistakes);
						}
						if (i == 0)
						{
							vehicle.gameObject.AddComponent<VehicleStats>();
						}
						VehicleFactory.AddGhostRecorder(vehicle);
					}
					GameObject vehicle2 = vehicle.gameObject;
					vehicle.OnLapEvent += VehicleEvent_OnLapComplete;
					VehicleInfo vehicleInfo = new VehicleInfo(vehicle2);
					m_vehicles[num7] = vehicleInfo;
					if (m_vehiclesInRaceOrder != null)
					{
						m_vehiclesInRaceOrder[num3 - 1 - num7] = vehicleInfo;
					}
					num5++;
					PhysicsVehicle physicsVehicle = vehicleInfo.Vehicle as PhysicsVehicle;
					if (physicsVehicle != null)
					{
						physicsVehicle.HandbrakeOn = true;
					}
					vehicleInfo.m_launchPosition = o_launchPosition;
					vehicleInfo.m_launchRotation = o_launchRotation;
					vehicleInfo.IsHuman = i == 0;
					vehicleInfo.IsGhost = flag2;
					vehicleInfo.GridIndex = num9;
					vehicleInfo.DoLapPopups = vehicleInfo.IsHuman || flag3;
					string text = CurrentRace.GetPlayerName(num8);
					if (text.Length == 0)
					{
						if (vehicleInfo.IsHuman)
						{
							SocialManager component = GetComponent<SocialManager>();
							text = ((m_numHumans != 1 || !(component != null) || !component.IsPlayerAuthenticated) ? PlayerNames.GetName(num8) : component.PlayerAlias);
						}
						else
						{
							text = PlayerNames.GetName(num8, vehicleInfo.AIDifficulty);
						}
					}
					vehicleInfo.PlayerName = text;
					flag &= flag2;
				}
				num6++;
			}
		}
		AllGhosts = flag;
	}

	private void TrackRacers()
	{
		if (!m_trackingCamera)
		{
			return;
		}
		for (int i = 0; i < NumRacers; i++)
		{
			bool flag = i < m_numHumans;
			if (flag || m_vehicles[i].IsGhost || Settings.DynamicCameraTrackAI)
			{
				m_trackingCamera.TrackObject(m_vehicles[i].Vehicle, flag || Settings.DynamicCameraTrackAI);
			}
		}
	}

	private void CreateInRaceGUI()
	{
		InRaceGrappleButtons component = m_uiController.GetComponent<InRaceGrappleButtons>();
		if (component != null)
		{
			component.Configure(this, m_touchInputManager);
		}
		if (OnClearResults != null)
		{
			OnClearResults(NumRacers);
		}
	}

	private void CreatePrefabObjects()
	{
		m_instantiatedObjects = new GameObject[m_currentRace.m_raceControlPrefabs.Length];
		for (int i = 0; i < m_currentRace.m_raceControlPrefabs.Length; i++)
		{
			m_instantiatedObjects[i] = UnityEngine.Object.Instantiate(m_currentRace.m_raceControlPrefabs[i]) as GameObject;
			m_instantiatedObjects[i].transform.parent = base.transform;
		}
	}

	private void CreateLapTimings()
	{
		UnityEngine.Object original = Resources.Load("Prefabs/Popups/LapPopup");
		UnityEngine.Object original2 = Resources.Load("Prefabs/Popups/PositionPopup");
		int numRacers = NumRacers;
		m_lapPopups = new PositionPopup[numRacers];
		m_positionPopups = new PositionPopup[numRacers];
		for (int i = 0; i < numRacers; i++)
		{
			Vehicle vehicle = m_vehicles[i].Vehicle;
			if (DoLapPopUps && m_vehicles[i].DoLapPopups)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
				PositionPopup component = gameObject.GetComponent<PositionPopup>();
				component.Colour = vehicle.Colour;
				component.Target = vehicle;
				m_lapPopups[i] = component;
			}
			if (DoPositionPopups)
			{
				GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(original2);
				PositionPopup component2 = gameObject2.GetComponent<PositionPopup>();
				component2.Colour = vehicle.Colour;
				component2.Target = vehicle;
				m_positionPopups[i] = component2;
			}
		}
	}

	private void VehicleEvent_OnLapComplete(Vehicle vehicleComponent, float lapTime)
	{
		int playerIndex = vehicleComponent.PlayerIndex;
		VehicleInfo vehicleInfo = m_vehicles[playerIndex];
		if (vehicleInfo.IsFinished)
		{
			return;
		}
		vehicleInfo.OnLapComplete(lapTime);
		if (m_vehiclesInRaceOrder != null)
		{
			if (!m_handleLapsCompleted)
			{
				m_handleLapsCompleted = true;
				StartCoroutine(HandleLapsCompleted());
			}
		}
		else
		{
			vehicleInfo.LapJustCompleted = false;
		}
	}

	private void SortVehicles()
	{
		int num = m_vehiclesInRaceOrder.Length;
		for (int i = 1; i < num; i++)
		{
			VehicleInfo vehicleInfo = m_vehiclesInRaceOrder[i];
			int num2 = i;
			int num3 = i;
			while (num3-- != 0)
			{
				int num4 = VehicleInfo.PartialCompare(vehicleInfo, m_vehiclesInRaceOrder[num3]);
				if (num4 < 0)
				{
					num2 = num3;
				}
				else if (num4 > 1)
				{
					break;
				}
			}
			if (num2 != i)
			{
				for (int num5 = i; num5 != num2; num5--)
				{
					m_vehiclesInRaceOrder[num5] = m_vehiclesInRaceOrder[num5 - 1];
				}
				m_vehiclesInRaceOrder[num2] = vehicleInfo;
			}
		}
		for (int j = 0; j != num; j++)
		{
			VehicleInfo vehicleInfo2 = m_vehiclesInRaceOrder[j];
			if (vehicleInfo2.RacePosition == j)
			{
				continue;
			}
			if (vehicleInfo2.RacePosition != -1)
			{
				PositionPopup positionPopup = m_positionPopups[vehicleInfo2.Vehicle.PlayerIndex];
				if (positionPopup != null)
				{
					positionPopup.Popup(m_positionStrings[j]);
				}
			}
			vehicleInfo2.RacePosition = j;
		}
	}

	private IEnumerator HandleLapsCompleted()
	{
		yield return null;
		SortVehicles();
		bool allHumansFinished = true;
		bool allVehiclesFinished = true;
		for (int i = 0; i != m_vehiclesInRaceOrder.Length; i++)
		{
			VehicleInfo vehicle = m_vehiclesInRaceOrder[i];
			int vehicleID = vehicle.Vehicle.PlayerIndex;
			int colourIndex = vehicle.Vehicle.ColourIndex;
			if (vehicle.LapJustCompleted)
			{
				vehicle.LapJustCompleted = false;
				PositionPopup popup = m_lapPopups[vehicleID];
				if (popup != null)
				{
					string text = null;
					switch (m_currentRace.AwardPointsType)
					{
					case RaceEvent.PointsType.Position:
						text = ((vehicle.LapsRemaining != 0) ? ((vehicle.LapsRemaining != 1) ? string.Format("{0} {1}/{2}", m_popup_Lap, 1 + NumLaps - vehicle.LapsRemaining, NumLaps) : m_popup_FinalLap) : m_popup_Finished);
						break;
					case RaceEvent.PointsType.BestLap:
						text = string.Format("{0:0.00}s", vehicle.BestLapTime);
						break;
					case RaceEvent.PointsType.TotalTime:
						text = string.Format("{0:0.00}s", vehicle.TimeForCompletedLaps);
						break;
					}
					if (text != null)
					{
						m_lapPopups[vehicleID].Popup(text);
					}
				}
				PositionPopup positionPopup = m_positionPopups[vehicleID];
				if (positionPopup != null)
				{
					positionPopup.Popup(m_positionStrings[i]);
				}
				if (!vehicle.IsFinished && (vehicle.LapsRemaining == 0 || m_raceState != RaceState.Running))
				{
					if (m_raceState == RaceState.Running)
					{
						m_raceState = RaceState.RunningFinishing;
					}
					vehicle.FinishNow(false);
					for (int j = 0; j != i; j++)
					{
						if (vehicle.TimeForCompletedLaps < m_vehiclesInRaceOrder[j].TimeForCompletedLaps)
						{
							vehicle.TimeForCompletedLaps = m_vehiclesInRaceOrder[j].TimeForCompletedLaps;
						}
					}
					if (OnAddRaceResult != null)
					{
						int lapsBehind = ((i != 0) ? (vehicle.LapsRemaining - m_vehiclesInRaceOrder[0].LapsRemaining) : (-1));
						float raceTime = ((i != 0) ? (vehicle.TimeForCompletedLaps - m_vehiclesInRaceOrder[0].TimeForCompletedLaps) : vehicle.TimeForCompletedLaps);
						int points = RaceCampaign.GetPointsForPosition(i + 1);
						if (m_tournamentScores != null)
						{
							m_tournamentScores[vehicleID].m_lastResult = i + 1;
							if (CurrentRace.DontAddRacePoints)
							{
								points = 0;
							}
							points = ((!CurrentRace.DontTrackTournamentPoints) ? (m_tournamentScores[vehicleID].m_points += points) : (points + m_tournamentScores[vehicleID].m_points));
						}
						string name = vehicle.PlayerName;
						OnAddRaceResult(i + 1, name, vehicleID, colourIndex, lapsBehind, raceTime, vehicle.BestLapTime, points);
					}
					if (AllGhosts && NumRacers > 1 && vehicle.IsGhost && i == 0)
					{
						SlowMotion.DoSlowMotion(base.gameObject, 0.1f, 0.1f, 0.9f);
					}
				}
			}
			allHumansFinished &= !vehicle.IsHuman | vehicle.IsFinished;
			allVehiclesFinished &= vehicle.IsFinished;
		}
		if ((m_raceState == RaceState.RunningFinishing && allHumansFinished && CurrentRace.m_humanMask != 0) || (CurrentRace.m_humanMask == 0 && allVehiclesFinished))
		{
			StartCoroutine(EndRaceSequence());
		}
		if (allVehiclesFinished)
		{
			m_raceState = RaceState.AllFinished;
		}
		else if (CurrentRace.m_humanMask != 0 && allHumansFinished)
		{
			m_raceState = RaceState.HumansFinished;
		}
		m_handleLapsCompleted = false;
	}

	private IEnumerator EndRaceSequence()
	{
		MusicPlayer musicPlayer = GameObject.Find("__MusicPlayer").GetComponent<MusicPlayer>();
		m_audioPlayer.Refresh();
		bool played = false;
		if (musicPlayer != null && musicPlayer.Volume > 0.01f)
		{
			played = musicPlayer.PlayFanfare();
		}
		if (!played && m_endOfRace != null)
		{
			m_audioPlayer.PlayRandomGUIClip(m_endOfRace);
		}
		if (OnRaceCompleteEvent != null)
		{
			OnRaceCompleteEvent(this, EventArgs.Empty);
		}
		if (OnAllowPause != null)
		{
			OnAllowPause(false);
		}
		SlowMotion.DoSlowMotion(base.gameObject, 0.1f, 0.1f, 0.9f);
		VehicleInfo[] vehicles = m_vehicles;
		foreach (VehicleInfo vehicleInfo in vehicles)
		{
			RaceCampaignSave.RaceStatsImproved improvements = ((m_saveData == null) ? new RaceCampaignSave.RaceStatsImproved() : vehicleInfo.UpdateSave(m_saveData, NumLaps, OnUpdateRaceStats, OnFinalisedRaceStatsInternal));
			if (OnSaveUpdatedEvent != null && improvements != null)
			{
				OnSaveUpdatedEvent(this, new SaveUpdatedArgs(m_saveData, improvements));
			}
		}
		SendMessage("UpdateTourAchievements");
		Dictionary<string, string> eventInfo = new Dictionary<string, string>();
		float fpsAverage = 0f;
		if (m_profileTime > 0f)
		{
			fpsAverage = (float)m_profileNumFrames / m_profileTime;
			Debug.Log("Average FPS after race: " + fpsAverage);
			eventInfo["fps"] = Mathf.RoundToInt(fpsAverage).ToString();
		}
		eventInfo["name"] = m_currentRace.name;
		eventInfo["device"] = SystemInfo.deviceModel;
		eventInfo["graphicsDevice"] = SystemInfo.graphicsDeviceName;
		eventInfo["graphicsVendor"] = SystemInfo.graphicsDeviceVendor;
		eventInfo["resolution"] = PlatformSettings.NativeResolution.ToString();
		eventInfo["settingsVersion"] = PlatformSettings.SettingsVersion.ToString();
		Analytics.LogEvent("RaceFinished", eventInfo);
		if ((bool)m_trackingCamera && !Settings.DynamicCameraTrackAI)
		{
			for (int j = 0; j < NumHumans; j++)
			{
				if (!m_vehicles[j].IsGhost)
				{
					m_trackingCamera.UntrackObject(m_vehicles[j].Vehicle);
				}
			}
			for (int k = 0; k < NumRacers; k++)
			{
				if (!m_vehicles[k].IsGhost)
				{
					m_trackingCamera.TrackObject(m_vehicles[k].Vehicle, false);
				}
			}
		}
		yield return YieldCache.WaitForHalfSecond;
		UIEvent_NextScreen();
		while (m_tournamentScores != null && m_raceState != RaceState.AllFinished)
		{
			yield return null;
		}
		bool recordingGhosts = false;
		bool allRecording = true;
		bool allGhosts = true;
		if (m_vehicles != null)
		{
			VehicleInfo[] vehicles2 = m_vehicles;
			foreach (VehicleInfo vehicleInfo2 in vehicles2)
			{
				bool isRecording = vehicleInfo2.RecordingGhost;
				recordingGhosts |= isRecording;
				allRecording &= isRecording;
				allGhosts &= vehicleInfo2.IsGhost;
			}
		}
		bool ghostsFinalised = false;
		if (CurrentRace.RecordPlayerGhost || PersistentState.Get().RecordPlayerGhost)
		{
			yield return StartCoroutine(WaitForGhostRecording());
			if (m_vehicles != null)
			{
				VehicleInfo[] vehicles3 = m_vehicles;
				foreach (VehicleInfo vehicleInfo3 in vehicles3)
				{
					if (!vehicleInfo3.IsFinished || m_saveData == null)
					{
						continue;
					}
					byte[] ghostData = vehicleInfo3.GhostData;
					if (ghostData != null)
					{
						if (CurrentRace.RecordPlayerGhost && OnGhostRecorded != null)
						{
							OnGhostRecorded(vehicleInfo3.Vehicle, m_saveData, ghostData);
						}
						if (PersistentState.Get().RecordPlayerGhost && vehicleInfo3.Vehicle.PlayerIndex == 0)
						{
							Directory.CreateDirectory("Ghosts");
							string filename = string.Format("Ghosts/{0}_{1:yyyy-MM-dd_hh-mm-ss}.bytes", CurrentRace.name, DateTime.Now);
							FileStream file = File.Open(filename, FileMode.Create, FileAccess.Write);
							file.Write(ghostData, 0, ghostData.Length);
							file.Flush();
						}
					}
				}
			}
			ghostsFinalised = true;
		}
		m_allowShowReplay = true;
		if (OnDisplayEndRaceOptions != null)
		{
			OnDisplayEndRaceOptions(true);
		}
		if (!CurrentRace.NoRaceReplay && (recordingGhosts || allGhosts))
		{
			if (recordingGhosts && !ghostsFinalised)
			{
				yield return StartCoroutine(WaitForGhostRecording());
			}
			if (m_allowShowReplay && OnDisplayReplayOptions != null)
			{
				OnDisplayReplayOptions(true);
			}
		}
		if (OnReplayDataReady != null)
		{
			OnReplayDataReady();
		}
		if (AutoFinish)
		{
			yield return YieldCache.WaitForFiveSeconds;
			UIEvent_LeaveRace();
		}
	}

	private IEnumerator WaitForGhostRecording()
	{
		while (m_raceState != RaceState.AllFinished)
		{
			yield return null;
		}
		yield return YieldCache.WaitForSeconds(3f);
	}

	private void OnFinalisedRaceStatsInternal(RaceStats _raceStats)
	{
		int num = Mathf.RoundToInt(_raceStats.m_raceTime * 1000f);
		SocialManager component = GetComponent<SocialManager>();
		if (component != null && m_currentRace.m_leaderboard != null && _raceStats.m_raceTime > 0f)
		{
			component.UpdateScore(m_currentRace.m_leaderboard, num, false);
		}
		GameCenterChallenge currentChallenge = CurrentChallenge;
		if (currentChallenge != null && currentChallenge.score != null && num < currentChallenge.score.value)
		{
			CompletedChallenge = true;
		}
		SendMessage("OnUpdateTotalNuts", (long)_raceStats.m_award);
		if (OnFinalisedRaceStats != null)
		{
			OnFinalisedRaceStats(_raceStats);
		}
	}

	public void PrepareToReplayRace()
	{
		if (m_trackingCamera != null)
		{
			UnityEngine.Object.Destroy(m_trackingCamera);
			m_trackingCamera = null;
		}
		DestroyScreens();
		DestroyPrefabObjects();
		DestroyVehicles(true);
		DestroyPopups();
		if (OnRaceReplayEvent != null)
		{
			OnRaceReplayEvent(this, EventArgs.Empty);
		}
	}

	private void CacheLocalisedText()
	{
		int num = 8;
		if (m_positionStrings == null || m_positionStrings.Length != num)
		{
			m_positionStrings = new string[num];
		}
		for (int i = 0; i < num; i++)
		{
			m_positionStrings[i] = Language.Get("ID_Position" + (i + 1));
		}
		m_popup_Lap = Language.Get("ID_Popup_Lap");
		m_popup_FinalLap = Language.Get("ID_Popup_FinalLap");
		m_popup_Finished = Language.Get("ID_Popup_Finished");
	}

	private void DestroyVehicles(bool _destroyObjects)
	{
		if (m_vehicles != null)
		{
			VehicleInfo[] vehicles = m_vehicles;
			foreach (VehicleInfo vehicleInfo in vehicles)
			{
				vehicleInfo.Vehicle.OnLapEvent -= VehicleEvent_OnLapComplete;
				if (_destroyObjects)
				{
					UnityEngine.Object.Destroy(vehicleInfo.VehicleGO);
				}
			}
		}
		if (_destroyObjects)
		{
			m_vehicles = null;
			m_vehiclesInRaceOrder = null;
		}
		StopCoroutine("UpdatePositions");
	}

	private void DestroyPopups()
	{
		if (m_lapPopups != null)
		{
			PositionPopup[] lapPopups = m_lapPopups;
			foreach (PositionPopup positionPopup in lapPopups)
			{
				if (positionPopup != null)
				{
					UnityEngine.Object.Destroy(positionPopup.gameObject);
				}
			}
			m_lapPopups = null;
		}
		if (m_positionPopups == null)
		{
			return;
		}
		PositionPopup[] positionPopups = m_positionPopups;
		foreach (PositionPopup positionPopup2 in positionPopups)
		{
			if (positionPopup2 != null)
			{
				UnityEngine.Object.Destroy(positionPopup2.gameObject);
			}
		}
		m_positionPopups = null;
	}

	private void DestroyScreens()
	{
		m_uiController = null;
	}

	private void DestroyPrefabObjects()
	{
		if (m_instantiatedObjects != null)
		{
			GameObject[] instantiatedObjects = m_instantiatedObjects;
			foreach (GameObject obj in instantiatedObjects)
			{
				UnityEngine.Object.Destroy(obj);
			}
			m_instantiatedObjects = null;
		}
	}
}
