using System;
using UnityEngine;

public class TimeEx : MonoBehaviour
{
	private struct PauseData
	{
		private int m_refs;

		public int RefCount
		{
			get
			{
				return m_refs;
			}
		}

		public void AddRef()
		{
			m_refs++;
		}

		public void Release()
		{
			m_refs--;
		}
	}

	private static float s_lastTimeSinceStartup;

	private static float s_unscaledDeltaTime;

	private static float s_timeScale = 1f;

	private static PauseLevel s_highestPauseLevel;

	private static int s_numPauseLevels = Enum.GetValues(typeof(PauseLevel)).Length;

	private static PauseData[] s_pauseData = new PauseData[Enum.GetValues(typeof(PauseLevel)).Length];

	private static bool s_paused;

	private static bool s_valid;

	public static bool IsAvailable
	{
		get
		{
			return s_valid;
		}
	}

	public static float TimeScale
	{
		get
		{
			return s_timeScale;
		}
		set
		{
			s_timeScale = value;
			CalculatePauseLevels();
		}
	}

	private void Awake()
	{
		s_valid = true;
	}

	private void Update()
	{
		s_unscaledDeltaTime = Mathf.Clamp(Time.realtimeSinceStartup - s_lastTimeSinceStartup, 0f, Time.maximumDeltaTime);
		s_lastTimeSinceStartup = Time.realtimeSinceStartup;
	}

	public static void RequestPause(PauseLevel _level)
	{
		s_pauseData[(int)_level].AddRef();
		CalculatePauseLevels();
	}

	public static void RequestUnPause(PauseLevel _level)
	{
		s_pauseData[(int)_level].Release();
		CalculatePauseLevels();
	}

	public static float DeltaTime(PauseLevel _level)
	{
		if (s_paused && _level <= s_highestPauseLevel)
		{
			return s_unscaledDeltaTime;
		}
		return Time.deltaTime;
	}

	private static void CalculatePauseLevels()
	{
		s_paused = false;
		for (int i = 0; i < s_numPauseLevels; i++)
		{
			if (s_pauseData[i].RefCount > 0)
			{
				s_paused = true;
				s_highestPauseLevel = (PauseLevel)i;
				break;
			}
		}
		Time.timeScale = ((!s_paused) ? s_timeScale : 0f);
		AudioListener.volume = ((!s_paused) ? Settings.SoundVolume : 0f);
	}
}
