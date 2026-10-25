public interface ITBTournament
{
	int NumRaces { get; }

	TurnBasedMatchState MatchState { get; }

	bool IsLocalPlayerInMatch { get; }

	ITBRace GetRace(int _index);
}
