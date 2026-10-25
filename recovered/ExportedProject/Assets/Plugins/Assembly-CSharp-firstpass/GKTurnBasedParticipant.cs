using System;
using System.Collections;
using System.Collections.Generic;

public class GKTurnBasedParticipant
{
	public string playerId;

	public GKTurnBasedParticipantStatus status;

	public DateTime lastTurnDate;

	public GKTurnBasedMatchOutcome matchOutcome;

	public GKTurnBasedParticipant()
	{
	}

	public GKTurnBasedParticipant(Hashtable ht)
	{
		if (ht != null)
		{
			if (ht.Contains("playerId"))
			{
				playerId = ht["playerId"].ToString();
			}
			if (ht.Contains("status"))
			{
				status = (GKTurnBasedParticipantStatus)int.Parse(ht["status"].ToString());
			}
			if (ht.Contains("matchOutcome"))
			{
				matchOutcome = (GKTurnBasedMatchOutcome)int.Parse(ht["matchOutcome"].ToString());
			}
			if (ht.Contains("lastTurnDate"))
			{
				double value = double.Parse(ht["lastTurnDate"].ToString());
				lastTurnDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(value);
			}
		}
	}

	public static List<GKTurnBasedParticipant> fromJSON(string json)
	{
		List<GKTurnBasedParticipant> list = new List<GKTurnBasedParticipant>();
		ArrayList arrayList = json.arrayListFromJson();
		foreach (Hashtable item in arrayList)
		{
			list.Add(new GKTurnBasedParticipant(item));
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<Participant> playerId: {0}, status: {1}, matchOutcome: {2}, lastTurnDate: {3}", playerId, status, matchOutcome, lastTurnDate);
	}
}
