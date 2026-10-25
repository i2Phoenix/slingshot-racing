public interface ITBRaceSequence
{
	int NumTurns { get; }

	int MaxRetries { get; }

	int NumEvents { get; }

	ITBRaceEvent GetEvent(int _eventIndex);
}
