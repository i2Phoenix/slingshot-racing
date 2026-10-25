using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCircleManager : MonoBehaviour
{
	public static event Action serviceReadyEvent;

	public static event Action<string> serviceNotReadyEvent;

	public static event Action<string> playerAliasReceivedEvent;

	public static event Action<string> playerAliasFailedEvent;

	public static event Action<string> submitScoreFailedEvent;

	public static event Action submitScoreSucceededEvent;

	public static event Action<string> requestLeaderboardsFailedEvent;

	public static event Action<List<GameCircleLeaderboard>> requestLeaderboardsSucceededEvent;

	public static event Action<string> requestLocalPlayerScoreFailedEvent;

	public static event Action<string, string> requestLocalPlayerScoreSucceededEvent;

	public static event Action<string> requestScoresFailedEvent;

	public static event Action<GameCircleLeaderboard> requestScoresSucceededEvent;

	public static event Action<string> updateAchievementFailedEvent;

	public static event Action updateAchievementSucceededEvent;

	public static event Action<string> loadIconFailedEvent;

	public static event Action<string> loadIconSucceededEvent;

	public static event Action onAlreadySynchronizedEvent;

	public static event Action onConflictDeferralEvent;

	public static event Action onGameUploadSuccessEvent;

	public static event Action<string> onSynchronizeFailureEvent;

	public static event Action onNewGameDataEvent;

	public static event Action onPlayerCancelledEvent;

	public static event Action<string> onRevertFailureEvent;

	public static event Action onRevertedGameDataEvent;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void serviceReady(string empty)
	{
		GameCircle.requestLocalPlayerProfile();
		if (serviceReadyEvent != null)
		{
			serviceReadyEvent();
		}
	}

	public void serviceNotReady(string param)
	{
		if (serviceNotReadyEvent != null)
		{
			serviceNotReadyEvent(param);
		}
	}

	public void playerAliasReceived(string playerAlias)
	{
		if (playerAliasReceivedEvent != null)
		{
			playerAliasReceivedEvent(playerAlias);
		}
	}

	public void playerAliasFailed(string error)
	{
		if (playerAliasFailedEvent != null)
		{
			playerAliasFailedEvent(error);
		}
	}

	public void submitScoreFailed(string param)
	{
		if (submitScoreFailedEvent != null)
		{
			submitScoreFailedEvent(param);
		}
	}

	public void submitScoreSucceeded(string empty)
	{
		if (submitScoreSucceededEvent != null)
		{
			submitScoreSucceededEvent();
		}
	}

	public void requestLeaderboardsFailed(string param)
	{
		if (requestLeaderboardsFailedEvent != null)
		{
			requestLeaderboardsFailedEvent(param);
		}
	}

	public void requestLeaderboardsSucceeded(string json)
	{
		if (requestLeaderboardsSucceededEvent == null)
		{
			return;
		}
		List<GameCircleLeaderboard> list = new List<GameCircleLeaderboard>();
		ArrayList arrayList = json.arrayListFromJson();
		foreach (Hashtable item in arrayList)
		{
			list.Add(GameCircleLeaderboard.fromHashtable(item));
		}
		requestLeaderboardsSucceededEvent(list);
	}

	public void requestLocalPlayerScoreFailed(string param)
	{
		if (requestLocalPlayerScoreFailedEvent != null)
		{
			requestLocalPlayerScoreFailedEvent(param);
		}
	}

	public void requestLocalPlayerScoreSucceeded(string scoreInfo)
	{
		if (requestLocalPlayerScoreSucceededEvent != null)
		{
			string[] array = scoreInfo.Split(new string[1] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 2)
			{
				requestLocalPlayerScoreSucceededEvent(array[0], array[1]);
			}
		}
	}

	public void requestScoresFailed(string error)
	{
		if (requestScoresFailedEvent != null)
		{
			requestScoresFailedEvent(error);
		}
	}

	public void requestScoresSucceeded(string json)
	{
		if (requestScoresSucceededEvent != null)
		{
			GameCircleLeaderboard obj = GameCircleLeaderboard.fromHashtable(json.hashtableFromJson());
			requestScoresSucceededEvent(obj);
		}
	}

	public void updateAchievementFailed(string param)
	{
		if (updateAchievementFailedEvent != null)
		{
			updateAchievementFailedEvent(param);
		}
	}

	public void updateAchievementSucceeded(string empty)
	{
		if (updateAchievementSucceededEvent != null)
		{
			updateAchievementSucceededEvent();
		}
	}

	public void loadIconFailed(string param)
	{
		if (loadIconFailedEvent != null)
		{
			loadIconFailedEvent(param);
		}
	}

	public void loadIconSucceeded(string file)
	{
		if (loadIconSucceededEvent != null)
		{
			loadIconSucceededEvent(file);
		}
	}

	public void onAlreadySynchronized(string empty)
	{
		if (onAlreadySynchronizedEvent != null)
		{
			onAlreadySynchronizedEvent();
		}
	}

	public void onConflictDeferral(string empty)
	{
		if (onConflictDeferralEvent != null)
		{
			onConflictDeferralEvent();
		}
	}

	public void onGameUploadSuccess(string empty)
	{
		if (onGameUploadSuccessEvent != null)
		{
			onGameUploadSuccessEvent();
		}
	}

	public void onSynchronizeFailure(string error)
	{
		if (onSynchronizeFailureEvent != null)
		{
			onSynchronizeFailureEvent(error);
		}
	}

	public void onNewGameData(string empty)
	{
		if (onNewGameDataEvent != null)
		{
			onNewGameDataEvent();
		}
	}

	public void onPlayerCancelled(string empty)
	{
		if (onPlayerCancelledEvent != null)
		{
			onPlayerCancelledEvent();
		}
	}

	public void onRevertFailure(string error)
	{
		if (onRevertFailureEvent != null)
		{
			onRevertFailureEvent(error);
		}
	}

	public void onRevertedGameData(string empty)
	{
		if (onRevertedGameDataEvent != null)
		{
			onRevertedGameDataEvent();
		}
	}
}
