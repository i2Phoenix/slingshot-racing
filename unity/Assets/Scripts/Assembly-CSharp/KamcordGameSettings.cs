using UnityEngine;

public class KamcordGameSettings : MonoBehaviour
{
	public static bool CanBeEnabled
	{
		get
		{
			return false;
		}
	}

	public static void Init()
	{
		if (!CanBeEnabled)
		{
			Settings.KamcordEnabled = false;
		}
		ScreenSettings.DisableAntiAliasing = Settings.KamcordEnabled;
		Debug.Log("AA disabled is: " + ScreenSettings.DisableAntiAliasing);
	}
}
