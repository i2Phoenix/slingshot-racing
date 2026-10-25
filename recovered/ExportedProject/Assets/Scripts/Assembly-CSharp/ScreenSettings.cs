using UnityEngine;

public static class ScreenSettings
{
	public static int AntiAliasingMode { get; set; }

	public static bool DisableAntiAliasing { get; set; }

	public static void Apply()
	{
		QualitySettings.antiAliasing = ((!DisableAntiAliasing) ? AntiAliasingMode : 0);
	}
}
