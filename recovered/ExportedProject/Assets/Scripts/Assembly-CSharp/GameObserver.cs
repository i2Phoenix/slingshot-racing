using UnityEngine;

public class GameObserver : MonoBehaviour
{
	private void Start()
	{
		OnGameRun();
	}

	private void OnApplicationPause(bool _pause)
	{
		if (!_pause)
		{
			OnGameRun();
		}
	}

	private void OnGameRun()
	{
		if (AppSettings.Version != Settings.LastVersion)
		{
			Settings.LastVersion = AppSettings.Version;
			Settings.PlayedCount = 0;
		}
		else
		{
			Settings.PlayedCount++;
		}
	}
}
