using UnityEngine;

public class PipeGUI_RaceHelp : MonoBehaviour
{
	public SpriteText m_raceText;

	public SpriteText m_hintText;

	public SpriteText[] m_objectives;

	public void SetRace(RaceEvent _race)
	{
		if (m_raceText != null)
		{
			m_raceText.Text = _race.GetRaceTypeText();
		}
		if (m_hintText != null)
		{
			m_hintText.Text = _race.GetHintText();
		}
		if (m_objectives == null)
		{
			return;
		}
		for (int i = 0; i < m_objectives.Length; i++)
		{
			if (m_objectives[i] != null)
			{
				m_objectives[i].Text = _race.GetObjectiveText(i);
			}
		}
	}
}
