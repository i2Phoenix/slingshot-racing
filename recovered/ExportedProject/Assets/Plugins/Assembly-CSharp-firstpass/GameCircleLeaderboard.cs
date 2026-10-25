using System.Collections;
using System.Collections.Generic;

public class GameCircleLeaderboard
{
	public string name;

	public string id;

	public string displayText;

	public string scoreFormat;

	public List<GameCircleScore> scores = new List<GameCircleScore>();

	public static GameCircleLeaderboard fromHashtable(Hashtable ht)
	{
		GameCircleLeaderboard gameCircleLeaderboard = new GameCircleLeaderboard();
		gameCircleLeaderboard.name = ht["name"].ToString();
		gameCircleLeaderboard.id = ht["id"].ToString();
		gameCircleLeaderboard.displayText = ht["displayText"].ToString();
		gameCircleLeaderboard.scoreFormat = ht["scoreFormat"].ToString();
		if (ht.ContainsKey("scores") && ht["scores"] is ArrayList)
		{
			ArrayList list = ht["scores"] as ArrayList;
			gameCircleLeaderboard.scores = GameCircleScore.fromArrayList(list);
		}
		return gameCircleLeaderboard;
	}

	public override string ToString()
	{
		return string.Format("name: {0}, id: {1}, displayText: {2}, scoreFormat: {3}", name, id, displayText, scoreFormat);
	}
}
