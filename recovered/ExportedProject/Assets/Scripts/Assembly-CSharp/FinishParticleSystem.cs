using System;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[AddComponentMenu("Graphics/Finish Particle System")]
public class FinishParticleSystem : MonoBehaviour
{
	private ParticleSystem m_particleSystem;

	private RaceManager m_raceManager;

	private void Awake()
	{
		m_particleSystem = base.particleSystem;
	}

	private void Start()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceCompleteEvent += OnRaceComplete;
		}
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceCompleteEvent -= OnRaceComplete;
		}
	}

	private void OnRaceComplete(object _sender, EventArgs _args)
	{
		if (m_raceManager == null || (m_raceManager.CurrentRace.m_objective != RaceEvent.ObjectiveType.RacePosition && m_raceManager.CurrentRace.m_objective != RaceEvent.ObjectiveType.HeadsUp))
		{
			return;
		}
		for (int i = 0; i < m_raceManager.NumRacers; i++)
		{
			if (m_raceManager.GetVehicleRacePosition(i) == 0)
			{
				Vehicle vehicle = m_raceManager.GetVehicle(i);
				m_particleSystem.startColor = vehicle.BrightColour;
				m_particleSystem.Play();
				break;
			}
		}
	}
}
