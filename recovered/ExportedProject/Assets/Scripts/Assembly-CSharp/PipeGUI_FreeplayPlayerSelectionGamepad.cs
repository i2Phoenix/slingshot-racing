using UnityEngine;

[AddComponentMenu("GUI/PipeGUI/FreeplayPlayerSelectionGamepad")]
public class PipeGUI_FreeplayPlayerSelectionGamepad : MonoBehaviour
{
	private GameManager m_gameManager;

	private PipeGUI_PlayerSelectButtonGamepad[] m_playerButtons;

	private PipeGUI_PlayerSelectButtonGamepad[] m_colourMap;

	public UIButton m_nextButton;

	public PipeGUI_AIDifficultyButtonGamepad m_aiDifficultyButton;

	private void Start()
	{
		LoadSettings();
		m_gameManager = GameObjectUtils.FindComponentWithTag<GameManager>("RaceControl");
		m_nextButton.controlIsEnabled = false;
		m_playerButtons = new PipeGUI_PlayerSelectButtonGamepad[4];
		m_colourMap = new PipeGUI_PlayerSelectButtonGamepad[4];
		m_playerButtons[0] = GameObject.Find("PipeGUI_PlayerSelectButtonGamepad_P1").GetComponent<PipeGUI_PlayerSelectButtonGamepad>();
		m_playerButtons[1] = GameObject.Find("PipeGUI_PlayerSelectButtonGamepad_P2").GetComponent<PipeGUI_PlayerSelectButtonGamepad>();
		m_playerButtons[2] = GameObject.Find("PipeGUI_PlayerSelectButtonGamepad_P3").GetComponent<PipeGUI_PlayerSelectButtonGamepad>();
		m_playerButtons[3] = GameObject.Find("PipeGUI_PlayerSelectButtonGamepad_P4").GetComponent<PipeGUI_PlayerSelectButtonGamepad>();
		for (int i = 0; i != 4; i++)
		{
			m_playerButtons[i].Controller = this;
		}
		Refresh();
	}

	private void OnDestroy()
	{
		SaveSettings();
	}

	private void LoadSettings()
	{
	}

	private void SaveSettings()
	{
	}

	public void Refresh()
	{
		bool flag = false;
		for (int i = 0; i != 4; i++)
		{
			flag |= m_playerButtons[i].Joined;
		}
		m_nextButton.controlIsEnabled = flag;
	}

	public void OnBackSelected()
	{
		if (m_gameManager != null)
		{
			m_gameManager.GoBack();
			m_gameManager = null;
		}
	}

	public void OnJoinedGame(PipeGUI_PlayerSelectButtonGamepad _button)
	{
		for (int i = 0; i != 4; i++)
		{
			if (m_colourMap[i] == null)
			{
				m_colourMap[i] = _button;
				_button.Colour = i;
				break;
			}
		}
	}

	public void NextColour(PipeGUI_PlayerSelectButtonGamepad _button)
	{
		int num = _button.Colour;
		m_colourMap[num] = null;
		for (int i = 0; i < 4; i++)
		{
			num = ++num % 4;
			if (m_colourMap[num] == null)
			{
				m_colourMap[num] = _button;
				_button.Colour = num;
				break;
			}
		}
	}

	public void OnLeftGame(PipeGUI_PlayerSelectButtonGamepad _button)
	{
		m_colourMap[_button.Colour] = null;
	}

	private void OnNextSelected()
	{
		if (!(m_gameManager != null))
		{
			return;
		}
		int[] array = new int[4];
		bool aIEnabled = m_aiDifficultyButton.AIEnabled;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i != 4; i++)
		{
			if (m_colourMap[i] != null && m_colourMap[i].Joined)
			{
				num |= 1 << (i & 0x1F);
				array[num3++] = m_colourMap[i].ControllerIndex;
			}
			else if (aIEnabled)
			{
				num2 |= 1 << (i & 0x1F);
			}
		}
		m_gameManager.FreeplayHumanPlayerMask = num;
		m_gameManager.FreeplayAIPlayerMask = num2;
		m_gameManager.FreeplayAiDifficulty = m_aiDifficultyButton.Difficulty;
		m_gameManager.FreeplayControllers = array;
		m_gameManager.GoForward();
		m_gameManager = null;
	}
}
