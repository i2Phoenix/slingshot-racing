using System;
using UnityEngine;

[Serializable]
public class RaceCampaign : ScriptableObject
{
	public enum Type
	{
		SinglePlayer = 0,
		Multiplayer = 1,
		TurnBased = 2
	}

	public enum EventUnlockType
	{
		None = 0,
		LockLast = 1
	}

	public string m_displayName;

	public Type m_type;

	public EventUnlockType m_eventUnlockType;

	public RaceTour[] m_tours;

	public LeaderboardInfo m_totalNutsLeaderboard;

	public string DisplayName
	{
		get
		{
			return m_displayName;
		}
	}

	public static int GetPointsForPosition(int _position)
	{
		switch (_position)
		{
		case 1:
			return 3;
		case 2:
			return 2;
		case 3:
			return 1;
		default:
			return 0;
		}
	}

	public static int GetPointsForBonus()
	{
		return 0;
	}
}
