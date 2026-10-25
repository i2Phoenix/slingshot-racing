using UnityEngine;

[AddComponentMenu("Tests/AsyncGUITest")]
public class AsyncGUITest : MonoBehaviour
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
		if (m_turnBasedMatchManager != null)
		{
			m_turnBasedMatchManager.OnGotoMatchEvent += OnGotoMatch;
		}
	}

	private void OnDestroy()
	{
		if (m_turnBasedMatchManager != null)
		{
			m_turnBasedMatchManager.OnGotoMatchEvent -= OnGotoMatch;
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

	private void OnFindMatchSelected()
	{
		if (m_turnBasedMatchManager.IsSupported)
		{
			Debug.Log("Async: Finding match");
			m_turnBasedMatchManager.FindMatch();
		}
	}

	private void OnGotoMatch()
	{
		Debug.Log("Multiplay: Started new match");
		if (m_turnBasedMatchManager.CurrentMatchData.NumRaces == 0 && m_turnBasedMatchManager.IsCurrentPlayersTurn())
		{
			Application.LoadLevel("AsyncLevelSelectTest");
		}
		else
		{
			Application.LoadLevel("AsyncMatchViewTest");
		}
	}
}
