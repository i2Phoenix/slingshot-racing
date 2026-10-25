using System.IO;
using UnityEngine;

public class GameCircleUIManager : MonoBehaviour
{
	private void OnGUI()
	{
		float num = 5f;
		float left = 5f;
		float num2 = 320f;
		float num3 = 80f;
		float num4 = num3 + 10f;
		if (GUI.Button(new Rect(left, num, num2, num3), "Init"))
		{
			GameCircle.init(false);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Submit Score"))
		{
			GameCircle.submitScore("leaderboard_1", Random.Range(10, 99999));
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Request Leaderboards"))
		{
			GameCircle.requestLeaderboards();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Show Leaderboard"))
		{
			GameCircle.showLeaderboardsOverlay();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Request Local Players Score"))
		{
			GameCircle.requestLocalPlayerScore("leaderboard_1", GameCircleLeaderboardScope.GlobalAllTime);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Request All Scores"))
		{
			GameCircle.requestScores("leaderboard_1", GameCircleLeaderboardScope.GlobalAllTime);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Do Not Auto Unpack Synced Data"))
		{
			GameCircle.setShouldAutoUnpackSyncedData(false);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Manually Unpack Synced Data"))
		{
			GameCircle.whisperSyncUnpackNewMultiFileGameData();
		}
		left = (float)Screen.width - num2 - 5f;
		num = 5f;
		if (GUI.Button(new Rect(left, num, num2, num3), "Update Achievement"))
		{
			GameCircle.updateAchievementProgress("achievement_1", Random.Range(10, 100));
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Show Achievements Overlay"))
		{
			GameCircle.showAchievementsOverlay();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Set Popup Location"))
		{
			GameCircle.setPopUpLocation(GameCirclePopupLocation.BOTTOM_RIGHT);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Load Icon"))
		{
			GameCircle.loadIcon("achievement_1");
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Whisper Sync Synchronize Progress"))
		{
			string text = Path.Combine(GameCircle.rootWhisperSyncFolder, "file.txt");
			string contents = "blah blah blah blah text";
			File.WriteAllText(text, contents);
			Debug.Log("dumping file to: " + text);
			GameCircle.whisperSyncSynchronizeProgress("description_of_progress", "txt", GameCircleConflictStrategy.AUTO_RESOLVE_TO_CLOUD);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Dump Sync Directory To Log"))
		{
			string[] files = Directory.GetFiles(GameCircle.rootWhisperSyncFolder);
			foreach (string text2 in files)
			{
				Debug.Log(string.Format("[{0}] {1}", text2, File.ReadAllText(text2)));
			}
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Whisper Sync Synchronize"))
		{
			GameCircle.whisperSyncSynchronize();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Whisper Sync Request Revert"))
		{
			GameCircle.whisperSyncRequestRevert();
		}
	}
}
