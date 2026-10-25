using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnLaunch : MonoBehaviour
{
	private RaceManager m_raceManager;

	private List<Transform> m_activationList;

	public float m_delay;

	private void Start()
	{
		m_activationList = new List<Transform>();
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			AddToActivationList(child);
		}
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceLaunchEvent += OnRaceStart;
			m_raceManager.OnRaceReplayEvent += OnRaceReplay;
		}
		else
		{
			OnRaceStart(null, null);
		}
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceLaunchEvent -= OnRaceStart;
			m_raceManager.OnRaceReplayEvent -= OnRaceReplay;
		}
	}

	private void OnRaceStart(object _sender, EventArgs _args)
	{
		if (base.gameObject.active)
		{
			StartCoroutine(ActivateCoroutine(m_delay));
		}
	}

	private IEnumerator ActivateCoroutine(float _delay)
	{
		if (_delay > 0f)
		{
			yield return new WaitForSeconds(_delay);
		}
		SetActivationList(true);
		base.gameObject.BroadcastMessage("ActivateOnLaunch_Launch", SendMessageOptions.DontRequireReceiver);
	}

	private void OnRaceReplay(object _sender, EventArgs _args)
	{
		base.gameObject.BroadcastMessage("ActivateOnLaunch_Replay", SendMessageOptions.DontRequireReceiver);
		SetActivationList(false);
		StopAllCoroutines();
	}

	private void AddToActivationList(Transform _transform)
	{
		if (!_transform.gameObject.active)
		{
			m_activationList.Add(_transform);
		}
		for (int i = 0; i < _transform.childCount; i++)
		{
			Transform child = _transform.GetChild(i);
			AddToActivationList(child);
		}
	}

	private void SetActivationList(bool _active)
	{
		for (int i = 0; i < m_activationList.Count; i++)
		{
			m_activationList[i].gameObject.active = _active;
		}
	}
}
