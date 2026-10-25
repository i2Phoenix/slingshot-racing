using UnityEngine;

[AddComponentMenu("Tests/AsyncLevelSelectTest")]
public class AsyncLevelSelectTest : MonoBehaviour
{
	private GameManager m_gameManager;

	private TurnBasedMatchManager m_turnBasedMatchManager;

	private void Start()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject != null)
		{
			m_gameManager = gameObject.GetComponent<GameManager>();
			m_turnBasedMatchManager = gameObject.GetComponent<TurnBasedMatchManager>();
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

	private void OnTrack1Selected()
	{
		if (m_turnBasedMatchManager != null)
		{
			TurnBasedMatchData currentMatchData = m_turnBasedMatchManager.CurrentMatchData;
			TurnBasedMatchData.RaceEventData raceEventData = currentMatchData.AddRaceEventData("Track1");
			raceEventData.NumLaps = 5;
			raceEventData.Reversed = false;
			raceEventData.Variant = RaceLayout.Variant.Standard;
			raceEventData.TBRaceState = TurnBasedMatchData.TBRaceState.Waiting;
			Application.LoadLevel("AsyncMatchViewTest");
		}
	}
}
