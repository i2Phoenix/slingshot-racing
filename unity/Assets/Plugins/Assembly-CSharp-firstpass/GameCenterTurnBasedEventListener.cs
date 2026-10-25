using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameCenterTurnBasedEventListener : MonoBehaviour
{
	private void OnEnable()
	{
		GameCenterTurnBasedManager.loadMatchDataEvent += loadMatchDataEvent;
		GameCenterTurnBasedManager.endTurnWithNextParticipantFailedEvent += endTurnWithNextParticipantFailedEvent;
		GameCenterTurnBasedManager.endTurnWithNextParticipantFinishedEvent += endTurnWithNextParticipantFinishedEvent;
		GameCenterTurnBasedManager.endMatchInTurnWithMatchDataFailedEvent += endMatchInTurnWithMatchDataFailedEvent;
		GameCenterTurnBasedManager.endMatchInTurnWithMatchDataFinishedEvent += endMatchInTurnWithMatchDataFinishedEvent;
		GameCenterTurnBasedManager.loadMatchDataFailedEvent += loadMatchDataFailedEvent;
		GameCenterTurnBasedManager.participantQuitInTurnFailedEvent += participantQuitInTurnFailedEvent;
		GameCenterTurnBasedManager.participantQuitInTurnFinishedEvent += participantQuitInTurnFinishedEvent;
		GameCenterTurnBasedManager.participantQuitOutOfTurnFailedEvent += participantQuitOutOfTurnFailedEvent;
		GameCenterTurnBasedManager.participantQuitOutOfTurnFinishedEvent += participantQuitOutOfTurnFinishedEvent;
		GameCenterTurnBasedManager.findMatchProgramaticallyFailedEvent += findMatchProgramaticallyFailedEvent;
		GameCenterTurnBasedManager.findMatchProgramaticallyFinishedEvent += findMatchProgramaticallyFinishedEvent;
		GameCenterTurnBasedManager.loadMatchesDidFailEvent += loadMatchesDidFailEvent;
		GameCenterTurnBasedManager.loadMatchesDidFinishEvent += loadMatchesDidFinishEvent;
		GameCenterTurnBasedManager.removeMatchFailedEvent += removeMatchFailedEvent;
		GameCenterTurnBasedManager.removeMatchFinishedEvent += removeMatchFinishedEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerWasCancelledEvent += turnBasedMatchmakerViewControllerWasCancelledEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerFailedEvent += turnBasedMatchmakerViewControllerFailedEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerPlayerQuitEvent += turnBasedMatchmakerViewControllerPlayerQuitEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerDidFindMatchEvent += turnBasedMatchmakerViewControllerDidFindMatchEvent;
		GameCenterTurnBasedManager.handleTurnEventEvent += handleTurnEventEvent;
		GameCenterTurnBasedManager.handleMatchEndedEvent += handleMatchEndedEvent;
	}

	private void OnDisable()
	{
		GameCenterTurnBasedManager.loadMatchDataEvent -= loadMatchDataEvent;
		GameCenterTurnBasedManager.endTurnWithNextParticipantFailedEvent -= endTurnWithNextParticipantFailedEvent;
		GameCenterTurnBasedManager.endTurnWithNextParticipantFinishedEvent -= endTurnWithNextParticipantFinishedEvent;
		GameCenterTurnBasedManager.endMatchInTurnWithMatchDataFailedEvent -= endMatchInTurnWithMatchDataFailedEvent;
		GameCenterTurnBasedManager.endMatchInTurnWithMatchDataFinishedEvent -= endMatchInTurnWithMatchDataFinishedEvent;
		GameCenterTurnBasedManager.loadMatchDataFailedEvent -= loadMatchDataFailedEvent;
		GameCenterTurnBasedManager.participantQuitInTurnFailedEvent -= participantQuitInTurnFailedEvent;
		GameCenterTurnBasedManager.participantQuitInTurnFinishedEvent -= participantQuitInTurnFinishedEvent;
		GameCenterTurnBasedManager.participantQuitOutOfTurnFailedEvent -= participantQuitOutOfTurnFailedEvent;
		GameCenterTurnBasedManager.participantQuitOutOfTurnFinishedEvent -= participantQuitOutOfTurnFinishedEvent;
		GameCenterTurnBasedManager.findMatchProgramaticallyFailedEvent -= findMatchProgramaticallyFailedEvent;
		GameCenterTurnBasedManager.findMatchProgramaticallyFinishedEvent -= findMatchProgramaticallyFinishedEvent;
		GameCenterTurnBasedManager.loadMatchesDidFailEvent -= loadMatchesDidFailEvent;
		GameCenterTurnBasedManager.loadMatchesDidFinishEvent -= loadMatchesDidFinishEvent;
		GameCenterTurnBasedManager.removeMatchFailedEvent -= removeMatchFailedEvent;
		GameCenterTurnBasedManager.removeMatchFinishedEvent -= removeMatchFinishedEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerWasCancelledEvent -= turnBasedMatchmakerViewControllerWasCancelledEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerFailedEvent -= turnBasedMatchmakerViewControllerFailedEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerPlayerQuitEvent -= turnBasedMatchmakerViewControllerPlayerQuitEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerDidFindMatchEvent -= turnBasedMatchmakerViewControllerDidFindMatchEvent;
		GameCenterTurnBasedManager.handleTurnEventEvent -= handleTurnEventEvent;
		GameCenterTurnBasedManager.handleMatchEndedEvent -= handleMatchEndedEvent;
	}

	private void loadMatchDataEvent(byte[] bytes)
	{
		string text = Encoding.UTF8.GetString(bytes);
		Debug.Log("loadMatchDataEvent: " + text);
	}

	private void endTurnWithNextParticipantFailedEvent(string error)
	{
		Debug.Log("endTurnWithNextParticipantFailedEvent: " + error);
	}

	private void endTurnWithNextParticipantFinishedEvent()
	{
		Debug.Log("endTurnWithNextParticipantFinishedEvent");
	}

	private void endMatchInTurnWithMatchDataFailedEvent(string error)
	{
		Debug.Log("endMatchInTurnWithMatchDataFailedEvent: " + error);
	}

	private void endMatchInTurnWithMatchDataFinishedEvent()
	{
		Debug.Log("endMatchInTurnWithMatchDataFinishedEvent");
	}

	private void loadMatchDataFailedEvent(string error)
	{
		Debug.Log("loadMatchDataFailedEvent: " + error);
	}

	private void participantQuitInTurnFailedEvent(string error)
	{
		Debug.Log("participantQuitInTurnFailedEvent: " + error);
	}

	private void participantQuitInTurnFinishedEvent()
	{
		Debug.Log("participantQuitInTurnFinishedEvent");
	}

	private void participantQuitOutOfTurnFailedEvent(string error)
	{
		Debug.Log("participantQuitOutOfTurnFailedEvent: " + error);
	}

	private void participantQuitOutOfTurnFinishedEvent()
	{
		Debug.Log("participantQuitOutOfTurnFinishedEvent");
	}

	private void findMatchProgramaticallyFailedEvent(string error)
	{
		Debug.Log("findMatchProgramaticallyFailedEvent: " + error);
	}

	private void findMatchProgramaticallyFinishedEvent()
	{
		Debug.Log("findMatchProgramaticallyFinishedEvent");
	}

	private void loadMatchesDidFailEvent(string error)
	{
		Debug.Log("loadMatchesDidFailEvent: " + error);
	}

	private void loadMatchesDidFinishEvent(List<GKTurnBasedMatch> list)
	{
		Debug.Log("loadMatchesDidFinishEvent");
		foreach (GKTurnBasedMatch item in list)
		{
			Debug.Log(item);
		}
	}

	private void removeMatchFailedEvent(string error)
	{
		Debug.Log("removeMatchFailedEvent: " + error);
	}

	private void removeMatchFinishedEvent()
	{
		Debug.Log("removeMatchFinishedEvent");
	}

	private void turnBasedMatchmakerViewControllerWasCancelledEvent()
	{
		Debug.Log("turnBasedMatchmakerViewControllerWasCancelledEvent");
	}

	private void turnBasedMatchmakerViewControllerFailedEvent(string error)
	{
		Debug.Log("turnBasedMatchmakerViewControllerFailedEvent: " + error);
	}

	private void turnBasedMatchmakerViewControllerPlayerQuitEvent(GKTurnBasedMatch match)
	{
		Debug.Log("turnBasedMatchmakerViewControllerPlayerQuitEvent: " + match);
	}

	private void turnBasedMatchmakerViewControllerDidFindMatchEvent(GKTurnBasedMatch match)
	{
		Debug.Log("turnBasedMatchmakerViewControllerDidFindMatchEvent: " + match);
	}

	private void handleTurnEventEvent(GKTurnBasedMatch match)
	{
		Debug.Log("handleTurnEventEvent: " + match);
	}

	private void handleMatchEndedEvent(GKTurnBasedMatch match)
	{
		Debug.Log("handleMatchEndedEvent: " + match);
	}
}
