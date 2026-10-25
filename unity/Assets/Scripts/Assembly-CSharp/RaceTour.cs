using System;
using UnityEngine;

[Serializable]
public class RaceTour : ScriptableObject
{
	public enum Type
	{
		Standard = 0,
		Tutorial = 1
	}

	public int m_pointsToUnlock;

	public int m_major;

	public int m_minor;

	public Type m_type;

	public RaceEvent[] m_races = new RaceEvent[0];

	public AchievementInfo m_allCompleteAchievement;

	public AchievementInfo m_allMasteredAchievement;

	public string DisplayName
	{
		get
		{
			return Language.Get("ID_TOUR_" + base.name);
		}
	}

	public string DisplayRaceName(RaceEvent _race)
	{
		for (int i = 0; i < m_races.Length; i++)
		{
			if (m_races[i] == _race)
			{
				string arg = string.Format(Language.Get("ID_TourRaceName"), i + 1);
				return string.Format("{0} - {1}", DisplayName, arg);
			}
		}
		return DisplayName;
	}

	public string GetFullDescription(bool _locked)
	{
		string text = ((!_locked) ? (string.Format("{0}-{1}: ", m_major, m_minor) + DisplayName + "\n\n") : string.Empty);
		return text + GetDescription(_locked);
	}

	public string GetDescription(bool _locked)
	{
		if (_locked)
		{
			return string.Format(Language.Get("ID_Description_LockedTour"), m_pointsToUnlock);
		}
		return string.Empty;
	}

	public RaceEvent FindRace(string _name)
	{
		RaceEvent[] races = m_races;
		foreach (RaceEvent raceEvent in races)
		{
			if (raceEvent.name == _name)
			{
				return raceEvent;
			}
		}
		return null;
	}
}
