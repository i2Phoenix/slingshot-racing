using System;
using UnityEngine;

public class RaceTimer : MonoBehaviour
{
	private RaceManager m_raceManager;

	private float m_startTime;

	private float m_time;

	private PipeGUI_TimerNumber[] m_allNumbers;

	private bool m_active;

	private bool m_countDown;

	public GameObject m_visuals;

	public PipeGUI_TimerNumber m_minutes;

	public PipeGUI_TimerNumber m_tens;

	public PipeGUI_TimerNumber m_seconds;

	public PipeGUI_TimerNumber m_tenths;

	public bool ManualStart { get; set; }

	public bool CountDown
	{
		set
		{
			m_countDown = value;
		}
	}

	private void Start()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceLaunchEvent += OnLaunch;
			m_raceManager.OnRaceCompleteEvent += OnRaceComplete;
			m_raceManager.OnRaceAbortedEvent += OnRaceAborted;
		}
		m_allNumbers = new PipeGUI_TimerNumber[4];
		m_allNumbers[0] = m_minutes;
		m_allNumbers[1] = m_tens;
		m_allNumbers[2] = m_seconds;
		m_allNumbers[3] = m_tenths;
		Hide(true);
		if (m_raceManager == null)
		{
			Invoke("StartTimer", 1f);
			Invoke("StopTimer", 15f);
		}
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceLaunchEvent -= OnLaunch;
			m_raceManager.OnRaceCompleteEvent -= OnRaceComplete;
			m_raceManager.OnRaceAbortedEvent -= OnRaceAborted;
		}
	}

	private void FixedUpdate()
	{
		if (m_active)
		{
			m_time += ((!m_countDown) ? Time.fixedDeltaTime : (0f - Time.fixedDeltaTime));
			SetText(false);
		}
	}

	private void OnLaunch(object _sender, EventArgs _args)
	{
		if (base.enabled && !ManualStart)
		{
			StartTimer();
		}
	}

	private void OnRaceComplete(object _sender, EventArgs _args)
	{
		if (base.enabled && !ManualStart)
		{
			StopTimer(true);
		}
	}

	private void OnRaceAborted(object _sender, EventArgs _args)
	{
		if (base.enabled)
		{
			StopTimer(true);
		}
	}

	private void Hide(bool _hide)
	{
		PipeGUI_TimerNumber[] componentsInChildren = GetComponentsInChildren<PipeGUI_TimerNumber>();
		foreach (PipeGUI_TimerNumber pipeGUI_TimerNumber in componentsInChildren)
		{
			pipeGUI_TimerNumber.Hide(_hide);
		}
		if (m_visuals != null)
		{
			m_visuals.GetComponent<Renderer>().enabled = !_hide;
		}
	}

	public float GetCurrentTime()
	{
		return m_time;
	}

	public float GetStartTime()
	{
		return m_startTime;
	}

	public void StartTimer()
	{
		m_active = true;
		Reset();
		PipeGUI_TimerNumber[] allNumbers = m_allNumbers;
		foreach (PipeGUI_TimerNumber pipeGUI_TimerNumber in allNumbers)
		{
			pipeGUI_TimerNumber.OnStart();
		}
	}

	public void StopTimer()
	{
		StopTimer(true);
	}

	public void StopTimer(bool _hide)
	{
		m_active = false;
		PipeGUI_TimerNumber[] allNumbers = m_allNumbers;
		foreach (PipeGUI_TimerNumber pipeGUI_TimerNumber in allNumbers)
		{
			pipeGUI_TimerNumber.OnStop();
		}
		if (_hide)
		{
			Hide(true);
		}
		else
		{
			SetText(true);
		}
	}

	public void SetStartTime(float _startTime)
	{
		m_startTime = _startTime;
	}

	public void Reset()
	{
		m_time = m_startTime;
		SetText(true);
		Hide(false);
	}

	private void SetText(bool _immediate)
	{
		float num = Mathf.Max(0f, m_time);
		float num2 = num * (1f / 60f);
		float num3 = Mathf.Floor(num2);
		float num4 = (num2 - num3) * 6f;
		float num5 = Mathf.Floor(num4);
		float num6 = (num4 - num5) * 10f;
		float num7 = Mathf.Floor(num6);
		float f = (num6 - num7) * 10f;
		float num8 = Mathf.Floor(f);
		num3 = Mathf.Clamp(num3, 0f, 9f);
		if (m_visuals != null)
		{
			m_minutes.Set((int)num3, _immediate);
			m_tens.Set((int)num5, _immediate);
			m_seconds.Set((int)num7, _immediate);
			m_tenths.Set((int)num8, _immediate);
		}
	}
}
