public class TransitionPlayer : TransitionStore
{
	private EZTransition m_currentTransition;

	private bool m_looping;

	public string m_playOnStart;

	public bool m_playOnStartLooped;

	protected override void Start()
	{
		m_transitions = m_transitionsArray[0];
		EZTransition[] list = m_transitions.list;
		foreach (EZTransition eZTransition in list)
		{
			eZTransition.AddTransitionEndDelegate(OnTransitionEnd);
			eZTransition.MainSubject = base.gameObject;
		}
		if (m_playOnStart.Length > 0)
		{
			Play(FindIndex(m_playOnStart), m_playOnStartLooped);
		}
	}

	private void OnDestroy()
	{
		EZTransition[] list = m_transitions.list;
		foreach (EZTransition eZTransition in list)
		{
			eZTransition.RemoveTransitionEndDelegate(OnTransitionEnd);
		}
	}

	public int FindIndex(string _name)
	{
		for (int i = 0; i < m_transitions.list.Length; i++)
		{
			if (m_transitions.list[i].name == _name)
			{
				return i;
			}
		}
		return -1;
	}

	public EZTransition Play(int _index, bool _loop)
	{
		if (m_currentTransition != null)
		{
			m_currentTransition.StopSafe();
		}
		m_currentTransition = m_transitions.list[_index];
		m_currentTransition.Start();
		m_looping = _loop;
		return m_currentTransition;
	}

	public EZTransition Stop()
	{
		EZTransition currentTransition = m_currentTransition;
		if (m_currentTransition != null)
		{
			m_currentTransition.StopSafe();
			m_currentTransition = null;
			m_looping = false;
		}
		return currentTransition;
	}

	private void OnTransitionEnd(EZTransition _transition)
	{
		if (m_looping && m_currentTransition != null)
		{
			m_currentTransition.Start();
		}
		else
		{
			m_currentTransition = null;
		}
	}
}
