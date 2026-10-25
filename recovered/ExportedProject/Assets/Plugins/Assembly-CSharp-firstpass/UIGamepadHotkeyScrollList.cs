using System;
using UnityEngine;

public class UIGamepadHotkeyScrollList : MonoBehaviour
{
	[Serializable]
	public class ButtonHandler : IUIGamepadHotkey
	{
		[NonSerialized]
		public UIGamepadHotkeyScrollList m_owner;

		public string m_keyName;

		public bool IsHotkeyActive
		{
			get
			{
				return true;
			}
		}

		public int HotkeyLayer
		{
			get
			{
				return m_owner.m_inputLayer;
			}
		}

		public string HotkeyName
		{
			get
			{
				return m_keyName;
			}
		}

		public void OnEvent(POINTER_INFO.INPUT_EVENT _event)
		{
			m_owner.OnEvent(_event, this);
		}
	}

	private bool m_registered;

	public UIScrollList m_scrollList;

	public ButtonHandler m_left;

	public ButtonHandler m_right;

	public int m_inputLayer;

	public bool m_global;

	public float m_scrollTime = 0.5f;

	public bool m_sendMoveNotification = true;

	private void Awake()
	{
		m_left.m_owner = this;
		m_right.m_owner = this;
	}

	protected virtual void OnDestroy()
	{
		if (m_registered)
		{
			UIGamepadManager.Unregister(m_left);
			UIGamepadManager.Unregister(m_right);
			m_registered = false;
		}
	}

	protected virtual void OnEnable()
	{
		if (m_global)
		{
			UIGamepadManager.Register(m_left);
			UIGamepadManager.Register(m_right);
			m_registered = true;
		}
	}

	protected virtual void OnDisable()
	{
		if (m_registered)
		{
			UIGamepadManager.Unregister(m_left);
			UIGamepadManager.Unregister(m_right);
			m_registered = false;
		}
	}

	private void OnEvent(POINTER_INFO.INPUT_EVENT _event, ButtonHandler _sender)
	{
		if (_event != POINTER_INFO.INPUT_EVENT.PRESS)
		{
			return;
		}
		bool flag = _sender == m_right;
		IUIListObject snappedItem = m_scrollList.SnappedItem;
		int num = 0;
		if (snappedItem != null)
		{
			num = snappedItem.Index + (flag ? 1 : (-1));
			num = Mathf.Clamp(num, 0, m_scrollList.Count);
			if (num != snappedItem.Index && m_sendMoveNotification)
			{
				UIGamepadManager.OnTargetMoved();
			}
		}
		m_scrollList.ScrollToItem(num, m_scrollTime);
	}
}
