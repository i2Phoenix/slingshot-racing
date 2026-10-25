using UnityEngine;

public class PipeGUI_EndRaceOptions : MonoBehaviour
{
	private GameObject m_raceControl;

	private RaceManager m_raceManager;

	public InRacePause m_pause;

	public UIBistateInteractivePanel m_panel;

	public UIBistateInteractivePanel m_replayPanel;

	public float m_messageDelay;

	public GameObject m_replayButton;

	public GameObject m_showReplayBusy;

	public UIButton m_showReplayButton;

	private void Start()
	{
		m_raceControl = GameObject.FindGameObjectWithTag("RaceControl");
		if (m_raceControl != null)
		{
			m_raceManager = m_raceControl.GetComponent<RaceManager>();
			m_raceManager.OnDisplayEndRaceOptions += OnDisplayOptions;
			m_raceManager.OnDisplayReplayOptions += OnDisplayReplayOptions;
			m_raceManager.OnReplayDataReady += OnReplayDataReady;
		}
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnDisplayEndRaceOptions -= OnDisplayOptions;
			m_raceManager.OnDisplayReplayOptions -= OnDisplayReplayOptions;
			m_raceManager.OnReplayDataReady -= OnReplayDataReady;
		}
	}

	public void OnDisplayOptions(bool _display)
	{
		if (_display)
		{
			m_panel.Reveal();
			if (!m_raceManager.CurrentRace.NoWatchReplay)
			{
				m_replayPanel.Reveal();
			}
			m_showReplayButton.controlIsEnabled = false;
		}
		else
		{
			m_panel.Hide();
			m_replayPanel.Hide();
		}
		if (m_raceManager != null && m_raceManager.CurrentRace.m_replaysRemaining == 0 && m_replayButton != null)
		{
			m_replayButton.SetActiveRecursively(false);
		}
	}

	public void OnDisplayReplayOptions(bool _display)
	{
		if (!_display)
		{
			m_showReplayButton.controlIsEnabled = false;
		}
	}

	private void OnReplayDataReady()
	{
		m_showReplayBusy.SetActiveRecursively(false);
		m_showReplayButton.controlIsEnabled = true;
	}

	private void OnReplaySelected()
	{
		if (m_raceControl != null)
		{
			m_raceControl.BroadcastMessage("UIEvent_Replay", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnShowReplaySelected()
	{
		if (m_raceControl != null)
		{
			m_raceControl.BroadcastMessage("UIEvent_ShowReplay", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnContinueSelected()
	{
		if (m_raceControl != null)
		{
			m_raceControl.BroadcastMessage("UIEvent_NextScreen", SendMessageOptions.DontRequireReceiver);
		}
	}
}
