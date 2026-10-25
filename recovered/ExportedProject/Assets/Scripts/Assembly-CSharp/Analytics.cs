using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Analytics : MonoBehaviour
{
	private static readonly string c_apiKey = "RRX4RCHSC9N3WGZVCG68";

	private static AndroidJavaClass s_flurryAgent;

	private static AndroidJavaObject s_context;

	private static Analytics s_instance;

	private static string s_currentState;

	private static Dictionary<string, string> s_currentStateArgs;

	private static int s_eventNum;

	public static void Init()
	{
		if (s_instance == null)
		{
			GameObject gameObject = new GameObject("Analytics");
			s_instance = gameObject.AddComponent<Analytics>();
		}
		s_flurryAgent = new AndroidJavaClass("com.flurry.android.FlurryAgent");
		FlurrySetVersionName(AppSettings.VersionTrimmed);
		FlurryStartSession(c_apiKey);
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			FlurryStartSession(c_apiKey);
			if (s_currentState != null)
			{
				LogState(s_currentState, s_currentStateArgs);
			}
		}
		else
		{
			LogEvent("Suspended");
			FlurryEndSession(c_apiKey);
		}
	}

	public static void LogState(string name)
	{
		s_currentState = name;
		s_currentStateArgs = null;
		FlurryLogEvent(name, false);
	}

	public static void LogState(string name, Dictionary<string, string> args)
	{
		s_currentState = name;
		s_currentStateArgs = args;
		FlurryLogEvent(name, false, args);
	}

	public static void LogEvent(string _name)
	{
		FlurryLogEvent(_name, false);
	}

	public static void LogEvent(string _name, string _key, string _value)
	{
		FlurryLogEvent1Arg(_name, false, _key, _value);
	}

	public static void LogEvent(string _name, string _key0, string _value0, string _key1, string _value1)
	{
		FlurryLogEvent2Arg(_name, false, _key0, _value0, _key1, _value1);
	}

	public static void LogEvent(string name, Dictionary<string, string> args)
	{
		FlurryLogEvent(name, false, args);
	}

	[Conditional("ANALYTICS_LOGGING_UNITY")]
	private static void Log(params string[] _strings)
	{
		string text = string.Format("ID {0}: ", s_eventNum++);
		foreach (string text2 in _strings)
		{
			text = text + text2 + "; ";
		}
		UnityEngine.Debug.Log(text);
	}

	[Conditional("ANALYTICS_LOGGING_UNITY")]
	private static void Log(string type, string name, Dictionary<string, string> args)
	{
		string text = string.Format("ID {0}: ", s_eventNum++);
		string text2 = text;
		text = text2 + type + "; " + name + "; ";
		if (args != null)
		{
			foreach (KeyValuePair<string, string> arg in args)
			{
				text2 = text;
				text = text2 + arg.Key + "=" + arg.Value + "; ";
			}
		}
		UnityEngine.Debug.Log(text);
	}

	private static void FlurrySetVersionName(string versionName)
	{
		if (s_flurryAgent != null)
		{
			s_flurryAgent.CallStatic("setVersionName", versionName);
		}
	}

	private static void FlurryStartSession(string apiKey)
	{
		if (s_context != null)
		{
			FlurryEndSession(apiKey);
		}
		if (s_flurryAgent != null)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				s_context = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			s_flurryAgent.CallStatic("onStartSession", s_context, apiKey);
		}
	}

	private static void FlurryEndSession(string apiKey)
	{
		if (s_flurryAgent != null && s_context != null)
		{
			s_flurryAgent.CallStatic("onEndSession", s_context);
		}
		s_context = null;
	}

	private static void FlurryLogEvent(string name, bool timed)
	{
		if (s_flurryAgent != null)
		{
			s_flurryAgent.CallStatic("logEvent", name, timed);
		}
	}

	private static void FlurryLogEvent1Arg(string name, bool timed, string key0, string val0)
	{
		if (s_flurryAgent != null)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap");
			androidJavaObject.Call<string>("put", new object[2] { key0, val0 });
			s_flurryAgent.CallStatic("logEvent", name, androidJavaObject, timed);
		}
	}

	private static void FlurryLogEvent2Arg(string name, bool timed, string key0, string val0, string key1, string val1)
	{
		if (s_flurryAgent != null)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap");
			androidJavaObject.Call<string>("put", new object[2] { key0, val0 });
			androidJavaObject.Call<string>("put", new object[2] { key1, val1 });
			s_flurryAgent.CallStatic("logEvent", name, androidJavaObject, timed);
		}
	}

	private static void FlurryLogEvent(string name, bool timed, Dictionary<string, string> args)
	{
		if (s_flurryAgent == null)
		{
			return;
		}
		if (args != null)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap");
			foreach (KeyValuePair<string, string> arg in args)
			{
				androidJavaObject.Call<string>("put", new object[2] { arg.Key, arg.Value });
			}
			s_flurryAgent.CallStatic("logEvent", name, androidJavaObject, timed);
		}
		else
		{
			s_flurryAgent.CallStatic("logEvent", name, timed);
		}
	}
}
