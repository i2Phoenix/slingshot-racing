using UnityEngine;

[AddComponentMenu("Tests/AsyncMatchViewTest")]
public class AsyncMatchViewTest : MonoBehaviour
{
	private GameManager m_gameManager;

	private SocialManager m_socialManager;

	private TurnBasedMatchManager m_turnBasedMatchManager;

	public SpriteText m_levelNameLabel;

	public GameObject m_takeTurnButton;

	public GameObject m_deleteMatchButton;

	private void Start()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject != null)
		{
			m_gameManager = gameObject.GetComponent<GameManager>();
			m_socialManager = gameObject.GetComponent<SocialManager>();
			m_turnBasedMatchManager = gameObject.GetComponent<TurnBasedMatchManager>();
		}
		if (m_turnBasedMatchManager != null)
		{
			TurnBasedMatchData currentMatchData = m_turnBasedMatchManager.CurrentMatchData;
			Debug.Log(currentMatchData);
			TurnBasedMatchData.RaceEventData currentRace = currentMatchData.CurrentRace;
			if (currentRace != null)
			{
				m_levelNameLabel.Text = currentRace.RaceEventName;
			}
			if (!m_turnBasedMatchManager.IsCurrentPlayersTurn())
			{
				m_takeTurnButton.SetActiveRecursively(false);
			}
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

	private void OnTakeTurnSelected()
	{
		if (m_turnBasedMatchManager != null && m_turnBasedMatchManager.IsCurrentPlayersTurn())
		{
			string playerID = m_socialManager.PlayerID;
			TurnBasedMatchData currentMatchData = m_turnBasedMatchManager.CurrentMatchData;
			TurnBasedMatchData.PlayerData playerData;
			if (currentMatchData.HasDataForPlayer(playerID))
			{
				playerData = currentMatchData.FindPlayerData(playerID);
			}
			else
			{
				playerData = currentMatchData.AddPlayerData(playerID);
				playerData.Name = m_socialManager.PlayerAlias;
			}
			TurnBasedMatchData.RaceEventData currentRace = currentMatchData.CurrentRace;
			if (currentRace != null)
			{
				TurnBasedMatchData.PlayerResultsData playerResultsData = currentRace.AddPlayerResultsData(playerData.PlayerID);
				playerResultsData.Position = 1;
			}
		}
	}

	private void OnDeleteMatchSelected()
	{
		if (m_turnBasedMatchManager != null && !m_turnBasedMatchManager.RemoveCurrentMatch())
		{
			Debug.Log("Failed to remove match");
		}
	}
}
