using System;
using UnityEngine;

[AddComponentMenu("Debug/Debug Garbage Collection")]
public class DebugGarbageCollection : MonoBehaviour
{
	private int[] m_collectionCounts;

	private long m_gcMemory;

	private uint m_unityMemory;

	private void Start()
	{
		m_collectionCounts = new int[GC.MaxGeneration + 1];
		for (int i = 0; i < m_collectionCounts.Length; i++)
		{
			m_collectionCounts[i] = GC.CollectionCount(i);
		}
		m_gcMemory = GC.GetTotalMemory(false);
		m_unityMemory = UnityEngine.Profiling.Profiler.usedHeapSize;
	}

	private void LateUpdate()
	{
		long totalMemory = GC.GetTotalMemory(false);
		uint usedHeapSize = UnityEngine.Profiling.Profiler.usedHeapSize;
		bool flag = false;
		for (int i = 0; i < m_collectionCounts.Length; i++)
		{
			int num = GC.CollectionCount(i);
			if (m_collectionCounts[i] < num)
			{
				int num2 = num - m_collectionCounts[i];
				Debug.LogWarning(string.Format("{0} Garbage Collection(s) at {1}s (Gen {2})", num2, Time.time, i));
				m_collectionCounts[i] = num;
				flag = true;
			}
		}
		if (flag)
		{
			if (m_gcMemory > totalMemory)
			{
				Debug.LogWarning(string.Format("GC mem delta {0}", m_gcMemory - totalMemory));
			}
			if (m_unityMemory > usedHeapSize)
			{
				Debug.LogWarning(string.Format("Unity mem delta {0}", m_unityMemory - usedHeapSize));
			}
		}
		m_gcMemory = totalMemory;
		m_unityMemory = usedHeapSize;
	}
}
