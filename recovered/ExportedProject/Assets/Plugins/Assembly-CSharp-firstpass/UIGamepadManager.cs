using System;
using System.Collections.Generic;
using UnityEngine;

public class UIGamepadManager : MonoBehaviour
{
	private static List<IUIGamepadTarget> s_targets = new List<IUIGamepadTarget>();

	private static IUIGamepadTarget s_currentTarget = null;

	private static List<IUIGamepadHotkey>[] s_hotkeys = new List<IUIGamepadHotkey>[3];

	private float m_inputTimer;

	public float m_repeatRate = 0.2f;

	public static bool IsGamepadSupported
	{
		get
		{
			return false;
		}
	}

	public static bool IsGamepadActive
	{
		get
		{
			return IsGamepadSupported;
		}
	}

	public static int ActiveLayer { get; private set; }

	public static event Action OnTargetMovedEvent;

	public static void Register(IUIGamepadTarget _target)
	{
		s_targets.Add(_target);
	}

	public static void Unregister(IUIGamepadTarget _target)
	{
		s_targets.Remove(_target);
		if (s_currentTarget == _target)
		{
			s_currentTarget = null;
		}
	}

	public static void Register(IUIGamepadHotkey _hotkey)
	{
		int hotkeyLayer = _hotkey.HotkeyLayer;
		if (hotkeyLayer >= s_hotkeys.Length)
		{
			Debug.LogError("Unsupported hotkey layer");
			return;
		}
		if (s_hotkeys[hotkeyLayer] == null)
		{
			s_hotkeys[hotkeyLayer] = new List<IUIGamepadHotkey>();
		}
		s_hotkeys[hotkeyLayer].Add(_hotkey);
	}

	public static void Unregister(IUIGamepadHotkey _hotkey)
	{
		int hotkeyLayer = _hotkey.HotkeyLayer;
		if (hotkeyLayer >= s_hotkeys.Length)
		{
			Debug.LogError("Unsupported hotkey layer");
		}
		else
		{
			s_hotkeys[hotkeyLayer].Remove(_hotkey);
		}
	}

	public static void OnTargetMoved()
	{
		if (OnTargetMovedEvent != null && s_currentTarget != null)
		{
			OnTargetMovedEvent();
		}
	}

	public static void OnTargetUnavailable(IUIGamepadTarget _target)
	{
		if (_target == s_currentTarget)
		{
			s_currentTarget.OnLostFocus();
			s_currentTarget = null;
		}
	}

	private void Update()
	{
		if (IsGamepadSupported && (ActiveLayer = UpdateGlobalHotkeys()) == 0)
		{
			UpdateTargets();
		}
	}

	private void UpdateTargets()
	{
		if (s_targets.Count == 0)
		{
			return;
		}
		if (s_currentTarget == null)
		{
			IUIGamepadTarget iUIGamepadTarget = null;
			for (int i = 0; i < s_targets.Count; i++)
			{
				if (s_targets[i].IsValidStart && s_targets[i].IsValidTarget)
				{
					iUIGamepadTarget = s_targets[i];
					break;
				}
			}
			if (iUIGamepadTarget != null)
			{
				iUIGamepadTarget.OnFocus();
				s_currentTarget = iUIGamepadTarget;
			}
		}
		if (s_currentTarget == null)
		{
			return;
		}
		GamepadTargetDirection gamepadTargetDirection = PollDirection();
		if (gamepadTargetDirection != GamepadTargetDirection.None)
		{
			IUIGamepadTarget iUIGamepadTarget2 = s_currentTarget.OnDirection(gamepadTargetDirection);
			if (iUIGamepadTarget2 != null && iUIGamepadTarget2.IsValidTarget)
			{
				s_currentTarget.OnLostFocus();
				s_currentTarget = iUIGamepadTarget2;
				s_currentTarget.OnFocus();
				OnTargetMoved();
			}
		}
		if (Input.GetButtonDown("Confirm"))
		{
			s_currentTarget.OnEvent(POINTER_INFO.INPUT_EVENT.PRESS);
		}
		else if (Input.GetButtonUp("Confirm"))
		{
			s_currentTarget.OnEvent(POINTER_INFO.INPUT_EVENT.TAP);
		}
		int numHotkeys = s_currentTarget.NumHotkeys;
		for (int j = 0; j < numHotkeys; j++)
		{
			IUIGamepadHotkey hotkey = s_currentTarget.GetHotkey(j);
			if (Input.GetButtonDown(hotkey.HotkeyName))
			{
				hotkey.OnEvent(POINTER_INFO.INPUT_EVENT.PRESS);
			}
			else if (Input.GetButtonUp(hotkey.HotkeyName))
			{
				hotkey.OnEvent(POINTER_INFO.INPUT_EVENT.TAP);
			}
		}
	}

	private int UpdateGlobalHotkeys()
	{
		for (int num = s_hotkeys.Length - 1; num >= 0; num--)
		{
			List<IUIGamepadHotkey> list = s_hotkeys[num];
			int num2 = ((list != null) ? list.Count : 0);
			for (int i = 0; i < num2; i++)
			{
				IUIGamepadHotkey iUIGamepadHotkey = list[i];
				if (Input.GetButtonDown(iUIGamepadHotkey.HotkeyName))
				{
					iUIGamepadHotkey.OnEvent(POINTER_INFO.INPUT_EVENT.PRESS);
				}
				else if (Input.GetButtonUp(iUIGamepadHotkey.HotkeyName))
				{
					iUIGamepadHotkey.OnEvent(POINTER_INFO.INPUT_EVENT.TAP);
				}
			}
			if (num2 > 0)
			{
				return num;
			}
		}
		return 0;
	}

	private float GetAxis(float _p, float _a)
	{
		return (!(Mathf.Abs(_p) > 0.1f)) ? _a : _p;
	}

	private GamepadTargetDirection PollDirection()
	{
		if (Time.realtimeSinceStartup < m_inputTimer)
		{
			return GamepadTargetDirection.None;
		}
		GamepadTargetDirection gamepadTargetDirection = GamepadTargetDirection.None;
		float axis = Input.GetAxis("Vertical");
		float axis2 = Input.GetAxis("Horizontal");
		if (Input.GetButton("DPadUp") || axis > 0.5f)
		{
			gamepadTargetDirection = GamepadTargetDirection.Up;
		}
		else if (Input.GetButton("DPadDown") || axis < -0.5f)
		{
			gamepadTargetDirection = GamepadTargetDirection.Down;
		}
		else if (Input.GetButton("DPadLeft") || axis2 < -0.5f)
		{
			gamepadTargetDirection = GamepadTargetDirection.Left;
		}
		else if (Input.GetButton("DPadRight") || axis2 > 0.5f)
		{
			gamepadTargetDirection = GamepadTargetDirection.Right;
		}
		if (gamepadTargetDirection != GamepadTargetDirection.None)
		{
			m_inputTimer = Time.realtimeSinceStartup + m_repeatRate;
		}
		return gamepadTargetDirection;
	}
}
