using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/In Race Pause")]
public class InRacePause : MonoBehaviour
{
	private GameObject m_raceControl;

	private RaceManager m_raceManager;

	private bool m_enableRestart = true;

	public GameObject m_pauseButton;

	public GameObject m_restartButton;

	public UIBistateInteractivePanel m_optionsPanel;

	public float m_delayFromPause;

	public bool PauseEnabled
	{
		get
		{
			return m_pauseButton.active;
		}
		set
		{
			Screen.sleepTimeout = ((!value) ? (-2) : (-1));
			if ((bool)m_pauseButton)
			{
				m_pauseButton.active = value;
			}
			if ((bool)m_restartButton)
			{
				m_restartButton.active = value && RestartEnabled;
			}
		}
	}

	public bool RestartEnabled
	{
		get
		{
			return m_enableRestart;
		}
		set
		{
			m_enableRestart = value;
			m_restartButton.active = value;
		}
	}

	public static bool HidePauseMenuOptions { private get; set; }

	private void Start()
	{
		m_raceControl = GameObject.FindGameObjectWithTag("RaceControl");
		if (m_raceControl != null)
		{
			m_raceManager = m_raceControl.GetComponent<RaceManager>();
			if (m_raceManager != null)
			{
				m_raceManager.OnAllowPause += OnAllowPause;
				m_raceManager.OnRacePrepared += OnRacePrepared;
			}
		}
		m_optionsPanel.gameObject.SetActiveRecursively(false);
		PauseEnabled = false;
		if (HidePauseMenuOptions)
		{
			Invoke("HideOptions", 0.25f);
		}
	}

	private void OnDestroy()
	{
		PauseEnabled = false;
		if (m_raceManager != null)
		{
			m_raceManager.OnAllowPause -= OnAllowPause;
			m_raceManager.OnRacePrepared -= OnRacePrepared;
		}
	}

	private void OnApplicationPause(bool _pause)
	{
		if (_pause)
		{
			if (m_raceManager.CurrentRace.ReturnToMainMenuOnSuspend)
			{
				m_raceControl.BroadcastMessage("UIEvent_GotoMainMenu", SendMessageOptions.DontRequireReceiver);
			}
			else if (PauseEnabled)
			{
				OnPauseGame();
			}
		}
	}

	private void OnAllowPause(bool _enable)
	{
		PauseEnabled = _enable;
	}

	private void OnRacePrepared()
	{
		if (m_raceManager.NumHumans > 1 || m_raceManager.CurrentRace.NoMidRaceRestart)
		{
			RestartEnabled = false;
			Vector3 localPosition = m_pauseButton.transform.localPosition;
			localPosition.x = 0f;
			m_pauseButton.transform.localPosition = localPosition;
		}
	}

	private void OnPauseGame()
	{
		m_optionsPanel.Reveal();
		TimeEx.RequestPause(PauseLevel.Game);
		PauseEnabled = false;
	}

	private void OnRestartGame()
	{
		if (m_raceControl != null)
		{
			PauseEnabled = false;
			m_raceControl.BroadcastMessage("UIEvent_RestartRace", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void Unpause()
	{
		TimeEx.RequestUnPause(PauseLevel.Game);
		PauseEnabled = true;
	}

	private void OnResumeSelected()
	{
		m_optionsPanel.Hide();
		StartCoroutine(DelayedRaceControlBroadcast("UIEvent_ResumeRace"));
	}

	private void OnLeaveRaceSelected()
	{
		m_optionsPanel.Hide();
		StartCoroutine(DelayedRaceControlBroadcast("UIEvent_LeaveRace"));
	}

	private void OnMainMenuSelected()
	{
		m_optionsPanel.Hide();
		StartCoroutine(DelayedRaceControlBroadcast("UIEvent_GotoMainMenu"));
	}

	private void OnHelpSelected()
	{
		Application.OpenURL(AppSettings.FAQURL);
	}

	private IEnumerator DelayedRaceControlBroadcast(string _methodName)
	{
		float startTime = 0f;
		if (TimeEx.IsAvailable)
		{
			for (; startTime < m_delayFromPause; startTime += TimeEx.DeltaTime(PauseLevel.Game))
			{
				yield return null;
			}
		}
		Unpause();
		if (m_raceControl != null)
		{
			m_raceControl.BroadcastMessage(_methodName, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void HideOptions()
	{
		m_optionsPanel.gameObject.SetActiveRecursively(true);
		UIButton[] componentsInChildren = m_optionsPanel.GetComponentsInChildren<UIButton>();
		if (componentsInChildren != null)
		{
			UIButton[] array = componentsInChildren;
			foreach (UIButton uIButton in array)
			{
				uIButton.Color = new Color(1f, 1f, 1f, 0f);
				uIButton.transform.localScale = uIButton.transform.localScale * 1.35f;
				SpriteText componentInChildren = uIButton.GetComponentInChildren<SpriteText>();
				Object.DestroyObject(componentInChildren.gameObject);
			}
		}
	}
}
