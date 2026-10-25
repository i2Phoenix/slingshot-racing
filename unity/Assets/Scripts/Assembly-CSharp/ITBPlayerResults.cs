public interface ITBPlayerResults
{
	ITBPlayer Player { get; }

	int Position { get; }

	float RaceTime { get; }

	float BestLap { get; }

	byte[] GhostData { get; }
}
