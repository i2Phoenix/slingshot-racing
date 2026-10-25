using System;
using UnityEngine;

public class FrontendHoistSway : MonoBehaviour
{
	[Serializable]
	public class MinMaxFloat
	{
		public float m_min;

		public float m_max;

		public float Value
		{
			get
			{
				return UnityEngine.Random.Range(m_min, m_max);
			}
		}
	}

	[Serializable]
	public class MinMaxVector3
	{
		public Vector3 m_min;

		public Vector3 m_max;

		public Vector3 Value
		{
			get
			{
				return Vector3.Scale(Vector3Utils.RandomRange(m_min, m_max).normalized, m_max);
			}
		}
	}

	private float m_activeTime;

	private float m_activeTimeRemaining;

	private Vector3 m_currentForce;

	public MinMaxFloat m_activeTimeRange = new MinMaxFloat();

	public MinMaxFloat m_forceRange = new MinMaxFloat();

	public MinMaxVector3 m_changeRange = new MinMaxVector3();

	private void Start()
	{
		m_currentForce = m_forceRange.Value * Vector3Utils.RandomRange(new Vector3(-1f, 0f, -1f), new Vector3(1f, 0f, 1f)).normalized;
	}

	private void FixedUpdate()
	{
		m_activeTimeRemaining -= Time.deltaTime;
		if (m_activeTimeRemaining < 0f)
		{
			m_activeTime = m_activeTimeRange.Value;
			m_activeTimeRemaining = m_activeTime;
			m_currentForce = (m_currentForce.normalized + m_changeRange.Value) * m_forceRange.Value;
		}
		if (base.rigidbody != null && m_activeTime > 0f)
		{
			float num = m_activeTime - m_activeTimeRemaining;
			float num2 = num / m_activeTime;
			float num3 = Mathf.Sin((float)Math.PI * 2f * num2);
			base.rigidbody.AddTorque(m_currentForce * num3);
		}
	}
}
