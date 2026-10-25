using System;
using UnityEngine;

[AddComponentMenu("Gameplay/Flee/Manager")]
public class FleeManager : MonoBehaviour
{
	private RaceManager m_raceManager;

	private int m_numVehiclesLeft;

	private int m_numDead;

	public bool m_displayTimer;

	private void Start()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceLaunchEvent += OnRaceStart;
		}
		m_raceManager.EnableTimer = m_displayTimer;
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceLaunchEvent -= OnRaceStart;
		}
	}

	private void OnRaceStart(object _sender, EventArgs _args)
	{
		m_numVehiclesLeft = m_raceManager.NumRacers;
		for (int i = 0; i < m_raceManager.NumRacers; i++)
		{
			Vehicle vehicle = m_raceManager.GetVehicle(i);
			vehicle.OnDeathEvent += OnVehicleDeath;
		}
		BroadcastMessage("FleeManager_Go");
	}

	private void OnVehicleDeath(GameObject _vehicleGO)
	{
		m_numVehiclesLeft--;
		int vehicleIndex = m_raceManager.GetVehicleIndex(_vehicleGO);
		m_raceManager.VehicleFinished(vehicleIndex, m_numVehiclesLeft);
		if (m_numVehiclesLeft != 1)
		{
			return;
		}
		BroadcastMessage("FleeManager_StopPursuit");
		for (int i = 0; i < m_raceManager.NumRacers; i++)
		{
			Vehicle vehicle = m_raceManager.GetVehicle(i);
			if (!vehicle.IsDead)
			{
				m_raceManager.VehicleFinished(i, 0);
				PhysicsVehicle physicsVehicle = vehicle as PhysicsVehicle;
				if (physicsVehicle != null)
				{
					physicsVehicle.HandbrakeOn = true;
				}
			}
		}
	}

	private void OnCollected()
	{
		BroadcastMessage("OnRespite", SendMessageOptions.RequireReceiver);
	}
}
