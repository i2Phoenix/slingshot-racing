using UnityEngine;

public class PipeGUI_Pulse : MonoBehaviour
{
	private bool m_invokeStarted;

	public Vector3 m_magnitude;

	public float m_delay;

	public float m_duration;

	public float m_repeatTime;

	public PipeGUI_PulseSync m_sync;

	private void Start()
	{
		if (!m_invokeStarted)
		{
			float num = ((!(m_sync != null)) ? m_delay : m_sync.GetDelay(this));
			InvokeRepeating("DoPulse", 1f + num, m_repeatTime);
		}
	}

	private void DoPulse()
	{
		PunchScale.Do(base.gameObject, m_magnitude, m_duration, 0f, null, null);
	}

	public void SetSync(PipeGUI_PulseSync _sync, int _index)
	{
		m_sync = _sync;
		if (m_invokeStarted)
		{
			CancelInvoke("DoPulse");
		}
		float delay = m_sync.GetDelay(_index);
		InvokeRepeating("DoPulse", 1f + delay, m_repeatTime);
		m_invokeStarted = true;
	}
}
