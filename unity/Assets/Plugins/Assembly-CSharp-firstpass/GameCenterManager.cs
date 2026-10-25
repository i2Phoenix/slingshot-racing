using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCenterManager : MonoBehaviour
{
	public static event Action<string> loadPlayerDataFailed;

	public static event Action<List<GameCenterPlayer>> playerDataLoaded;

	public static event Action playerAuthenticated;

	public static event Action<string> playerFailedToAuthenticate;

	public static event Action playerLoggedOut;

	public static event Action<string> profilePhotoLoaded;

	public static event Action<string> profilePhotoFailed;

	public static event Action<string> loadCategoryTitlesFailed;

	public static event Action<List<GameCenterLeaderboard>> categoriesLoaded;

	public static event Action<string> reportScoreFailed;

	public static event Action<string> reportScoreFinished;

	public static event Action<string> retrieveScoresFailed;

	public static event Action<List<GameCenterScore>> scoresLoaded;

	public static event Action<string> retrieveScoresForPlayerIdFailed;

	public static event Action<List<GameCenterScore>> scoresForPlayerIdLoaded;

	public static event Action<string> reportAchievementFailed;

	public static event Action<string> reportAchievementFinished;

	public static event Action<string> loadAchievementsFailed;

	public static event Action<List<GameCenterAchievement>> achievementsLoaded;

	public static event Action<string> resetAchievementsFailed;

	public static event Action resetAchievementsFinished;

	public static event Action<string> retrieveAchievementMetadataFailed;

	public static event Action<List<GameCenterAchievementMetadata>> achievementMetadataLoaded;

	public static event Action<string> selectChallengeablePlayerIDsDidFailEvent;

	public static event Action<ArrayList> selectChallengeablePlayerIDsDidFinishEvent;

	public static event Action<GameCenterChallenge> localPlayerDidSelectChallengeEvent;

	public static event Action<GameCenterChallenge> localPlayerDidCompleteChallengeEvent;

	public static event Action<GameCenterChallenge> remotePlayerDidCompleteChallengeEvent;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void loadPlayerDataDidFail(string error)
	{
		if (loadPlayerDataFailed != null)
		{
			loadPlayerDataFailed(error);
		}
	}

	public void loadPlayerDataDidLoad(string jsonFriendList)
	{
		List<GameCenterPlayer> obj = GameCenterPlayer.fromJSON(jsonFriendList);
		if (playerDataLoaded != null)
		{
			playerDataLoaded(obj);
		}
	}

	public void playerDidLogOut()
	{
		if (playerLoggedOut != null)
		{
			playerLoggedOut();
		}
	}

	public void playerDidAuthenticate()
	{
		if (playerAuthenticated != null)
		{
			playerAuthenticated();
		}
	}

	public void playerAuthenticationFailed(string error)
	{
		if (playerFailedToAuthenticate != null)
		{
			playerFailedToAuthenticate(error);
		}
	}

	public void loadProfilePhotoDidLoad(string path)
	{
		if (profilePhotoLoaded != null)
		{
			profilePhotoLoaded(path);
		}
	}

	public void loadProfilePhotoDidFail(string error)
	{
		if (profilePhotoFailed != null)
		{
			profilePhotoFailed(error);
		}
	}

	public void loadCategoryTitlesDidFail(string error)
	{
		if (loadCategoryTitlesFailed != null)
		{
			loadCategoryTitlesFailed(error);
		}
	}

	public void categoriesDidLoad(string jsonCategoryList)
	{
		List<GameCenterLeaderboard> obj = GameCenterLeaderboard.fromJSON(jsonCategoryList);
		if (categoriesLoaded != null)
		{
			categoriesLoaded(obj);
		}
	}

	public void reportScoreDidFail(string error)
	{
		if (reportScoreFailed != null)
		{
			reportScoreFailed(error);
		}
	}

	public void reportScoreDidFinish(string category)
	{
		if (reportScoreFinished != null)
		{
			reportScoreFinished(category);
		}
	}

	public void retrieveScoresDidFail(string category)
	{
		if (retrieveScoresFailed != null)
		{
			retrieveScoresFailed(category);
		}
	}

	public void retrieveScoresDidLoad(string jsonScoresList)
	{
		List<GameCenterScore> obj = GameCenterScore.fromJSON(jsonScoresList);
		if (scoresLoaded != null)
		{
			scoresLoaded(obj);
		}
	}

	public void retrieveScoresForPlayerIdDidFail(string error)
	{
		if (retrieveScoresForPlayerIdFailed != null)
		{
			retrieveScoresForPlayerIdFailed(error);
		}
	}

	public void retrieveScoresForPlayerIdDidLoad(string jsonScoresList)
	{
		List<GameCenterScore> obj = GameCenterScore.fromJSON(jsonScoresList);
		if (scoresForPlayerIdLoaded != null)
		{
			scoresForPlayerIdLoaded(obj);
		}
	}

	public void reportAchievementDidFail(string error)
	{
		if (reportAchievementFailed != null)
		{
			reportAchievementFailed(error);
		}
	}

	public void reportAchievementDidFinish(string identifier)
	{
		if (reportAchievementFinished != null)
		{
			reportAchievementFinished(identifier);
		}
	}

	public void loadAchievementsDidFail(string error)
	{
		if (loadAchievementsFailed != null)
		{
			loadAchievementsFailed(error);
		}
	}

	public void achievementsDidLoad(string jsonAchievmentList)
	{
		List<GameCenterAchievement> obj = GameCenterAchievement.fromJSON(jsonAchievmentList);
		if (achievementsLoaded != null)
		{
			achievementsLoaded(obj);
		}
	}

	public void resetAchievementsDidFail(string error)
	{
		if (resetAchievementsFailed != null)
		{
			resetAchievementsFailed(error);
		}
	}

	public void resetAchievementsDidFinish(string emptyString)
	{
		if (resetAchievementsFinished != null)
		{
			resetAchievementsFinished();
		}
	}

	public void retrieveAchievementsMetadataDidFail(string error)
	{
		if (retrieveAchievementMetadataFailed != null)
		{
			retrieveAchievementMetadataFailed(error);
		}
	}

	public void achievementMetadataDidLoad(string jsonAchievementDescriptionList)
	{
		List<GameCenterAchievementMetadata> obj = GameCenterAchievementMetadata.fromJSON(jsonAchievementDescriptionList);
		if (achievementMetadataLoaded != null)
		{
			achievementMetadataLoaded(obj);
		}
	}

	public void selectChallengeablePlayerIDsDidFail(string error)
	{
		if (selectChallengeablePlayerIDsDidFailEvent != null)
		{
			selectChallengeablePlayerIDsDidFailEvent(error);
		}
	}

	public void selectChallengeablePlayerIDsDidFinish(string json)
	{
		if (selectChallengeablePlayerIDsDidFinishEvent != null)
		{
			selectChallengeablePlayerIDsDidFinishEvent(json.arrayListFromJson());
		}
	}

	public void localPlayerDidSelectChallenge(string json)
	{
		if (localPlayerDidSelectChallengeEvent != null)
		{
			localPlayerDidSelectChallengeEvent(new GameCenterChallenge(json.hashtableFromJson()));
		}
	}

	public void localPlayerDidCompleteChallenge(string json)
	{
		if (localPlayerDidCompleteChallengeEvent != null)
		{
			localPlayerDidCompleteChallengeEvent(new GameCenterChallenge(json.hashtableFromJson()));
		}
	}

	public void remotePlayerDidCompleteChallenge(string json)
	{
		if (remotePlayerDidCompleteChallengeEvent != null)
		{
			remotePlayerDidCompleteChallengeEvent(new GameCenterChallenge(json.hashtableFromJson()));
		}
	}
}
