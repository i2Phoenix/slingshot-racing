using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/In Race Help")]
public class InRaceHelp : MonoBehaviour
{
	public PipeGUI_InstructorSpeech m_instructorSpeech;

	public PipeGUI_RaceHelp m_help;

	public IEnumerator DisplayHelp(RaceEvent _race)
	{
		if ((bool)m_help)
		{
			m_help.SetRace(_race);
		}
		if (m_instructorSpeech != null)
		{
			yield return StartCoroutine(m_instructorSpeech.DisplayAndWait(string.Empty, Language.Get("ID_TapToRace")));
		}
	}

	public void HideImmediate()
	{
		StopAllCoroutines();
		if (m_instructorSpeech != null)
		{
			m_instructorSpeech.HideImmediate();
		}
	}
}
