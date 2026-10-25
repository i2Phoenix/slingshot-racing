using UnityEngine;

[AddComponentMenu("GUI/PipeGUI/FreeplayPlayerSelection")]
public class PipeGUI_FreeplayPlayerSelection : MonoBehaviour
{
	private static readonly string s_freeplayAIDifficulty = "Freeplay_AIDifficulty";

	private GameManager m_gameManager;

	private PipeGUI_FreeplayPlayerSelectButton[] m_playerButtons;

	public UIButton m_nextButton;

	public UIStateToggleBtn m_aiDifficultyButton;

	private void Start()
	{
		LoadSettings();
		m_gameManager = GameObjectUtils.FindComponentWithTag<GameManager>("RaceControl");
		m_nextButton.controlIsEnabled = false;
		m_playerButtons = new PipeGUI_FreeplayPlayerSelectButton[4];
		m_playerButtons[0] = GameObject.Find("PipeGUI_PlayerSelectButton_BL").GetComponent<PipeGUI_FreeplayPlayerSelectButton>();
		m_playerButtons[1] = GameObject.Find("PipeGUI_PlayerSelectButton_BR").GetComponent<PipeGUI_FreeplayPlayerSelectButton>();
		m_playerButtons[2] = GameObject.Find("PipeGUI_PlayerSelectButton_TL").GetComponent<PipeGUI_FreeplayPlayerSelectButton>();
		m_playerButtons[3] = GameObject.Find("PipeGUI_PlayerSelectButton_TR").GetComponent<PipeGUI_FreeplayPlayerSelectButton>();
		int num = ((m_gameManager != null) ? m_gameManager.FreeplayHumanPlayerMask : 0);
		int num2 = ((!(m_gameManager != null)) ? 15 : m_gameManager.FreeplayAIPlayerMask);
		for (int i = 0; i != 4; i++)
		{
			m_playerButtons[i].Controller = this;
			if ((num & (1 << i)) != 0)
			{
				m_playerButtons[i].StateNum = PipeGUI_FreeplayPlayerSelectButton.State.Human;
			}
			else if ((num2 & (1 << i)) != 0)
			{
				m_playerButtons[i].StateNum = PipeGUI_FreeplayPlayerSelectButton.State.AI;
			}
			PipeGUI_Audio.EnableAudioInChildren(m_playerButtons[i].gameObject);
		}
		PipeGUI_Audio.EnableAudioInChildren(m_aiDifficultyButton.gameObject);
		PipeGUI_PunchAnim.EnableInChildren(m_aiDifficultyButton.gameObject);
		Refresh();
	}

	private void OnDestroy()
	{
		SaveSettings();
	}

	private void LoadSettings()
	{
		m_aiDifficultyButton.SetToggleState(Settings.GetInt(s_freeplayAIDifficulty, 1));
		RefreshAIText();
	}

	private void SaveSettings()
	{
		Settings.SetInt(s_freeplayAIDifficulty, m_aiDifficultyButton.StateNum);
	}

	public void Refresh()
	{
		bool flag = false;
		for (int i = 0; i != 4; i++)
		{
			flag |= m_playerButtons[i].StateNum == PipeGUI_FreeplayPlayerSelectButton.State.Human;
		}
		m_nextButton.controlIsEnabled = flag;
	}

	private void OnBackSelected()
	{
		if (m_gameManager != null)
		{
			m_gameManager.GoBack();
			m_gameManager = null;
		}
	}

	private void RefreshAIText()
	{
		SpriteText componentInChildren = m_aiDifficultyButton.GetComponentInChildren<SpriteText>();
		if (componentInChildren != null)
		{
			string key = "ID_" + m_aiDifficultyButton.StateName + "AIDifficulty";
			componentInChildren.Text = Language.Get(key);
		}
	}

	private void OnNextSelected()
	{
		if (!(m_gameManager != null))
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i != 4; i++)
		{
			if (m_playerButtons[i].StateNum == PipeGUI_FreeplayPlayerSelectButton.State.Human)
			{
				num |= 1 << (i & 0x1F);
			}
			else if (m_playerButtons[i].StateNum == PipeGUI_FreeplayPlayerSelectButton.State.AI)
			{
				num2 |= 1 << (i & 0x1F);
			}
		}
		m_gameManager.FreeplayHumanPlayerMask = num;
		m_gameManager.FreeplayAIPlayerMask = num2;
		m_gameManager.FreeplayAiDifficulty = (AIDifficulty)m_aiDifficultyButton.StateNum;
		m_gameManager.FreeplayControllers = null;
		m_gameManager.GoForward();
		m_gameManager = null;
	}
}
