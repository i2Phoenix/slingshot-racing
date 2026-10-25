using System;
using System.Collections.Generic;
using UnityEngine;

public class GameCenterTurnBasedManager : MonoBehaviour
{
	public static event Action<byte[]> loadMatchDataEvent;

	public static event Action<string> endTurnWithNextParticipantFailedEvent;

	public static event Action endTurnWithNextParticipantFinishedEvent;

	public static event Action<string> endMatchInTurnWithMatchDataFailedEvent;

	public static event Action endMatchInTurnWithMatchDataFinishedEvent;

	public static event Action<string> loadMatchDataFailedEvent;

	public static event Action<string> participantQuitInTurnFailedEvent;

	public static event Action participantQuitInTurnFinishedEvent;

	public static event Action<string> participantQuitOutOfTurnFailedEvent;

	public static event Action participantQuitOutOfTurnFinishedEvent;

	public static event Action<string> findMatchProgramaticallyFailedEvent;

	public static event Action findMatchProgramaticallyFinishedEvent;

	public static event Action<string> loadMatchesDidFailEvent;

	public static event Action<List<GKTurnBasedMatch>> loadMatchesDidFinishEvent;

	public static event Action<string> removeMatchFailedEvent;

	public static event Action removeMatchFinishedEvent;

	public static event Action turnBasedMatchmakerViewControllerWasCancelledEvent;

	public static event Action<string> turnBasedMatchmakerViewControllerFailedEvent;

	public static event Action<GKTurnBasedMatch> turnBasedMatchmakerViewControllerPlayerQuitEvent;

	public static event Action<GKTurnBasedMatch> turnBasedMatchmakerViewControllerDidFindMatchEvent;

	public static event Action<GKTurnBasedMatch> handleTurnEventEvent;

	public static event Action<GKTurnBasedMatch> handleMatchEndedEvent;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public static void loadMatchDataFinished(byte[] bytes)
	{
		if (loadMatchDataEvent != null)
		{
			loadMatchDataEvent(bytes);
		}
	}

	public void endTurnWithNextParticipantFailed(string error)
	{
		if (endTurnWithNextParticipantFailedEvent != null)
		{
			endTurnWithNextParticipantFailedEvent(error);
		}
	}

	public void endTurnWithNextParticipantFinished(string empty)
	{
		if (endTurnWithNextParticipantFinishedEvent != null)
		{
			endTurnWithNextParticipantFinishedEvent();
		}
	}

	public void endMatchInTurnWithMatchDataFailed(string error)
	{
		if (endMatchInTurnWithMatchDataFailedEvent != null)
		{
			endMatchInTurnWithMatchDataFailedEvent(error);
		}
	}

	public void endMatchInTurnWithMatchDataFinished(string empty)
	{
		if (endMatchInTurnWithMatchDataFinishedEvent != null)
		{
			endMatchInTurnWithMatchDataFinishedEvent();
		}
	}

	public void loadMatchDataFailed(string error)
	{
		if (loadMatchDataFailedEvent != null)
		{
			loadMatchDataFailedEvent(error);
		}
	}

	public void participantQuitInTurnFailed(string error)
	{
		if (participantQuitInTurnFailedEvent != null)
		{
			participantQuitInTurnFailedEvent(error);
		}
	}

	public void participantQuitInTurnFinished(string empty)
	{
		if (participantQuitInTurnFinishedEvent != null)
		{
			participantQuitInTurnFinishedEvent();
		}
	}

	public void participantQuitOutOfTurnFailed(string error)
	{
		if (participantQuitOutOfTurnFailedEvent != null)
		{
			participantQuitOutOfTurnFailedEvent(error);
		}
	}

	public void participantQuitOutOfTurnFinished(string empty)
	{
		if (participantQuitOutOfTurnFinishedEvent != null)
		{
			participantQuitOutOfTurnFinishedEvent();
		}
	}

	public void findMatchProgramaticallyFailed(string error)
	{
		if (findMatchProgramaticallyFailedEvent != null)
		{
			findMatchProgramaticallyFailedEvent(error);
		}
	}

	public void findMatchProgramaticallyFinished(string empty)
	{
		if (findMatchProgramaticallyFinishedEvent != null)
		{
			findMatchProgramaticallyFinishedEvent();
		}
	}

	public void loadMatchesDidFail(string error)
	{
		if (loadMatchesDidFailEvent != null)
		{
			loadMatchesDidFailEvent(error);
		}
	}

	public void loadMatchesDidFinish(string json)
	{
		if (loadMatchesDidFinishEvent != null)
		{
			loadMatchesDidFinishEvent(GKTurnBasedMatch.fromJSON(json));
		}
	}

	public void removeMatchFailed(string error)
	{
		if (removeMatchFailedEvent != null)
		{
			removeMatchFailedEvent(error);
		}
	}

	public void removeMatchFinished(string empty)
	{
		if (removeMatchFinishedEvent != null)
		{
			removeMatchFinishedEvent();
		}
	}

	public void turnBasedMatchmakerViewControllerWasCancelled(string empty)
	{
		if (turnBasedMatchmakerViewControllerWasCancelledEvent != null)
		{
			turnBasedMatchmakerViewControllerWasCancelledEvent();
		}
	}

	public void turnBasedMatchmakerViewControllerFailed(string error)
	{
		if (turnBasedMatchmakerViewControllerFailedEvent != null)
		{
			turnBasedMatchmakerViewControllerFailedEvent(error);
		}
	}

	public void turnBasedMatchmakerViewControllerPlayerQuit(string json)
	{
		if (turnBasedMatchmakerViewControllerPlayerQuitEvent != null)
		{
			turnBasedMatchmakerViewControllerPlayerQuitEvent(new GKTurnBasedMatch(json.hashtableFromJson()));
		}
	}

	public void turnBasedMatchmakerViewControllerDidFindMatch(string json)
	{
		if (turnBasedMatchmakerViewControllerDidFindMatchEvent != null)
		{
			turnBasedMatchmakerViewControllerDidFindMatchEvent(new GKTurnBasedMatch(json.hashtableFromJson()));
		}
	}

	public void handleTurnEvent(string json)
	{
		if (handleTurnEventEvent != null)
		{
			handleTurnEventEvent(new GKTurnBasedMatch(json.hashtableFromJson()));
		}
	}

	public void handleMatchEnded(string json)
	{
		if (handleMatchEndedEvent != null)
		{
			handleMatchEndedEvent(new GKTurnBasedMatch(json.hashtableFromJson()));
		}
	}
}
