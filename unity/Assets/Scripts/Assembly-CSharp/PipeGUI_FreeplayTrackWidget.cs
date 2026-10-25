using UnityEngine;

[AddComponentMenu("GUI/PipeGUI/FreeplayTrackWidget")]
public class PipeGUI_FreeplayTrackWidget : MonoBehaviour
{
	private MonoBehaviour m_manager;

	private UIListItemContainer m_listItemContainer;

	private int m_index;

	private RaceEvent m_race;

	public bool m_autoScroll = true;

	public float m_autoScrollTime = 0.5f;

	public PackedSprite m_plate;

	public UIButton3D m_button;

	public AudioBank m_selectedAudioBank;

	public PackedSprite m_indexPlate;

	public SpriteText m_indexText;

	public PackedSprite m_reversedSprite;

	public SpriteText m_debugText;

	public RaceEvent RaceEvent
	{
		get
		{
			return m_race;
		}
	}

	public int Index
	{
		get
		{
			return m_index;
		}
		set
		{
			if (value != 0 && m_indexText != null)
			{
				m_indexText.Text = value.ToString();
			}
			if (m_indexPlate != null)
			{
				m_indexPlate.gameObject.SetActiveRecursively(value != 0);
			}
			m_index = value;
		}
	}

	public MonoBehaviour Manager
	{
		set
		{
			m_manager = value;
		}
	}

	private void Start()
	{
		m_listItemContainer = base.gameObject.GetComponent<UIListItemContainer>();
		Index = 0;
	}

	public void SetRaceData(RaceEvent _race)
	{
		m_race = _race;
		Refresh();
	}

	public void OnSelected()
	{
		if (m_autoScroll && m_listItemContainer != null)
		{
			UIScrollList scrollList = m_listItemContainer.GetScrollList();
			if (scrollList != null)
			{
				scrollList.ScrollToItem(m_listItemContainer, m_autoScrollTime);
			}
		}
		if (m_manager != null)
		{
			m_manager.SendMessage("OnWidgetSelected", this);
		}
	}

	private void Refresh()
	{
		UVAnimation anim = m_plate.GetAnim(m_race.m_layout.ID);
		if (anim != null)
		{
			m_plate.PlayAnim(anim);
			m_plate.defaultAnim = anim.index;
		}
		m_reversedSprite.Hide(!m_race.m_layout.m_reverseTrack);
		if (m_debugText != null)
		{
			if (Settings.DevFreeplay)
			{
				m_debugText.gameObject.active = true;
				m_debugText.Text = m_race.m_layout.name;
			}
			else
			{
				m_debugText.gameObject.active = false;
			}
		}
	}
}
