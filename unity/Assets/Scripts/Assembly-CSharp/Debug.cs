using System.Diagnostics;
using UnityEngine;

public class Debug
{
	public static bool isDebugBuild
	{
		get
		{
			return UnityEngine.Debug.isDebugBuild;
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool _condition)
	{
		if (!_condition)
		{
			UnityEngine.Debug.LogError("Assertion Failure");
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool _condition, string _message)
	{
		if (!_condition)
		{
			UnityEngine.Debug.LogError("Assertion Failure : " + _message);
		}
	}

	public static void Break()
	{
		UnityEngine.Debug.Break();
	}

	public static void Log(object message)
	{
		UnityEngine.Debug.Log(message);
	}

	public static void Log(object message, Object context)
	{
		UnityEngine.Debug.Log(message, context);
	}

	public static void LogError(object message)
	{
		UnityEngine.Debug.LogError(message);
	}

	public static void LogError(object message, Object context)
	{
		UnityEngine.Debug.LogError(message, context);
	}

	public static void LogWarning(object message)
	{
		UnityEngine.Debug.LogWarning(message);
	}

	public static void LogWarning(object message, Object context)
	{
		UnityEngine.Debug.LogWarning(message, context);
	}
}
