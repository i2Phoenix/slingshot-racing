using System.Diagnostics;
using UnityEngine;

public static class MemProfiler
{
	public static bool ProduceErrors { get; set; }

	public static bool IsSupported
	{
		get
		{
			return false;
		}
	}

	static MemProfiler()
	{
	}

	[Conditional("ENABLE_MEM_PROFILE")]
	public static void BeginSample(string _name)
	{
	}

	[Conditional("ENABLE_MEM_PROFILE")]
	public static void BeginSample(string _name, Object _object)
	{
	}

	[Conditional("ENABLE_MEM_PROFILE")]
	public static void UpdateSample(string _name)
	{
	}

	[Conditional("ENABLE_MEM_PROFILE")]
	public static void UpdateSample(string _name, Object _object)
	{
	}

	[Conditional("ENABLE_MEM_PROFILE")]
	public static void EndSample()
	{
	}
}
