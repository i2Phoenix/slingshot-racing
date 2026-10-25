using UnityEngine;

public class PipeGUI_RaceTitle : MonoBehaviour
{
	private GameManager m_gameManager;

	private RaceManager m_raceManager;

	public SpriteText m_text;

	private void Start()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnRacePrepared += OnRacePreparedEvent;
			m_gameManager = m_raceManager.GetComponent<GameManager>();
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
		if (m_gameManager != null)
		{
			RaceTour currentTour = m_gameManager.CurrentTour;
			if (currentTour != null)
			{
				m_text.Text = currentTour.DisplayRaceName(m_gameManager.CurrentRace).ToUpper();
			}
		}
	}
}
