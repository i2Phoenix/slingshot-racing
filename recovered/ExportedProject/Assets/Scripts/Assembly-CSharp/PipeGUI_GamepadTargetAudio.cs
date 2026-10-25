using UnityEngine;

public class PipeGUI_GamepadTargetAudio : MonoBehaviour
{
	private AudioPlayer m_audioPlayer;

	public AudioBank m_targetMovedAudio;

	private void Start()
	{
		m_audioPlayer = GetComponent<AudioPlayer>();
		if (m_audioPlayer == null)
		{
			m_audioPlayer = base.gameObject.AddComponent<AudioPlayer>();
		}
		UIGamepadManager.OnTargetMovedEvent += OnTargetMoved;
	}

	private void OnDestroy()
	{
		UIGamepadManager.OnTargetMovedEvent -= OnTargetMoved;
	}

	private void OnTargetMoved()
	{
		m_audioPlayer.PlayRandomGUIClip(m_targetMovedAudio);
	}
}
