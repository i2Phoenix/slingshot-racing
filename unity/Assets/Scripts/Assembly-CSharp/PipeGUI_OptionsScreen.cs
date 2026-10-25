using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/PipeGUI/Options Screen")]
[RequireComponent(typeof(AudioSource))]
public class PipeGUI_OptionsScreen : MonoBehaviour
{
	private const float c_audioLoopTimeout = 0.5f;

	private GameManager m_gameManager;

	private AudioSource m_audio;

	private bool m_insideIAPBlock;

	public UISlider m_soundVolume;

	public UISlider m_musicVolume;

	public UIStateToggleBtn m_inRaceMusic;

	public UIStateToggleBtn m_playerMarker;

	public UIStateToggleBtn m_playerColour;

	public UIStateToggleBtn m_flipRaceGUI;

	public UIStateToggleBtn m_movieCapture;

	public PipeGUI_InstructorYesNo m_confirmationDialog;

	public LanguageCode[] m_languages;

	public SpriteText m_languageText;

	[SerializeField]
	private GameObject m_restoreIAPButton;

	[SerializeField]
	private GameObject m_debugButton;

	[SerializeField]
	private GameObject m_kamcordPrefab;

	private IEnumerator Start()
	{
		float soundVolume = Settings.SoundVolume;
		m_soundVolume.defaultValue = soundVolume;
		m_soundVolume.Value = soundVolume;
		float musicVolume = Settings.MusicVolume;
		m_musicVolume.defaultValue = musicVolume;
		m_musicVolume.Value = musicVolume;
		m_inRaceMusic.SetToggleState(Settings.InRaceMusic ? 1 : 0);
		if (m_playerMarker != null)
		{
			m_playerMarker.SetToggleState(Settings.HighlightPlayer ? 1 : 0);
		}
		if (m_playerColour != null)
		{
			m_playerColour.SetToggleState(Settings.PlayerColour);
		}
		if (m_flipRaceGUI != null)
		{
			m_flipRaceGUI.SetToggleState(Settings.RaceButtonOnRight ? 1 : 0);
		}
		if (m_movieCapture != null)
		{
			if (KamcordGameSettings.CanBeEnabled)
			{
				m_movieCapture.SetToggleState(Settings.KamcordEnabled ? 1 : 0);
			}
			else
			{
				Object.Destroy(m_movieCapture.transform.parent.gameObject);
				m_movieCapture = null;
			}
		}
		GameObject raceControl = GameObject.FindGameObjectWithTag("RaceControl");
		if (raceControl != null)
		{
			m_gameManager = raceControl.GetComponent<GameManager>();
		}
		GameObject go = GameObject.Find("PersistentGUI");
		if (go != null)
		{
			PersistentGUI gui = go.GetComponent<PersistentGUI>();
			if (!(gui != null))
			{
			}
		}
		if (m_restoreIAPButton != null)
		{
			m_restoreIAPButton.SetActiveRecursively(false);
		}
		if (m_debugButton != null)
		{
			m_debugButton.SetActiveRecursively(false);
		}
		yield return null;
		m_audio = base.GetComponent<AudioSource>();
	}

	private void OnResetGameSelected()
	{
		StartCoroutine(m_confirmationDialog.DisplayAndWait(Language.Get("ID_ConfirmReset"), OnConfirmResetGame, null));
	}

	private void OnConfirmResetGame(Object _dialog)
	{
		SaveManager saveManager = SaveManager.Find();
		if (saveManager != null)
		{
			saveManager.ResetSaveData();
		}
	}

	private void OnCreditsSelected()
	{
		if (m_gameManager != null)
		{
			m_gameManager.OnCreditsSelected();
			m_gameManager = null;
		}
	}

	private void OnLanguageSelected()
	{
		LanguageCode languageCode = Language.CurrentLanguage();
		for (int i = 0; i < m_languages.Length; i++)
		{
			if (languageCode == m_languages[i])
			{
				if (++i >= m_languages.Length)
				{
					i = 0;
				}
				Language.SwitchLanguage(m_languages[i]);
				PlayerPrefs.Save();
				CancelInvoke("ReloadLevel");
				Invoke("ReloadLevel", 0.25f);
				break;
			}
		}
	}

	private void ReloadLevel()
	{
		if (m_gameManager != null)
		{
			m_gameManager.OnOptionsSelected();
		}
		else
		{
			Application.LoadLevel("OptionsScreen");
		}
	}

	private void OnBackSelected()
	{
		if (m_gameManager != null)
		{
			m_gameManager.OnOptionsReturnToMainMenu();
			m_gameManager = null;
		}
	}

	private void OnDebugOptionsSelected()
	{
	}

	private void OnDisableLocksSelected()
	{
		RaceCampaignSave.DisableAllLocks();
	}

	private void OnUpdateSoundVolume()
	{
		Settings.SoundVolume = m_soundVolume.Value;
		AudioListener.volume = m_soundVolume.Value;
		if (m_audio != null)
		{
			m_audio.loop = true;
			MonoBehaviourUtils.ReInvoke(this, "StopAudioLooping", 0.5f);
			if (!m_audio.isPlaying)
			{
				m_audio.Play();
			}
		}
	}

	private void StopAudioLooping()
	{
		if (m_audio != null)
		{
			m_audio.loop = false;
		}
	}

	private void OnUpdateMusicVolume()
	{
		Settings.MusicVolume = m_musicVolume.Value;
	}

	private void OnUpdateInRaceMusicToggle()
	{
		Settings.InRaceMusic = m_inRaceMusic.StateNum == 1;
	}

	private void OnUpdatePlayerMarkerToggle()
	{
		Settings.HighlightPlayer = m_playerMarker.StateNum == 1;
	}

	private void OnUpdateFlipRaceGUIToggle()
	{
		Settings.RaceButtonOnRight = m_flipRaceGUI.StateNum == 1;
	}

	private void OnUpdatePlayerColour()
	{
		Settings.PlayerColour = m_playerColour.StateNum;
	}

	private void OnUpdateMovieCaptureToggle()
	{
		Settings.KamcordEnabled = m_movieCapture.StateNum == 1;
		StartCoroutine(ManageKamcord());
	}

	private IEnumerator ManageKamcord()
	{
		GameObject kamcordObject = GameObject.Find("KamcordPrefab");
		if (Settings.KamcordEnabled)
		{
			if (kamcordObject == null)
			{
				KamcordGameSettings.Init();
				ScreenSettings.Apply();
				yield return null;
				yield return null;
				Object.Instantiate(m_kamcordPrefab);
			}
		}
		else if (kamcordObject != null)
		{
			Object.Destroy(kamcordObject);
			yield return null;
			yield return null;
			KamcordGameSettings.Init();
			ScreenSettings.Apply();
		}
	}

	private void OnRestoreIAP()
	{
		InAppPurchaseManager get = InAppPurchaseManager.Get;
		if (!(get != null))
		{
		}
	}

	private void OnRestoreIAPComplete(bool _success)
	{
	}
}
