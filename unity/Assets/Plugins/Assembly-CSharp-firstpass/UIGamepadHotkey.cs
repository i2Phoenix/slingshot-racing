using UnityEngine;

public class UIGamepadHotkey : MonoBehaviour, IUIGamepadHotkey
{
	private IUIObject m_control;

	private bool m_registered;

	public GameObject m_target;

	public string m_keyName;

	public int m_inputLayer;

	public bool m_global;

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
			return m_inputLayer;
		}
	}

	public string HotkeyName
	{
		get
		{
			return m_keyName;
		}
	}

	protected bool AllowInput
	{
		get
		{
			return m_control != null && m_control.controlIsEnabled && base.enabled && base.gameObject.active && UIManager.instance != null && UIManager.instance.enabled && !UIManager.instance.blockInput;
		}
	}

	protected virtual void Start()
	{
		GameObject gameObject = ((!(m_target != null)) ? base.gameObject : m_target);
		m_control = (IUIObject)gameObject.GetComponent(typeof(IUIObject));
	}

	protected virtual void OnDestroy()
	{
		if (m_registered)
		{
			UIGamepadManager.Unregister(this);
			m_registered = false;
		}
	}

	protected virtual void OnEnable()
	{
		if (m_global)
		{
			UIGamepadManager.Register(this);
			m_registered = true;
		}
	}

	protected virtual void OnDisable()
	{
		if (m_registered)
		{
			UIGamepadManager.Unregister(this);
			m_registered = false;
		}
	}

	public virtual void OnEvent(POINTER_INFO.INPUT_EVENT _event)
	{
		if (AllowInput)
		{
			POINTER_INFO ptr = new POINTER_INFO
			{
				evt = _event
			};
			m_control.OnInput(ptr);
		}
	}
}
