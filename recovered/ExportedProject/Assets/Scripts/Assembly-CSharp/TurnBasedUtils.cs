using UnityEngine;

public static class TurnBasedUtils
{
	public static RaceEvent CreateTurn(ITBRace _race, RaceCampaign _base, bool _setStyles)
	{
		RaceEvent raceEvent = _base.m_tours[0].FindRace(_race.RaceEventName);
		if (raceEvent == null)
		{
			return null;
		}
		RaceEvent raceEvent2 = Object.Instantiate(raceEvent) as RaceEvent;
		raceEvent2.name = raceEvent.name;
		raceEvent2.m_numLaps = _race.NumLaps;
		raceEvent2.m_numRacers = 1;
		raceEvent2.m_humanMask = 1 << Settings.PlayerColour;
		raceEvent2.m_aiMask = 0;
		raceEvent2.m_replaysRemaining = 0;
		raceEvent2.m_variant = _race.Variant;
		raceEvent2.m_forceReverseTrack = _race.Reversed;
		raceEvent2.m_runFlags = RaceEvent.RunFlags.RecordPlayerGhost | RaceEvent.RunFlags.NoGrade | RaceEvent.RunFlags.NoHelp | RaceEvent.RunFlags.NoMidRaceRestart | RaceEvent.RunFlags.ReturnToMainMenuOnSuspend | RaceEvent.RunFlags.NoRaceReplay | RaceEvent.RunFlags.NoUpgrades | RaceEvent.RunFlags.NoWatchReplay;
		if (_setStyles)
		{
			VehicleFactory.SetStyle(0, Settings.VehicleStyleIndex);
		}
		return raceEvent2;
	}

	public static RaceEvent CreateReplay(ITBRace _race, RaceCampaign _base, bool _setStyles)
	{
		if (_race.RaceState != TurnBasedRaceState.CompleteWithGhost)
		{
			return null;
		}
		RaceEvent raceEvent = null;
		RaceEvent raceEvent2 = _base.m_tours[0].FindRace(_race.RaceEventName);
		if (raceEvent2 == null)
		{
			return null;
		}
		raceEvent = Object.Instantiate(raceEvent2) as RaceEvent;
		raceEvent.name = raceEvent2.name;
		int numRaceResults = _race.NumRaceResults;
		raceEvent.m_numLaps = _race.NumLaps;
		raceEvent.m_numRacers = numRaceResults;
		raceEvent.m_humanMask = 0;
		raceEvent.m_aiMask = 0;
		raceEvent.m_runFlags = RaceEvent.RunFlags.SameStartPosition | RaceEvent.RunFlags.DontTrackTournamentPoints | RaceEvent.RunFlags.ReturnToMainMenuOnSuspend | RaceEvent.RunFlags.NoRaceReplay | RaceEvent.RunFlags.NoUpgrades | RaceEvent.RunFlags.NoWatchReplay;
		raceEvent.m_variant = _race.Variant;
		raceEvent.m_forceReverseTrack = _race.Reversed;
		raceEvent.m_playerData = new RaceEvent.PlayerData[4];
		for (int i = 0; i < numRaceResults; i++)
		{
			ITBPlayerResults raceResults = _race.GetRaceResults(i);
			ITBPlayer player = raceResults.Player;
			raceEvent.m_aiMask |= 1 << player.ColourIndex;
			raceEvent.m_playerData[player.ColourIndex].m_ghostData = raceResults.GhostData;
			raceEvent.m_playerData[player.ColourIndex].m_name = player.Name;
			raceEvent.m_playerData[player.ColourIndex].m_playerIndex = player.PlayerIndex;
			raceEvent.m_playerData[player.ColourIndex].m_localPlayer = player.PlayerIndex == 0;
			if (_setStyles)
			{
				VehicleFactory.SetStyle(player.PlayerIndex, player.VehicleStyleIndex);
			}
		}
		return raceEvent;
	}
}
