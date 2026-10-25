using System.Collections;
using UnityEngine;

public class PipeGUI_RaceHelp_Grades : MonoBehaviour
{
	private RaceManager m_raceManager;

	private bool m_suppressDeactivateOnStart;

	public PipeGUI_InstructorSpeech m_speech;

	public UIBistateInteractivePanel m_panel;

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
	}

	private void OnSpeechStateChange(PipeGUI_InstructorSpeech.State _state)
	{
		if (m_raceManager.CurrentChallenge == null || m_raceManager.CurrentChallenge.score == null)
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
