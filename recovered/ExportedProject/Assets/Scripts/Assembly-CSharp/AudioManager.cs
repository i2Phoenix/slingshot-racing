using UnityEngine;

[AddComponentMenu("Audio/Audio Manager")]
public class AudioManager : MonoBehaviour
{
	private const int c_maxAudioObjects = 12;

	private static readonly string c_name = "__AudioManager";

	private static readonly string c_find = "/" + c_name;

	private AudioObject[] m_freeObjects = new AudioObject[12];

	private AudioObject[] m_usedObjects = new AudioObject[12];

	private int m_numFreeObjects;

	private int m_numUsedObjects;

	private void Awake()
	{
		m_numFreeObjects = m_freeObjects.Length;
		for (int i = 0; i < m_freeObjects.Length; i++)
		{
			GameObject gameObject = new GameObject("AudioObject");
			gameObject.transform.parent = base.transform;
			m_freeObjects[i] = gameObject.AddComponent<AudioObject>();
			gameObject.active = false;
		}
	}

	private void OnDestroy()
	{
		m_freeObjects = null;
		m_usedObjects = null;
		m_numFreeObjects = 0;
		m_numUsedObjects = 0;
	}

	public bool PlayRandomClip(AudioPlayer _player, AudioBank _bank, float _volumeScale, float _pitch)
	{
		if (m_numFreeObjects > 0)
		{
			AudioObject audioObject = m_freeObjects[m_numFreeObjects - 1];
			if (audioObject.PlayRandomClip(_player, _bank, _volumeScale, _pitch, false))
			{
				m_freeObjects[--m_numFreeObjects] = null;
				m_usedObjects[m_numUsedObjects++] = audioObject;
				return true;
			}
		}
		if (m_numUsedObjects > 0)
		{
			int num = LowestPriorityIndex();
			AudioObject audioObject2 = m_usedObjects[num];
			if (audioObject2.PlayRandomClip(_player, _bank, _volumeScale, _pitch, false))
			{
				return true;
			}
		}
		return false;
	}

	public bool PlayRandomGUIClip(AudioPlayer _player, AudioBank _bank, float _volumeScale, float _pitch)
	{
		float volumeScale = _volumeScale * Settings.SoundVolume;
		if (m_numFreeObjects > 0)
		{
			AudioObject audioObject = m_freeObjects[m_numFreeObjects - 1];
			if (audioObject.PlayRandomClip(_player, _bank, volumeScale, _pitch, true))
			{
				m_freeObjects[--m_numFreeObjects] = null;
				m_usedObjects[m_numUsedObjects++] = audioObject;
				return true;
			}
		}
		if (m_numUsedObjects > 0)
		{
			int num = LowestPriorityIndex();
			AudioObject audioObject2 = m_usedObjects[num];
			if (audioObject2.PlayRandomClip(_player, _bank, volumeScale, _pitch, true))
			{
				return true;
			}
		}
		return false;
	}

	public bool Play(AudioPlayer _player, AudioClip _clip)
	{
		if (m_numFreeObjects > 0)
		{
			AudioObject audioObject = m_freeObjects[m_numFreeObjects - 1];
			if (audioObject.Play(_player, _clip))
			{
				m_freeObjects[--m_numFreeObjects] = null;
				m_usedObjects[m_numUsedObjects++] = audioObject;
				return true;
			}
		}
		if (m_numUsedObjects > 0)
		{
			int num = LowestPriorityIndex();
			AudioObject audioObject2 = m_usedObjects[num];
			if (audioObject2.Play(_player, _clip))
			{
				return true;
			}
		}
		return false;
	}

	public void ObjectFree(AudioObject _audioObject)
	{
		m_freeObjects[m_numFreeObjects++] = _audioObject;
		int num = -1;
		for (int i = 0; i < m_numUsedObjects; i++)
		{
			if (_audioObject == m_usedObjects[i])
			{
				num = i;
				break;
			}
		}
		m_usedObjects[num] = m_usedObjects[--m_numUsedObjects];
		m_usedObjects[m_numUsedObjects] = null;
	}

	private int LowestPriorityIndex()
	{
		int result = -1;
		int num = -1;
		for (int i = 0; i < m_numUsedObjects; i++)
		{
			int priority = m_usedObjects[i].Priority;
			if (priority > num)
			{
				result = i;
				num = priority;
			}
		}
		return result;
	}

	public static AudioManager Get()
	{
		GameObject gameObject = GameObject.Find(c_find);
		if (gameObject == null)
		{
			gameObject = new GameObject(c_name);
			gameObject.AddComponent<AudioManager>();
		}
		return gameObject.GetComponent<AudioManager>();
	}
}
