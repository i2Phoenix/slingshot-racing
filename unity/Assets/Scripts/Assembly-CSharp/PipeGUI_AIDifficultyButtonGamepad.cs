using UnityEngine;

public class PipeGUI_AIDifficultyButtonGamepad : MonoBehaviour
{
	private bool m_aiEnabled;

	public GameObject m_aiEnabledObjects;

	public GameObject m_aiDisabledObjects;

	public UIStateToggleBtn m_aiState;

	public string m_buttonName;

	public bool AIEnabled
	{
		get
		{
			return m_aiEnabled;
		}
		private set
		{
			m_aiEnabled = value;
			m_aiEnabledObjects.SetActiveRecursively(m_aiEnabled);
			m_aiDisabledObjects.SetActiveRecursively(!m_aiEnabled);
		}
	}

	public AIDifficulty Difficulty
	{
		get
		{
			return (AIDifficulty)m_aiState.StateNum;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!Input.GetButtonDown(m_buttonName))
		{
			return;
		}
		if (AIEnabled)
		{
			int num = m_aiState.StateNum + 1;
			if (num == 3)
			{
				AIEnabled = false;
				return;
			}
			m_aiState.SetToggleState(num);
			RefreshAIText();
		}
		else
		{
			AIEnabled = true;
			m_aiState.SetToggleState(0);
			RefreshAIText();
		}
	}

	private void RefreshAIText()
	{
		SpriteText componentInChildren = m_aiState.GetComponentInChildren<SpriteText>();
		if (componentInChildren != null)
		{
			string key = "ID_" + m_aiState.StateName + "AIDifficulty";
			componentInChildren.Text = Language.Get(key);
		}
	}
}
