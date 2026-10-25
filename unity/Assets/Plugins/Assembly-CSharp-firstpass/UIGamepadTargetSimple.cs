using System;
using System.Collections;
using UnityEngine;

public class UIGamepadTargetSimple : MonoBehaviour, IUIGamepadTarget
{
	private IUIObject m_control;

	private IUIGamepadTarget[] m_directions;

	private float m_pulseTimer;

	public GameObject m_target;

	public Transform m_scale;

	public GameObject m_up;

	public GameObject m_down;

	public GameObject m_left;

	public GameObject m_right;

	public bool m_validStart;

	public UIGamepadHotkey[] m_hotkeys;

	public bool IsValidTarget
	{
		get
		{
			return base.gameObject.active && base.enabled;
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
			return (m_hotkeys != null) ? m_hotkeys.Length : 0;
		}
	}

	protected bool AllowInput
	{
		get
		{
			return m_control != null && m_control.controlIsEnabled && base.enabled && base.gameObject.active && UIManager.instance != null && UIManager.instance.enabled && !UIManager.instance.blockInput;
		}
	}

	GameObject IUIGamepadTarget.gameObject
	{
		get
		{
			return base.gameObject;
		}
	}

	Transform IUIGamepadTarget.transform
	{
		get
		{
			return base.transform;
		}
	}

	public event Action OnFocusEvent;

	public event Action OnLostFocusEvent;

	private void Start()
	{
		GameObject gameObject = ((!(m_target != null)) ? base.gameObject : m_target);
		m_control = (IUIObject)gameObject.GetComponent(typeof(IUIObject));
		if (m_scale == null)
		{
			m_scale = base.transform;
		}
		m_directions = new IUIGamepadTarget[Enum.GetValues(typeof(GamepadTargetDirection)).Length];
		m_directions[3] = GetTarget(m_up);
		m_directions[4] = GetTarget(m_down);
		m_directions[1] = GetTarget(m_left);
		m_directions[2] = GetTarget(m_right);
		UIGamepadManager.Register(this);
	}

	private void OnDestroy()
	{
		UIGamepadManager.Unregister(this);
	}

	public void OnEvent(POINTER_INFO.INPUT_EVENT _event)
	{
		if (AllowInput)
		{
			POINTER_INFO ptr = new POINTER_INFO
			{
				evt = _event
			};
			m_control.OnInput(ptr);
			if (_event == POINTER_INFO.INPUT_EVENT.PRESS)
			{
				m_pulseTimer = Time.realtimeSinceStartup;
			}
		}
	}

	public void OnFocus()
	{
		m_scale.localScale = Vector3.one * 1.1f;
		if (base.gameObject.active)
		{
			m_pulseTimer = 0f;
			StartCoroutine("FocusAnimation");
		}
		m_scale.gameObject.BroadcastMessage("OnScaleChange", SendMessageOptions.DontRequireReceiver);
		if (OnFocusEvent != null)
		{
			OnFocusEvent();
		}
	}

	public void OnLostFocus()
	{
		StopCoroutine("FocusAnimation");
		m_scale.localScale = Vector3.one;
		m_scale.gameObject.BroadcastMessage("OnScaleChange", SendMessageOptions.DontRequireReceiver);
		if (OnLostFocusEvent != null)
		{
			OnLostFocusEvent();
		}
	}

	public IUIGamepadTarget OnDirection(GamepadTargetDirection _direction)
	{
		return m_directions[(int)_direction];
	}

	public IUIGamepadHotkey GetHotkey(int _index)
	{
		return m_hotkeys[_index];
	}

	private IEnumerator FocusAnimation()
	{
		Vector3 scale = m_scale.localScale;
		float lookup = 0f;
		float lastTime = Time.realtimeSinceStartup;
		while (true)
		{
			float time = Time.realtimeSinceStartup;
			float deltaTime = time - lastTime;
			lastTime = time;
			float duration = 0.8f;
			float valScale = 0.025f;
			float timeScale = 5f;
			float val = Mathf.Sin(lookup) * valScale;
			float magnitude = 0.1f;
			if (m_pulseTimer + duration > time)
			{
				float timeElapsed = time - m_pulseTimer;
				float factor = timeElapsed / duration;
				val -= EZAnimation.punch(magnitude, factor);
				timeScale *= factor;
			}
			lookup += deltaTime * timeScale;
			m_scale.localScale = scale + new Vector3(val, val, val);
			m_scale.gameObject.BroadcastMessage("OnScaleChange", SendMessageOptions.DontRequireReceiver);
			yield return null;
		}
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
