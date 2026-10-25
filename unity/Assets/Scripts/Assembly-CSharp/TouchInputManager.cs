using UnityEngine;

public class TouchInputManager : MonoBehaviour
{
	public enum Cutout
	{
		Disabled = 0,
		BottomLeft = 1,
		BottomRight = 2
	}

	public enum SinglePlayerMode
	{
		Screen = 0,
		Edges = 1
	}

	public enum Corner
	{
		BottomLeft = 0,
		BottomRight = 1,
		TopLeft = 2,
		TopRight = 3
	}

	public enum CornerMask
	{
		None = 0,
		BottomLeft = 1,
		BottomRight = 2,
		TopLeft = 4,
		TopRight = 8,
		All = 15
	}

	private CornerMask[] m_playerCorners;

	private TouchRegion[] m_playerTouchRegions;

	private int m_numHumanPlayers;

	private float m_touchLeft;

	private float m_touchRight;
	private int m_humanPlayerMask;
	private int m_layoutWidth, m_layoutHeight;
	private Cutout m_currentCutout;

	public float m_widescreenTouchScale = 1.3333334f;

	public float m_touchButtonSize = 0.25f;

	public float m_visibleButtonSize = 0.125f;

	public float m_maxTouchBorder = 0.5f;

	public int NumHumans
	{
		get
		{
			return m_numHumanPlayers;
		}
	}

	public float TouchLeft
	{
		get
		{
			return m_touchLeft;
		}
	}

	public float TouchRight
	{
		get
		{
			return m_touchRight;
		}
	}

	private Vector2 TouchButtonSize()
	{
		float num = m_touchButtonSize * ((float)m_layoutHeight / m_layoutWidth);
		float num2 = m_touchButtonSize;
		if ((float)m_layoutWidth / m_layoutHeight >= 1.499f)
		{
			num *= m_widescreenTouchScale;
			num2 *= m_widescreenTouchScale;
		}
		return new Vector2(num, num2);
	}

	public void Event_ConfigureForHumanPlayers(int _humanPlayerMask)
	{
		m_humanPlayerMask = _humanPlayerMask;
		m_numHumanPlayers = BitUtils.BitCount(_humanPlayerMask);
		m_playerTouchRegions = new TouchRegion[m_numHumanPlayers];
		m_playerCorners = new CornerMask[m_numHumanPlayers];
		for (int i = 0; i < m_numHumanPlayers; i++)
			m_playerTouchRegions[i] = new TouchRegion(default(Rect));
		RefreshTouchLayout(Screen.width, Screen.height);
	}

	private void RefreshTouchLayout(int width, int height)
	{
		m_layoutWidth = Mathf.Max(1, width);
		m_layoutHeight = Mathf.Max(1, height);
		Vector2 vector = TouchButtonSize();
		bool flag = (m_humanPlayerMask & 5) != 0;
		bool flag2 = (m_humanPlayerMask & 0xA) != 0;
		if (m_numHumanPlayers == 1)
		{
			m_playerTouchRegions[0].SetRegion(new Rect(0f, 0f, 1f, 1f));
			m_playerCorners[0] = CornerMask.All;
		}
		else if (m_numHumanPlayers == 2 && flag && flag2)
		{
			int num = 0;
			for (int i = 0; i != 4; i++)
			{
				if ((m_humanPlayerMask & (1 << i)) != 0)
				{
					float left = 0f;
					CornerMask cornerMask = CornerMask.None;
					switch ((Corner)i)
					{
					case Corner.BottomLeft:
					case Corner.TopLeft:
						left = 0f;
						cornerMask = (CornerMask)5;
						break;
					case Corner.BottomRight:
					case Corner.TopRight:
						left = 1f - vector.x;
						cornerMask = (CornerMask)10;
						break;
					}
					m_playerTouchRegions[num].SetRegion(new Rect(left, 0f, vector.x, 1f));
					m_playerCorners[num] = cornerMask;
					num++;
				}
			}
		}
		else
		{
			int num2 = 0;
			for (int j = 0; j != 4; j++)
			{
				if ((m_humanPlayerMask & (1 << j)) != 0)
				{
					float left2 = 0f;
					float top = 0f;
					switch ((Corner)j)
					{
					case Corner.BottomLeft:
						left2 = 0f;
						top = 0f;
						break;
					case Corner.BottomRight:
						left2 = 1f - vector.x;
						top = 0f;
						break;
					case Corner.TopLeft:
						left2 = 0f;
						top = 1f - vector.y;
						break;
					case Corner.TopRight:
						left2 = 1f - vector.x;
						top = 1f - vector.y;
						break;
					}
					m_playerTouchRegions[num2].SetRegion(new Rect(left2, top, vector.x, vector.y));
					m_playerCorners[num2] = (CornerMask)(1 << j);
					num2++;
				}
			}
		}
		m_touchLeft = 1f;
		m_touchRight = 1f;
	}

	public void Event_ConfigureCutout(Cutout cutout)
	{
		m_currentCutout = cutout;
		Vector2 vector = TouchButtonSize();
		float left;
		float top;
		switch (cutout)
		{
		default:
		{
			TouchRegion[] playerTouchRegions = m_playerTouchRegions;
			foreach (TouchRegion touchRegion in playerTouchRegions)
			{
				touchRegion.ClearCutout();
			}
			return;
		}
		case Cutout.BottomLeft:
			left = 0f;
			top = 0f;
			break;
		case Cutout.BottomRight:
			left = 1f - vector.x;
			top = 0f;
			break;
		}
		Rect cutout2 = new Rect(left, top, vector.x, vector.y);
		TouchRegion[] playerTouchRegions2 = m_playerTouchRegions;
		foreach (TouchRegion touchRegion2 in playerTouchRegions2)
		{
			touchRegion2.SetCutout(cutout2);
		}
	}

	private void Update()
	{
		if (m_playerTouchRegions == null)
		{
			return;
		}
		if (m_layoutWidth != Screen.width || m_layoutHeight != Screen.height)
		{
			RefreshTouchLayout(Screen.width, Screen.height);
			Event_ConfigureCutout(m_currentCutout);
		}
		TouchRegion[] playerTouchRegions = m_playerTouchRegions;
		foreach (TouchRegion touchRegion in playerTouchRegions)
		{
			touchRegion.PreUpdate();
		}
		bool firstTouch = true;
		for (int j = 0; j < Input.touchCount; j++)
		{
			Touch touch = Input.GetTouch(j);
			TouchPhase phase = touch.phase;
			if (phase != TouchPhase.Ended && phase != TouchPhase.Canceled)
			{
				HandleTouch(ref firstTouch, touch.position);
			}
		}
	}

	private void HandleTouch(ref bool firstTouch, Vector3 position)
	{
		if (m_numHumanPlayers == 1)
		{
			if (!m_playerTouchRegions[0].Touch2(position))
			{
				return;
			}
			if (firstTouch)
			{
				firstTouch = false;
				m_touchLeft = 1f;
				m_touchRight = 1f;
			}
			float num = 2f * position.x / (float)Screen.width - 1f;
			if (num > m_maxTouchBorder)
			{
				if (num < m_touchRight)
				{
					m_touchRight = num;
				}
			}
			else if (0f - num > m_maxTouchBorder && 0f - num < m_touchLeft)
			{
				m_touchLeft = 0f - num;
			}
		}
		else
		{
			TouchRegion[] playerTouchRegions = m_playerTouchRegions;
			foreach (TouchRegion touchRegion in playerTouchRegions)
			{
				touchRegion.Touch(position);
			}
		}
	}

	public CornerMask GetCornerMask(int _playerIndex)
	{
		return m_playerCorners[_playerIndex];
	}

	public TouchRegion GetTouchRegion(int _playerIndex)
	{
		return m_playerTouchRegions[_playerIndex];
	}
}
