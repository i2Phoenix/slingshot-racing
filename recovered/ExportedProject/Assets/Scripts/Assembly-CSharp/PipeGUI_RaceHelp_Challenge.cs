using System;
using System.Collections;
using UnityEngine;

public class PipeGUI_RaceHelp_Challenge : MonoBehaviour
{
	private RaceManager m_raceManager;

	private GameCenterChallenge m_challenge;

	private bool m_suppressDeactivateOnStart;

	public PipeGUI_InstructorSpeech m_speech;

	public UIBistateInteractivePanel m_panel;

	public SpriteText m_challengeTime;

	private IEnumerator Start()
	{
		if (m_speech != null)
		{
			m_speech.OnStateChange += OnSpeechStateChange;
		}
		GameObject raceControl = GameObject.FindGameObjectWithTag("RaceControl");
		if (raceControl != null)
		{
			m_raceManager = raceControl.GetComponent<RaceManager>();
			if (m_raceManager != null)
			{
				m_raceManager.OnRacePrepared += OnRacePrepared;
			}
		}
		yield return null;
		if (!m_suppressDeactivateOnStart)
		{
			base.gameObject.SetActiveRecursively(false);
		}
	}

	private void OnDestroy()
	{
		if (m_speech != null)
		{
			m_speech.OnStateChange -= OnSpeechStateChange;
		}
		if (m_raceManager != null)
		{
			m_raceManager.OnRacePrepared -= OnRacePrepared;
		}
	}

	private void OnRacePrepared()
	{
		GameCenterChallenge currentChallenge = m_raceManager.CurrentChallenge;
		if (currentChallenge != null && currentChallenge.score != null)
		{
			m_challenge = currentChallenge;
			float num = (float)m_challenge.score.value / 1000f;
			TimeSpan timeSpan = TimeSpan.FromSeconds(num);
			m_challengeTime.Text = string.Format("{0:D1}:{1:D2}.{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
		}
		else
		{
			m_challenge = null;
		}
	}

	private void OnSpeechStateChange(PipeGUI_InstructorSpeech.State _state)
	{
		if (m_raceManager.CurrentChallenge != null && m_raceManager.CurrentChallenge.score != null)
		{
			if (_state == PipeGUI_InstructorSpeech.State.Show)
			{
				m_suppressDeactivateOnStart = true;
				m_panel.gameObject.active = true;
				m_panel.SendMessage("BringIn");
			}
			else if (m_panel.gameObject.active)
			{
				m_panel.SendMessage("Dismiss");
			}
		}
	}
}
