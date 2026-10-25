using System;
using System.Collections;
using System.Collections.Generic;

public class GKTurnBasedMatch
{
	public string matchId;

	public GKTurnBasedMatchStatus status;

	public List<GKTurnBasedParticipant> participants;

	public DateTime creationDate;

	public GKTurnBasedParticipant currentParticipant;

	public string message;

	public GKTurnBasedMatch(Hashtable ht)
	{
		if (ht.Contains("matchId"))
		{
			matchId = ht["matchId"].ToString();
		}
		if (ht.Contains("status"))
		{
			status = (GKTurnBasedMatchStatus)int.Parse(ht["status"].ToString());
		}
		if (ht.Contains("message"))
		{
			message = ht["message"].ToString();
		}
		if (ht.Contains("creationDate"))
		{
			double value = double.Parse(ht["creationDate"].ToString());
			creationDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(value);
		}
		if (ht.Contains("participants"))
		{
			ArrayList arrayList = ht["participants"] as ArrayList;
			participants = new List<GKTurnBasedParticipant>(arrayList.Count);
			foreach (Hashtable item in arrayList)
			{
				if (item != null)
				{
					participants.Add(new GKTurnBasedParticipant(item));
				}
				else
				{
					participants.Add(new GKTurnBasedParticipant());
				}
			}
		}
		if (ht.Contains("currentParticipant"))
		{
			currentParticipant = new GKTurnBasedParticipant(ht["currentParticipant"] as Hashtable);
		}
	}

	public static List<GKTurnBasedMatch> fromJSON(string json)
	{
		List<GKTurnBasedMatch> list = new List<GKTurnBasedMatch>();
		ArrayList arrayList = json.arrayListFromJson();
		foreach (Hashtable item in arrayList)
		{
			list.Add(new GKTurnBasedMatch(item));
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<GKTurnBasedMatch> matchId: {0}, status: {1}, participants: {2}, creationDate: {3}, currentParticipant: {4}, message: {5}", matchId, status, participants, creationDate, currentParticipant, message);
	}
}
