using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/In Race Identifiers")]
public class InRaceIdentifiers : MonoBehaviour
{
	private RaceManager m_raceManager;

	public GameObject m_identifierPrefab;

	public float m_distAbove = 1.5f;

	public float m_initialDelay = 0.5f;

	public float m_quickDelay = 0.2f;

	public float m_cascadeDelay = 0.125f;

	public void Play(RaceManager _raceManager, bool _quick)
	{
		m_raceManager = _raceManager;
		m_raceManager.OnRaceLaunchEvent += AbortOnEvent;
		m_raceManager.OnRaceReplayEvent += AbortOnEvent;
		m_raceManager.OnRaceAbortedEvent += AbortOnEvent;
		StartCoroutine(PlayCoroutine(_raceManager, _quick));
	}

	private IEnumerator PlayCoroutine(RaceManager _raceManager, bool _quick)
	{
		yield return new WaitForSeconds((!_quick) ? m_initialDelay : m_quickDelay);
		int numRacers = _raceManager.NumRacers;
		int numHumans = _raceManager.NumHumans;
		if (numRacers > 1 && numHumans > 0)
		{
			Quaternion cameraRotation = MainCamera.Transform.rotation;
			for (int i = 0; i != numHumans; i++)
			{
				yield return new WaitForSeconds(m_cascadeDelay);
				Vehicle vehicle = _raceManager.GetVehicle(i);
				if (vehicle != null)
				{
					GameObject identifierGO = (GameObject)UnityEngine.Object.Instantiate(position: vehicle.Position + cameraRotation * Vector3.up * m_distAbove, original: m_identifierPrefab, rotation: cameraRotation);
					identifierGO.transform.parent = vehicle.transform;
					AddColourToMesh addColourToMesh = identifierGO.GetComponentInChildren<AddColourToMesh>();
					if (addColourToMesh != null)
					{
						addColourToMesh.m_colour = vehicle.BrightColour;
					}
					RaceIdentifier raceIdentifier = identifierGO.GetComponentInChildren<RaceIdentifier>();
					if (raceIdentifier != null)
					{
						raceIdentifier.Init(_raceManager, _quick);
					}
				}
			}
		}
		m_raceManager.OnRaceLaunchEvent -= AbortOnEvent;
		m_raceManager.OnRaceReplayEvent -= AbortOnEvent;
		m_raceManager.OnRaceAbortedEvent -= AbortOnEvent;
		m_raceManager = null;
	}

	private void AbortOnEvent(object _sender, EventArgs _args)
	{
		StopAllCoroutines();
		m_raceManager.OnRaceLaunchEvent -= AbortOnEvent;
		m_raceManager.OnRaceReplayEvent -= AbortOnEvent;
		m_raceManager.OnRaceAbortedEvent -= AbortOnEvent;
		m_raceManager = null;
	}
}
