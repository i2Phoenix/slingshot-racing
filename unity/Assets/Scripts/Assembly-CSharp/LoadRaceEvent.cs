using UnityEngine;

[AddComponentMenu("Tools/Load Race Event")]
public class LoadRaceEvent : MonoBehaviour
{
	private const string c_loadRaceEventFind = "/__LoadRaceEvent";

	public GameObject m_raceControlPrefab;

	public RaceEvent m_raceEvent;

	public bool m_replay;

	public static LoadRaceEvent Get
	{
		get
		{
			GameObject gameObject = GameObject.Find("/__LoadRaceEvent");
			if (gameObject == null)
			{
				return null;
			}
			return gameObject.GetComponent<LoadRaceEvent>();
		}
	}

	private void Start()
	{
		if (m_raceEvent == null)
		{
			Debug.LogError("No race event set");
			return;
		}
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject == null)
		{
			gameObject = Object.Instantiate(m_raceControlPrefab) as GameObject;
		}
		GameManager component = gameObject.GetComponent<GameManager>();
		if (component != null)
		{
			if (m_replay)
			{
				component.ShowReplay(m_raceEvent);
			}
			else
			{
				component.RaceSelected(m_raceEvent, false);
			}
		}
	}
}
