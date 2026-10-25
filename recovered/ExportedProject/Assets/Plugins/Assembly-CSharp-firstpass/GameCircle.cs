using UnityEngine;

public class GameCircle
{
	private static AndroidJavaObject _plugin;

	public static string rootWhisperSyncFolder;

	static GameCircle()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.amazon.GameCirclePlugin"))
			{
				_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			}
			rootWhisperSyncFolder = string.Format("/data/data/{0}", _plugin.Get<string>("packageName"));
		}
	}

	public static void init(bool hasNoLocalGameProgress)
	{
		init(hasNoLocalGameProgress, true, true, true);
	}

	public static void init(bool hasNoLocalGameProgress, bool supportsLeaderboards, bool supportsAchievements, bool supportsWhisperSync)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("init", hasNoLocalGameProgress, supportsLeaderboards, supportsAchievements, supportsWhisperSync);
		}
	}

	public static void requestLocalPlayerProfile()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("requestLocalPlayerProfile");
		}
	}

	public static void setShouldAutoUnpackSyncedData(bool shouldAutoUnpackSyncedData)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("setShouldAutoUnpackSyncedData", shouldAutoUnpackSyncedData);
		}
	}

	public static void submitScore(string leaderboardId, long score)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("submitScore", leaderboardId, score);
		}
	}

	public static void showLeaderboardsOverlay()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("showLeaderboardsOverlay");
		}
	}

	public static void requestLeaderboards()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("requestLeaderboards");
		}
	}

	public static void requestLocalPlayerScore(string leaderboardId, GameCircleLeaderboardScope scope)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("requestLocalPlayerScore", leaderboardId, (int)scope);
		}
	}

	public static void requestScores(string leaderboardId, GameCircleLeaderboardScope scope)
	{
		requestScores(leaderboardId, scope, 1, 1000);
	}

	public static void requestScores(string leaderboardId, GameCircleLeaderboardScope scope, int startRank, int count)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("requestScores", leaderboardId, (int)scope, startRank, count);
		}
	}

	public static void updateAchievementProgress(string achievementId, float progress)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("updateAchievementProgress", achievementId, progress, string.Empty);
		}
	}

	public static void setPopUpLocation(GameCirclePopupLocation location)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("setPopUpLocation", location.ToString());
		}
	}

	public static void showAchievementsOverlay()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("showAchievementsOverlay");
		}
	}

	public static void loadIcon(string achievementId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("loadIcon", achievementId);
		}
	}

	public static void whisperSyncSynchronizeProgress(string description)
	{
		whisperSyncSynchronizeProgress(description, string.Empty, GameCircleConflictStrategy.AUTO_RESOLVE_TO_CLOUD);
	}

	public static void whisperSyncSynchronizeProgress(string description, string filenameExtensionFilter, GameCircleConflictStrategy conflictStrategy)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("whisperSyncSynchronizeProgress", description, filenameExtensionFilter, conflictStrategy.ToString());
		}
	}

	public static void whisperSyncSynchronize()
	{
		whisperSyncSynchronize(GameCircleConflictStrategy.AUTO_RESOLVE_TO_CLOUD);
	}

	public static void whisperSyncSynchronize(GameCircleConflictStrategy conflictStrategy)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("whisperSyncSynchronize", conflictStrategy.ToString());
		}
	}

	public static void whisperSyncRequestRevert()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("whisperSyncRequestRevert");
		}
	}

	public static void whisperSyncUnpackNewMultiFileGameData()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("unpackNewMultiFileGameData");
		}
	}
}
