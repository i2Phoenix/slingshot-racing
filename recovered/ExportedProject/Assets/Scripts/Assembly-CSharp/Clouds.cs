using UnityEngine;

public class Clouds : MonoBehaviour
{
	private void Start()
	{
		if (!PlatformSettings.EnableClouds)
		{
			base.gameObject.SetActiveRecursively(false);
		}
	}
}
