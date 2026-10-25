using System.Collections;
using UnityEngine;

public class RaceControlTest : MonoBehaviour
{
	public GameObject m_raceControlPrefab;

	public RaceEvent m_testEvent;

	public bool m_quickStart = true;

	private IEnumerator Start()
	{
		if (m_testEvent != null)
		{
			GameObject go = GameObject.FindGameObjectWithTag("RaceControl");
			if (go == null)
			{
				go = Object.Instantiate(m_raceControlPrefab) as GameObject;
				yield return null;
				GameManager gm = go.GetComponent<GameManager>();
				gm.RaceSelected(m_testEvent, m_quickStart);
			}
		}
	}
}
