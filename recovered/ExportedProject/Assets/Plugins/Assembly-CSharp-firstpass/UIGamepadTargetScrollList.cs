using System;
using UnityEngine;

public class UIGamepadTargetScrollList : MonoBehaviour, IUIGamepadTarget
{
	private IUIGamepadTarget m_current;

	private IUIGamepadTarget[] m_directions;

	private UIScrollList m_scrollList;

	public GameObject m_up;

	public GameObject m_down;

	public GameObject m_left;

	public GameObject m_right;

	public bool m_validStart;

	public bool IsValidTarget
	{
		get
		{
			return true;
		}
	}

	public int TargetLayer
	{
		get
		{
			return 0;
		}
	}

	public bool IsValidStart
	{
		get
		{
			return m_validStart;
		}
	}

	public int NumHotkeys
	{
		get
		{
			return (m_current != null) ? m_current.NumHotkeys : 0;
		}
	}

	virtual GameObject IUIGamepadTarget.gameObject
	{
		get
		{
			return base.gameObject;
		}
	}

	virtual Transform IUIGamepadTarget.transform
	{
		get
		{
			return base.transform;
		}
	}

	private void Awake()
	{
		m_scrollList = GetComponent<UIScrollList>();
		m_directions = new IUIGamepadTarget[Enum.GetValues(typeof(GamepadTargetDirection)).Length];
		m_directions[3] = GetTarget(m_up);
		m_directions[4] = GetTarget(m_down);
		m_directions[1] = GetTarget(m_left);
		m_directions[2] = GetTarget(m_right);
	}

	private void Start()
	{
		UIGamepadManager.Register(this);
		m_scrollList.AddItemSnappedDelegate(OnItemSnapped);
	}

	private void OnDestroy()
	{
		UIGamepadManager.Unregister(this);
		m_scrollList.RemoveItemSnappedDelegate(OnItemSnapped);
	}

	public void OnEvent(POINTER_INFO.INPUT_EVENT _event)
	{
		if (m_current != null)
		{
			m_current.OnEvent(_event);
		}
	}

	public void OnFocus()
	{
		ScrollList(GamepadTargetDirection.None);
	}

	public void OnLostFocus()
	{
		if (m_current != null)
		{
			m_current.OnLostFocus();
		}
	}

	public IUIGamepadTarget OnDirection(GamepadTargetDirection _direction)
	{
		if (m_current != null)
		{
			IUIGamepadTarget iUIGamepadTarget = m_current.OnDirection(_direction);
			if (iUIGamepadTarget != null)
			{
				m_current.OnLostFocus();
				m_current = iUIGamepadTarget;
				m_current.OnFocus();
				UIGamepadManager.OnTargetMoved();
				return null;
			}
		}
		if (_direction == GamepadTargetDirection.Left || _direction == GamepadTargetDirection.Right)
		{
			ScrollList(_direction);
			return null;
		}
		return m_directions[(int)_direction];
	}

	public IUIGamepadHotkey GetHotkey(int _index)
	{
		object result;
		if (m_current != null)
		{
			IUIGamepadHotkey hotkey = m_current.GetHotkey(_index);
			result = hotkey;
		}
		else
		{
			result = null;
		}
		return (IUIGamepadHotkey)result;
	}

	private void ScrollList(GamepadTargetDirection _direction)
	{
		IUIListObject snappedItem = m_scrollList.SnappedItem;
		int index = ((snappedItem != null) ? snappedItem.Index : 0);
		if (snappedItem != null && _direction != GamepadTargetDirection.None)
		{
			index = snappedItem.Index + ((_direction == GamepadTargetDirection.Right) ? 1 : (-1));
			index = Mathf.Clamp(index, 0, m_scrollList.Count);
			UIGamepadManager.OnTargetMoved();
		}
		m_scrollList.ScrollToItem(index, 0.5f);
	}

	private IUIGamepadTarget GetTarget(GameObject _obj)
	{
		IUIGamepadTarget result = null;
		if (_obj != null)
		{
			bool active = _obj.active;
			_obj.active = true;
			result = (IUIGamepadTarget)_obj.GetComponent(typeof(IUIGamepadTarget));
			_obj.active = active;
		}
		return result;
	}

	private void OnItemSnapped(IUIListObject _item)
	{
		if (_item == null || !UIGamepadManager.IsGamepadActive)
		{
			return;
		}
		IUIGamepadTarget target = GetTarget(_item.gameObject);
		if (target != m_current)
		{
			if (m_current != null)
			{
				m_current.OnLostFocus();
			}
			m_current = target;
			if (m_current != null)
			{
				m_current.OnFocus();
			}
		}
	}
}
