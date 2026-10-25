using System.Collections;
using UnityEngine;

[AddComponentMenu("Debug/Profile Reporter")]
internal class ProfileReporter : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		StartCoroutine(UpdateLoop());
	}

	private IEnumerator UpdateLoop()
	{
		while (true)
		{
			yield return YieldCache.WaitForEndOfFrame;
		}
	}
}
