using UnityEngine;

public class RenderSettings : MonoBehaviour
{
	private const bool c_bypassRenderTrackSnow = true;

	private const bool c_bypassRenderSparks = true;

	private const float c_minHighlightAngle = 60f;

	private static RenderSettings s_this;

	private MeshBatcher m_meshBatcher;

	private Vector4 m_windTime;

	private Vector4 m_windSnowTime;

	private Vector3 m_highlightDir;

	private float m_doubleLightmapScale;

	private bool m_blobShadowDataDirty;

	private Vector2[] m_blobShadowData = new Vector2[4];

	private float m_spotlightSparkleTime;

	private GameObject m_trackSnowGO;

	private ParticleEmitter m_trackSnowEmitter;

	private GameObject m_trackSnowParticleSystemGO;

	private ParticleSys m_trackSnowParticleSystem;

	private GameObject m_sparkGO;

	private ParticleEmitter m_sparkEmitter;

	private GameObject m_sparkParticleSystemGO;

	private ParticleSys m_sparkParticleSystem;

	private GameObject m_miniRewardGO;

	private ParticleEmitter m_miniRewardEmitter;

	private GameObject m_miniRewardParticleSystemGO;

	private ParticleSys m_miniRewardParticleSystem;

	public Color m_vehicleShadowColour;

	public GameObject m_trackSnowPrefab;

	public GameObject m_sparkPrefab;

	public GameObject m_miniRewardPrefab;

	public Color m_windSnowColour = Color.white;

	public bool m_night;

	public float m_sparkleUVScale = 8f;

	public Color m_skyAmbientColor;

	public float m_skyAmbientIntensity;

	public Color m_spotLightAmbientColor = Color.white;

	public float m_spotLightAmbientIntensity;

	public MeshBatcher MeshBatcher
	{
		get
		{
			if (m_meshBatcher == null)
			{
				m_meshBatcher = base.gameObject.AddComponent<MeshBatcher>();
			}
			return m_meshBatcher;
		}
	}

	public static RenderSettings Get
	{
		get
		{
			return s_this;
		}
	}

	public Vector3 HighlightDirection
	{
		get
		{
			return m_highlightDir;
		}
	}

	public float DoubleLightmapScale
	{
		get
		{
			return m_doubleLightmapScale;
		}
	}

	private void Awake()
	{
		s_this = this;
	}

	private void Start()
	{
		m_meshBatcher = base.gameObject.GetComponent<MeshBatcher>();
		if (m_meshBatcher == null)
		{
			m_meshBatcher = base.gameObject.AddComponent<MeshBatcher>();
		}
		SetLightGlobals();
		SetConstantGlobals();
		if (m_trackSnowPrefab != null)
		{
			m_trackSnowGO = (GameObject)Object.Instantiate(m_trackSnowPrefab, Vector3.zero, Quaternion.identity);
			m_trackSnowEmitter = m_trackSnowGO.GetComponent<ParticleEmitter>();
			m_trackSnowParticleSystemGO = new GameObject("TrackSnowParticleSystem");
			m_trackSnowParticleSystem = m_trackSnowParticleSystemGO.AddComponent<ParticleSys>();
			m_trackSnowParticleSystem.m_unityParticlePrefab = m_trackSnowGO;
			m_trackSnowParticleSystem.m_maxParticles = 120;
			m_trackSnowParticleSystem.m_combineColours = true;
			m_trackSnowGO.GetComponent<ParticleAnimator>().doesAnimateColor = true;
		}
		if (m_sparkPrefab != null)
		{
			m_sparkGO = (GameObject)Object.Instantiate(m_sparkPrefab, Vector3.zero, Quaternion.identity);
			m_sparkEmitter = m_sparkGO.GetComponent<ParticleEmitter>();
			m_sparkParticleSystemGO = new GameObject("SparkParticleSystem");
			m_sparkParticleSystem = m_sparkParticleSystemGO.AddComponent<ParticleSys>();
			m_sparkParticleSystem.m_unityParticlePrefab = m_sparkGO;
			m_sparkParticleSystem.m_maxParticles = 200;
			m_sparkParticleSystem.m_combineColours = true;
			m_sparkGO.GetComponent<ParticleAnimator>().doesAnimateColor = true;
		}
		if (m_miniRewardPrefab != null)
		{
			m_miniRewardGO = (GameObject)Object.Instantiate(m_miniRewardPrefab, Vector3.zero, Quaternion.identity);
			m_miniRewardEmitter = m_miniRewardGO.GetComponent<ParticleEmitter>();
			m_miniRewardParticleSystemGO = new GameObject("MiniRewardParticleSystem");
			m_miniRewardParticleSystem = m_miniRewardParticleSystemGO.AddComponent<ParticleSys>();
			m_miniRewardParticleSystem.m_unityParticlePrefab = m_miniRewardGO;
			m_miniRewardParticleSystem.m_maxParticles = 32;
			m_miniRewardParticleSystem.MaintainIds();
		}
	}

	private void SetLightGlobals()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("MainLight");
		Vector3 vector = ((!(gameObject != null)) ? Vector3.down : gameObject.transform.forward);
		Shader.SetGlobalVector("g_lightDir", vector);
		Vector3 eulerAngles = Quaternion.LookRotation(vector).eulerAngles;
		eulerAngles.x = Mathf.Max(60f, eulerAngles.x);
		Quaternion quaternion = Quaternion.Euler(eulerAngles);
		m_highlightDir = quaternion * Vector3.forward;
		m_doubleLightmapScale = 1.1764706f / Mathf.Abs(m_highlightDir.y);
		Shader.SetGlobalVector("g_highlightDirection", new Vector4(m_highlightDir.x, m_highlightDir.y, m_highlightDir.z, m_doubleLightmapScale));
		Shader.SetGlobalColor("g_ambientLight", UnityEngine.RenderSettings.ambientLight + m_skyAmbientColor * m_skyAmbientIntensity + m_spotLightAmbientColor * m_spotLightAmbientIntensity);
		Light light = ((!(gameObject != null)) ? null : gameObject.GetComponent<Light>());
		Color color = ((!(light != null)) ? Color.white : (light.color * light.intensity)) * 1.1764706f;
		Shader.SetGlobalColor("g_mainLightColour", color);
	}

	private void SetConstantGlobals()
	{
		Shader.SetGlobalColor("g_windSnowColour", m_windSnowColour);
		Shader.SetGlobalVector("g_sparkleData", new Vector4(m_sparkleUVScale, 0f, 0f, 1f));
	}

	private void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		m_windTime.x += deltaTime * 2f;
		if (m_windTime.x >= 1f)
		{
			m_windTime.x--;
		}
		m_windTime.y += deltaTime * 1.8f;
		if (m_windTime.y >= 1f)
		{
			m_windTime.y--;
		}
		m_windTime.z += deltaTime * 0.35f;
		if (m_windTime.z >= 1f)
		{
			m_windTime.z--;
		}
		m_windTime.w += deltaTime * 0.2f;
		if (m_windTime.w >= 1f)
		{
			m_windTime.w--;
		}
		Shader.SetGlobalVector("g_windData", m_windTime);
		m_windSnowTime.x += deltaTime * 0.1f;
		if (m_windSnowTime.x >= 1f)
		{
			m_windSnowTime.x--;
		}
		m_windSnowTime.y += deltaTime * 0.2f;
		if (m_windSnowTime.y >= 1f)
		{
			m_windSnowTime.y--;
		}
		m_windSnowTime.z += deltaTime * 0.4f;
		if (m_windSnowTime.z >= 1f)
		{
			m_windSnowTime.z--;
		}
		m_windSnowTime.w = m_windSnowTime.x;
		Shader.SetGlobalVector("g_windSnowData", m_windSnowTime);
		m_spotlightSparkleTime += deltaTime * 0.05f;
		if (m_spotlightSparkleTime >= 1f)
		{
			m_spotlightSparkleTime--;
		}
		Shader.SetGlobalVector("g_spotlightSparkleData", new Vector4(m_spotlightSparkleTime, m_spotlightSparkleTime, m_spotlightSparkleTime, m_spotlightSparkleTime));
		Transform transform = MainCamera.Transform;
		if (transform == null)
		{
			Camera[] allCameras = Camera.allCameras;
			if (allCameras.Length > 0)
			{
				transform = allCameras[0].transform;
			}
		}
		if (transform != null)
		{
			Vector3 right = transform.right;
			Shader.SetGlobalVector("g_cameraRight", new Vector4(right.x, right.y, right.z, 0f));
			Vector3 up = transform.up;
			Shader.SetGlobalVector("g_cameraUp", new Vector4(up.x, up.y, up.z, 0f));
			Vector3 forward = transform.forward;
			Shader.SetGlobalVector("g_cameraForward", new Vector4(forward.x, forward.y, forward.z, 0f));
			Vector3 position = transform.position;
			Shader.SetGlobalVector("g_cameraPos", new Vector4(position.x, position.y, position.z, 1f));
		}
		if ((bool)m_trackSnowEmitter && m_trackSnowEmitter.particleCount > 0)
		{
			Particle[] particles = m_trackSnowEmitter.particles;
			for (int i = 0; i < particles.Length; i++)
			{
				Color color = particles[i].color;
				float num = particles[i].angularVelocity * 1000f;
				float num2 = particles[i].energy / particles[i].startEnergy;
				float a = 1f - Mathf.Pow(1f - num2, 2f);
				a = Mathf.Min(a, (1f - num2) * 5f);
				color.a = num * a;
				particles[i].color = color;
			}
			m_trackSnowEmitter.particles = particles;
		}
		if (m_blobShadowDataDirty)
		{
			m_blobShadowDataDirty = false;
			Vector4 vec = default(Vector4);
			vec.x = m_blobShadowData[0].x;
			vec.y = m_blobShadowData[1].x;
			vec.z = m_blobShadowData[2].x;
			vec.w = m_blobShadowData[3].x;
			Shader.SetGlobalVector("g_blobShadowGlow", vec);
			Vector4 vec2 = default(Vector4);
			vec2.x = m_blobShadowData[0].y;
			vec2.y = m_blobShadowData[1].y;
			vec2.z = m_blobShadowData[2].y;
			vec2.w = m_blobShadowData[3].y;
			Shader.SetGlobalVector("g_blobShadowAlpha", vec2);
		}
	}

	private void OnDestroy()
	{
		s_this = null;
		Shader.SetGlobalVector("g_windData", Vector4.zero);
		Shader.SetGlobalVector("g_highlightDirection", Vector4.zero);
	}

	public void EmitTrackSnowParticle(Vector3 _position, Vector3 _velocity, float _size, float _energy, Color _colour, float _rotation)
	{
		if (m_trackSnowParticleSystem != null)
		{
			m_trackSnowParticleSystem.Emit(_position, _velocity, _size, _energy, _colour, _rotation, 0f);
		}
		else if ((bool)m_trackSnowEmitter)
		{
			float angularVelocity = _colour.a * 0.001f;
			m_trackSnowEmitter.Emit(_position, _velocity, _size, _energy, _colour, _rotation, angularVelocity);
		}
	}

	public void EmitSparkParticle(Vector3 _position, Vector3 _velocity)
	{
		if (m_sparkParticleSystem != null)
		{
			m_sparkParticleSystem.Emit(_position, _velocity, 0.15f, 0.5f, Color.white, 0f, 0f);
		}
		else if ((bool)m_sparkEmitter)
		{
			m_sparkEmitter.Emit(_position, _velocity, 0.15f, 0.5f, Color.white);
		}
	}

	public void EmitSparkParticle(Vector3 _position, Vector3 _velocity, float _size, float _energy, Color _colour)
	{
		if (m_sparkParticleSystem != null)
		{
			m_sparkParticleSystem.Emit(_position, _velocity, _size, _energy, _colour, 0f, 0f);
		}
		else if ((bool)m_sparkEmitter)
		{
			m_sparkEmitter.Emit(_position, _velocity, _size, _energy, _colour);
		}
	}

	public int EmitMiniRewardParticle(Vector3 _position, Vector3 _velocity, float _size, float _energy)
	{
		if (m_miniRewardParticleSystem != null)
		{
			return m_miniRewardParticleSystem.Emit(_position, _velocity, _size, _energy, Color.white, 0f, 0f);
		}
		if ((bool)m_miniRewardEmitter)
		{
			m_miniRewardEmitter.Emit(_position, _velocity, _size, _energy, Color.white);
		}
		return -1;
	}

	public bool SetMiniRewardParticleData(int _index, Vector3 _position)
	{
		if ((bool)m_miniRewardParticleSystem)
		{
			return m_miniRewardParticleSystem.SetParticleData(_index, _position);
		}
		return false;
	}

	public void SetBlobShadowData(int index, float glow, float alpha)
	{
		m_blobShadowData[index].x = glow;
		m_blobShadowData[index].y = 0.75f * (1f - glow) * alpha;
		m_blobShadowDataDirty = true;
	}
}
