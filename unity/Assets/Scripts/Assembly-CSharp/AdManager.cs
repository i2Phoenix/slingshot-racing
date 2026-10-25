using UnityEngine;

public class AdManager : MonoBehaviour
{
	public enum State
	{
		TitleScreen = 0
	}

	private static AdManager s_instance;

	private void Awake()
	{
		if (s_instance != null)
		{
			Debug.LogError("This shouldn't happen");
			Object.Destroy(base.gameObject);
		}
		else
		{
			s_instance = this;
		}
	}

	private void OnDestroy()
	{
		if (s_instance == this)
		{
			s_instance = null;
		}
	}

	public static void OnEnterState(State _state)
	{
		if (s_instance != null)
		{
			s_instance.OnEnterStateImpl(_state);
		}
	}

	public static void OnLeaveState(State _state)
	{
		if (s_instance != null)
		{
			s_instance.OnLeaveStateImpl(_state);
		}
	}

	public void OnEnterStateImpl(State _state)
	{
	}

	public void OnLeaveStateImpl(State _state)
	{
	}
}
