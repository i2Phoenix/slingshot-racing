using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YieldCache
{
	private const float c_waitForSecondsGranularity = 0.01f;

	private const float c_rcpWaitForSecondsGranularity = 100f;

	private const int c_maxCapacity = 128;

	private static Dictionary<int, WaitForSeconds> s_waitForSeconds;

	private static WaitForSeconds s_wait15Hz;

	private static WaitForSeconds s_wait10Hz;

	private static WaitForSeconds s_wait5Hz;

	private static WaitForSeconds s_waitForHalfSecond;

	private static WaitForSeconds s_waitForOneSecond;

	private static WaitForSeconds s_waitForTwoSeconds;

	private static WaitForSeconds s_waitForThreeSeconds;

	private static WaitForSeconds s_waitForFourSeconds;

	private static WaitForSeconds s_waitForFiveSeconds;

	private static WaitForSeconds s_waitForTenSeconds;

	private static WaitForSeconds s_waitForThirtySeconds;

	private static WaitForSeconds s_waitForOneMinute;

	private static WaitForEndOfFrame s_waitForEndOfFrame;

	private static WaitForFixedUpdate s_waitForFixedUpdate;

	public static WaitForSeconds Wait15Hz
	{
		get
		{
			return s_wait15Hz;
		}
	}

	public static WaitForSeconds Wait10Hz
	{
		get
		{
			return s_wait10Hz;
		}
	}

	public static WaitForSeconds Wait5Hz
	{
		get
		{
			return s_wait5Hz;
		}
	}

	public static WaitForSeconds WaitForHalfSecond
	{
		get
		{
			return s_waitForHalfSecond;
		}
	}

	public static WaitForSeconds WaitForOneSecond
	{
		get
		{
			return s_waitForOneSecond;
		}
	}

	public static WaitForSeconds WaitForTwoSeconds
	{
		get
		{
			return s_waitForTwoSeconds;
		}
	}

	public static WaitForSeconds WaitForThreeSeconds
	{
		get
		{
			return s_waitForThreeSeconds;
		}
	}

	public static WaitForSeconds WaitForFourSeconds
	{
		get
		{
			return s_waitForFourSeconds;
		}
	}

	public static WaitForSeconds WaitForFiveSeconds
	{
		get
		{
			return s_waitForFiveSeconds;
		}
	}

	public static WaitForSeconds WaitForTenSeconds
	{
		get
		{
			return s_waitForTenSeconds;
		}
	}

	public static WaitForSeconds WaitForThirtySeconds
	{
		get
		{
			return s_waitForThirtySeconds;
		}
	}

	public static WaitForSeconds WaitForOneMinute
	{
		get
		{
			return s_waitForOneMinute;
		}
	}

	public static WaitForEndOfFrame WaitForEndOfFrame
	{
		get
		{
			return s_waitForEndOfFrame;
		}
	}

	public static WaitForFixedUpdate WaitForFixedUpdate
	{
		get
		{
			return s_waitForFixedUpdate;
		}
	}

	static YieldCache()
	{
		s_waitForSeconds = new Dictionary<int, WaitForSeconds>();
		s_wait15Hz = new WaitForSeconds(1f / 15f);
		s_wait10Hz = new WaitForSeconds(0.1f);
		s_wait5Hz = new WaitForSeconds(0.2f);
		s_waitForHalfSecond = new WaitForSeconds(0.5f);
		s_waitForOneSecond = new WaitForSeconds(1f);
		s_waitForTwoSeconds = new WaitForSeconds(2f);
		s_waitForThreeSeconds = new WaitForSeconds(3f);
		s_waitForFourSeconds = new WaitForSeconds(4f);
		s_waitForFiveSeconds = new WaitForSeconds(5f);
		s_waitForTenSeconds = new WaitForSeconds(10f);
		s_waitForThirtySeconds = new WaitForSeconds(30f);
		s_waitForOneMinute = new WaitForSeconds(60f);
		s_waitForEndOfFrame = new WaitForEndOfFrame();
		s_waitForFixedUpdate = new WaitForFixedUpdate();
	}

	public static WaitForSeconds WaitForSeconds(float _seconds)
	{
		int key = WaitForSecondsKey(_seconds);
		if (s_waitForSeconds.ContainsKey(key))
		{
			return s_waitForSeconds[key];
		}
		if (s_waitForSeconds.Count < 128)
		{
			WaitForSeconds waitForSeconds = new WaitForSeconds(WaitForSecondsDuration(key));
			s_waitForSeconds[key] = waitForSeconds;
			return waitForSeconds;
		}
		return new WaitForSeconds(_seconds);
	}

	public static IEnumerator WaitForRealSeconds(float _seconds, PauseLevel _level)
	{
		if (TimeEx.IsAvailable)
		{
			for (float startTime = 0f; startTime < _seconds; startTime += TimeEx.DeltaTime(_level))
			{
				yield return null;
			}
			yield break;
		}
		float time = Time.realtimeSinceStartup;
		float endTime = time + _seconds;
		while (time < endTime)
		{
			yield return null;
			time = Time.realtimeSinceStartup;
		}
	}

	public static void Evict()
	{
		s_waitForSeconds.Clear();
	}

	private static int WaitForSecondsKey(float _seconds)
	{
		return Mathf.RoundToInt(_seconds * 100f);
	}

	private static float WaitForSecondsDuration(int _key)
	{
		return (float)_key * 0.01f;
	}
}
