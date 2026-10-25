using UnityEngine;

public interface IUIGamepadTarget
{
	GameObject gameObject { get; }

	Transform transform { get; }

	bool IsValidTarget { get; }

	int TargetLayer { get; }

	bool IsValidStart { get; }

	int NumHotkeys { get; }

	void OnEvent(POINTER_INFO.INPUT_EVENT _event);

	void OnFocus();

	void OnLostFocus();

	IUIGamepadTarget OnDirection(GamepadTargetDirection _direction);

	IUIGamepadHotkey GetHotkey(int _index);
}
