using System;
using System.Collections.Generic;
using UnityEngine;

public class MagnetUpgrade : MonoBehaviour, Vehicle.IUpgrade
{
	private struct PickupInfo : IEquatable<PickupInfo>
	{
		public Pickup m_pickup;

		public float m_speed;

		public bool m_trapped;

		public PickupInfo(Pickup _pickup)
		{
			m_pickup = _pickup;
			m_speed = 0f;
			m_trapped = false;
		}

		public bool Equals(PickupInfo _other)
		{
			return m_pickup == _other.m_pickup;
		}
	}

	private float m_activeTimeRemaining;

	private float m_rechargeTimeRemaining;

	private bool m_processing;

	private List<PickupInfo> m_pickups = new List<PickupInfo>(32);

	public float m_startingSpeed = 20f;

	public float m_speedScalar = 2f;

	public bool m_manualActivation;

	public float m_activeTime = 3f;

	public float m_rechargeTime = 3f;

	public bool Expended
	{
		get
		{
			return false;
		}
	}

	public bool CanBeActivated
	{
		get
		{
			return m_rechargeTimeRemaining <= 0f;
		}
	}

	public float Charge
	{
		get
		{
			float value = ((m_rechargeTimeRemaining <= 0f) ? 1f : ((!(m_activeTimeRemaining > 0f)) ? (1f - m_rechargeTimeRemaining / m_rechargeTime) : (m_activeTimeRemaining / m_activeTime)));
			return Mathf.Clamp01(value);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (m_manualActivation)
		{
			if (m_activeTimeRemaining > 0f)
			{
				m_activeTimeRemaining -= Time.deltaTime;
			}
			if (m_rechargeTimeRemaining > 0f)
			{
				m_rechargeTimeRemaining -= Time.deltaTime;
			}
		}
		m_processing = true;
		Vector3 position = base.transform.position;
		Vector3 forward = base.transform.forward;
		for (int i = 0; i < m_pickups.Count; i++)
		{
			PickupInfo value = m_pickups[i];
			Transform transform = value.m_pickup.transform;
			Vector3 position2 = transform.position;
			Vector3 normalized = (position - position2).normalized;
			if (!value.m_trapped)
			{
				float f = Vector3.Dot(-normalized, forward);
				if (Mathf.Abs(f) < 0.9f)
				{
					value.m_trapped = true;
					value.m_speed = m_startingSpeed;
				}
			}
			if (value.m_trapped)
			{
				position2 += normalized * (value.m_speed * Time.deltaTime);
				transform.position = position2;
				value.m_speed += m_speedScalar * value.m_speed * Time.deltaTime;
			}
			m_pickups[i] = value;
		}
		m_processing = false;
	}

	public void OnEnter(Pickup _pickup)
	{
		if (!m_manualActivation || !(m_activeTimeRemaining <= 0f))
		{
			PickupInfo item = new PickupInfo(_pickup);
			if (!m_pickups.Contains(item))
			{
				_pickup.OnCollected += OnCollected;
				m_pickups.Add(item);
			}
		}
	}

	public void OnExit(Pickup _pickup)
	{
		int num = m_pickups.RemoveAll((PickupInfo elem) => elem.m_pickup == _pickup && !elem.m_trapped);
		if (num == 1)
		{
			_pickup.OnCollected -= OnCollected;
		}
	}

	public void Activate()
	{
		m_activeTimeRemaining = m_activeTime;
		m_rechargeTimeRemaining = m_activeTime + m_rechargeTime;
	}

	private void OnCollected(Pickup _pickup)
	{
		m_pickups.RemoveAll((PickupInfo elem) => elem.m_pickup == _pickup);
	}
}
