using UnityEngine;

[AddComponentMenu("Audio/Skid Mixer")]
public class SkidMixer : MonoBehaviour
{
	private struct ClientData
	{
		public Vector3 m_position;

		public float m_speed;

		public float m_skid;

		public bool m_isHuman;
	}

	private const string c_skidMixerName = "__SkidMixer";

	private const string c_skidMixerFind = "/__SkidMixer";

	private const string c_skidMixerPrefab = "Prefabs/Audio/SkidMixer";

	private const float c_minWeight = 0.0001f;

	private const string c_infrequentUpdate = "InfrequentUpdate";

	private const float c_infrequentUpdateStep = 1f / 15f;

	private Transform m_normalTransform;

	private AudioSource m_normalAudioSource;

	private Transform m_corneringTransform;

	private AudioSource m_corneringAudioSource;

	private MapList<int, ClientData> m_clientData;

	private int m_numHumanClients;

	private int m_numNonHumanClients;

	public AudioClip m_normal;

	public float m_normalVolumeScale;

	public float m_normalPitchScale;

	public AudioClip m_cornering;

	public float m_corneringVolumeScale;

	public float m_corneringPitchScale;

	public AnimationCurve m_speedVolumeCurve;

	public AnimationCurve m_speedPitchCurve;

	public AnimationCurve m_skidCurve;

	public float m_humanPlayerBalance;

	public float m_panLevel;

	private void Awake()
	{
		GameObject gameObject = new GameObject("Normal");
		m_normalTransform = gameObject.transform;
		m_normalTransform.parent = base.transform;
		m_normalAudioSource = gameObject.AddComponent<AudioSource>();
		m_normalAudioSource.playOnAwake = false;
		m_normalAudioSource.rolloffMode = AudioRolloffMode.Linear;
		m_normalAudioSource.minDistance = 0f;
		m_normalAudioSource.maxDistance = 200f;
		m_normalAudioSource.panLevel = m_panLevel;
		m_normalAudioSource.volume = 0f;
		GameObject gameObject2 = new GameObject("Cornering");
		m_corneringTransform = gameObject2.transform;
		m_corneringTransform.parent = base.transform;
		m_corneringAudioSource = gameObject2.AddComponent<AudioSource>();
		m_corneringAudioSource.playOnAwake = false;
		m_corneringAudioSource.rolloffMode = AudioRolloffMode.Linear;
		m_corneringAudioSource.minDistance = 0f;
		m_corneringAudioSource.maxDistance = 200f;
		m_corneringAudioSource.panLevel = m_panLevel;
		m_corneringAudioSource.volume = 0f;
		m_clientData = new MapList<int, ClientData>();
	}

	private void Start()
	{
		m_normalAudioSource.clip = m_normal;
		m_normalAudioSource.loop = true;
		m_corneringAudioSource.clip = m_cornering;
		m_corneringAudioSource.loop = true;
		if (m_numHumanClients + m_numNonHumanClients > 0)
		{
			EnableAudio();
		}
	}

	private void OnDestroy()
	{
		m_normalAudioSource = null;
		m_corneringAudioSource = null;
		m_clientData = null;
	}

	private void EnableAudio()
	{
		if (!m_normalAudioSource.isPlaying)
		{
			m_normalAudioSource.Play();
		}
		if (!m_corneringAudioSource.isPlaying)
		{
			m_corneringAudioSource.Play();
		}
		if (!IsInvoking("InfrequentUpdate"))
		{
			InvokeRepeating("InfrequentUpdate", 1f / 15f, 1f / 15f);
		}
	}

	private void DisableAudio()
	{
		CancelInvoke("InfrequentUpdate");
		if (m_normalAudioSource != null)
		{
			m_normalAudioSource.Stop();
		}
		if (m_corneringAudioSource != null)
		{
			m_corneringAudioSource.Stop();
		}
	}

	private void InfrequentUpdate()
	{
		float humanPlayerBalance = m_humanPlayerBalance;
		float num = 1f - m_humanPlayerBalance;
		float num2 = 0f;
		Vector3 zero = Vector3.zero;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		Vector3 zero2 = Vector3.zero;
		float num6 = 0f;
		float num7 = 0f;
		for (int i = 0; i < m_clientData.Count; i++)
		{
			ClientData valueByIndex = m_clientData.GetValueByIndex(i);
			float num8 = m_speedVolumeCurve.Evaluate(valueByIndex.m_speed);
			float num9 = m_speedPitchCurve.Evaluate(valueByIndex.m_speed);
			float num10 = Mathf.Clamp01(m_skidCurve.Evaluate(valueByIndex.m_skid));
			float num11 = 1f - num10;
			float num12 = ((!valueByIndex.m_isHuman) ? num : humanPlayerBalance);
			float num13 = num12 * num8;
			float num14 = Mathf.Max(0.0001f, num11 * num13);
			Vector3 vector = valueByIndex.m_position * num14;
			float num15 = num8 * num11 * num14;
			float num16 = num9 * num14;
			num2 += num14;
			zero += vector;
			num3 += num15;
			num4 += num16;
			float num17 = Mathf.Max(0.0001f, num10 * num13);
			Vector3 vector2 = valueByIndex.m_position * num17;
			float num18 = num8 * num10 * num17;
			float num19 = num9 * num17;
			num5 += num17;
			zero2 += vector2;
			num6 += num18;
			num7 += num19;
		}
		float num20 = 1f / num2;
		float num21 = 1f / num5;
		m_normalTransform.position = zero * num20;
		m_normalAudioSource.volume = m_normalVolumeScale * num3 * num20;
		m_normalAudioSource.pitch = m_normalPitchScale * num4 * num20;
		m_corneringTransform.position = zero2 * num21;
		m_corneringAudioSource.volume = m_corneringVolumeScale * num6 * num21;
		m_corneringAudioSource.pitch = m_corneringPitchScale * num7 * num21;
	}

	public void AddClient(Object _client, bool _isHuman)
	{
		int instanceID = _client.GetInstanceID();
		ClientData value = new ClientData
		{
			m_isHuman = _isHuman
		};
		if (m_clientData.TryAdd(instanceID, value))
		{
			if (_isHuman)
			{
				m_numHumanClients++;
			}
			else
			{
				m_numNonHumanClients++;
			}
			EnableAudio();
		}
	}

	public void RemoveClient(Object _client)
	{
		int instanceID = _client.GetInstanceID();
		int indexOf = m_clientData.GetIndexOf(instanceID);
		if (indexOf >= 0)
		{
			if (m_clientData.GetValueByIndex(indexOf).m_isHuman)
			{
				m_numHumanClients--;
			}
			else
			{
				m_numNonHumanClients--;
			}
			m_clientData.RemoveAt(indexOf);
			if (m_numHumanClients + m_numNonHumanClients == 0)
			{
				DisableAudio();
			}
		}
	}

	public void UpdateClient(Object _client, Vector3 _position, float _speed, float _skid)
	{
		int instanceID = _client.GetInstanceID();
		int indexOf = m_clientData.GetIndexOf(instanceID);
		if (indexOf >= 0)
		{
			ClientData valueByIndex = m_clientData.GetValueByIndex(indexOf);
			valueByIndex.m_position = _position;
			valueByIndex.m_speed = _speed;
			valueByIndex.m_skid = _skid;
			m_clientData.SetValueByIndex(indexOf, valueByIndex);
		}
	}

	public static SkidMixer Get()
	{
		GameObject gameObject = GameObject.Find("/__SkidMixer");
		if (gameObject == null)
		{
			GameObject original = (GameObject)Resources.Load("Prefabs/Audio/SkidMixer", typeof(GameObject));
			gameObject = (GameObject)Object.Instantiate(original);
			gameObject.name = "__SkidMixer";
		}
		return gameObject.GetComponent<SkidMixer>();
	}
}
