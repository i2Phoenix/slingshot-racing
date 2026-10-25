using System.Collections;
using System.Collections.Generic;

public class GameCircleScore
{
	public string playerAlias;

	public int rank;

	public string scoreString;

	public long scoreValue;

	public static GameCircleScore fromHashtable(Hashtable ht)
	{
		GameCircleScore gameCircleScore = new GameCircleScore();
		gameCircleScore.playerAlias = ht["playerAlias"].ToString();
		gameCircleScore.rank = int.Parse(ht["rank"].ToString());
		gameCircleScore.scoreString = ht["scoreString"].ToString();
		gameCircleScore.scoreValue = long.Parse(ht["scoreValue"].ToString());
		return gameCircleScore;
	}

	public static List<GameCircleScore> fromArrayList(ArrayList list)
	{
		List<GameCircleScore> list2 = new List<GameCircleScore>();
		foreach (Hashtable item in list)
		{
			list2.Add(fromHashtable(item));
		}
		return list2;
	}

	public override string ToString()
	{
		return string.Format("playerAlias: {0}, rank: {1}, scoreString: {2}", playerAlias, rank, scoreString);
	}
}
