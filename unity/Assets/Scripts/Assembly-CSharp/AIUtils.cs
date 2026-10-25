using UnityEngine;

public static class AIUtils
{
	public static readonly Color[] s_debugColours = new Color[4]
	{
		Color.green,
		Color.yellow,
		Color.red,
		Color.white
	};

	public static AIDifficulty GetAIRacerDifficulty(AIDifficulty _difficulty, int _aiIndex, int _numAIs)
	{
		if (_numAIs >= 2)
		{
			AIDifficulty worseDifficulty = GetWorseDifficulty(_difficulty);
			if (_aiIndex == 0)
			{
				return worseDifficulty;
			}
		}
		return _difficulty;
	}

	public static AIDifficulty GetBetterDifficulty(AIDifficulty _difficulty)
	{
		switch (_difficulty)
		{
		case AIDifficulty.Easy:
			return AIDifficulty.Medium;
		case AIDifficulty.Medium:
			return AIDifficulty.Hard;
		default:
			return _difficulty;
		}
	}

	public static AIDifficulty GetWorseDifficulty(AIDifficulty _difficulty)
	{
		switch (_difficulty)
		{
		case AIDifficulty.Medium:
			return AIDifficulty.Easy;
		case AIDifficulty.Hard:
			return AIDifficulty.Medium;
		default:
			return _difficulty;
		}
	}
}
