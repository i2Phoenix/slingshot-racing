using UnityEngine;

public class TimeManager : MonoBehaviour
{
	private const int c_windowSize = 16;

	private const float c_targetLatency = 1f / 30f;

	private const float c_convergeRate = 0.025f;

	private static bool s_enableSmoothing;

	private static TimeManager s_this;

	private float m_fixedDeltaTime;

	private float m_deltaTime;

	private float m_relativeTime;

	private float m_frameProp;

	private int m_index;

	private float m_deltasSum;

	private float[] m_deltas = new float[16];

	public static bool EnableSmoothing
	{
		get
		{
			return s_enableSmoothing;
		}
		set
		{
			if (value != s_enableSmoothing)
			{
				s_enableSmoothing = value;
				if (s_this != null)
				{
					s_this.Reset();
				}
			}
		}
	}

	public static float DeltaTime
	{
		get
		{
			return s_this.m_deltaTime;
		}
	}

	public static float FrameProp
	{
		get
		{
			return s_this.m_frameProp;
		}
	}

	public static void Init()
	{
		if (s_this == null)
		{
			GameObject gameObject = new GameObject("__TimeManager");
			gameObject.AddComponent<TimeManager>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}

	private void Awake()
	{
		s_this = this;
		m_fixedDeltaTime = Time.fixedDeltaTime;
	}

	private void OnDestroy()
	{
		if (s_this == this)
		{
			s_this = null;
		}
	}

	private void Start()
	{
		Reset();
	}

	private void Reset()
	{
		m_deltaTime = 0f;
		m_relativeTime = 0f;
		m_frameProp = 0f;
		if (s_enableSmoothing)
		{
			m_deltas = new float[16];
			float num = 1f / ((Application.targetFrameRate <= 0) ? 60f : ((float)Application.targetFrameRate));
			for (int i = 0; i != 16; i++)
			{
				m_deltas[i] = num;
			}
			m_deltasSum = 16f * num;
		}
	}

	private void Update()
	{
		if (s_enableSmoothing)
		{
			UpdateWithSmoothing();
		}
		else
		{
			UpdateNoSmoothing();
		}
	}

	private void UpdateWithSmoothing()
	{
		if (Time.timeScale < 0.001f)
		{
			m_deltaTime = Time.deltaTime;
			m_relativeTime = Mathf.Clamp(m_relativeTime + m_deltaTime, 0f - m_fixedDeltaTime, m_fixedDeltaTime);
			m_frameProp = m_relativeTime / m_fixedDeltaTime;
			return;
		}
		m_index = (m_index + 1) % 16;
		m_deltasSum -= m_deltas[m_index];
		m_deltas[m_index] = Time.deltaTime / Time.timeScale;
		m_deltasSum += m_deltas[m_index];
		m_deltaTime = m_deltasSum * 0.0625f * Time.timeScale;
		m_relativeTime += m_deltaTime;
		float num = (m_fixedDeltaTime - 1f / 30f - m_relativeTime) * 0.025f;
		m_relativeTime += num;
		m_deltaTime += num;
		float num2 = 0f;
		if (m_relativeTime > m_fixedDeltaTime)
		{
			num2 = m_fixedDeltaTime - m_relativeTime;
		}
		else if (m_relativeTime < 0f - m_fixedDeltaTime)
		{
			num2 = 0f - m_fixedDeltaTime - m_relativeTime;
		}
		if (num2 != 0f)
		{
			m_deltasSum -= num2;
			m_deltas[m_index] -= num2;
			m_deltaTime += num2;
			m_relativeTime += num2;
		}
		m_frameProp = m_relativeTime / m_fixedDeltaTime;
	}

	private void UpdateNoSmoothing()
	{
		m_deltaTime = Time.deltaTime;
		m_relativeTime = Mathf.Clamp(m_relativeTime + m_deltaTime, 0f, m_fixedDeltaTime);
		m_frameProp = m_relativeTime / m_fixedDeltaTime;
	}

	private void FixedUpdate()
	{
		m_relativeTime -= m_fixedDeltaTime;
	}
}
