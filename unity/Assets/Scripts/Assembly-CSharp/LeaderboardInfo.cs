using UnityEngine;

public class LeaderboardInfo : ScriptableObject
{
	public enum SortOrder
	{
		LowToHigh = 0,
		HighToLow = 1
	}

	public string m_gameCenterID;

	public string m_googleID;

	public SortOrder m_sortOrder;

	public string ID
	{
		get
		{
			return string.Empty;
		}
	}
}
