using System;

public static class PlayerNames
{
	private static string[] s_names;

	private static string[,] s_aiNames;

	public static string GetName(int _colourIndex)
	{
		if (s_names == null)
		{
			Load();
		}
		return s_names[_colourIndex];
	}

	public static string GetName(int _colourIndex, AIDifficulty _difficulty)
	{
		if (s_aiNames == null)
		{
			Load();
		}
		return s_aiNames[(int)_difficulty, _colourIndex];
	}

	private static void Load()
	{
		s_names = new string[4];
		for (int i = 0; i < 4; i++)
		{
			s_names[i] = Language.Get("ID_PlayerName_" + i);
		}
		string[] names = Enum.GetNames(typeof(AIDifficulty));
		int num = names.Length;
		s_aiNames = new string[num, 4];
		for (int j = 0; j < num; j++)
		{
			for (int k = 0; k < 4; k++)
			{
				s_aiNames[j, k] = Language.Get("ID_PlayerName_" + names[j] + k);
			}
		}
	}
}
