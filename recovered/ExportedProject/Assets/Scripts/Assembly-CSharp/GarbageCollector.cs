using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Core/Garbage Collector")]
public class GarbageCollector : MonoBehaviour
{
	public float m_frequency = 1f;

	private void Awake()
	{
		object[] array = new object[1024];
		for (int i = 0; i < 1024; i++)
		{
			array[i] = new byte[1024];
		}
	}

	private IEnumerator Start()
	{
		WaitForSeconds waitForFrequency = new WaitForSeconds(m_frequency);
		while (true)
		{
			yield return waitForFrequency;
			yield return YieldCache.WaitForFixedUpdate;
			yield return YieldCache.WaitForEndOfFrame;
			yield return YieldCache.WaitForEndOfFrame;
			GC.Collect(0);
		}
	}
}
