using UnityEngine;

[AddComponentMenu("Gameplay/Time Trial/Manager")]
public class TimeTrialManager : MonoBehaviour
{
	private RaceManager m_raceManager;

	private void Start()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.EnableTimer = true;
		}
	}

	private void OnDestroy()
	{
	}
}
