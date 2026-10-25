using System.Collections;
using UnityEngine;

public class PipeGUI_RacePlateSelection : MonoBehaviour
{
	private AudioSource m_audioSource;

	private RaceTour m_tour;

	private RaceCampaignSave.RaceTourSaveData m_saveData;

	private GameManager m_gameManager;

	private bool m_hasRefreshed;

	public PipeGUI_TrackPlateWidget[] m_plates;

	public AudioBank m_lockedAudioBank;

	public AudioBank m_raceAudioBank;

	public PipeGUI_InstructorSpeech m_raceInfo;

	public PipeGUI_InstructorSpeech m_newTourInfo;

	public PipeGUI_Instructor m_instructor;

	public UIButton m_leaderboardButton;

	public SpriteText m_tourName;

	public SpriteText m_raceName;

	public SpriteText m_raceDescription;

	public SpriteText m_raceTime;

	public GameObject m_raceTimeLabel;

	public SpriteText m_lapTime;

	public GameObject m_lapTimeLabel;

	public PipeGUI_InstructorIAP m_IAP;

	public RaceTour m_testTour;

	public RaceCampaign m_testCampaign;

	public bool m_testFanfare;

	private bool EnableInput
	{
		set
		{
			for (int i = 0; i < m_plates.Length; i++)
			{
				PipeGUI_TrackPlateWidget pipeGUI_TrackPlateWidget = m_plates[i];
				if (pipeGUI_TrackPlateWidget != null)
				{
					pipeGUI_TrackPlateWidget.EnableInput = value;
				}
			}
		}
	}

	public static LeaderboardInfo Leaderboard { get; private set; }

	private bool Standalone
	{
		get
		{
			return m_gameManager == null;
		}
	}

	public RaceTour Tour
	{
		get
		{
			return m_tour;
		}
	}

	private void Awake()
	{
		m_audioSource = base.gameObject.AddComponent<AudioSource>();
		m_audioSource.rolloffMode = AudioRolloffMode.Linear;
		m_audioSource.minDistance = 0f;
		m_audioSource.maxDistance = 200f;
	}

	private void Start()
	{
		m_gameManager = GameObjectUtils.FindComponentWithTag<GameManager>("RaceControl");
		if (Standalone && m_testTour != null)
		{
			RaceCampaignSave.RaceTourSaveData saveData = null;
			if (m_testCampaign != null)
			{
				RaceCampaignSave raceCampaignSave = base.gameObject.AddComponent<RaceCampaignSave>();
				raceCampaignSave.SetCampaign(m_testCampaign, null);
				saveData = raceCampaignSave.GetSaveData(m_testTour);
			}
			SetTour(m_testTour, saveData);
			if (m_testFanfare)
			{
				PlayFanfare(saveData, null, m_testTour);
			}
		}
	}

	private void OnEnable()
	{
		Refresh();
	}

	public void SetTour(RaceTour _tour, RaceCampaignSave.RaceTourSaveData _saveData)
	{
		m_tour = _tour;
		m_saveData = _saveData;
		if (base.gameObject.active)
		{
			Refresh();
		}
	}

	private void Refresh()
	{
		if (m_tour == null)
		{
			return;
		}
		for (int i = 0; i < m_plates.Length; i++)
		{
			PipeGUI_TrackPlateWidget pipeGUI_TrackPlateWidget = m_plates[i];
			if (!(pipeGUI_TrackPlateWidget != null))
			{
				continue;
			}
			if (i < m_tour.m_races.Length)
			{
				pipeGUI_TrackPlateWidget.gameObject.SetActiveRecursively(true);
				RaceCampaignSave.RaceEventSaveData saveData = ((m_saveData == null) ? null : m_saveData.GetSaveData(m_tour.m_races[i]));
				pipeGUI_TrackPlateWidget.SetRaceData(m_tour.m_races[i], saveData);
				if (!m_hasRefreshed)
				{
					pipeGUI_TrackPlateWidget.OnRaceSelectedEvent += OnRaceSelected;
					pipeGUI_TrackPlateWidget.OnInformationSelectedEvent += OnInformationSelected;
				}
			}
			else
			{
				pipeGUI_TrackPlateWidget.gameObject.SetActiveRecursively(false);
			}
		}
		if (m_tourName != null)
		{
			m_tourName.Text = m_tour.DisplayName.ToUpper();
		}
		m_hasRefreshed = true;
	}

	public void OnRaceSelected(RaceEvent _race, RaceCampaignSave.RaceEventSaveData _saveData)
	{
		if (_saveData != null && _saveData.Locked)
		{
			if (m_lockedAudioBank != null)
			{
				m_lockedAudioBank.PlayRandomClip(m_audioSource);
			}
			StartCoroutine(DisplayLocked(_race, _saveData));
			return;
		}
		if (m_raceAudioBank != null)
		{
			m_raceAudioBank.PlayRandomClip(m_audioSource);
		}
		if (m_gameManager != null)
		{
			StartCoroutine(RunRace(m_gameManager, _race));
			m_gameManager = null;
			UIManager.instance.enabled = false;
		}
	}

	public void OnInformationSelected(RaceEvent _race, RaceCampaignSave.RaceEventSaveData _saveData)
	{
		if (_saveData != null && _saveData.Locked)
		{
			StartCoroutine(DisplayLocked(_race, _saveData));
		}
		else
		{
			StartCoroutine(DisplayInformation(_race, _saveData));
		}
	}

	private IEnumerator DisplayInformation(RaceEvent _race, RaceCampaignSave.RaceEventSaveData _saveData)
	{
		EnableInput = false;
		yield return YieldCache.WaitForSeconds(0.25f);
		string raceName = _race.DisplayName;
		string description = _race.GetRaceTypeText();
		string raceTime = string.Empty;
		string lapTime = string.Empty;
		if (_saveData != null)
		{
			float bestTime = ((_saveData == null) ? RaceCampaignSave.MaxTime : _saveData.BestTotalTime);
			float bestLapTime = ((_saveData == null) ? RaceCampaignSave.MaxTime : _saveData.BestLapTime);
			raceTime = UIUtils.FormatBestRaceTime(bestTime);
			lapTime = UIUtils.FormatBestLapTime(bestLapTime);
		}
		m_raceName.Text = raceName;
		m_raceTime.Text = raceTime;
		m_lapTime.Text = lapTime;
		m_raceTimeLabel.active = raceTime.Length > 0;
		m_lapTimeLabel.active = lapTime.Length > 0;
		m_raceDescription.Text = description;
		m_instructor.SetTalkingStance();
		m_instructor.PlayTalkAnim();
		Leaderboard = _race.m_leaderboard;
		bool isPlayerAuthenticated = false;
		bool isGameCenterSupported = false;
		if (m_gameManager != null)
		{
			SocialManager socialManager = m_gameManager.GetComponent<SocialManager>();
			if (socialManager != null)
			{
				isPlayerAuthenticated = socialManager.IsPlayerAuthenticated;
				isGameCenterSupported = socialManager.IsGameCenterSupported || socialManager.IsGoogleGameServicesSupported;
			}
		}
		bool showLeaderboardButton = isPlayerAuthenticated && isGameCenterSupported && _race.m_leaderboard != null;
		yield return StartCoroutine(m_raceInfo.DisplayAndWait(string.Empty, Language.Get("ID_TapToClose"), showLeaderboardButton));
		Leaderboard = null;
		m_instructor.SetDefaultStance();
		EnableInput = true;
	}

	private IEnumerator DisplayLocked(RaceEvent _race, RaceCampaignSave.RaceEventSaveData _saveData)
	{
		EnableInput = false;
		yield return YieldCache.WaitForSeconds(0.25f);
		string messageForIAP = string.Empty;
		string messageForNoIAP = string.Empty;
		if (_saveData != null && _saveData.LockReason == RaceCampaign.EventUnlockType.LockLast)
		{
			messageForIAP = Language.Get("ID_Description_UnlockLockLastIAP");
			messageForNoIAP = Language.Get("ID_Description_UnlockLockLast");
		}
		else
		{
			messageForIAP = string.Format(Language.Get("ID_Description_LockedTourIAP"), m_tour.m_pointsToUnlock);
			messageForNoIAP = string.Format(Language.Get("ID_Description_LockedTour"), m_tour.m_pointsToUnlock);
		}
		m_instructor.SetTalkingStance();
		m_instructor.PlayTalkAnim();
		yield return StartCoroutine(m_IAP.DisplayAndWait(messageForIAP, messageForNoIAP));
		m_instructor.SetDefaultStance();
		EnableInput = true;
	}

	private IEnumerator RunRace(GameManager _gameManager, RaceEvent _race)
	{
		yield return YieldCache.WaitForHalfSecond;
		_gameManager.RaceSelected(m_tour, _race);
	}

	public void PlayFanfare(RaceCampaignSave.RaceTourSaveData _saveData, BusyIcon _busyIcon, RaceTour _unlockedTour)
	{
		StartCoroutine(PlayFanfareImpl(_saveData, _busyIcon, _unlockedTour));
	}

	public IEnumerator PlayFanfareImpl(RaceCampaignSave.RaceTourSaveData _saveData, BusyIcon _busyIcon, RaceTour _unlockedTour)
	{
		EnableInput = false;
		if (_busyIcon != null)
		{
			_busyIcon.ShowIcon();
		}
		yield return YieldCache.WaitForHalfSecond;
		for (int index = 0; index < m_plates.Length; index++)
		{
			PipeGUI_TrackPlateWidget widget = m_plates[index].gameObject.GetComponent<PipeGUI_TrackPlateWidget>();
			if (!(widget != null) || index >= m_tour.m_races.Length)
			{
				continue;
			}
			RaceCampaignSave.RaceEventSaveData oldSaveData = ((m_saveData == null) ? null : m_saveData.GetSaveData(m_tour.m_races[index]));
			RaceCampaignSave.RaceEventSaveData newSaveData = ((_saveData == null) ? null : _saveData.GetSaveData(m_tour.m_races[index]));
			if (oldSaveData != null && newSaveData != null)
			{
				if (widget.NeedsFanfare(newSaveData) || m_testFanfare)
				{
					yield return StartCoroutine(widget.PlayFanfare((!m_testFanfare) ? newSaveData : null));
				}
				else
				{
					widget.SetRaceData(newSaveData.Race, newSaveData);
				}
			}
		}
		m_saveData = _saveData;
		if (_busyIcon != null)
		{
			_busyIcon.HideIcon();
		}
		if (_unlockedTour != null)
		{
			yield return YieldCache.WaitForHalfSecond;
			yield return StartCoroutine(m_newTourInfo.DisplayAndWait(Language.Get("ID_NewTourUnlocked"), Language.Get("ID_TapToClose")));
		}
		EnableInput = true;
	}

	private void OnBackSelected()
	{
		if (m_gameManager != null)
		{
			m_gameManager.GoBack();
			m_gameManager = null;
		}
	}

	private void OnJumpToTour()
	{
		if (m_gameManager != null)
		{
			m_gameManager.GotoUnlockedTour();
			m_gameManager = null;
		}
	}
}
