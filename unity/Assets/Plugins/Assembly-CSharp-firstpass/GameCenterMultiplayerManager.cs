using System;
using System.Collections.Generic;
using UnityEngine;

public class GameCenterMultiplayerManager : MonoBehaviour
{
	public static event Action matchmakerCancelled;

	public static event Action<string> matchmakerFailedEvent;

	public static event Action<int> matchmakerFoundMatch;

	public static event Action friendRequestControllerFinished;

	public static event Action<string> playerConnected;

	public static event Action<string> playerDisconnected;

	public static event Action<string> playerConnectionFailed;

	public static event Action inviteRequestWasReceived;

	public static event Action<string> playerBeganSpeaking;

	public static event Action<string> playerStoppedSpeaking;

	public static event Action<string> findMatchFailed;

	public static event Action<int> findMatchFinished;

	public static event Action<string> addPlayersToMatchFailed;

	public static event Action<string> findActivityFailed;

	public static event Action<int> findActivityFinished;

	public static event Action<string> findActivityForGroupFailed;

	public static event Action<int> findActivityForGroupFinished;

	public static event Action<string> retrieveMatchesBestScoresFailed;

	public static event Action<List<GameCenterScore>> retrieveMathesBestScoresFinished;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void matchmakerWasCancelled(string empty)
	{
		if (matchmakerCancelled != null)
		{
			matchmakerCancelled();
		}
	}

	public void matchmakerFailed(string error)
	{
		if (matchmakerFailedEvent != null)
		{
			matchmakerFailedEvent(error);
		}
	}

	public void matchmakerFoundMatchWithExpectedPlayerCount(string playerCount)
	{
		int obj = int.Parse(playerCount);
		if (matchmakerFoundMatch != null)
		{
			matchmakerFoundMatch(obj);
		}
	}

	public void friendRequestComposeViewControllerDidFinish(string empty)
	{
		if (friendRequestControllerFinished != null)
		{
			friendRequestControllerFinished();
		}
	}

	public void playerDidConnectToMatch(string playerId)
	{
		if (playerConnected != null)
		{
			playerConnected(playerId);
		}
	}

	public void playerDidDisconnectFromMatch(string playerId)
	{
		if (playerDisconnected != null)
		{
			playerDisconnected(playerId);
		}
	}

	public void connectionWithPlayerFailed(string playerId)
	{
		if (playerConnectionFailed != null)
		{
			playerConnectionFailed(playerId);
		}
	}

	public void inviteRequestReceived(string emtpy)
	{
		if (inviteRequestWasReceived != null)
		{
			inviteRequestWasReceived();
		}
	}

	public void playerIsSpeaking(string playerId)
	{
		if (playerBeganSpeaking != null)
		{
			playerBeganSpeaking(playerId);
		}
	}

	public void playerIsSilent(string playerId)
	{
		if (playerStoppedSpeaking != null)
		{
			playerStoppedSpeaking(playerId);
		}
	}

	public void findMatchProgramaticallyFailed(string error)
	{
		if (findMatchFailed != null)
		{
			findMatchFailed(error);
		}
	}

	public void findMatchProgramaticallyFinishedWithExpectedPlayerCount(string playerCount)
	{
		int obj = int.Parse(playerCount);
		if (findMatchFinished != null)
		{
			findMatchFinished(obj);
		}
	}

	public void addPlayersToCurrentMatchFailed(string error)
	{
		if (addPlayersToMatchFailed != null)
		{
			addPlayersToMatchFailed(error);
		}
	}

	public void findAllActivityFailed(string error)
	{
		if (findActivityFailed != null)
		{
			findActivityFailed(error);
		}
	}

	public void findAllActivityFinished(string activity)
	{
		int obj = int.Parse(activity);
		if (findActivityFinished != null)
		{
			findActivityFinished(obj);
		}
	}

	public void findAllActivityForPlayerGroupFailed(string error)
	{
		if (findActivityForGroupFailed != null)
		{
			findActivityForGroupFailed(error);
		}
	}

	public void findAllActivityForPlayerGroupFinished(string activity)
	{
		int obj = int.Parse(activity);
		if (findActivityForGroupFinished != null)
		{
			findActivityForGroupFinished(obj);
		}
	}

	public void retrieveMatchesBestScoresDidFail(string error)
	{
		if (retrieveMatchesBestScoresFailed != null)
		{
			retrieveMatchesBestScoresFailed(error);
		}
	}

	public void retrieveMatchesBestScoresDidFinish(string jsonScores)
	{
		List<GameCenterScore> obj = GameCenterScore.fromJSON(jsonScores);
		if (retrieveMathesBestScoresFinished != null)
		{
			retrieveMathesBestScoresFinished(obj);
		}
	}
}
