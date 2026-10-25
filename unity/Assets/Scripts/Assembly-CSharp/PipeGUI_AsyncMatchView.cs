using UnityEngine;

public class PipeGUI_AsyncMatchView : MonoBehaviour
{
	private AudioPlayer m_audioPlayer;

	private GameManager m_gameManager;

	private TurnBasedMatchManager m_turnBasedMatchManager;

	private ITBTournament m_tournament;

	public RaceCampaign m_campaign;

	public UIScrollList m_trackList;

	public GameObject m_listItemPrefab;

	public SpriteText m_title;

	public AudioBank m_alertBank;

	public PackedSprite m_resignDeleteSprite;

	public GameObject m_watchReplayButton;

	private void Awake()
	{
		m_audioPlayer = GetComponent<AudioPlayer>();
		if (m_audioPlayer == null)
		{
			m_audioPlayer = base.gameObject.AddComponent<AudioPlayer>();
		}
	}

	private void Start()
	{
		ITBTournament tour = null;
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject != null)
		{
			m_gameManager = gameObject.GetComponent<GameManager>();
			m_turnBasedMatchManager = gameObject.GetComponent<TurnBasedMatchManager>();
			if (m_turnBasedMatchManager != null)
			{
				tour = m_turnBasedMatchManager.CurrentMatchData;
				m_turnBasedMatchManager.OnMatchDataUpdatedEvent += OnMatchDataUpdated;
			}
		}
		SetTour(tour);
	}

	private void OnDestroy()
	{
		if (m_turnBasedMatchManager != null)
		{
			m_turnBasedMatchManager.OnMatchDataUpdatedEvent -= OnMatchDataUpdated;
		}
	}

	public void SetTour(ITBTournament _tour)
	{
		m_tournament = _tour;
		Refresh();
	}

	private void Refresh()
	{
		m_trackList.ClearList(true);
		if (m_tournament == null)
		{
			return;
		}
		ITBRace iTBRace = FindPlayerTurn();
		ITBRace iTBRace2 = FindActiveRace();
		int numRaces = m_tournament.NumRaces;
		for (int i = 0; i < numRaces; i++)
		{
			PipeGUI_FreeplayTrackWidget component = m_trackList.CreateItem(m_listItemPrefab).gameObject.GetComponent<PipeGUI_FreeplayTrackWidget>();
			component.Manager = this;
		}
		RaceTour raceTour = m_campaign.m_tours[0];
		int num = numRaces - 1;
		for (int j = 0; j < numRaces; j++)
		{
			ITBRace race = m_tournament.GetRace(j);
			IUIListObject item = m_trackList.GetItem(j);
			PipeGUI_FreeplayTrackWidget component2 = item.gameObject.GetComponent<PipeGUI_FreeplayTrackWidget>();
			if (component2 != null)
			{
				RaceEvent raceData = raceTour.FindRace(race.RaceEventName);
				component2.SetRaceData(raceData);
			}
			PipeGUI_AsyncTrackWidget component3 = item.gameObject.GetComponent<PipeGUI_AsyncTrackWidget>();
			if (component3 != null)
			{
				component3.SetData(race, j, numRaces);
			}
			if (iTBRace2 == race)
			{
				num = j;
			}
		}
		if (num >= 0)
		{
			m_trackList.ScrollToItem(num, 0f);
		}
		if (m_title != null)
		{
			string text = string.Empty;
			if (iTBRace != null)
			{
				text = Language.Get("ID_AsyncTitlePlayerTurn");
			}
			else
			{
				switch (m_tournament.MatchState)
				{
				case TurnBasedMatchState.InProgress:
					text = Language.Get("ID_AsyncTitleInProgress");
					break;
				case TurnBasedMatchState.WaitingForPlayers:
					text = Language.Get("ID_AsyncTitleWaitingForPlayers");
					break;
				case TurnBasedMatchState.Ended:
					text = Language.Get("ID_AsyncTitleMatchFinished");
					break;
				}
			}
			m_title.Text = text;
		}
		if (m_resignDeleteSprite != null)
		{
			m_resignDeleteSprite.gameObject.SetActiveRecursively(true);
			UVAnimation anim = m_resignDeleteSprite.GetAnim((!m_tournament.IsLocalPlayerInMatch) ? "Delete" : "Resign");
			if (anim != null)
			{
				m_resignDeleteSprite.PlayAnim(anim);
				m_resignDeleteSprite.defaultAnim = anim.index;
			}
		}
		if (m_watchReplayButton != null)
		{
			m_watchReplayButton.SetActiveRecursively(FindLastCompleteWithGhost() != null);
		}
	}

	private void OnMatchDataUpdated()
	{
		if (m_turnBasedMatchManager != null)
		{
			ITBTournament currentMatchData = m_turnBasedMatchManager.CurrentMatchData;
			SetTour(currentMatchData);
			if (FindPlayerTurn() != null)
			{
				PlayAlert();
			}
		}
		else
		{
			Refresh();
		}
	}

	private void OnWidgetSelected(PipeGUI_FreeplayTrackWidget _widget)
	{
		PipeGUI_AsyncTrackWidget component = _widget.GetComponent<PipeGUI_AsyncTrackWidget>();
		if (!(component != null))
		{
			return;
		}
		switch (component.Race.RaceState)
		{
		case TurnBasedRaceState.InActive:
		case TurnBasedRaceState.WaitingNetworkPlayer:
			break;
		case TurnBasedRaceState.WaitingLocalPlayer:
			if (m_gameManager != null)
			{
				RaceEvent race2 = TurnBasedUtils.CreateTurn(component.Race, m_campaign, true);
				m_gameManager.RaceSelected(race2);
			}
			break;
		case TurnBasedRaceState.CompleteWithGhost:
			if (m_gameManager != null)
			{
				RaceEvent race = TurnBasedUtils.CreateReplay(component.Race, m_campaign, true);
				m_gameManager.TournamentReplaySelected(race, FindRaceIndex(component.Race) + 1, m_tournament.NumRaces, CalculateScores(component.Race));
			}
			break;
		case TurnBasedRaceState.WaitingServer:
		case TurnBasedRaceState.Completed:
			break;
		}
	}

	private void OnBackSelected()
	{
		if (m_gameManager != null)
		{
			m_gameManager.GoBack();
		}
	}

	private void OnResignDeleteSelected()
	{
		if (!(m_turnBasedMatchManager == null))
		{
			if (m_tournament.IsLocalPlayerInMatch)
			{
				Platform.ShowDialog(Language.Get("ID_AsyncResignTitle"), Language.Get("ID_AsyncResignBody"), Language.Get("ID_AsyncCancel"), Language.Get("ID_AsyncConfirm"), OnResignMatch);
			}
			else
			{
				Platform.ShowDialog(Language.Get("ID_AsyncDeleteTitle"), Language.Get("ID_AsyncDeleteBody"), Language.Get("ID_AsyncCancel"), Language.Get("ID_AsyncConfirm"), OnDeleteMatch);
			}
		}
	}

	private void OnResignMatch(bool _confirmed)
	{
		if (_confirmed)
		{
			if (m_turnBasedMatchManager.QuitLocalPlayer())
			{
				m_resignDeleteSprite.gameObject.SetActiveRecursively(false);
			}
			else
			{
				Debug.LogError("Failed to resign match");
			}
		}
	}

	private void OnDeleteMatch(bool _confirmed)
	{
		if (!_confirmed)
		{
			return;
		}
		if (m_turnBasedMatchManager.RemoveCurrentMatch())
		{
			if (m_gameManager != null)
			{
				m_gameManager.OnOptionsReturnToMainMenu();
			}
		}
		else
		{
			Debug.LogError("Failed to remove match");
		}
	}

	private void OnWatchReplaySelected()
	{
		ITBRace iTBRace = FindLastCompleteWithGhost();
		if (iTBRace != null)
		{
			m_trackList.ScrollToItem(FindRaceIndex(iTBRace), 0.5f);
			if (m_gameManager != null)
			{
				RaceEvent race = TurnBasedUtils.CreateReplay(iTBRace, m_campaign, true);
				m_gameManager.TournamentReplaySelected(race, FindRaceIndex(iTBRace) + 1, m_tournament.NumRaces, CalculateScores(iTBRace));
			}
		}
	}

	private ITBRace FindPlayerTurn()
	{
		for (int i = 0; i < m_tournament.NumRaces; i++)
		{
			ITBRace race = m_tournament.GetRace(i);
			if (race.RaceState == TurnBasedRaceState.WaitingLocalPlayer)
			{
				return race;
			}
		}
		return null;
	}

	private ITBRace FindActiveRace()
	{
		for (int i = 0; i < m_tournament.NumRaces; i++)
		{
			ITBRace race = m_tournament.GetRace(i);
			if (race.RaceState != TurnBasedRaceState.Completed && race.RaceState != TurnBasedRaceState.CompleteWithGhost)
			{
				return race;
			}
		}
		return null;
	}

	private ITBRace FindLastCompleteWithGhost()
	{
		for (int num = m_tournament.NumRaces - 1; num >= 0; num--)
		{
			ITBRace race = m_tournament.GetRace(num);
			if (race.RaceState == TurnBasedRaceState.CompleteWithGhost)
			{
				return race;
			}
		}
		return null;
	}

	private int FindRaceIndex(ITBRace _race)
	{
		for (int i = 0; i < m_tournament.NumRaces; i++)
		{
			if (_race == m_tournament.GetRace(i))
			{
				return i;
			}
		}
		return 0;
	}

	private GameManager.TournamentScores[] CalculateScores(ITBRace _race)
	{
		GameManager.TournamentScores[] array = new GameManager.TournamentScores[4];
		for (int i = 0; i < m_tournament.NumRaces; i++)
		{
			ITBRace race = m_tournament.GetRace(i);
			if (race == _race)
			{
				break;
			}
			if (race.RaceState != TurnBasedRaceState.Completed && race.RaceState != TurnBasedRaceState.CompleteWithGhost)
			{
				continue;
			}
			for (int j = 0; j < race.NumRaceResults; j++)
			{
				ITBPlayerResults raceResults = race.GetRaceResults(j);
				if (raceResults != null)
				{
					int pointsForPosition = RaceCampaign.GetPointsForPosition(raceResults.Position);
					array[raceResults.Player.PlayerIndex].m_points += pointsForPosition;
				}
			}
		}
		return array;
	}

	private void PlayAlert()
	{
		if (m_alertBank != null)
		{
			m_audioPlayer.PlayRandomGUIClip(m_alertBank);
		}
	}
}
