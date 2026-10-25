using System.Collections.Generic;
using UnityEngine;

public class GameCircleEventListener : MonoBehaviour
{
	private void OnEnable()
	{
		GameCircleManager.serviceReadyEvent += serviceReadyEvent;
		GameCircleManager.serviceNotReadyEvent += serviceNotReadyEvent;
		GameCircleManager.playerAliasReceivedEvent += playerAliasReceivedEvent;
		GameCircleManager.playerAliasFailedEvent += playerAliasFailedEvent;
		GameCircleManager.submitScoreFailedEvent += submitScoreFailedEvent;
		GameCircleManager.submitScoreSucceededEvent += submitScoreSucceededEvent;
		GameCircleManager.requestLeaderboardsFailedEvent += requestLeaderboardsFailedEvent;
		GameCircleManager.requestLeaderboardsSucceededEvent += requestLeaderboardsSucceededEvent;
		GameCircleManager.requestLocalPlayerScoreFailedEvent += requestLocalPlayerScoreFailedEvent;
		GameCircleManager.requestLocalPlayerScoreSucceededEvent += requestLocalPlayerScoreSucceededEvent;
		GameCircleManager.requestScoresFailedEvent += requestScoreFailedEvent;
		GameCircleManager.requestScoresSucceededEvent += requestScoreSucceededEvent;
		GameCircleManager.updateAchievementFailedEvent += updateAchievementFailedEvent;
		GameCircleManager.updateAchievementSucceededEvent += updateAchievementSucceededEvent;
		GameCircleManager.loadIconFailedEvent += loadIconFailedEvent;
		GameCircleManager.loadIconSucceededEvent += loadIconSucceededEvent;
		GameCircleManager.onAlreadySynchronizedEvent += onAlreadySynchronizedEvent;
		GameCircleManager.onConflictDeferralEvent += onConflictDeferralEvent;
		GameCircleManager.onGameUploadSuccessEvent += onGameUploadSuccessEvent;
		GameCircleManager.onSynchronizeFailureEvent += onSynchronizeFailureEvent;
		GameCircleManager.onNewGameDataEvent += onNewGameDataEvent;
		GameCircleManager.onPlayerCancelledEvent += onPlayerCancelledEvent;
		GameCircleManager.onRevertFailureEvent += onRevertFailureEvent;
		GameCircleManager.onRevertedGameDataEvent += onRevertedGameDataEvent;
	}

	private void OnDisable()
	{
		GameCircleManager.serviceReadyEvent -= serviceReadyEvent;
		GameCircleManager.serviceNotReadyEvent -= serviceNotReadyEvent;
		GameCircleManager.playerAliasReceivedEvent -= playerAliasReceivedEvent;
		GameCircleManager.playerAliasFailedEvent -= playerAliasFailedEvent;
		GameCircleManager.submitScoreFailedEvent -= submitScoreFailedEvent;
		GameCircleManager.submitScoreSucceededEvent -= submitScoreSucceededEvent;
		GameCircleManager.requestLeaderboardsFailedEvent -= requestLeaderboardsFailedEvent;
		GameCircleManager.requestLeaderboardsSucceededEvent -= requestLeaderboardsSucceededEvent;
		GameCircleManager.requestLocalPlayerScoreFailedEvent -= requestLocalPlayerScoreFailedEvent;
		GameCircleManager.requestLocalPlayerScoreSucceededEvent -= requestLocalPlayerScoreSucceededEvent;
		GameCircleManager.requestScoresFailedEvent -= requestScoreFailedEvent;
		GameCircleManager.requestScoresSucceededEvent -= requestScoreSucceededEvent;
		GameCircleManager.updateAchievementFailedEvent -= updateAchievementFailedEvent;
		GameCircleManager.updateAchievementSucceededEvent -= updateAchievementSucceededEvent;
		GameCircleManager.loadIconFailedEvent -= loadIconFailedEvent;
		GameCircleManager.loadIconSucceededEvent -= loadIconSucceededEvent;
		GameCircleManager.onAlreadySynchronizedEvent -= onAlreadySynchronizedEvent;
		GameCircleManager.onConflictDeferralEvent -= onConflictDeferralEvent;
		GameCircleManager.onGameUploadSuccessEvent -= onGameUploadSuccessEvent;
		GameCircleManager.onSynchronizeFailureEvent -= onSynchronizeFailureEvent;
		GameCircleManager.onNewGameDataEvent -= onNewGameDataEvent;
		GameCircleManager.onPlayerCancelledEvent -= onPlayerCancelledEvent;
		GameCircleManager.onRevertFailureEvent -= onRevertFailureEvent;
		GameCircleManager.onRevertedGameDataEvent -= onRevertedGameDataEvent;
	}

	private void serviceReadyEvent()
	{
		Debug.Log("serviceReadyEvent");
	}

	private void serviceNotReadyEvent(string param)
	{
		Debug.Log("serviceNotReadyEvent: " + param);
	}

	private void playerAliasReceivedEvent(string playerAlias)
	{
		Debug.Log("playerAliasReceivedEvent: " + playerAlias);
	}

	private void playerAliasFailedEvent(string error)
	{
		Debug.Log("playerAliasFailedEvent: " + error);
	}

	private void submitScoreFailedEvent(string param)
	{
		Debug.Log("submitScoreFailedEvent: " + param);
	}

	private void submitScoreSucceededEvent()
	{
		Debug.Log("submitScoreSucceededEvent");
	}

	private void requestLeaderboardsFailedEvent(string param)
	{
		Debug.Log("requestLeaderboardsFailedEvent: " + param);
	}

	private void requestLeaderboardsSucceededEvent(List<GameCircleLeaderboard> leaderboards)
	{
		Debug.Log("requestLeaderboardsSucceededEvent");
		foreach (GameCircleLeaderboard leaderboard in leaderboards)
		{
			Debug.Log(leaderboard);
		}
	}

	private void requestLocalPlayerScoreFailedEvent(string error)
	{
		Debug.Log("requestLocalPlayerScoreFailedEvent: " + error);
	}

	private void requestLocalPlayerScoreSucceededEvent(string rank, string score)
	{
		Debug.Log("requestLocalPlayerScoreSucceededEvent with rank: " + rank + ", score: " + score);
	}

	private void requestScoreFailedEvent(string error)
	{
		Debug.Log("requestScoreFailedEvent: " + error);
	}

	private void requestScoreSucceededEvent(GameCircleLeaderboard leaderboard)
	{
		Debug.Log("requestScoreSucceededEvent: " + leaderboard);
		foreach (GameCircleScore score in leaderboard.scores)
		{
			Debug.Log(score);
		}
	}

	private void updateAchievementFailedEvent(string param)
	{
		Debug.Log("updateAchievementFailedEvent: " + param);
	}

	private void updateAchievementSucceededEvent()
	{
		Debug.Log("updateAchievementSucceededEvent");
	}

	private void loadIconFailedEvent(string param)
	{
		Debug.Log("loadIconFailedEvent: " + param);
	}

	private void loadIconSucceededEvent(string file)
	{
		Debug.Log("loadIconSucceededEvent: " + file);
	}

	private void onAlreadySynchronizedEvent()
	{
		Debug.Log("onAlreadySynchronizedEvent");
	}

	private void onConflictDeferralEvent()
	{
		Debug.Log("onConflictDeferralEvent");
	}

	private void onGameUploadSuccessEvent()
	{
		Debug.Log("onGameUploadSuccessEvent");
	}

	private void onSynchronizeFailureEvent(string error)
	{
		Debug.Log("onSynchronizeFailureEvent: " + error);
	}

	private void onNewGameDataEvent()
	{
		Debug.Log("onNewGameDataEvent");
	}

	private void onPlayerCancelledEvent()
	{
		Debug.Log("onPlayerCancelledEvent");
	}

	private void onRevertFailureEvent(string error)
	{
		Debug.Log("onRevertFailureEvent: " + error);
	}

	private void onRevertedGameDataEvent()
	{
		Debug.Log("onRevertedGameDataEvent");
	}
}
