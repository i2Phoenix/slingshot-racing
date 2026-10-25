using UnityEngine;

public class PipeGUI_RaceEventNumber : MonoBehaviour
{
	private RaceManager m_raceManager;

	public SpriteText m_text;

	private void Start()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnRacePrepared += OnRacePreparedEvent;
		}
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnRacePrepared -= OnRacePreparedEvent;
		}
	}

	private void OnRacePreparedEvent()
	{
		int raceNumber = m_raceManager.RaceNumber;
		int numRaces = m_raceManager.NumRaces;
		if (m_raceManager.IsTournament && raceNumber > 0 && numRaces > 0)
		{
			m_text.Text = string.Format(Language.Get("ID_Summary_RaceNumber"), raceNumber, numRaces);
		}
		else
		{
			base.gameObject.SetActiveRecursively(false);
		}
	}
}
