using System.Collections;
using UnityEngine;

[AddComponentMenu("Debug/Soak Test")]
public class SoakTest : MonoBehaviour
{
	private GameManager m_gameManager;

	public GameObject m_raceControlPrefab;

	public RaceCampaign m_campaign;

	private void Awake()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject == null)
		{
			gameObject = Object.Instantiate(m_raceControlPrefab) as GameObject;
		}
		GameManager component = gameObject.GetComponent<GameManager>();
		m_gameManager = component;
	}

	private IEnumerator Start()
	{
		yield return YieldCache.WaitForHalfSecond;
		if (m_gameManager != null)
		{
			m_gameManager.StartSoakTest(m_campaign, null);
		}
	}

	private void OnApplicationQuit()
	{
		PersistentState.Get().m_aiControlHumans = false;
	}

	private void Update()
	{
		if ((Input.touchCount > 4 || (Input.GetKeyDown(KeyCode.Q) && Input.GetKey(KeyCode.LeftShift))) && m_gameManager != null)
		{
			m_gameManager.StopAllCoroutines();
			Application.LoadLevel("OptionsScreenDebug");
			Object.Destroy(m_gameManager.gameObject);
			PersistentState.Get().m_aiControlHumans = false;
			Object.Destroy(base.gameObject);
		}
	}
}
