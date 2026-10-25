using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioPlayer))]
public class PipeGUI_Panel_Audio : MonoBehaviour
{
	private AudioPlayer m_audioPlayer;

	public AudioBank m_audioBankBringIn;

	public float m_delayBringIn;

	public AudioBank m_audioBankDismiss;

	public float m_delayDismiss;

	private void Start()
	{
		m_audioPlayer = GetComponent<AudioPlayer>();
	}

	private void BringIn()
	{
		StopAllCoroutines();
		if (m_audioBankBringIn != null)
		{
			StartCoroutine(PlayAudio(m_audioBankBringIn, m_delayBringIn));
		}
	}

	private void Dismiss()
	{
		StopAllCoroutines();
		if (m_audioBankDismiss != null)
		{
			StartCoroutine(PlayAudio(m_audioBankDismiss, m_delayDismiss));
		}
	}

	private IEnumerator PlayAudio(AudioBank _bank, float _delay)
	{
		yield return StartCoroutine(YieldCache.WaitForRealSeconds(_delay, PauseLevel.Game));
		m_audioPlayer.PlayRandomGUIClip(_bank);
	}
}
