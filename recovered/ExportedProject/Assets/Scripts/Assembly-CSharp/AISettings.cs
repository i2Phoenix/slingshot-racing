using UnityEngine;

public class AISettings : ScriptableObject
{
	public float m_reactionTime = 0.0625f;

	public float m_suppressGrappleTime = 1f;

	public float[] m_difficultyLevelMistakeTime = new float[4] { 0.33f, 0.22f, 0.11f, 0f };

	public float[] m_difficultyLevelMistakeChance = new float[4] { 0.1f, 0.067f, 0.033f, 0f };

	public float[] m_difficultyForwardForceScale = new float[4] { 0.85f, 0.925f, 1f, 1f };

	public float[] m_grudgeThresholds = new float[4] { 25f, 21.67f, 18.33f, 15f };

	public float[] m_grudgeDuration = new float[4] { 10f, 20f, 30f, 40f };

	public static AISettings Load()
	{
		return (AISettings)Resources.Load("AI/AISettings");
	}
}
