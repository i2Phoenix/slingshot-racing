using UnityEngine;

[AddComponentMenu("GUI/Debug Options GUI")]
public class DebugOptionsGUI : MonoBehaviour
{
	private GameManager m_gameManager;

	private TurnBasedMatchManager m_turnBasedMatchManager;

	public GameObject m_vehicleSelect;

	public UIStateToggleBtn m_dynamicCamera;

	public UIStateToggleBtn m_cameraTrackAI;

	public UIStateToggleBtn m_devFreeplay;

	public UISlider m_soundVolume;

	public UISlider m_musicVolume;

	public void Start()
	{
		m_dynamicCamera.SetToggleState(Settings.DynamicCamera ? 1 : 0);
		m_cameraTrackAI.SetToggleState(Settings.DynamicCameraTrackAI ? 1 : 0);
		m_devFreeplay.SetToggleState(Settings.DevFreeplay ? 1 : 0);
		bool flag = false;
		string vehicleType = Settings.VehicleType;
		GameObject gameObject = GameObject.Find("UIControllerVehicleSelect");
		if (gameObject != null)
		{
			int childCount = gameObject.transform.childCount;
			for (int i = 0; i != childCount; i++)
			{
				GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
				UIRadioBtn component = gameObject2.GetComponent<UIRadioBtn>();
				UITextField componentInChildren = gameObject2.GetComponentInChildren<UITextField>();
				if (componentInChildren == null)
				{
					if (!flag && component.Text == vehicleType)
					{
						flag = true;
						component.Value = true;
					}
				}
				else
				{
					if (!flag)
					{
						component.Value = true;
					}
					componentInChildren.Text = vehicleType;
				}
			}
		}
		if (m_soundVolume != null)
		{
			float soundVolume = Settings.SoundVolume;
			m_soundVolume.defaultValue = soundVolume;
			m_soundVolume.Value = soundVolume;
		}
		if (m_musicVolume != null)
		{
			float musicVolume = Settings.MusicVolume;
			m_musicVolume.defaultValue = musicVolume;
			m_musicVolume.Value = musicVolume;
		}
		GameObject gameObject3 = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject3 != null)
		{
			m_gameManager = gameObject3.GetComponent<GameManager>();
			m_turnBasedMatchManager = gameObject3.GetComponent<TurnBasedMatchManager>();
		}
		if (m_turnBasedMatchManager != null)
		{
			m_turnBasedMatchManager.OnGotoMatchEvent += OnGotoMatch;
		}
	}

	private void OnDestroy()
	{
		if (m_turnBasedMatchManager != null)
		{
			m_turnBasedMatchManager.OnGotoMatchEvent -= OnGotoMatch;
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

	private void OnToggleDynamicCamera()
	{
		Settings.DynamicCamera = m_dynamicCamera.StateNum == 1;
	}

	private void OnToggleCameraTrackAI()
	{
		Settings.DynamicCameraTrackAI = m_cameraTrackAI.StateNum == 1;
	}

	private void OnToggleDevFreeplay()
	{
		Settings.DevFreeplay = m_devFreeplay.StateNum == 1;
	}

	private void OnRunSoakTest()
	{
		Application.LoadLevel("SoakTest");
	}

	private void OnSelectVehicle()
	{
		GameObject gameObject = GameObject.Find("UIControllerVehicleSelect");
		if (!(gameObject != null))
		{
			return;
		}
		int childCount = gameObject.transform.childCount;
		for (int i = 0; i != childCount; i++)
		{
			GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
			UIRadioBtn component = gameObject2.GetComponent<UIRadioBtn>();
			if ((bool)component && component.Value)
			{
				UITextField componentInChildren = gameObject2.GetComponentInChildren<UITextField>();
				if (componentInChildren != null)
				{
					Settings.VehicleType = componentInChildren.Content;
				}
				else
				{
					Settings.VehicleType = component.Text;
				}
				break;
			}
		}
	}

	private void OnDebugMenuSelected()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("RaceControl");
		if (array != null)
		{
			GameObject[] array2 = array;
			foreach (GameObject obj in array2)
			{
				Object.Destroy(obj);
			}
		}
		Application.LoadLevel("Menu");
	}

	private void OnDisableLocksSelected()
	{
		RaceCampaignSave.DisableAllLocks();
	}

	private void OnUpdateSoundVolume()
	{
		Settings.SoundVolume = m_soundVolume.Value;
	}

	private void OnUpdateMusicVolume()
	{
		Settings.MusicVolume = m_musicVolume.Value;
	}

	private void OnResetAchievements()
	{
		SocialManager socialManager = GameObjectUtils.FindComponentWithTag<SocialManager>("RaceControl");
		if (socialManager != null)
		{
			socialManager.ResetAllAchievements();
		}
	}

	private void OnResetReview()
	{
		RateAppPrompt.Reset();
	}

	private void OnRestoreTransactions()
	{
		InAppPurchaseManager get = InAppPurchaseManager.Get;
		if (get != null)
		{
			get.RestoreCompletedTransactions(null);
		}
	}

	private void OnEasyCaptureSelected()
	{
		RaceCampaignSave.DisableAllLocks();
		InRacePause.HidePauseMenuOptions = true;
		GameObject gameObject = GameObject.Find("__FPSDisplay");
		if (gameObject != null)
		{
			Object.Destroy(gameObject);
		}
		TitleScreenGUI.SuppressFPS = true;
	}

	private void OnAsyncMultiplayerTest()
	{
		if (m_turnBasedMatchManager.IsSupported)
		{
			Debug.Log("Async: Finding match");
			m_turnBasedMatchManager.FindMatch();
		}
	}

	private void OnGotoMatch()
	{
		Debug.Log("Multiplay: Started new match");
		if (m_turnBasedMatchManager.CurrentMatchData.NumRaces == 0 && m_turnBasedMatchManager.IsCurrentPlayersTurn())
		{
			Application.LoadLevel("AsyncLevelSelectTest");
		}
		else
		{
			Application.LoadLevel("AsyncMatchViewTest");
		}
	}
}
