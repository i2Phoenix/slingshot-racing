using UnityEngine;

public class PipeGUI_PlayerSelectButtonGamepad : MonoBehaviour
{
	private AudioPlayer m_audioPlayer;

	private string m_joinInputName;

	private string m_leaveInputName;

	private string m_colourInputName;

	private bool m_hasJoined;

	private int m_colourIndex;

	public int m_playerIndex;

	public GameObject m_joinedObjects;

	public GameObject m_notJoinedObjects;

	public PackedSprite m_colourSprite;

	public AudioBank m_audioBank;

	public bool Joined
	{
		get
		{
			return m_hasJoined;
		}
		private set
		{
			m_hasJoined = value;
			m_joinedObjects.SetActiveRecursively(m_hasJoined);
			m_notJoinedObjects.SetActiveRecursively(!m_hasJoined);
			if (Controller != null)
			{
				Controller.Refresh();
			}
		}
	}

	public PipeGUI_FreeplayPlayerSelectionGamepad Controller { get; set; }

	public int Colour
	{
		get
		{
			return m_colourIndex;
		}
		set
		{
			m_colourIndex = value;
			m_colourSprite.PlayAnim(m_colourIndex);
			m_colourSprite.defaultAnim = m_colourIndex;
		}
	}

	public int ControllerIndex
	{
		get
		{
			return m_playerIndex;
		}
	}

	private void Start()
	{
		m_audioPlayer = GetComponent<AudioPlayer>();
		if (m_audioPlayer == null)
		{
			m_audioPlayer = base.gameObject.AddComponent<AudioPlayer>();
		}
		m_joinInputName = string.Format("Player{0}Join", m_playerIndex);
		m_leaveInputName = string.Format("Player{0}Leave", m_playerIndex);
		m_colourInputName = string.Format("Player{0}Colour", m_playerIndex);
		Joined = false;
	}

	private void Update()
	{
		if (m_hasJoined)
		{
			if (Input.GetButtonDown(m_leaveInputName))
			{
				Joined = false;
				m_audioPlayer.PlayRandomGUIClip(m_audioBank);
				if (Controller != null)
				{
					Controller.OnLeftGame(this);
				}
			}
			else if (Input.GetButtonDown(m_colourInputName))
			{
				if (Controller != null)
				{
					Controller.NextColour(this);
				}
				m_audioPlayer.PlayRandomGUIClip(m_audioBank);
			}
		}
		else if (Input.GetButtonDown(m_joinInputName))
		{
			Joined = true;
			m_audioPlayer.PlayRandomGUIClip(m_audioBank);
			if (Controller != null)
			{
				Controller.OnJoinedGame(this);
			}
		}
		else if (Input.GetButtonDown(m_leaveInputName) && Controller != null)
		{
			Controller.OnBackSelected();
			m_audioPlayer.PlayRandomGUIClip(m_audioBank);
		}
	}
}
