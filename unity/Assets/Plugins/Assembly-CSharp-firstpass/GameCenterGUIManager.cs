using System.Collections.Generic;
using UnityEngine;

public class GameCenterGUIManager : MonoBehaviour
{
	private List<GameCenterLeaderboard> leaderboards;

	private List<GameCenterAchievementMetadata> achievementMetadata;

	private void Start()
	{
		GameCenterManager.categoriesLoaded += (List<GameCenterLeaderboard> leaderboards) =>
		{
			this.leaderboards = leaderboards;
		};
		GameCenterManager.achievementMetadataLoaded += (List<GameCenterAchievementMetadata> achievementMetadata) =>
		{
			this.achievementMetadata = achievementMetadata;
		};
		GameCenterManager.playerAuthenticated += () =>
		{
			GameCenterBinding.loadProfilePhotoForLocalPlayer();
		};
	}

	private void OnGUI()
	{
		float num = 5f;
		float left = 5f;
		float num2 = ((Screen.width < 960 && Screen.height < 960) ? 160 : 320);
		float num3 = ((Screen.width < 960 && Screen.height < 960) ? 40 : 80);
		float num4 = num3 + 5f;
		if (GUI.Button(new Rect(left, num, num2, num3), "Authenticate"))
		{
			GameCenterBinding.authenticateLocalPlayer();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Load Achievement Metadata"))
		{
			GameCenterBinding.retrieveAchievementMetadata();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Get Raw Achievements"))
		{
			GameCenterBinding.getAchievements();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Post Achievement"))
		{
			if (achievementMetadata != null && achievementMetadata.Count > 0)
			{
				int num5 = Random.Range(2, 60);
				Debug.Log("sending percentComplete: " + num5);
				GameCenterBinding.reportAchievement(achievementMetadata[0].identifier, num5);
			}
			else
			{
				Debug.Log("you must load achievement metadata before you can post an achievement");
			}
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Issue Achievement Challenge"))
		{
			if (achievementMetadata != null && achievementMetadata.Count > 0)
			{
				string[] playerIds = new string[2] { "player1", "player2" };
				GameCenterBinding.issueAchievementChallenge(achievementMetadata[0].identifier, playerIds, "I challenge you");
			}
			else
			{
				Debug.Log("you must load achievement metadata before you can issue an achievement challenge");
			}
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Show Achievements"))
		{
			GameCenterBinding.showAchievements();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Reset Achievements"))
		{
			GameCenterBinding.resetAchievements();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Multiplayer Scene"))
		{
			Application.LoadLevel("GameCenterMultiplayerTestScene");
		}
		left = (float)Screen.width - num2 - 5f;
		num = 5f;
		if (GUI.Button(new Rect(left, num, num2, num3), "Get Player Alias"))
		{
			string text = GameCenterBinding.playerAlias();
			Debug.Log("Player alias: " + text);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Load Leaderboard Data"))
		{
			GameCenterBinding.loadLeaderboardTitles();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Post Score") && leaderboards != null && leaderboards.Count > 0)
		{
			Debug.Log("about to report a random score for leaderboard: " + leaderboards[0].leaderboardId);
			GameCenterBinding.reportScore(Random.Range(1, 99999), leaderboards[0].leaderboardId);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Issue Score Challenge"))
		{
			if (leaderboards != null && leaderboards.Count > 0)
			{
				string[] playerIds2 = new string[2] { "player1", "player2" };
				int num6 = Random.Range(1, 9999);
				GameCenterBinding.issueScoreChallenge(num6, 0L, leaderboards[0].leaderboardId, playerIds2, "Beat this score!");
			}
			else
			{
				Debug.Log("you must load your leaderboards before you can issue a score challenge");
			}
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Show Leaderboards"))
		{
			GameCenterBinding.showLeaderboardWithTimeScope(GameCenterLeaderboardTimeScope.AllTime);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Get Raw Score Data"))
		{
			if (leaderboards != null && leaderboards.Count > 0)
			{
				GameCenterBinding.retrieveScores(false, GameCenterLeaderboardTimeScope.AllTime, 1, 25, leaderboards[0].leaderboardId);
			}
			else
			{
				Debug.Log("Load leaderboard data before attempting to retrieve scores");
			}
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Get Scores for Me"))
		{
			if (leaderboards != null && leaderboards.Count > 0)
			{
				GameCenterBinding.retrieveScoresForPlayerId(GameCenterBinding.playerIdentifier(), leaderboards[0].leaderboardId);
			}
			else
			{
				Debug.Log("Load leaderboard data before attempting to retrieve scores");
			}
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Retrieve Friends"))
		{
			GameCenterBinding.retrieveFriends(true);
		}
	}
}
