using UnityEngine;

public class PipeGUI_AsyncMatchDisconnection : MonoBehaviour
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
			if (m_turnBasedMatchManager != null)
			{
				m_turnBasedMatchManager.OnDisconnectedEvent += OnDisconnected;
			}
		}
	}

	private void OnDestroy()
	{
		if (m_turnBasedMatchManager != null)
		{
			m_turnBasedMatchManager.OnDisconnectedEvent -= OnDisconnected;
		}
	}

	private void OnDisconnected()
	{
		if (m_gameManager != null)
		{
			m_gameManager.GoBack();
			m_gameManager = null;
		}
	}
}
