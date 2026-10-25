using UnityEngine;

[AddComponentMenu("Audio/Audio Player")]
public class AudioPlayer : MonoBehaviour
{
	private Transform m_transform;

	private AudioManager m_manager;

	public Vector3 Position
	{
		get
		{
			return m_transform.position;
		}
	}

	private void Awake()
	{
		m_transform = base.transform;
		m_manager = AudioManager.Get();
	}

	public void Refresh()
	{
		m_manager = AudioManager.Get();
	}

	public bool PlayRandomClip(AudioBank _bank, float _volumeScale = 1f, float _pitch = 1f)
	{
		if (_bank == null)
		{
			return false;
		}
		return m_manager.PlayRandomClip(this, _bank, _volumeScale, _pitch);
	}

	public bool PlayRandomGUIClip(AudioBank _bank, float _volumeScale = 1f, float _pitch = 1f)
	{
		if (_bank == null)
		{
			return false;
		}
		return m_manager.PlayRandomGUIClip(this, _bank, _volumeScale, _pitch);
	}

	public bool Play(AudioClip _clip)
	{
		if (_clip == null)
		{
			return false;
		}
		return m_manager.Play(this, _clip);
	}
}
