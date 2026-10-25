using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("Audio/Audio Object")]
public class AudioObject : MonoBehaviour
{
	private const float c_tryFreeRate = 1f / 15f;

	private const int c_defaultPriority = 128;

	private Transform m_transform;

	private AudioManager m_manager;

	private AudioSource m_audioSource;

	private AudioPlayer m_player;

	public int Priority
	{
		get
		{
			return m_audioSource.priority;
		}
	}

	private void Awake()
	{
		m_transform = base.transform;
		m_manager = AudioManager.Get();
		m_audioSource = base.audio;
		m_audioSource.rolloffMode = AudioRolloffMode.Linear;
		m_audioSource.minDistance = 0f;
		m_audioSource.maxDistance = 200f;
		m_audioSource.playOnAwake = false;
	}

	private void Update()
	{
		if (m_player != null)
		{
			m_transform.position = m_player.Position;
		}
	}

	public bool PlayRandomClip(AudioPlayer _player, AudioBank _bank, float _volumeScale, float _pitch, bool _ignoreListenerVolume)
	{
		if (_bank.PlayRandomClip(m_audioSource, _volumeScale, _pitch, _ignoreListenerVolume))
		{
			MonoBehaviourUtils.ReInvokeRepeating(this, "TryFreeObject", 1f / 15f, 1f / 15f);
			m_player = _player;
			if (m_player != null)
			{
				m_transform.position = m_player.Position;
			}
			return true;
		}
		return false;
	}

	public bool Play(AudioPlayer _player, AudioClip _clip)
	{
		if (m_audioSource.isPlaying)
		{
			if (m_audioSource.priority < 128)
			{
				return false;
			}
			m_audioSource.Stop();
		}
		m_audioSource.gameObject.active = true;
		m_audioSource.priority = 128;
		m_audioSource.pitch = 1f;
		m_audioSource.clip = _clip;
		m_audioSource.volume = 1f;
		m_audioSource.ignoreListenerVolume = false;
		m_audioSource.Play();
		MonoBehaviourUtils.ReInvokeRepeating(this, "TryFreeObject", 1f / 15f, 1f / 15f);
		m_player = _player;
		if (m_player != null)
		{
			m_transform.position = m_player.Position;
		}
		return true;
	}

	private void TryFreeObject()
	{
		if (!m_audioSource.isPlaying)
		{
			CancelInvoke("TryFreeObject");
			m_audioSource.Stop();
			m_audioSource.clip = null;
			m_audioSource.gameObject.active = false;
			m_manager.ObjectFree(this);
			m_player = null;
		}
	}
}
