using UnityEngine;

public class PipeGUI_Audio : MonoBehaviour
{
	private ControlBase m_control;

	private AutoSpriteControlBase m_spriteControl;

	private AudioPlayer m_audioPlayer;

	public AudioBank m_audioBank;

	public static void EnableAudioInChildren(GameObject gameObject)
	{
		PipeGUI_Audio[] componentsInChildren = GameObjectUtils.GetComponentsInChildren<PipeGUI_Audio>(gameObject);
		PipeGUI_Audio[] array = componentsInChildren;
		foreach (PipeGUI_Audio pipeGUI_Audio in array)
		{
			pipeGUI_Audio.enabled = true;
		}
	}

	private void Start()
	{
		m_control = GetComponent<ControlBase>();
		if (m_control != null)
		{
			m_control.AddValueChangedDelegate(OnValueChanged);
		}
		m_spriteControl = GetComponent<AutoSpriteControlBase>();
		if (m_spriteControl != null)
		{
			m_spriteControl.AddValueChangedDelegate(OnValueChanged);
		}
		m_audioPlayer = GetComponent<AudioPlayer>();
		if (m_audioPlayer == null)
		{
			m_audioPlayer = base.gameObject.AddComponent<AudioPlayer>();
		}
	}

	private void OnValueChanged(IUIObject obj)
	{
		GameObject gameObject = obj.gameObject;
		UIRadioBtn component = gameObject.GetComponent<UIRadioBtn>();
		if ((!(component != null) || component.Value) && m_audioBank != null)
		{
			m_audioPlayer.PlayRandomGUIClip(m_audioBank);
		}
	}
}
