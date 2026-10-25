using System;
using UnityEngine;

[AddComponentMenu("Gameplay/Pickups/Manager")]
public class PickupManager : MonoBehaviour
{
	private class PickupOrdering
	{
		public static int Compare(Pickup left, Pickup right)
		{
			Vector3 position = left.transform.position;
			Vector3 position2 = right.transform.position;
			if (position.x != position2.x)
			{
				return (!(position.x < position2.x)) ? 1 : (-1);
			}
			if (position.z != position2.z)
			{
				return (!(position.z < position2.z)) ? 1 : (-1);
			}
			return 0;
		}
	}

	private static PickupManager s_this;

	private RaceManager m_raceManager;

	private RaceTimer m_raceTimer;

	private bool m_raceActive;

	private Vehicle m_vehicle;

	private PositionPopup[] m_pickupPopups;

	private Pickup[] m_pickups;

	private int m_nextPopup;

	private float m_maxRaceDuration;

	private float m_timeToStartScaling;

	private float m_timeToComplete;

	private float m_currentScale;

	private string m_finalCogText;

	private string m_finishedText;

	private int m_numActivePickups;

	private int m_totalPickups;

	public static PickupManager Get
	{
		get
		{
			return s_this;
		}
	}

	private void Awake()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceStartCountdownEvent += OnStart;
			m_raceManager.OnRaceLaunchEvent += OnLaunch;
			m_raceManager.OnUpdateRaceStats += OnUpdateRaceStats;
		}
		if (GetComponent<PickupCollected>() == null)
		{
			base.gameObject.AddComponent<PickupCollected>();
		}
		if (s_this == null)
		{
			s_this = this;
		}
	}

	private void Start()
	{
		if (m_raceManager != null)
		{
			m_raceTimer = m_raceManager.Timer;
			if (m_raceTimer != null)
			{
				m_raceTimer.enabled = true;
				m_raceTimer.CountDown = true;
			}
		}
		m_finalCogText = Language.Get("ID_Popup_FinalCog");
		m_finishedText = Language.Get("ID_Popup_Finished");
		m_pickupPopups = new PositionPopup[4];
		for (int i = 0; i != m_pickupPopups.Length; i++)
		{
			UnityEngine.Object original = Resources.Load("Prefabs/Popups/PickupPopup");
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
			PositionPopup component = gameObject.GetComponent<PositionPopup>();
			m_pickupPopups[i] = component;
		}
	}

	private void OnStart(object _sender, EventArgs _args)
	{
		m_vehicle = m_raceManager.GetVehicle(0);
		m_pickups = GetComponentsInChildren<Pickup>(false);
		m_numActivePickups = ((m_pickups != null) ? m_pickups.Length : 0);
		m_totalPickups = m_numActivePickups;
		Array.Sort(m_pickups, PickupOrdering.Compare);
		for (int i = 0; i != m_numActivePickups; i++)
		{
			m_pickups[i].Index = i;
		}
		for (int j = 0; j != m_pickupPopups.Length; j++)
		{
			m_pickupPopups[j].Colour = m_vehicle.Colour;
			m_pickupPopups[j].Target = m_vehicle;
		}
		RaceEvent currentRace = m_raceManager.CurrentRace;
		m_maxRaceDuration = currentRace.m_maxDuration;
		m_timeToStartScaling = m_maxRaceDuration - m_maxRaceDuration * 0.5f;
		if (m_raceTimer != null)
		{
			m_raceTimer.SetStartTime(m_maxRaceDuration);
			m_raceTimer.Reset();
		}
		m_currentScale = 1f;
	}

	private void OnLaunch(object _sender, EventArgs _args)
	{
		m_raceActive = true;
	}

	private void OnUpdateRaceStats(RaceStats _raceStats)
	{
		_raceStats.m_raceTime = m_timeToComplete;
	}

	private void FixedUpdate()
	{
		if (m_raceActive && m_raceTimer != null)
		{
			if (m_raceTimer.GetCurrentTime() <= m_timeToStartScaling)
			{
				m_currentScale = 1f + 0.5f * Mathf.Min(1f, (m_timeToStartScaling - m_raceTimer.GetCurrentTime()) / m_timeToStartScaling);
				Vector3 vector = new Vector3(m_currentScale, m_currentScale, m_currentScale);
				BroadcastMessage("SetScale", vector);
			}
			if (m_raceManager != null && m_raceTimer.GetCurrentTime() <= 0f)
			{
				m_raceManager.VehicleFinished(0, 0);
				m_raceActive = false;
				m_timeToComplete = 0f;
			}
		}
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceStartCountdownEvent -= OnStart;
			m_raceManager.OnRaceLaunchEvent -= OnLaunch;
			m_raceManager.OnUpdateRaceStats -= OnUpdateRaceStats;
		}
		if (m_pickupPopups != null)
		{
			for (int i = 0; i != m_pickupPopups.Length; i++)
			{
				if ((bool)m_pickupPopups[i])
				{
					UnityEngine.Object.Destroy(m_pickupPopups[i].gameObject);
				}
			}
		}
		if (s_this == this)
		{
			s_this = null;
		}
	}

	private void OnCollected(Pickup _object)
	{
		if (m_raceActive && m_raceManager != null)
		{
			m_numActivePickups--;
			string text = ((m_numActivePickups > 1) ? string.Format("{0}/{1} ", m_totalPickups - m_numActivePickups, m_totalPickups) : ((m_numActivePickups != 1) ? m_finishedText : m_finalCogText));
			PositionPopup positionPopup = m_pickupPopups[m_nextPopup];
			if (++m_nextPopup == m_pickupPopups.Length)
			{
				m_nextPopup = 0;
			}
			positionPopup.Popup(text);
			if (m_numActivePickups == 0)
			{
				m_timeToComplete = m_maxRaceDuration - m_raceManager.GetVehicleTimeSinceStart(0);
				m_raceManager.VehicleFinished(0, 0);
				m_raceActive = false;
				_object.SendMessage("PlayCollectedFinal");
			}
			else
			{
				_object.SendMessage("PlayCollected");
			}
			m_vehicle.SendPickupEvent(_object.Index);
		}
	}

	public void TriggerCollection(int _index)
	{
		m_pickups[_index].TriggerCollection(m_vehicle.transform);
	}
}
