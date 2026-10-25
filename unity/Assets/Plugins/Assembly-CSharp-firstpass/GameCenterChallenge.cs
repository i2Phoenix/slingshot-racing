using System;
using System.Collections;

public class GameCenterChallenge
{
	public string issuingPlayerID;

	public string receivingPlayerID;

	public GameCenterChallengeState state;

	public DateTime issueDate;

	public DateTime completionDate;

	public string message;

	public GameCenterScore score;

	public GameCenterAchievement achievement;

	public GameCenterChallenge(Hashtable ht)
	{
		if (ht.Contains("issuingPlayerID"))
		{
			issuingPlayerID = ht["issuingPlayerID"] as string;
		}
		if (ht.Contains("receivingPlayerID"))
		{
			receivingPlayerID = ht["receivingPlayerID"] as string;
		}
		if (ht.Contains("state"))
		{
			int num = int.Parse(ht["state"].ToString());
			state = (GameCenterChallengeState)num;
		}
		if (ht.Contains("issueDate"))
		{
			double value = double.Parse(ht["issueDate"].ToString());
			issueDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(value);
		}
		if (ht.Contains("completionDate"))
		{
			double value2 = double.Parse(ht["completionDate"].ToString());
			completionDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(value2);
		}
		if (ht.Contains("message"))
		{
			message = ht["message"] as string;
		}
		if (ht.Contains("score"))
		{
			score = new GameCenterScore(ht["score"] as Hashtable);
		}
		if (ht.Contains("achievement"))
		{
			achievement = new GameCenterAchievement(ht["achievement"] as Hashtable);
		}
	}

	public override string ToString()
	{
		return string.Format("<Challenge> issuingPlayerID: {0}, receivingPlayerID: {1}, message: {2}, state: {3}, score: {4}, achievement: {5}", issuingPlayerID, receivingPlayerID, message, state, score, achievement);
	}
}
