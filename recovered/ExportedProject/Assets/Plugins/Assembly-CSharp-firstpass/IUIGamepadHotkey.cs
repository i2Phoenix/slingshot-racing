public interface IUIGamepadHotkey
{
	bool IsHotkeyActive { get; }

	int HotkeyLayer { get; }

	string HotkeyName { get; }

	void OnEvent(POINTER_INFO.INPUT_EVENT _event);
}
