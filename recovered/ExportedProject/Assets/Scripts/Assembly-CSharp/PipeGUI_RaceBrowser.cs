using UnityEngine;

public class PipeGUI_RaceBrowser : MonoBehaviour
{
	private GameManager m_gameManager;

	private string m_filter = string.Empty;

	public GameObject m_raceControlPrefab;

	public GameObject m_listPrefab;

	public UITextField m_textField;

	public RaceTour m_tour;

	private void Start()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject == null)
		{
			gameObject = Object.Instantiate(m_raceControlPrefab) as GameObject;
		}
		GameManager component = gameObject.GetComponent<GameManager>();
		m_gameManager = component;
		Refresh();
	}

	public void Refresh()
	{
		UIScrollList component = GetComponent<UIScrollList>();
		component.ClearList(true);
		RaceEvent[] races = m_tour.m_races;
		foreach (RaceEvent raceEvent in races)
		{
			if (!(raceEvent == null) && (m_filter == null || m_filter.Length <= 0 || raceEvent.name.Contains(m_filter)))
			{
				GameObject gameObject = Object.Instantiate(m_listPrefab) as GameObject;
				UIButton3D component2 = gameObject.GetComponent<UIButton3D>();
				component2.Text = raceEvent.name;
				gameObject.GetComponent<PipeGUI_RaceBrowserListItem>().m_race = raceEvent;
				component.AddItem(gameObject);
			}
		}
	}

	public void OnRaceSelected(RaceEvent _race)
	{
		Debug.Log("Clicked " + _race.name);
		if (m_gameManager != null)
		{
			m_gameManager.RaceSelected(_race, false);
			m_gameManager = null;
		}
	}

	public void OnFilter()
	{
		m_filter = m_textField.Text;
		Refresh();
	}
}
