using UnityEngine;

[RequireComponent(typeof(UIStateToggleBtn))]
public class PipeGUI_StateToggle_Text : MonoBehaviour
{
	private UIStateToggleBtn m_toggle;

	public SpriteText m_text;

	public LocString[] m_strings;

	private void Awake()
	{
		m_toggle = GetComponent<UIStateToggleBtn>();
		m_toggle.AddValueChangedDelegate(OnValueChanged);
	}

	private void OnDestroy()
	{
		m_toggle.RemoveValueChangedDelegate(OnValueChanged);
	}

	public void OnValueChanged(IUIObject _obj)
	{
		m_text.Text = m_strings[m_toggle.StateNum].Text;
	}
}
