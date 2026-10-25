using System;
using System.Threading;
using UnityEngine;

[AddComponentMenu("Debug/Drop Framerate")]
public class DropFramerate : MonoBehaviour
{
	private TimeSpan m_updateDelay;

	private TimeSpan m_fixedUpdateDelay;

	public int m_updateMilliseconds;

	public int m_fixedUpdateMilliseconds;

	private void Awake()
	{
		m_updateDelay = new TimeSpan(0, 0, 0, 0, m_updateMilliseconds);
		m_fixedUpdateDelay = new TimeSpan(0, 0, 0, 0, m_fixedUpdateMilliseconds);
	}

	private void Update()
	{
		Thread.Sleep(m_updateDelay);
	}

	private void FixedUpdate()
	{
		Thread.Sleep(m_fixedUpdateDelay);
	}
}
