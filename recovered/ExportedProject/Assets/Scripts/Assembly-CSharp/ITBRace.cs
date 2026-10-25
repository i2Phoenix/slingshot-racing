public interface ITBRace
{
	string RaceEventName { get; }

	int NumLaps { get; }

	bool Reversed { get; }

	RaceLayout.Variant Variant { get; }

	int NumPlayers { get; }

	ITBPlayer LocalPlayer { get; }

	ITBPlayer CurrentPlayer { get; }

	TurnBasedRaceState RaceState { get; }

	int NumRaceResults { get; }

	ITBPlayer GetPlayer(int _playerIndex);

	ITBPlayerResults GetRaceResults(int _playerIndex);
}
