using UnityEngine;

public class PipeGUI_InstructorSummary : MonoBehaviour
{
	public PipeGUI_InstructorSpeech m_speech;

	private void Reveal()
	{
		if (m_speech != null)
		{
			StartCoroutine(m_speech.DisplayAndWait(string.Empty, "Tap to close"));
		}
	}
}
