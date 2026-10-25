using System;
using UnityEngine;

public class RaceAISchedule : ScriptableObject
{
	public enum Condition
	{
		None = 0,
		HumanInFront = 1,
		AIInFront = 2,
		Tie = 3,
		TieOrHumanInFront = 4,
		TieOrAIInFront = 5
	}

	[Serializable]
	public class KeyFrame
	{
		public float m_propThroughRace;

		public int m_globalAIBias;

		public int m_bestAIBias;

		public int m_worstAIBias;

		public Condition m_condition;
	}

	public KeyFrame[] m_keyFrames;
}
