public class UIGamepadHotkeySlider : UIGamepadHotkey
{
	public UISlider m_slider;

	public float m_increment;

	protected override void Start()
	{
	}

	public override void OnEvent(POINTER_INFO.INPUT_EVENT _event)
	{
		if (m_slider != null && _event == POINTER_INFO.INPUT_EVENT.PRESS)
		{
			m_slider.Value += m_increment;
		}
	}
}
