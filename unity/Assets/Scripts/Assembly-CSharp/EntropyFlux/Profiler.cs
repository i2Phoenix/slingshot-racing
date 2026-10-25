using System.Diagnostics;

namespace EntropyFlux
{
	internal class Profiler
	{
		public class Timer
		{
			public string m_name;

			public bool m_started;

			public long m_startTime;

			public long m_peakTime;

			public long m_lastPeakTime;

			public long m_maxPeakTime;

			public long m_time;

			public long m_lastTime;

			public long m_totalTime;

			public int m_count;

			public int m_lastCount;

			public int m_totalCount;
		}

		public static bool Enabled
		{
			get
			{
				return false;
			}
		}

		[Conditional("ENABLE_PROFILER_REPLACEMENT")]
		public static void BeginSample(string name)
		{
		}

		[Conditional("ENABLE_PROFILER_REPLACEMENT")]
		public static void BeginSample(Timer timer)
		{
		}

		[Conditional("ENABLE_PROFILER_REPLACEMENT")]
		public static void EndSample()
		{
		}

		[Conditional("ENABLE_PROFILER_REPLACEMENT")]
		public static void Update()
		{
		}
	}
}
