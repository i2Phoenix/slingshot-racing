using UnityEngine;

[AddComponentMenu("GUI/PipeGUI/AsyncTrackWidget")]
public class PipeGUI_AsyncTrackWidget : MonoBehaviour
{
	private ITBRace m_race;

	private int m_raceIndex;

	private int m_numRaces;

	public SpriteText m_status;

	public SpriteText m_raceNumber;

	public SpriteText m_lapCount;

	public GameObject m_replayIndicator;

	public ITBRace Race
	{
		get
		{
			return m_race;
		}
	}

	public void SetData(ITBRace _race, int _raceIndex, int _numRaces)
	{
		m_race = _race;
		m_raceIndex = _raceIndex;
		m_numRaces = _numRaces;
		Refresh();
	}

	private void Refresh()
	{
		bool flag = false;
		string text = string.Empty;
		switch (m_race.RaceState)
		{
		case TurnBasedRaceState.InActive:
			text = Language.Get("ID_AsyncRaceInactive");
			break;
		case TurnBasedRaceState.WaitingLocalPlayer:
			text = Language.Get("ID_AsyncRaceWaitingLocalPlayer");
			flag = true;
			break;
		case TurnBasedRaceState.WaitingNetworkPlayer:
		{
			ITBPlayer currentPlayer = m_race.CurrentPlayer;
			text = ((currentPlayer == null) ? Language.Get("ID_AsyncRaceWaitingNetworkPlayer") : currentPlayer.Name);
			break;
		}
		case TurnBasedRaceState.WaitingServer:
			text = Language.Get("ID_AsyncRaceWaitingServer");
			break;
		case TurnBasedRaceState.Completed:
			text = Language.Get("ID_AsyncRaceCompleted");
			break;
		case TurnBasedRaceState.CompleteWithGhost:
			text = Language.Get("ID_AsyncRaceCompletedWithGhost");
			break;
		}
		PipeGUI_Pulse[] componentsInChildren = GetComponentsInChildren<PipeGUI_Pulse>();
		if (componentsInChildren != null)
		{
			PipeGUI_Pulse[] array = componentsInChildren;
			foreach (PipeGUI_Pulse pipeGUI_Pulse in array)
			{
				pipeGUI_Pulse.enabled = flag;
			}
		}
		m_status.Text = text;
		if (m_raceNumber != null)
		{
			m_raceNumber.Text = string.Format(Language.Get("ID_Summary_RaceNumber"), m_raceIndex + 1, m_numRaces);
		}
		if (m_lapCount != null)
		{
			m_lapCount.Text = string.Format("{0} {1}", m_race.NumLaps, Language.Get("ID_laps"));
		}
		if (m_replayIndicator != null)
		{
			m_replayIndicator.SetActiveRecursively(m_race.RaceState == TurnBasedRaceState.CompleteWithGhost);
		}
	}
}
