public interface ITBRaceEvent
{
	string RaceEventName { get; }

	int NumLaps { get; }

	bool Reversed { get; }

	RaceLayout.Variant Variant { get; }
}
