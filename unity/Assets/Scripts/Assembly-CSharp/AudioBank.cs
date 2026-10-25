using System;
using UnityEngine;

public class AudioBank : ScriptableObject
{
	[Serializable]
	public class Clip
	{
		public AudioClip m_clip;

		public float m_volumeScale;
	}

	private int m_lastClipPlayed = -1;

	[HideInInspector]
	public Clip[] m_clips;

	public int m_priority = 128;

	public bool m_interuptEqualPriority;

	public float m_pitch = 1f;

	public float m_volumeScale = 1f;

	public void PlayRandomClip(AudioSource _audioSource)
	{
		PlayRandomClip(_audioSource, 1f, 1f, false);
	}

	public bool PlayRandomClip(AudioSource _audioSource, float _volumeScale, float _pitch, bool _ignoreListenerVolume)
	{
		if (m_clips == null || m_clips.Length == 0 || (_audioSource.isPlaying && !m_interuptEqualPriority && m_priority >= _audioSource.priority) || (_audioSource.isPlaying && m_interuptEqualPriority && m_priority > _audioSource.priority))
		{
			return false;
		}
		int num;
		if (m_clips.Length > 1 && m_lastClipPlayed >= 0)
		{
			num = UnityEngine.Random.Range(0, m_clips.Length - 1);
			if (num >= m_lastClipPlayed)
			{
				num++;
			}
		}
		else
		{
			num = UnityEngine.Random.Range(0, m_clips.Length);
		}
		m_lastClipPlayed = num;
		AudioClip clip = m_clips[num].m_clip;
		if (clip == null)
		{
			return false;
		}
		if (_audioSource.isPlaying)
		{
			_audioSource.Stop();
		}
		_audioSource.gameObject.active = true;
		_audioSource.priority = m_priority;
		_audioSource.pitch = _pitch * m_pitch;
		_audioSource.clip = clip;
		_audioSource.volume = m_clips[num].m_volumeScale * _volumeScale * m_volumeScale;
		_audioSource.ignoreListenerVolume = _ignoreListenerVolume;
		_audioSource.Play();
		return true;
	}
}
