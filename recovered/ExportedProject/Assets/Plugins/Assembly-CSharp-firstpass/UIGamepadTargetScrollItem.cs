using UnityEngine;

public class UIGamepadTargetScrollItem : MonoBehaviour, IUIGamepadTarget
{
	private UIBistateInteractivePanel m_panel;

	private IUIGamepadTarget m_focusTarget;

	public GameObject m_focus;

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
			return false;
		}
	}

	public int NumHotkeys
	{
		get
		{
			return m_focusTarget.NumHotkeys;
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
		m_focusTarget = GetTarget(m_focus);
	}

	private void Start()
	{
		UIGamepadManager.Register(this);
	}

	private void OnDestroy()
	{
		UIGamepadManager.Unregister(this);
	}

	public void OnEvent(POINTER_INFO.INPUT_EVENT _event)
	{
		m_focusTarget.OnEvent(_event);
	}

	public void OnFocus()
	{
		m_focusTarget.OnFocus();
	}

	public void OnLostFocus()
	{
		m_focusTarget.OnLostFocus();
	}

	public IUIGamepadTarget OnDirection(GamepadTargetDirection _direction)
	{
		return m_focusTarget.OnDirection(_direction);
	}

	public IUIGamepadHotkey GetHotkey(int _index)
	{
		return m_focusTarget.GetHotkey(_index);
	}

	private IUIGamepadTarget GetTarget(GameObject _obj)
	{
		if (_obj != null)
		{
			return (IUIGamepadTarget)_obj.GetComponent(typeof(IUIGamepadTarget));
		}
		return null;
	}
}
