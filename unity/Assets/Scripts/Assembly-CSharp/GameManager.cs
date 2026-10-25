using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RaceManager))]
[AddComponentMenu("Gameplay/Game Manager")]
public class GameManager : MonoBehaviour
{
	public interface IFreeplayRaceSequence
	{
		bool IsEmpty { get; }

		RaceEvent NextEvent { get; }

		int NumEvents { get; }

		bool IsFinite { get; }
	}

	private enum ScreenType
	{
		Race = 0,
		RaceSelection = 1,
		TourSelection = 2,
		MainMenu = 3,
		FreeplayPlayerSelection = 4,
		FreeplayTrackSelection = 5,
		FreeplayRaceSequence = 6,
		AsyncMatchSelection = 7,
		AsyncMatchView = 8,
		None = 9
	}

	public struct TournamentScores
	{
		public int m_points;

		public int m_lastResult;
	}

	private const string c_tourSelectionScene = "TourSelection";

	private const string c_raceSelectionScene = "RaceSelection";

	private RaceManager m_raceManager;

	private TurnBasedMatchManager m_turnBasedMatchManager;

	private bool m_raceCompleted;

	private bool m_nextRace;

	private bool m_showReplay;

	private ScreenType m_prevScreen;

	private ScreenType m_nextScreen;

	private RaceCampaign m_campaign;

	private RaceTour m_tour;

	private RaceEvent m_race;

	private SaveManager m_saveManager;

	private RaceCampaignSave m_saveData;

	private RaceCampaignSave.RaceTourSaveData m_previousSaveState;

	private RaceCampaignSave.UnlockFlags m_unlocks;

	private GameObject m_loadingScreenGO;

	private LoadscreenManager m_loadScreen;

	private BusyIcon m_busyIcon;

	private GameObject m_musicPlayerGO;

	private bool m_reverseTrack;

	private GameCenterChallenge m_challenge;

	private TournamentScores[] m_tournamentScores;

	private IFreeplayRaceSequence m_freeplayRaceSequence;

	private AIDifficulty m_freeplayAiDifficulty;

	private int m_freeplayHumanPlayerMask = 1;

	private int m_freeplayAIPlayerMask = 14;

	private int[] m_freeplayControllers;

	private bool m_freeplayRunning;

	public GameObject m_loadScreenPrefab;

	public GameObject m_musicPlayerPrefab;

	public GameObject m_saveManagerPrefab;

	public Playlist m_menuPlaylist;

	public float m_loadScreenFadeTime;

	public float m_loadScreenToRaceFadeTime = 0.75f;

	public float m_blankScreenFadeTime;

	public float m_fadeInDelay;

	public float m_fadeOutDelay = 0.5f;

	public bool m_loadBlankScreen = true;

	public static bool s_slowLoading;

	public RaceCampaignSave SaveData
	{
		get
		{
			return m_saveData;
		}
	}

	private bool RespondToChallenges { get; set; }

	public bool ReverseTrack
	{
		get
		{
			return m_reverseTrack;
		}
	}

	public IFreeplayRaceSequence FreeplayRaceSequence
	{
		set
		{
			m_freeplayRaceSequence = value;
		}
	}

	public AIDifficulty FreeplayAiDifficulty
	{
		get
		{
			return m_freeplayAiDifficulty;
		}
		set
		{
			m_freeplayAiDifficulty = value;
		}
	}

	public int FreeplayHumanPlayerMask
	{
		get
		{
			return m_freeplayHumanPlayerMask;
		}
		set
		{
			m_freeplayHumanPlayerMask = value;
		}
	}

	public int FreeplayAIPlayerMask
	{
		get
		{
			return m_freeplayAIPlayerMask;
		}
		set
		{
			m_freeplayAIPlayerMask = value;
		}
	}

	public int[] FreeplayControllers
	{
		get
		{
			return m_freeplayControllers;
		}
		set
		{
			m_freeplayControllers = value;
		}
	}

	public RaceTour CurrentTour
	{
		get
		{
			return m_tour;
		}
	}

	public RaceEvent CurrentRace
	{
		get
		{
			return m_race;
		}
	}

	public RaceCampaign CurrentCampaign
	{
		get
		{
			return m_campaign;
		}
	}

	public SaveManager SaveManager
	{
		get
		{
			return m_saveManager;
		}
	}

	private void Awake()
	{
		base.gameObject.AddComponent<TimeManager>();
		Application.backgroundLoadingPriority = ThreadPriority.Low;
		GameObject[] array = GameObject.FindGameObjectsWithTag("RaceControl");
		if (array.Length > 1)
		{
			Debug.LogError("There should only be one instance of RaceControl");
			Object.Destroy(base.gameObject);
		}
		else
		{
			m_loadingScreenGO = Object.Instantiate(m_loadScreenPrefab) as GameObject;
			m_loadScreen = m_loadingScreenGO.GetComponentInChildren<LoadscreenManager>();
			m_busyIcon = m_loadingScreenGO.GetComponentInChildren<BusyIcon>();
			if (m_musicPlayerPrefab != null)
			{
				m_musicPlayerGO = Object.Instantiate(m_musicPlayerPrefab) as GameObject;
			}
		}
		m_raceManager = GetComponent<RaceManager>();
		m_turnBasedMatchManager = GetComponent<TurnBasedMatchManager>();
		m_saveManager = SaveManager.Find();
	}

	private void Start()
	{
		// The Android movie view can leave system bars visible after the intro.
		UIUtils.SetLowProfileMode(true);
		StartMusic();
		GameCenterManager.localPlayerDidSelectChallengeEvent += OnChallengeSelected;
		RespondToChallenges = true;
	}

	private void OnDestroy()
	{
		GameCenterManager.localPlayerDidSelectChallengeEvent -= OnChallengeSelected;
		if (m_loadingScreenGO != null)
		{
			Object.Destroy(m_loadingScreenGO);
		}
		if (m_musicPlayerGO != null)
		{
			Object.Destroy(m_musicPlayerGO);
		}
	}

	private void OnLevelWasLoaded()
	{
		if (m_reverseTrack)
		{
			TrackUtils.ReverseTrack();
		}
	}

	private void OnApplicationPause(bool _pause)
	{
		if (_pause)
		{
			UpdateSaveData();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			UIUtils.SetLowProfileMode(focus);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			UIUtils.SetLowProfileMode(true);
		}
	}

	private void SetCampaign(RaceCampaign _campaign)
	{
		m_campaign = _campaign;
		m_tour = null;
		m_race = null;
		m_saveData = m_saveManager.GetSave(m_campaign);
		if (m_saveData == null)
		{
			m_saveData = base.gameObject.AddComponent<RaceCampaignSave>();
			m_saveData.SetCampaign(_campaign, null);
		}
	}

	public void OnCampaignSelected(RaceCampaign _campaign)
	{
		SetCampaign(_campaign);
		switch (_campaign.m_type)
		{
		case RaceCampaign.Type.SinglePlayer:
			StartCoroutine(RunSinglePlayerRaceCampaign());
			break;
		case RaceCampaign.Type.Multiplayer:
			StartCoroutine(RunMultiplayerRaceCampaign());
			break;
		case RaceCampaign.Type.TurnBased:
			if (m_turnBasedMatchManager != null && m_turnBasedMatchManager.CurrentMatchData != null)
			{
				if (m_turnBasedMatchManager.CurrentMatchData.NumRaces == 0 && m_turnBasedMatchManager.IsCurrentPlayersTurn())
				{
					StartCoroutine(StartNewTurnBasedRaceCampaign());
				}
				else
				{
					StartCoroutine(ResumeTurnBasedRaceCampaign());
				}
			}
			break;
		}
	}

	public void StartLoadTest(RaceCampaign _campaign)
	{
		m_campaign = _campaign;
		m_saveData = base.gameObject.AddComponent<RaceCampaignSave>();
		m_saveData.SetCampaign(_campaign, null);
		StartCoroutine(RunLoadTest());
	}

	private IEnumerator RunLoadTest()
	{
		RaceTour[] tours = m_campaign.m_tours;
		foreach (RaceTour tour in tours)
		{
			RaceEvent[] races = tour.m_races;
			foreach (RaceEvent _race in races)
			{
				yield return StartCoroutine(LoadRaceBase(_race));
				yield return StartCoroutine(LoadRaceExtras(_race));
			}
		}
	}

	public void StartSoakTest(RaceCampaign _campaign, RaceTour _tour)
	{
		m_campaign = _campaign;
		m_saveData = base.gameObject.AddComponent<RaceCampaignSave>();
		m_saveData.SetCampaign(_campaign, null);
		if (_tour != null)
		{
			StartCoroutine(RunSoakTest(_tour, 50));
		}
		else
		{
			StartCoroutine(RunSoakTest());
		}
	}

	private IEnumerator RunSoakTest(RaceTour _tour, int _numTests)
	{
		PersistentState persistentState = PersistentState.Get();
		persistentState.m_aiControlHumans = true;
		persistentState.aiDifficulty = AIDifficulty.Hard;
		m_raceManager.AutoFinish = true;
		m_tour = _tour;
		m_nextScreen = ScreenType.None;
		int numTests = ((_numTests <= 0) ? _tour.m_races.Length : _numTests);
		for (int index = 0; index < numTests; index++)
		{
			int raceIndex = index % _tour.m_races.Length;
			RaceEvent race = _tour.m_races[raceIndex];
			RaceEvent.ObjectiveType objective = race.m_objective;
			if (objective != RaceEvent.ObjectiveType.Pickups && objective != RaceEvent.ObjectiveType.Tutorial)
			{
				yield return StartCoroutine(RunSinglePlayerRaceEvent(race, false));
			}
		}
		persistentState.m_aiControlHumans = false;
		m_raceManager.AutoFinish = false;
	}

	private IEnumerator RunSoakTest()
	{
		while (true)
		{
			RaceTour[] tours = m_campaign.m_tours;
			foreach (RaceTour tour in tours)
			{
				yield return StartCoroutine(RunSoakTest(tour, 0));
			}
		}
	}

	public void OnOptionsSelected()
	{
		StartCoroutine(GotoScreenSimple("OptionsScreen", true, m_fadeOutDelay));
	}

	public void OnCreditsSelected()
	{
		StartCoroutine(GotoScreenSimple("CreditsScreen", true, m_fadeOutDelay));
	}

	public void BootToTitleScreen()
	{
		PlatformSettings.ApplyResolution();
		StartCoroutine(GotoScreenSimple("TitleScreen", false, 0f));
	}

	public void OnOptionsReturnToMainMenu()
	{
		StartCoroutine(GotoTitleScreen());
	}

	public void OnFreeplayOptionsGoBack()
	{
		StartCoroutine(GotoTitleScreen());
	}

	public bool OnAccessAllAreas()
	{
		if (m_saveManager == null)
		{
			return false;
		}
		m_saveManager.OnAccessAllAreas();
		m_previousSaveState = null;
		m_unlocks = (RaceCampaignSave.UnlockFlags)0;
		GameObject gameObject = GameObject.FindGameObjectWithTag("UIController");
		if (gameObject != null)
		{
			if ((bool)gameObject.GetComponent<PipeGUI_RaceSelectionUIController>())
			{
				StartCoroutine(RunRaceSelection(m_tour));
			}
			else if ((bool)gameObject.GetComponent<PipeGUI_TourPlateSelection>())
			{
				StartCoroutine(RunSinglePlayerRaceCampaign());
			}
		}
		return true;
	}

	public bool OnRevokeAccessAllAreas()
	{
		if (m_saveManager == null)
		{
			return false;
		}
		m_saveManager.OnRevokeAccessAllAreas();
		m_previousSaveState = null;
		m_unlocks = (RaceCampaignSave.UnlockFlags)0;
		GameObject gameObject = GameObject.FindGameObjectWithTag("UIController");
		if (gameObject != null)
		{
			if ((bool)gameObject.GetComponent<PipeGUI_RaceSelectionUIController>())
			{
				StartCoroutine(RunRaceSelection(m_tour));
			}
			else if ((bool)gameObject.GetComponent<PipeGUI_TourPlateSelection>())
			{
				StartCoroutine(RunSinglePlayerRaceCampaign());
			}
		}
		return true;
	}

	private bool FindChallengeData(string _id, out RaceCampaign _campaign, out RaceTour _tour, out RaceEvent _race)
	{
		if (_id.Length > 0 && m_saveManager != null)
		{
			RaceCampaign[] campaigns = m_saveManager.m_campaigns;
			foreach (RaceCampaign raceCampaign in campaigns)
			{
				RaceTour[] tours = raceCampaign.m_tours;
				foreach (RaceTour raceTour in tours)
				{
					RaceEvent[] races = raceTour.m_races;
					foreach (RaceEvent raceEvent in races)
					{
						if (raceEvent.m_leaderboard != null && raceEvent.m_leaderboard.m_gameCenterID == _id)
						{
							_campaign = raceCampaign;
							_tour = raceTour;
							_race = raceEvent;
							return true;
						}
					}
				}
			}
		}
		_campaign = null;
		_tour = null;
		_race = null;
		return false;
	}

	public static bool IsChallengeSupported(GameCenterChallenge _challenge)
	{
		return true;
	}

	public void OnChallengeSelected(GameCenterChallenge _challenge)
	{
		if (IsChallengeSupported(_challenge))
		{
			if (RespondToChallenges)
			{
				Debug.Log("Responding to challenge");
				RaceEvent_GotoChallenge(_challenge);
			}
			else if (m_raceManager != null)
			{
				Debug.Log("Deferring challenge");
				m_raceManager.OnChallengeSelected(_challenge);
			}
		}
	}

	private IEnumerator RunRace(RaceEvent _raceEvent, bool _quickStart)
	{
		if (!m_raceCompleted)
		{
			BroadcastMessage("RaceEvent_StartRace", _quickStart);
		}
		while (!m_raceCompleted)
		{
			yield return null;
		}
		UpdateSaveData();
	}

	private void UpdateSaveData()
	{
		if (m_saveData != null)
		{
			m_unlocks |= m_saveData.UpdateSaveData();
		}
	}

	private void RaceEvent_Replay()
	{
		m_raceCompleted = true;
	}

	private void RaceEvent_ShowReplay()
	{
		m_raceCompleted = true;
		m_showReplay = true;
	}

	private void RaceEvent_NextScreen()
	{
		m_raceCompleted = true;
		m_nextRace = true;
	}

	private void RaceEvent_LeaveRace()
	{
		m_raceCompleted = true;
		m_nextRace = true;
		m_freeplayRunning = false;
	}

	private void RaceEvent_ReturnToMainMenu()
	{
		StartMusic();
		FlushSocialUpdates();
		m_raceCompleted = true;
		m_nextRace = true;
		m_freeplayRunning = false;
		m_nextScreen = ScreenType.MainMenu;
	}

	private void RaceEvent_GotoChallenge(GameCenterChallenge _challenge)
	{
		if (_challenge != null && _challenge.score != null)
		{
			GotoScoreChallenge(_challenge);
		}
	}

	private void GotoScoreChallenge(GameCenterChallenge _challenge)
	{
		RaceCampaign _campaign;
		RaceTour _tour;
		RaceEvent _race;
		if (FindChallengeData(_challenge.score.category, out _campaign, out _tour, out _race))
		{
			bool flag = false;
			RaceCampaignSave save = m_saveManager.GetSave(_campaign);
			if (save != null)
			{
				RaceCampaignSave.RaceEventSaveData saveData = save.GetSaveData(_tour, _race);
				if (saveData != null && saveData.Locked)
				{
					flag = true;
				}
			}
			StopAllCoroutines();
			FlushSocialUpdates();
			if (flag)
			{
				Cleanup();
				SetCampaign(_campaign);
				m_tour = _tour;
				StartCoroutine(RunRaceSelection(_tour));
			}
			else
			{
				StartCoroutine(GotoScoreChallenge(_challenge, _campaign, _tour, _race));
			}
		}
		else
		{
			RaceEvent_ReturnToMainMenu();
		}
	}

	private IEnumerator LoadRaceBase(RaceEvent _raceEvent)
	{
		yield return Application.LoadLevelAsync("BlankScene");
		m_reverseTrack = _raceEvent.m_layout.CanReverse && (_raceEvent.m_layout.m_reverseTrack | _raceEvent.m_forceReverseTrack);
		bool async = true;
		bool firstScene = true;
		for (int index = 0; index < _raceEvent.m_layout.NumScenes; index++)
		{
			string scene = _raceEvent.m_layout.GetScene(index, _raceEvent.m_variant);
			if (async)
			{
				if (Application.loadedLevelName != scene)
				{
					yield return (!firstScene) ? Application.LoadLevelAdditiveAsync(scene) : Application.LoadLevelAsync(scene);
				}
			}
			else
			{
				if (firstScene)
				{
					Application.LoadLevel(scene);
				}
				else
				{
					Application.LoadLevelAdditive(scene);
				}
				yield return null;
			}
			m_reverseTrack = false;
			firstScene = false;
		}
		if (async)
		{
			yield return Application.LoadLevelAdditiveAsync(_raceEvent.GUIScene);
			yield break;
		}
		Application.LoadLevelAdditive(_raceEvent.GUIScene);
		yield return null;
	}

	private IEnumerator LoadRaceExtras(RaceEvent _raceEvent)
	{
		bool async = true;
		string[] extraScenes = _raceEvent.m_extraScenes;
		foreach (string scene in extraScenes)
		{
			if (async)
			{
				yield return Application.LoadLevelAdditiveAsync(scene);
			}
			else
			{
				Application.LoadLevelAdditive(scene);
				yield return null;
			}
			yield return null;
		}
	}

	private IEnumerator PrepareRace(RaceEvent _raceEvent, bool _quickStart)
	{
		RaceCampaignSave.RaceEventSaveData saveData = null;
		if (m_tour != null && m_saveData != null)
		{
			saveData = m_saveData.GetSaveData(m_tour, _raceEvent);
		}
		if (saveData == null)
		{
			saveData = new RaceCampaignSave.RaceEventSaveData();
			saveData.InitSaveData(null, _raceEvent);
		}
		m_raceManager.PrepareRace(_raceEvent, saveData, _quickStart);
		yield return null;
		SceneSettings.RenderSettings.MeshBatcher.Batch();
		// Compile the material variants when rendered. WarmupAllShaders also
		// tries unsupported internal stereo variants and can crash Unity 6/GLES.
		yield return null;
	}

	private IEnumerator RunTour(RaceTour _tour)
	{
		m_nextScreen = ScreenType.None;
		for (int i = 0; i < _tour.m_races.Length; i++)
		{
			RaceEvent raceEvent = _tour.m_races[i];
			if (m_saveData != null)
			{
				RaceCampaignSave.RaceEventSaveData saveData = m_saveData.GetSaveData(_tour, raceEvent);
				if (saveData.Locked)
				{
					continue;
				}
			}
			yield return StartCoroutine(RunSinglePlayerRaceEvent(raceEvent, false));
		}
		m_nextScreen = ScreenType.TourSelection;
		HandleNextScreen();
		yield return null;
	}

	private IEnumerator RunSinglePlayerRaceEvent(RaceEvent _raceEvent, bool _quickStart)
	{
		int humanMask = 1 << Settings.PlayerColour;
		int num = 0;
		int num2 = _raceEvent.m_numRacers - 1;
		int num3 = 0;
		while (num2 > 0)
		{
			if (Settings.PlayerColour != num3)
			{
				num |= 1 << (num3 & 0x1F);
				num2--;
			}
			num3++;
		}
		_raceEvent.m_humanMask = humanMask;
		_raceEvent.m_aiMask = num;
		return RunRaceEvent(_raceEvent, _quickStart);
	}

	private IEnumerator RunRaceEvent(RaceEvent _raceEvent, bool _quickStart)
	{
		yield return StartCoroutine(RunRaceEvent2(_raceEvent, _quickStart));
		HandleNextScreen();
	}

	private IEnumerator RunRaceEvent2(RaceEvent _baseEvent, bool _quickStart)
	{
		RespondToChallenges = false;
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(true, m_loadScreenFadeTime, false, 0f));
		m_raceManager.TournamentScores = m_tournamentScores;
		m_raceManager.CurrentChallenge = m_challenge;
		yield return StartCoroutine(LoadRaceBase(_baseEvent));
		Analytics.LogState("InRace");
		bool haveRaced = false;
		m_nextRace = false;
		m_showReplay = false;
		RaceEvent replayRaceEvent = Object.Instantiate(_baseEvent) as RaceEvent;
		replayRaceEvent.name = _baseEvent.name;
		replayRaceEvent.m_humanMask = 0;
		replayRaceEvent.m_runFlags = RaceEvent.RunFlags.DontTrackTournamentPoints | RaceEvent.RunFlags.DontAddRacePoints | RaceEvent.RunFlags.NoUpgrades;
		replayRaceEvent.m_playerData = new RaceEvent.PlayerData[4];
		while (!m_nextRace)
		{
			RaceEvent activeEvent = ((!m_showReplay) ? _baseEvent : replayRaceEvent);
			if (haveRaced)
			{
				yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(true, m_blankScreenFadeTime, true, 0f));
				m_raceManager.PrepareToReplayRace();
				GameObject[] objectsToDelete = GameObject.FindGameObjectsWithTag("DeleteOnReplay");
				if (objectsToDelete != null)
				{
					GameObject[] array = objectsToDelete;
					foreach (GameObject obj in array)
					{
						Object.Destroy(obj);
					}
				}
				yield return null;
			}
			if (m_showReplay)
			{
				Analytics.LogEvent("RaceReplay", "name", activeEvent.name);
			}
			else
			{
				Analytics.LogEvent("RaceStarted", "name", activeEvent.name, "restarted", haveRaced.ToString());
			}
			FleePursuer.s_replayPlay = m_showReplay;
			yield return StartCoroutine(LoadRaceExtras(activeEvent));
			BroadcastMessage("Event_ConfigureForHumanPlayers", activeEvent.m_humanMask, SendMessageOptions.DontRequireReceiver);
			bool quickStart = !activeEvent.m_disableQuickStart && (haveRaced | _quickStart);
			yield return StartCoroutine(PrepareRace(activeEvent, quickStart));
			float fadeTime = ((!haveRaced) ? m_loadScreenToRaceFadeTime : m_blankScreenFadeTime);
			bool fadeToColour = haveRaced;
			yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(false, fadeTime, fadeToColour, m_fadeInDelay));
			m_raceCompleted = false;
			bool wasReplay = m_showReplay;
			m_showReplay = false;
			yield return StartCoroutine(RunRace(activeEvent, quickStart));
			if (m_showReplay && !wasReplay)
			{
				replayRaceEvent.m_aiMask = 0;
				for (int index = 0; index < replayRaceEvent.m_playerData.Length; index++)
				{
					if (index < activeEvent.m_numRacers)
					{
						Vehicle vehicle = m_raceManager.GetVehicle(index);
						int colourIndex = vehicle.ColourIndex;
						replayRaceEvent.m_aiMask |= 1 << colourIndex;
						replayRaceEvent.m_playerData[colourIndex].m_ghostData = m_raceManager.GetGhostData(index);
						replayRaceEvent.m_playerData[colourIndex].m_name = m_raceManager.GetPlayerName(index);
						replayRaceEvent.m_playerData[colourIndex].m_playerIndex = index;
						replayRaceEvent.m_playerData[colourIndex].m_gridIndex = m_raceManager.GetGridIndex(index);
						replayRaceEvent.m_playerData[colourIndex].m_localPlayer = true;
					}
				}
			}
			if (activeEvent.m_replaysRemaining > 0)
			{
				activeEvent.m_replaysRemaining--;
			}
			haveRaced = true;
		}
		m_challenge = null;
		Object.Destroy(replayRaceEvent);
		RespondToChallenges = true;
	}

	private void SetFreePlayVehicles()
	{
		int num = BitUtils.BitCount(m_freeplayHumanPlayerMask);
		int num2 = BitUtils.BitCount(m_freeplayAIPlayerMask);
		int num3 = num + num2;
		int[] array = new int[8];
		int num4 = 0;
		for (int i = 0; i < 8; i++)
		{
			if (GlobalSave.IsVehicleStyleUnlocked(i))
			{
				array[num4++] = i;
			}
		}
		int num5 = num4;
		for (int j = 0; j < num5; j++)
		{
			int num6 = array[j];
			int num7 = Random.Range(j, num5);
			array[j] = array[num7];
			array[num7] = num6;
		}
		if (num == 1)
		{
			for (int k = 0; k < num5; k++)
			{
				if (array[k] == Settings.VehicleStyleIndex)
				{
					int num8 = array[0];
					array[0] = array[k];
					array[k] = num8;
					break;
				}
			}
		}
		for (int l = 0; l < num3; l++)
		{
			VehicleFactory.SetStyle(l, array[l]);
		}
	}

	private IEnumerator RunFreeplayRaceSequence()
	{
		int numPlayers = BitUtils.BitCount(m_freeplayHumanPlayerMask | m_freeplayAIPlayerMask);
		m_tournamentScores = new TournamentScores[numPlayers];
		m_freeplayRunning = true;
		SetFreePlayVehicles();
		int raceCount = 0;
		m_raceManager.SetRaceCount(-1, -1);
		while (m_freeplayRunning && !m_freeplayRaceSequence.IsEmpty)
		{
			RaceEvent nextEvent = m_freeplayRaceSequence.NextEvent;
			RaceEvent raceEvent = Object.Instantiate(nextEvent) as RaceEvent;
			raceEvent.name = nextEvent.name;
			raceEvent.m_aiDifficulty = m_freeplayAiDifficulty;
			raceEvent.m_allAIsTheSame = true;
			raceEvent.m_humanMask = m_freeplayHumanPlayerMask;
			raceEvent.m_aiMask = m_freeplayAIPlayerMask;
			raceEvent.m_controllers = m_freeplayControllers;
			raceEvent.m_numRacers = BitUtils.BitCount(m_freeplayHumanPlayerMask) + BitUtils.BitCount(m_freeplayAIPlayerMask);
			raceEvent.m_runFlags = RaceEvent.RunFlags.NoUpgrades;
			if (m_freeplayRaceSequence.IsFinite)
			{
				raceCount++;
				m_raceManager.SetRaceCount(raceCount, m_freeplayRaceSequence.NumEvents);
			}
			yield return StartCoroutine(RunRaceEvent2(raceEvent, false));
			Object.Destroy(raceEvent);
		}
		m_raceManager.SetRaceCount(-1, -1);
		m_tournamentScores = null;
		m_nextScreen = ScreenType.FreeplayTrackSelection;
		HandleNextScreen();
	}

	private IEnumerator RunTournamentReplay(RaceEvent _raceEvent, int _raceIndex, int _raceCount, TournamentScores[] _scores)
	{
		m_tournamentScores = _scores;
		m_raceManager.SetRaceCount(_raceIndex, _raceCount);
		yield return StartCoroutine(RunRaceEvent2(_raceEvent, false));
		m_raceManager.SetRaceCount(-1, -1);
		m_tournamentScores = null;
		m_nextScreen = ScreenType.AsyncMatchView;
		HandleNextScreen();
	}

	private IEnumerator RunReplay(RaceEvent _raceEvent, RaceEvent.GhostType _ghostType)
	{
		byte[] ghostData = _raceEvent.GetGhostData(_ghostType);
		if (ghostData != null)
		{
			RaceCampaignSave prevSaveData = m_saveData;
			m_saveData = null;
			RaceEvent newEvent = Object.Instantiate(_raceEvent) as RaceEvent;
			int colourIndex = Settings.PlayerColour;
			newEvent.m_humanMask = 0;
			newEvent.m_aiMask = 1 << colourIndex;
			newEvent.m_runFlags = RaceEvent.RunFlags.DontTrackTournamentPoints;
			newEvent.m_playerData = new RaceEvent.PlayerData[4];
			newEvent.m_playerData[colourIndex].m_ghostData = ghostData;
			newEvent.m_playerData[colourIndex].m_name = "Replay";
			newEvent.m_playerData[colourIndex].m_playerIndex = 0;
			yield return StartCoroutine(RunRaceEvent2(newEvent, false));
			Object.Destroy(newEvent);
			m_saveData = prevSaveData;
		}
		m_nextScreen = ScreenType.RaceSelection;
		HandleNextScreen();
	}

	private IEnumerator RunRaceSelection(RaceTour _tour)
	{
		bool fadeToColour = false;
		float fadeTime = m_loadScreenFadeTime;
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(true, fadeTime, fadeToColour, 0f));
		if (m_loadBlankScreen)
		{
			Application.LoadLevel("BlankScene");
			yield return null;
		}
		if (m_saveData != null)
		{
			m_saveData.CloudSync();
		}
		GlobalSave.DoCloudSync();
		Application.LoadLevel("RaceSelection");
		Analytics.LogState("RaceSelection");
		yield return null;
		m_prevScreen = ScreenType.TourSelection;
		PipeGUI_RaceSelectionUIController gui = GameObjectUtils.FindComponentWithTag<PipeGUI_RaceSelectionUIController>("UIController");
		RaceCampaignSave.RaceTourSaveData saveData;
		if (m_previousSaveState != null && m_previousSaveState.Tour == m_tour)
		{
			saveData = m_previousSaveState;
		}
		else
		{
			m_previousSaveState = null;
			saveData = m_saveData.GetSaveData(m_tour);
		}
		gui.SetCampaign(m_campaign, m_saveData, m_tour, saveData);
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(false, fadeTime, fadeToColour, m_fadeInDelay));
		if (m_previousSaveState != null)
		{
			RaceTour unlockedTour = null;
			if ((m_unlocks & RaceCampaignSave.UnlockFlags.Tour) != 0)
			{
				unlockedTour = m_saveData.GetHighestUnlockedTour();
			}
			gui.PlayFanfare(m_saveData.GetSaveData(m_tour), m_busyIcon, unlockedTour);
		}
		m_previousSaveState = null;
	}

	private void SetSinglePlayerVehicles()
	{
		VehicleFactory.SetStyle(0, Settings.VehicleStyleIndex);
		int num = 0;
		int num2 = 1;
		while (num2 < 4)
		{
			if (num == Settings.VehicleStyleIndex)
			{
				num++;
			}
			VehicleFactory.SetStyle(num2, num);
			num2++;
			num++;
		}
	}

	private IEnumerator RunSinglePlayerRaceCampaign()
	{
		m_race = null;
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(true, m_blankScreenFadeTime, true, m_fadeOutDelay));
		if (m_loadBlankScreen)
		{
			Application.LoadLevel("BlankScene");
			yield return null;
		}
		if (m_saveData != null)
		{
			m_saveData.CloudSync();
		}
		GlobalSave.DoCloudSync();
		Application.LoadLevel("TourSelection");
		Analytics.LogState("TourSelection");
		yield return null;
		m_prevScreen = ScreenType.MainMenu;
		PipeGUI_TourPlateSelection gui = GameObjectUtils.FindComponentWithTag<PipeGUI_TourPlateSelection>("UIController");
		gui.SetCampaign(m_campaign, m_saveData, m_tour);
		gui.PlayFanfare(m_saveData, m_busyIcon);
		SetSinglePlayerVehicles();
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(false, m_blankScreenFadeTime, true, m_fadeInDelay));
	}

	private IEnumerator RunMultiplayerRaceCampaign()
	{
		yield return StartCoroutine(GotoFreeplayPlayerSelection());
	}

	private IEnumerator GotoFreeplayPlayerSelection()
	{
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(true, m_blankScreenFadeTime, true, m_fadeOutDelay));
		if (m_loadBlankScreen)
		{
			Application.LoadLevel("BlankScene");
			yield return null;
		}
		Application.LoadLevel((!UIGamepadManager.IsGamepadActive) ? "FreeplayPlayerSelection" : "FreeplayPlayerSelectionGamepad");
		Analytics.LogState("FreeplayPlayerSelection");
		yield return null;
		m_prevScreen = ScreenType.MainMenu;
		m_nextScreen = ScreenType.FreeplayTrackSelection;
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(false, m_blankScreenFadeTime, true, m_fadeInDelay));
	}

	private IEnumerator GotoFreeplayTrackSelection()
	{
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(true, m_blankScreenFadeTime, true, m_fadeOutDelay));
		if (m_loadBlankScreen)
		{
			Application.LoadLevel("BlankScene");
			yield return null;
		}
		Application.LoadLevel("FreeplayTrackSelection");
		Analytics.LogState("FreeplayTrackSelection");
		yield return null;
		m_prevScreen = ScreenType.FreeplayPlayerSelection;
		m_nextScreen = ScreenType.FreeplayRaceSequence;
		PipeGUI_TournamentTrackSelection gui = GameObjectUtils.FindComponentWithTag<PipeGUI_TournamentTrackSelection>("UIController");
		gui.SetTour(m_campaign.m_tours[0]);
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(false, m_blankScreenFadeTime, true, m_fadeInDelay));
	}

	private IEnumerator StartNewTurnBasedRaceCampaign()
	{
		yield return StartCoroutine(GotoAsyncMatchSelection());
	}

	private IEnumerator GotoAsyncMatchSelection()
	{
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(true, m_blankScreenFadeTime, true, m_fadeOutDelay));
		if (m_loadBlankScreen)
		{
			Application.LoadLevel("BlankScene");
			yield return null;
		}
		Application.LoadLevel("AsyncMatchSelection");
		Analytics.LogState("AsyncMatchSelection");
		yield return null;
		m_prevScreen = ScreenType.MainMenu;
		m_nextScreen = ScreenType.AsyncMatchView;
		PipeGUI_TournamentTrackSelection gui = GameObjectUtils.FindComponentWithTag<PipeGUI_TournamentTrackSelection>("UIController");
		gui.SetTour(m_campaign.m_tours[0]);
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(false, m_blankScreenFadeTime, true, m_fadeInDelay));
	}

	private IEnumerator ResumeTurnBasedRaceCampaign()
	{
		yield return StartCoroutine(GotoAsyncMatchView());
	}

	private IEnumerator GotoAsyncMatchView()
	{
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(true, m_blankScreenFadeTime, true, m_fadeOutDelay));
		if (m_loadBlankScreen)
		{
			Application.LoadLevel("BlankScene");
			yield return null;
		}
		Application.LoadLevel("AsyncMatchView");
		Analytics.LogState("AsyncMatchView");
		yield return null;
		m_prevScreen = ScreenType.MainMenu;
		m_nextScreen = ScreenType.AsyncMatchView;
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(false, m_blankScreenFadeTime, true, m_fadeInDelay));
	}

	private void Cleanup()
	{
		if ((bool)m_saveData && m_saveManager.GetSave(m_campaign) == null)
		{
			m_saveData.UpdateSaveData();
			Object.Destroy(m_saveData);
		}
		m_saveData = null;
		m_campaign = null;
		m_tour = null;
		m_previousSaveState = null;
		m_unlocks = (RaceCampaignSave.UnlockFlags)0;
		m_challenge = null;
		m_raceManager.SetRaceCount(-1, -1);
		m_tournamentScores = null;
		FreeplayRaceSequence = null;
	}

	private IEnumerator GotoTitleScreen()
	{
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(true, m_blankScreenFadeTime, true, m_fadeOutDelay));
		if (m_loadBlankScreen)
		{
			Application.LoadLevel("BlankScene");
			yield return null;
		}
		Application.LoadLevel("TitleScreen");
		Analytics.LogState("TitleScreen");
		yield return null;
		Cleanup();
		RespondToChallenges = true;
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(false, m_blankScreenFadeTime, true, m_fadeInDelay));
	}

	private IEnumerator GotoScreenSimple(string _levelName, bool _useColourFade, float _fadeOutDelay)
	{
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(true, m_blankScreenFadeTime, _useColourFade, _fadeOutDelay));
		if (m_loadBlankScreen)
		{
			Application.LoadLevel("BlankScene");
			yield return null;
		}
		Application.LoadLevel(_levelName);
		Analytics.LogState(_levelName);
		yield return null;
		yield return null;
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(false, m_blankScreenFadeTime, _useColourFade, m_fadeInDelay));
	}

	private IEnumerator GotoScoreChallenge(GameCenterChallenge _challenge, RaceCampaign _campaign, RaceTour _tour, RaceEvent _race)
	{
		yield return StartCoroutine(m_loadScreen.ShowLoadingScreen(true, m_blankScreenFadeTime, true, m_fadeOutDelay));
		if (m_loadBlankScreen)
		{
			Application.LoadLevel("BlankScene");
			yield return null;
		}
		Cleanup();
		SetCampaign(_campaign);
		RespondToChallenges = true;
		m_challenge = _challenge;
		RaceSelected(_tour, _race);
	}

	public void GoForward()
	{
		HandleNextScreen();
	}

	public void GoBack()
	{
		HandlePrevScreen();
	}

	public void GotoUnlockedTour()
	{
		if (m_saveData != null && (m_unlocks & RaceCampaignSave.UnlockFlags.Tour) != 0)
		{
			m_tour = m_saveData.GetHighestUnlockedTour();
		}
		HandlePrevScreen();
	}

	public void TournamentReplaySelected(RaceEvent _race, int _raceIndex, int _raceCount, TournamentScores[] _scores)
	{
		StartCoroutine(RunTournamentReplay(_race, _raceIndex, _raceCount, _scores));
	}

	public void ShowReplay(RaceEvent _race)
	{
		StartCoroutine(RunReplay(_race, RaceEvent.GhostType.Gold));
	}

	public void RaceSelected(RaceEvent _race)
	{
		RaceSelected(_race, false);
	}

	public void RaceSelected(RaceTour _tour, RaceEvent _race)
	{
		m_tour = _tour;
		RaceSelected(_race, false);
	}

	public void RaceSelected(RaceEvent _race, bool _quickStart)
	{
		m_race = _race;
		if (m_campaign == null || m_campaign.m_type == RaceCampaign.Type.SinglePlayer)
		{
			if ((bool)m_saveData)
			{
				m_previousSaveState = m_saveData.GetSaveData(m_tour).Clone();
			}
			m_unlocks = (RaceCampaignSave.UnlockFlags)0;
			m_nextScreen = ScreenType.RaceSelection;
			StartCoroutine(RunSinglePlayerRaceEvent(_race, _quickStart));
		}
		else if (m_campaign.m_type == RaceCampaign.Type.TurnBased)
		{
			m_nextScreen = ScreenType.AsyncMatchView;
			StartCoroutine(RunRaceEvent(_race, _quickStart));
		}
		else if (m_campaign.m_type == RaceCampaign.Type.Multiplayer)
		{
			m_nextScreen = ScreenType.FreeplayTrackSelection;
			m_tour = m_campaign.m_tours[0];
			int freePlayNumPlayers = Settings.FreePlayNumPlayers;
			int freePlayNumLaps = Settings.FreePlayNumLaps;
			int num = (Settings.FreePlayEnableAIs ? (4 - freePlayNumPlayers) : 0);
			int num2 = (1 << freePlayNumPlayers) - 1;
			int aiMask = (1 << freePlayNumPlayers + num) - 1 - num2;
			_race.m_numLaps = freePlayNumLaps;
			_race.m_humanMask = num2;
			_race.m_aiMask = aiMask;
			StartCoroutine(RunRaceEvent(_race, _quickStart));
		}
	}

	public void TourSelected(RaceTour _tour)
	{
		m_tour = _tour;
		if (_tour.m_type == RaceTour.Type.Tutorial && _tour.m_races.Length == 1)
		{
			RaceSelected(_tour.m_races[0]);
			m_nextScreen = ScreenType.TourSelection;
		}
		else
		{
			m_race = null;
			m_nextScreen = ScreenType.RaceSelection;
			StartCoroutine(RunRaceSelection(_tour));
		}
	}

	private void HandleNextScreen()
	{
		HandleScreen(m_nextScreen);
	}

	private void HandlePrevScreen()
	{
		HandleScreen(m_prevScreen);
	}

	private void HandleScreen(ScreenType _type)
	{
		bool flag = true;
		switch (_type)
		{
		case ScreenType.MainMenu:
			StartCoroutine(GotoTitleScreen());
			break;
		case ScreenType.TourSelection:
			StartCoroutine(RunSinglePlayerRaceCampaign());
			break;
		case ScreenType.FreeplayPlayerSelection:
			StartCoroutine(GotoFreeplayPlayerSelection());
			break;
		case ScreenType.FreeplayTrackSelection:
			StartCoroutine(GotoFreeplayTrackSelection());
			break;
		case ScreenType.FreeplayRaceSequence:
			StartCoroutine(RunFreeplayRaceSequence());
			break;
		case ScreenType.AsyncMatchSelection:
			StartCoroutine(GotoAsyncMatchSelection());
			break;
		case ScreenType.AsyncMatchView:
			StartCoroutine(GotoAsyncMatchView());
			break;
		default:
			StartCoroutine(RunRaceSelection(m_tour));
			break;
		case ScreenType.None:
			break;
		}
		if (flag)
		{
			StartMusic();
		}
		FlushSocialUpdates();
	}

	private void StartMusic()
	{
		if (m_musicPlayerGO != null)
		{
			m_musicPlayerGO.BroadcastMessage("TryTransitionToPlaylist", m_menuPlaylist);
		}
	}

	private void FlushSocialUpdates()
	{
		SocialManager component = GetComponent<SocialManager>();
		if (!(component == null))
		{
			component.FlushUpdates();
		}
	}

	public void UpdateTourAchievements()
	{
		UpdateTourAchievementsForTour(m_tour);
	}

	public void UpdateTourAchievementsForTour(RaceTour _tour)
	{
		SocialManager component = GetComponent<SocialManager>();
		if (_tour == null)
		{
			return;
		}
		RaceCampaignSave.RaceTourSaveData saveData = m_saveData.GetSaveData(_tour);
		if (saveData == null)
		{
			return;
		}
		RaceCampaignSave.AwardType award = saveData.Award;
		if (award >= RaceCampaignSave.AwardType.Bronze && _tour.m_allCompleteAchievement != null)
		{
			if (component != null)
			{
				component.UnlockAchievement(_tour.m_allCompleteAchievement, false);
			}
			Analytics.LogEvent("TourCompleted", "name", _tour.name, "displayName", _tour.DisplayName);
		}
		if (award >= RaceCampaignSave.AwardType.Gold && _tour.m_allMasteredAchievement != null)
		{
			if (component != null)
			{
				component.UnlockAchievement(_tour.m_allMasteredAchievement, false);
			}
			Analytics.LogEvent("TourMastered", "name", _tour.name, "displayName", _tour.DisplayName);
		}
	}

	public void OnUpdateTotalNuts(long _numNuts)
	{
		if (!(m_saveData == null) && m_saveData.ContributesTotalNuts)
		{
			m_saveManager.UpdateTotalNuts(_numNuts);
		}
	}
}
