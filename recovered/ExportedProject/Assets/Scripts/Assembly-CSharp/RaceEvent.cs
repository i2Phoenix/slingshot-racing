using System;
using UnityEngine;

public class RaceEvent : ScriptableObject
{
	public enum ObjectiveType
	{
		RacePosition = 0,
		Pickups = 1,
		Tutorial = 2,
		Overtake = 3,
		TimeTrial = 4,
		Survival = 5,
		LastStanding = 6,
		HeadsUp = 7
	}

	public enum PointsType
	{
		Position = 0,
		BestLap = 1,
		TotalTime = 2,
		InvTotalTime = 3,
		None = 4
	}

	public enum GhostType
	{
		Gold = 0
	}

	[Flags]
	public enum RunFlags
	{
		RecordPlayerGhost = 1,
		SameStartPosition = 2,
		NoGrade = 4,
		NoHelp = 8,
		NoMidRaceRestart = 0x10,
		DontTrackTournamentPoints = 0x20,
		ReturnToMainMenuOnSuspend = 0x40,
		NoRaceReplay = 0x80,
		DontAddRacePoints = 0x100,
		NoUpgrades = 0x200,
		NoWatchReplay = 0x400
	}

	public struct PlayerData
	{
		public string m_name;

		public int m_playerIndex;

		public int m_gridIndex;

		public bool m_localPlayer;

		public byte[] m_ghostData;
	}

	public RaceLayout m_layout;

	[HideInInspector]
	public string[] m_extraScenes;

	public GameObject[] m_raceControlPrefabs;

	public int m_numLaps;

	public int m_numRacers;

	public AIDifficulty m_aiDifficulty;

	public RaceAISchedule m_aiSchedule;

	public bool m_allAIsTheSame;

	public bool m_noAIMistakes;

	public float[] m_times;

	public float m_maxDuration;

	public int m_overtakesRequired;

	public ObjectiveType m_objective;

	public BonusObjective[] m_bonusObjectives;

	public VehicleFactory.Type m_vehicleType;

	public bool m_hasTrackHint;

	public bool m_disableQuickStart;

	public bool m_forceReverseTrack;

	public bool m_enableEasyRewards;

	[HideInInspector]
	public string m_hintStringID;

	public bool m_hasGamepadHint;

	public LeaderboardInfo m_leaderboard;

	public RaceLayout.Variant m_variant;

	[NonSerialized]
	public int m_humanMask;

	[NonSerialized]
	public int m_aiMask;

	[NonSerialized]
	public int[] m_controllers;

	[NonSerialized]
	public PlayerData[] m_playerData;

	[NonSerialized]
	public RunFlags m_runFlags;

	[NonSerialized]
	public int m_replaysRemaining = -1;

	public string DisplayName
	{
		get
		{
			return m_layout.DisplayName;
		}
	}

	public bool HasBonusObjectives
	{
		get
		{
			return m_bonusObjectives != null && m_bonusObjectives.Length > 0;
		}
	}

	public VehicleFactory.Type VehicleType
	{
		get
		{
			return m_vehicleType;
		}
	}

	public bool RecordPlayerGhost
	{
		get
		{
			return (m_runFlags & RunFlags.RecordPlayerGhost) != 0;
		}
	}

	public bool SameStartPosition
	{
		get
		{
			return (m_runFlags & RunFlags.SameStartPosition) != 0;
		}
	}

	public bool NoGrade
	{
		get
		{
			return (m_runFlags & RunFlags.NoGrade) != 0;
		}
	}

	public bool NoHelp
	{
		get
		{
			return (m_runFlags & RunFlags.NoHelp) != 0;
		}
	}

	public bool NoMidRaceRestart
	{
		get
		{
			return (m_runFlags & RunFlags.NoMidRaceRestart) != 0;
		}
	}

	public bool DontTrackTournamentPoints
	{
		get
		{
			return (m_runFlags & RunFlags.DontTrackTournamentPoints) != 0;
		}
	}

	public bool ReturnToMainMenuOnSuspend
	{
		get
		{
			return (m_runFlags & RunFlags.ReturnToMainMenuOnSuspend) != 0;
		}
	}

	public bool NoRaceReplay
	{
		get
		{
			return (m_runFlags & RunFlags.NoRaceReplay) != 0;
		}
	}

	public bool DontAddRacePoints
	{
		get
		{
			return (m_runFlags & RunFlags.DontAddRacePoints) != 0;
		}
	}

	public bool NoUpgrades
	{
		get
		{
			return (m_runFlags & RunFlags.NoUpgrades) != 0;
		}
	}

	public bool NoWatchReplay
	{
		get
		{
			return (m_runFlags & RunFlags.NoWatchReplay) != 0;
		}
	}

	public string GUIScene
	{
		get
		{
			switch (m_objective)
			{
			default:
				return "InRaceGUI";
			case ObjectiveType.Tutorial:
				return "InRaceGUI_Tutorial3";
			case ObjectiveType.Pickups:
			case ObjectiveType.Overtake:
			case ObjectiveType.TimeTrial:
			case ObjectiveType.Survival:
			case ObjectiveType.LastStanding:
				return "InRaceGUI_Simple";
			}
		}
	}

	public PointsType AwardPointsType
	{
		get
		{
			switch (m_objective)
			{
			default:
				return PointsType.Position;
			case ObjectiveType.Overtake:
			case ObjectiveType.TimeTrial:
				return PointsType.TotalTime;
			case ObjectiveType.Tutorial:
				return PointsType.None;
			case ObjectiveType.Pickups:
			case ObjectiveType.Survival:
				return PointsType.InvTotalTime;
			}
		}
	}

	public bool AwardMedals
	{
		get
		{
			ObjectiveType objective = m_objective;
			if (objective != ObjectiveType.Tutorial)
			{
				return true;
			}
			return false;
		}
	}

	public bool AwardResults
	{
		get
		{
			ObjectiveType objective = m_objective;
			if (objective != ObjectiveType.Tutorial)
			{
				return true;
			}
			return false;
		}
	}

	public bool InvTotalTime
	{
		get
		{
			switch (m_objective)
			{
			default:
				return false;
			case ObjectiveType.Pickups:
			case ObjectiveType.Survival:
			case ObjectiveType.LastStanding:
				return true;
			}
		}
	}

	public string GetPlayerName(int _index)
	{
		return (m_playerData == null || _index >= m_playerData.Length) ? string.Empty : m_playerData[_index].m_name;
	}

	public string GetObjectiveText(int _index)
	{
		return GetObjectiveText(_index, false);
	}

	public string GetObjectiveText(int _index, bool _prefixMedal)
	{
		string text = string.Empty;
		if (_prefixMedal)
		{
			switch (_index)
			{
			case 0:
				text = Language.Get("ID_MedalPrefixGold");
				break;
			case 1:
				text = Language.Get("ID_MedalPrefixSilver");
				break;
			case 2:
				text = Language.Get("ID_MedalPrefixBronze");
				break;
			}
		}
		switch (m_objective)
		{
		default:
			return text + Language.Get("ID_Position" + (_index + 1));
		case ObjectiveType.Pickups:
		case ObjectiveType.Tutorial:
		case ObjectiveType.Overtake:
		case ObjectiveType.TimeTrial:
		case ObjectiveType.Survival:
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(m_times[_index]);
			return string.Format("{0}{1:D0}:{2:D2}.{3:D2}", text, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
		}
		}
	}

	public string GetRaceTypeText()
	{
		string text = Language.Get("ID_Description" + m_objective);
		switch (m_objective)
		{
		default:
			return text;
		case ObjectiveType.RacePosition:
		case ObjectiveType.TimeTrial:
		case ObjectiveType.HeadsUp:
			return string.Format(text, m_numLaps);
		case ObjectiveType.Overtake:
			return string.Format(text, m_overtakesRequired);
		}
	}

	public string GetUnlockText()
	{
		return "TODO";
	}

	public string GetHintText()
	{
		string text = string.Empty;
		if (m_hasTrackHint)
		{
			text = text + Language.Get(m_layout.ID + "Hint") + "\n\n";
		}
		if (m_hintStringID.Length > 0)
		{
			string text2 = m_hintStringID;
			if (m_hasGamepadHint && UIGamepadManager.IsGamepadActive)
			{
				text2 += AppSettings.LocSuffixGamepad;
			}
			text += Language.Get(text2);
		}
		return text;
	}

	public byte[] GetGhostData(GhostType _type)
	{
		string arg = Enum.GetName(typeof(GhostType), _type);
		string path = string.Format("Ghosts/{0}_{1}", base.name, arg);
		TextAsset textAsset = Resources.Load(path) as TextAsset;
		return (!(textAsset != null)) ? null : textAsset.bytes;
	}
}
