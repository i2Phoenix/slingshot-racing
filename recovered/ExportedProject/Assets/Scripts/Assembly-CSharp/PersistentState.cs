using EntropyFlux;
using UnityEngine;

[ExecuteInEditMode]
public class PersistentState : MonoBehaviour
{
	public int numPlayersToSpawn;

	public int numAIPlayersToSpawn;

	public AIDifficulty aiDifficulty;

	public bool m_aiControlHumans;

	public int totalToSpawn
	{
		get
		{
			return numPlayersToSpawn + numAIPlayersToSpawn;
		}
	}

	public bool RecordPlayerGhost
	{
		get
		{
			return false;
		}
	}

	private void Awake()
	{
		numPlayersToSpawn = 1;
		numAIPlayersToSpawn = 3;
		aiDifficulty = AIDifficulty.Medium;
	}

	private void OnLevelWasLoaded()
	{
	}

	public static PersistentState Get()
	{
		GameObject gameObject = null;
		if (EntropyFlux.Profiler.Enabled && GameObject.Find("__ProfileReporter") == null)
		{
			GameObject gameObject2 = new GameObject("__ProfileReporter");
			gameObject2.AddComponent<ProfileReporter>();
		}
		gameObject = GameObject.FindWithTag("PersistentState");
		if (gameObject == null)
		{
			gameObject = new GameObject("PersistentState");
			gameObject.AddComponent<PersistentState>();
			gameObject.tag = "PersistentState";
			gameObject.hideFlags = HideFlags.DontSave;
			Object.DontDestroyOnLoad(gameObject);
		}
		if (gameObject != null)
		{
			return gameObject.GetComponent<PersistentState>();
		}
		return null;
	}
}
