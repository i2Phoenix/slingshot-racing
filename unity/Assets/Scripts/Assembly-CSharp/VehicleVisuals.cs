using System.Collections;
using UnityEngine;
using VehicleTypes;

public class VehicleVisuals : MonoBehaviour
{
	private const float c_snowSprayFadeRate = 2.5f;

	private const float c_snowSprayForceClip = 10f;

	private const float c_snowSprayForceScale = 1f / 60f;

	private const float c_snowSprayAlphaScale = 1.2f;

	private const float c_snowSpraySegmentTime = 0.02f;

	private const float c_snowSprayAlphaThreshold = 0.01f;

	private const float c_snowSprayWidth = 1.75f;

	private const float c_snowSprayUpOffset = 0.1f;

	private const float c_snowSpraySideOffset = 2f;

	private const int c_snowSpraySegmentCount = 20;

	private const float c_skidMarkAlphaThreshold = 0.4f;

	private const float c_skidMarkAlphaThresholdSkidding = 0.2f;

	private const float c_skidMarkFadeTime = 5f;

	private const float c_skidMarkWidth = 0.4f;

	private const float c_skidMarkUpOffset = 0.05f;

	private const float c_skidMarkSegmentSize = 1f;

	private const int c_numRopeParticles = 16;

	private const bool c_ropeSimEnabled = true;

	private const float c_humanVolumeScale = 1f;

	private const float c_nonHumanVolumeScale = 0.3f;

	private const float c_trailDeployTime = 0.5f;

	private const float c_trailDeployAmount = 0.01f;

	private const float c_trailDeployAlpha = 0f;

	private const float s_shadowAlpha = 0.5f;

	private const float s_trailAlpha = 0.5f;

	public static bool s_renderTrailShadows = true;

	private static Color c_miniRewardColour = new Color(0.4f, 0.6f, 1f, 1f);

	private static Rect c_trailUVRect = new Rect(0f, 3f / 64f, 0.2266f, 29f / 32f);

	private static float c_trailRectXOffset = 0.2031f;

	private Transform m_transform;

	private Vehicle m_vehicle;

	private bool m_enableFrustumCulling = true;

	private int m_trackParticleDistributorMask;

	private Vehicle[] m_otherVehicles;

	private float m_proximity = float.MaxValue;

	private float m_alpha = 1f;

	private float m_distAlpha = 1f;

	private Renderer m_chassisRenderer;

	private float m_materialAlpha;

	private LightmapSampler m_lightmapSampler;

	private Shadow m_shadow;

	private ShadowBlob m_shadowBlob;

	private float m_proximityAlpha;

	private PlayerIdentifier m_playerIdentifier;

	private Rect m_trailUVRect;

	private TrailMesh m_trail;

	private TrailMesh m_trailShadow;

	private bool m_trailDeployed;

	private float m_trailAlpha = 1f;

	private Color m_shadowBlend;

	private Color m_trailShadowBlend;

	private Vector3 m_trailShadowOffset;

	private Vector3 m_grapplePointPos;

	private float m_grapplePointRadius;

	private float m_grappleLengthProp;

	private float m_grappleLengthPropOld1;

	private float m_grappleLengthPropOld2;

	private Rope m_rope;

	private LineMesh m_ropeLine;

	private GameObject m_ropeLineGO;

	private ParticleEmitter m_collisionParticleEmitter;

	private ParticleEmitter m_sparksParticleEmitter;

	private ParticleEmitter m_miniRewardSparksParticleEmitter;

	private ParticleEmitter m_miniRewardSparksReversedParticleEmitter;

	private ParticleEmitter m_launchSteamEmitter;

	private ParticleSys m_launchSteamParticleSys;

	private ParticleEmitter m_snowBlastParticleEmitter;

	private ParticleSys m_snowBlastParticleSys;

	private TrailMesh m_snowSpray;

	private SimpleAtlas m_snowSprayAtlas;

	private float m_snowSprayForce;

	private Wheel m_snowSpraySide;

	private SkidMarkManager m_skidMarkManager;

	private SkidMark m_currentSkidMark;

	private SkidMark m_currentOilSlickMark;

	private Wheel m_currentOilSlickSide;

	private AudioPlayer m_audioPlayer;

	private SkidMixer m_skidMixer;

	private float m_volumeScale;

	private GrappleSteamRenderer m_grappleSteamRenderer;

	private int m_numMiniRewards;

	private float m_miniRewardTime;

	private float m_miniRewardBoostReleaseTime;

	private int m_miniRewardParticleId = -1;

	private ObscuredRenderer m_obscuredRenderer;

	private Shader m_opaqueShader;

	public Shader m_ghostShader;

	public Shader m_alphaShader;

	public AudioBank m_grappleBank;

	public AudioBank m_crashBank;

	public AudioBank m_recoilBank;

	public AudioBank m_wallCrashBank;

	public AudioBank m_miniRewardDeployedBank;

	public AudioBank m_miniRewardAcquiredBank;

	public AudioBank m_miniRewardReleasedBank;

	public AudioBank m_boostBank;

	public AudioBank m_catchAudioBank;

	public GameObject m_explosionPrefab;

	public GameObject m_collisionPrefab;

	public GameObject m_sparksPrefab;

	public GameObject m_miniRewardSparksPrefab;

	public GameObject m_miniRewardSparksReversedPrefab;

	public GameObject m_headlightPrefab;

	public GameObject m_launchSteamPrefab;

	public GameObject m_snowBlastPrefab;

	public GameObject m_shieldParticlePrefab;

	public GameObject m_catchParticlePrefab;

	public float m_trailEmissiveAmount = 0.5f;

	public Vector3 m_grappleGunOffset = Vector3.zero;

	public Vector2 m_trailOffset = new Vector3(0.1f, -1.5f);

	public float m_trailSegmentSize = 0.5f;

	public float m_trailWidth = 1f;

	public int m_trailSegmentCount = 20;

	public bool m_enableTrailColour = true;

	public Rect m_grappleUVRect;

	private bool RenderSnowSpray
	{
		get
		{
			return true;
		}
	}

	private bool RenderSkidmarks
	{
		get
		{
			return true;
		}
	}

	private bool RenderRope
	{
		get
		{
			return true;
		}
	}

	private bool RenderCollisions
	{
		get
		{
			return true;
		}
	}

	private bool RenderTrail
	{
		get
		{
			return !m_vehicle.NonCompetitive;
		}
	}

	private bool RenderShadow
	{
		get
		{
			return true;
		}
	}

	private bool RenderShadowBlob
	{
		get
		{
			return true;
		}
	}

	private bool RenderTrailShadow
	{
		get
		{
			return s_renderTrailShadows;
		}
	}

	public float VolumeScale
	{
		get
		{
			return m_volumeScale;
		}
	}

	public bool EnableFrustumCulling
	{
		set
		{
			m_enableFrustumCulling = value;
		}
	}

	public Vector3 Position
	{
		get
		{
			return m_transform.position;
		}
	}

	public Color CoarseLighting
	{
		get
		{
			if (m_lightmapSampler != null)
			{
				return m_lightmapSampler.CoarseLighting;
			}
			return Color.white;
		}
	}

	public bool IsVisible { get; private set; }

	public float Alpha
	{
		set
		{
			m_alpha = value;
		}
	}

	private void Awake()
	{
		m_transform = base.transform;
		m_audioPlayer = base.gameObject.AddComponent<AudioPlayer>();
		m_obscuredRenderer = base.gameObject.GetComponentInChildren<ObscuredRenderer>();
	}

	public void Init(Vehicle _vehicle)
	{
		m_vehicle = _vehicle;
		if (m_vehicle.IsGhost && m_obscuredRenderer != null)
		{
			m_obscuredRenderer.gameObject.active = false;
		}
		m_chassisRenderer = GameObjectUtils.GetComponentInChildren<SendVisibilityUpwards>(base.gameObject).GetComponent<Renderer>();
		m_opaqueShader = m_chassisRenderer.sharedMaterial.shader;
		m_chassisRenderer.sharedMaterial = Object.Instantiate(m_chassisRenderer.sharedMaterial) as Material;
		if (m_vehicle.IsGhost)
		{
			m_chassisRenderer.sharedMaterial.shader = m_ghostShader;
		}
		Material sharedMaterial = m_chassisRenderer.sharedMaterial;
		sharedMaterial.SetFloat("_UOffset", (float)m_vehicle.ColourIndex * 0.25f);
		m_shadowBlend = Color.white;
		RenderSettings renderSettings = SceneSettings.RenderSettings;
		if ((bool)renderSettings)
		{
			m_shadowBlend = renderSettings.m_vehicleShadowColour;
			m_shadowBlend.r = 1f - m_shadowBlend.r;
			m_shadowBlend.g = 1f - m_shadowBlend.g;
			m_shadowBlend.b = 1f - m_shadowBlend.b;
		}
		m_shadowBlend *= 0.7f;
		m_trailShadowBlend = m_shadowBlend;
		m_trackParticleDistributorMask = ((PlatformSettings.ParticleEffects == PlatformSettings.ParticleDetail.Low) ? 1 : 0);
		if (RenderShadow)
		{
			m_shadow = base.gameObject.AddComponent<Shadow>();
			m_shadow.Index = Mathf.Max(m_vehicle.ColourIndex, 0);
			m_shadow.Renderer = m_chassisRenderer;
		}
		if (RenderShadowBlob && !m_vehicle.IsGhost)
		{
			m_shadowBlob = base.gameObject.AddComponent<ShadowBlob>();
			m_shadowBlob.Index = Mathf.Max(m_vehicle.ColourIndex, 0);
		}
		if (m_vehicle.IsLocalHuman && Settings.HighlightPlayer)
		{
			m_playerIdentifier = base.gameObject.AddComponent<PlayerIdentifier>();
			m_playerIdentifier.Index = Mathf.Max(m_vehicle.ColourIndex, 0);
		}
		if (RenderTrail)
		{
			m_trailUVRect = c_trailUVRect;
			m_trailUVRect.x += c_trailRectXOffset * (float)m_vehicle.ColourIndex;
			m_trail = TrailMesh.Create(m_transform.position, m_trailUVRect, (!m_enableTrailColour) ? Color.white : m_vehicle.Colour, m_trailSegmentCount);
			m_trail.GetComponent<Renderer>().sharedMaterial = (Material)Resources.Load("Materials/TrailGrapple");
			if (RenderTrailShadow)
			{
				m_trailShadow = TrailMesh.Create(_uvRect: new Rect(0f, 3f / 64f, 0.2266f, 29f / 32f), _initialPoint: m_transform.position, _initialColour: m_trailShadowBlend, _pointCount: m_trailSegmentCount);
				m_trailShadow.GetComponent<Renderer>().sharedMaterial = (Material)Resources.Load("Materials/TrailGrappleShadow");
				m_trailShadowOffset = new Vector3(0.5f, -0.05f, -0.5f);
				GameObject gameObject = GameObject.FindGameObjectWithTag("MainLight");
				if (gameObject != null)
				{
					m_trailShadowOffset = gameObject.transform.forward * 1.25f;
					m_trailShadowOffset.y = 0f;
				}
			}
		}
		if (RenderSnowSpray)
		{
			m_snowSprayAtlas = (SimpleAtlas)Resources.Load("Atlases/SnowSprayAtlas");
			m_snowSpray = TrailMesh.Create(m_transform.position, m_snowSprayAtlas.GetFrameUVRect(0), Color.white, 20);
			m_snowSpray.GetComponent<Renderer>().sharedMaterial = (Material)Resources.Load("Materials/SnowSpray");
			LineMeshAnimator lineMeshAnimator = m_snowSpray.gameObject.AddComponent<LineMeshAnimator>();
			lineMeshAnimator.SetAtlas(m_snowSprayAtlas);
			lineMeshAnimator.PlayAnimation(0);
		}
		if (RenderSkidmarks)
		{
			m_skidMarkManager = SkidMarkManager.Get();
		}
		if (RenderRope)
		{
			m_rope = new Rope(16);
			int particleCount = ((m_rope != null) ? 16 : 2);
			m_ropeLine = LineMesh.Create(m_transform.position, m_grappleUVRect, Color.white, particleCount);
			m_ropeLine.GetComponent<Renderer>().sharedMaterial = (Material)Resources.Load("Materials/TrailGrapple");
			m_ropeLineGO = m_ropeLine.gameObject;
		}
		if (RenderCollisions)
		{
			SharedParticles sharedParticles = SharedParticles.Get();
			m_collisionParticleEmitter = sharedParticles.GetEmitter(m_collisionPrefab);
			m_sparksParticleEmitter = sharedParticles.GetEmitter(m_sparksPrefab);
			m_miniRewardSparksParticleEmitter = sharedParticles.GetEmitter(m_miniRewardSparksPrefab);
			m_miniRewardSparksReversedParticleEmitter = sharedParticles.GetEmitter(m_miniRewardSparksReversedPrefab);
			m_launchSteamEmitter = sharedParticles.GetEmitter(m_launchSteamPrefab);
			m_launchSteamParticleSys = m_launchSteamEmitter.gameObject.GetComponent<ParticleSys>();
			m_snowBlastParticleEmitter = sharedParticles.GetEmitter(m_snowBlastPrefab);
			m_snowBlastParticleSys = m_snowBlastParticleEmitter.gameObject.GetComponent<ParticleSys>();
		}
		if (SceneSettings.RenderSettings.m_night && m_headlightPrefab != null && m_vehicle.UsesHeadlights)
		{
			GameObject gameObject2 = Object.Instantiate(m_headlightPrefab) as GameObject;
			if (gameObject2 != null)
			{
				gameObject2.transform.parent = base.transform;
				gameObject2.transform.localPosition = new Vector3(-0.5f, 0.3f, 1.2f);
				gameObject2.transform.localRotation = Quaternion.identity;
			}
			gameObject2 = Object.Instantiate(m_headlightPrefab) as GameObject;
			if (gameObject2 != null)
			{
				gameObject2.transform.parent = base.transform;
				gameObject2.transform.localPosition = new Vector3(0.5f, 0.3f, 1.2f);
				gameObject2.transform.localRotation = Quaternion.identity;
			}
		}
		m_lightmapSampler = base.gameObject.AddComponent<LightmapSampler>();
		m_grappleSteamRenderer = GrappleSteamRenderer.Get();
		if (GameObject.FindWithTag("RaceControl") == null)
		{
			DeployTrail(0.5f, 0.01f);
		}
		if (m_skidMixer == null)
		{
			m_skidMixer = SkidMixer.Get();
			m_skidMixer.AddClient(this, m_vehicle.IsLocalHuman);
		}
		m_volumeScale = ((!m_vehicle.ScaleAudioVolume) ? 1f : 0.3f);
		m_vehicle.OnGrappleEvent += OnGrapple;
		m_vehicle.OnGrappleReleaseEvent += OnGrappleRelease;
		m_vehicle.OnOilSlickEvent += OnOilSlick;
		m_vehicle.OnDeathEvent += OnDeath;
		m_vehicle.OnBoostEvent += OnBoost;
		m_vehicle.OnCollectMiniReward += OnCollectMiniReward;
		m_vehicle.OnDropMiniReward += OnDropMiniReward;
		m_vehicle.OnDeployMiniReward += OnDeployMiniReward;
		m_vehicle.OnShieldEvent += OnShield;
		m_vehicle.OnSurroundCollisionEvent += OnSurroundCollision;
		m_vehicle.OnTrackCollisionEvent += OnTrackCollision;
		m_vehicle.OnVehicleCollisionEvent += OnVehicleCollision;
		m_vehicle.OnBatteringRamEvent += OnBatteringRam;
		Vehicle[] array = Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];
		m_otherVehicles = new Vehicle[array.Length - 1];
		int num = 0;
		for (int i = 0; i != array.Length; i++)
		{
			Vehicle vehicle = array[i];
			if (vehicle != m_vehicle)
			{
				m_otherVehicles[num++] = vehicle;
			}
		}
		AlphaUpdate(1f);
		if (m_obscuredRenderer != null)
		{
			AddColourToMesh component = m_obscuredRenderer.GetComponent<AddColourToMesh>();
			if (component != null)
			{
				component.m_colour = m_vehicle.BrightColour;
			}
		}
	}

	private void OnEnable()
	{
		if (m_trail != null)
		{
			m_trail.gameObject.active = true;
		}
		if (m_trailShadow != null)
		{
			m_trailShadow.gameObject.active = true;
		}
		if (m_snowSpray != null)
		{
			m_snowSpray.gameObject.active = true;
		}
		if (m_ropeLineGO != null)
		{
			m_ropeLineGO.active = true;
		}
		if (m_vehicle != null && m_skidMixer == null)
		{
			m_skidMixer = SkidMixer.Get();
			m_skidMixer.AddClient(this, m_vehicle.IsLocalHuman);
		}
	}

	private void OnDisable()
	{
		if (m_trail != null)
		{
			m_trail.gameObject.active = false;
		}
		if (m_trailShadow != null)
		{
			m_trailShadow.gameObject.active = false;
		}
		if (m_snowSpray != null)
		{
			m_snowSpray.gameObject.active = false;
		}
		if (m_ropeLineGO != null)
		{
			m_ropeLineGO.active = false;
		}
		if (m_skidMixer != null)
		{
			m_skidMixer.RemoveClient(this);
			m_skidMixer = null;
		}
		if (m_currentSkidMark != null)
		{
			m_currentSkidMark.End(5f);
			m_currentSkidMark = null;
		}
		if (m_currentOilSlickMark != null)
		{
			m_currentOilSlickMark.End(5f);
			m_currentOilSlickMark = null;
		}
	}

	private void OnDestroy()
	{
		if (m_vehicle != null)
		{
			m_vehicle.OnGrappleEvent -= OnGrapple;
			m_vehicle.OnGrappleReleaseEvent -= OnGrappleRelease;
			m_vehicle.OnOilSlickEvent -= OnOilSlick;
			m_vehicle.OnDeathEvent -= OnDeath;
			m_vehicle.OnBoostEvent -= OnBoost;
			m_vehicle.OnCollectMiniReward -= OnCollectMiniReward;
			m_vehicle.OnDropMiniReward -= OnDropMiniReward;
			m_vehicle.OnDeployMiniReward -= OnDeployMiniReward;
			m_vehicle.OnShieldEvent -= OnShield;
			m_vehicle.OnSurroundCollisionEvent -= OnSurroundCollision;
			m_vehicle.OnTrackCollisionEvent -= OnTrackCollision;
			m_vehicle.OnVehicleCollisionEvent -= OnVehicleCollision;
			m_vehicle.OnBatteringRamEvent -= OnBatteringRam;
		}
		if ((bool)m_skidMixer)
		{
			m_skidMixer.RemoveClient(this);
			m_skidMixer = null;
		}
		if (m_trail != null)
		{
			m_trail.Destroy();
			m_trail = null;
		}
		if (m_trailShadow != null)
		{
			m_trailShadow.Destroy();
			m_trailShadow = null;
		}
		if (m_snowSpray != null)
		{
			m_snowSpray.Destroy();
			m_snowSpray = null;
		}
		m_snowSprayAtlas = null;
		if (m_currentSkidMark != null)
		{
			m_currentSkidMark.End(5f);
			m_currentSkidMark = null;
		}
		if (m_currentOilSlickMark != null)
		{
			m_currentOilSlickMark.End(5f);
			m_currentOilSlickMark = null;
		}
		m_skidMarkManager = null;
		if (m_ropeLine != null)
		{
			m_ropeLine.Destroy();
			m_ropeLine = null;
		}
		m_lightmapSampler = null;
		m_grappleSteamRenderer = null;
		m_vehicle = null;
		m_otherVehicles = null;
	}

	public void VehicleFixedUpdate(int _distributor)
	{
		TrackSpline currentTrackSpline = m_vehicle.CurrentTrackSpline;
		TrackHub currentTrackHub = m_vehicle.CurrentTrackHub;
		m_miniRewardBoostReleaseTime -= Time.deltaTime;
		if (m_miniRewardBoostReleaseTime > 0f)
		{
			MiniRewardBoostUpdate();
		}
		else
		{
			m_miniRewardBoostReleaseTime = 0f;
		}
		if (m_shadowBlob != null)
		{
			m_shadowBlob.SetTrackData(Vector3.Cross(m_vehicle.TrackTangent, m_vehicle.TrackNormal), m_vehicle.Forward);
		}
		if (m_skidMixer != null && (_distributor & 1) == 0)
		{
			m_skidMixer.UpdateClient(this, m_transform.position, m_vehicle.Velocity.magnitude, m_snowSprayForce);
		}
		if ((_distributor & m_trackParticleDistributorMask) == 0)
		{
			Vector3 trackPoint = m_vehicle.TrackPoint;
			trackPoint.y += m_vehicle.TrackCentreHeight + 1f;
			ApplyTrackSnowParticles(m_vehicle.Position, m_vehicle.Velocity, trackPoint, m_vehicle.TrackTangent, m_vehicle.TrackNormal, m_vehicle.TrackWidth);
		}
		GrapplePoint grapplePoint = m_vehicle.GrapplePoint;
		float grappleForce = m_vehicle.GrappleForce;
		Vector3 grappleFrontPos = m_vehicle.GrappleFrontPos;
		if (m_rope != null)
		{
			m_grappleLengthPropOld2 = m_grappleLengthPropOld1;
			m_grappleLengthPropOld1 = m_grappleLengthProp;
			m_grappleLengthProp += ((!(grapplePoint != null)) ? (-0.05f) : 0.25f);
			m_grappleLengthProp = Mathf.Clamp01(m_grappleLengthProp);
			if (grapplePoint != null || m_grappleLengthProp > 0f)
			{
				m_rope.Update(grappleForce, grappleFrontPos, ref m_grapplePointPos, m_grapplePointRadius);
			}
		}
		if (grapplePoint != null)
		{
			Vector3 grapplePointPos = m_grapplePointPos;
			Vector3 vector = grappleFrontPos - grapplePointPos;
			vector.Normalize();
			grapplePointPos += vector * 1.4f;
			float num = 14f;
			float num2 = 6f;
			Vector3 zero = Vector3.zero;
			zero.x = Random.Range(0f - num2, num2);
			zero.z = Random.Range(0f - num2, num2);
			zero.x += vector.x * num;
			zero.z += vector.z * num;
			RenderSettings renderSettings = SceneSettings.RenderSettings;
			renderSettings.EmitSparkParticle(grapplePointPos, zero);
		}
		if (m_snowSpray != null)
		{
			if (grapplePoint != null)
			{
				float num3 = Vector3.Dot(grapplePoint.Position - m_vehicle.Position, m_vehicle.Right);
				Wheel wheel = ((num3 > 0f) ? Wheel.FL : Wheel.FR);
				if (wheel != m_snowSpraySide)
				{
					if (m_currentSkidMark != null)
					{
						m_currentSkidMark.End(5f);
						m_currentSkidMark = null;
					}
					m_snowSpraySide = wheel;
				}
				float num4 = 0f;
				if (IsVisible && m_vehicle.IsWheelGrounded(m_snowSpraySide))
				{
					num4 = Mathf.Clamp01((Mathf.Abs(grappleForce) - 10f) * (1f / 60f));
					num4 = Mathf.Clamp01(num4 * num4);
				}
				if (m_snowSprayForce > num4)
				{
					m_snowSprayForce = Mathf.Max(num4, m_snowSprayForce - Time.deltaTime * 2.5f);
				}
				else
				{
					m_snowSprayForce = Mathf.Min(num4, m_snowSprayForce + Time.deltaTime * 2.5f);
				}
			}
			else
			{
				m_snowSprayForce = Mathf.Max(0f, m_snowSprayForce - Time.deltaTime * 2.5f);
			}
		}
		MiniRewardUpdate();
		if ((_distributor & 1) == 0)
		{
			if (m_vehicle.IsLocalHuman || m_vehicle.IsGhost)
			{
				ProximityUpdate();
			}
			AlphaUpdate(Time.fixedDeltaTime * 2f);
		}
		if (m_lightmapSampler != null)
		{
			if (currentTrackSpline != null)
			{
				m_lightmapSampler.UpdateForTrackSpline(currentTrackSpline, m_vehicle.CurrentT, m_vehicle.TrackPoint, m_vehicle.TrackNormal, m_vehicle.TrackTangent, m_vehicle.TrackWidth);
			}
			else if (currentTrackHub != null)
			{
				m_lightmapSampler.UpdateForTrackHub(currentTrackHub);
			}
		}
	}

	private void ProximityUpdate()
	{
		TrackLocation location = m_vehicle.Location;
		m_proximity = float.MaxValue;
		for (int i = 0; i < m_otherVehicles.Length; i++)
		{
			Vehicle vehicle = m_otherVehicles[i];
			if (!vehicle.IsDead && (!m_vehicle.IsGhost || !vehicle.IsGhost))
			{
				float f = TrackUtils.MinDistance(location, m_otherVehicles[i].Location);
				if (float.IsNaN(f))
				{
					m_proximity = 0f;
					break;
				}
				m_proximity = Mathf.Min(m_proximity, Mathf.Abs(f));
			}
		}
	}

	private void AlphaUpdate(float deltaTime)
	{
		Material sharedMaterial = m_chassisRenderer.sharedMaterial;
		float num2;
		float shadowAlpha;
		float alpha;
		if (m_vehicle.IsGhost)
		{
			float num = Mathf.Min(1f, m_proximity * 0.25f);
			num += ((!(num < m_distAlpha)) ? (-1f) : 1f) * deltaTime;
			num = Mathf.Clamp01(num);
			num2 = m_alpha * num;
			shadowAlpha = num2 * 0.5f;
			alpha = num2 * 0.5f;
		}
		else
		{
			num2 = m_alpha;
			shadowAlpha = num2;
			alpha = num2;
			Shader shader = ((!(num2 < 1f)) ? m_opaqueShader : m_alphaShader);
			if (sharedMaterial.shader != shader)
			{
				sharedMaterial.shader = shader;
			}
		}
		if (num2 != m_materialAlpha)
		{
			m_materialAlpha = num2;
			sharedMaterial.SetFloat("_Alpha", num2);
			SetShadowAlpha(shadowAlpha);
			SetTrailAlpha(alpha, true);
		}
	}

	public void VehicleUpdate(float _interpolation)
	{
		if (m_shadowBlob != null)
		{
			m_shadowBlob.SetPosition(m_transform.position);
		}
		Color color = ((!(m_lightmapSampler != null)) ? Color.white : m_lightmapSampler.CoarseLighting);
		color.a = m_alpha;
		DynamicCamera dynamicCamera = ((!m_enableFrustumCulling) ? null : DynamicCamera.Get);
		if (PlatformSettings.ConstantExtraction)
		{
			m_lightmapSampler.UpdateObjectSpaceParams(m_chassisRenderer.transform);
		}
		if (m_ropeLine != null)
		{
			bool flag = m_vehicle.GrapplePoint != null;
			float num = ((!(_interpolation < 0f)) ? Mathf.Lerp(m_grappleLengthPropOld1, m_grappleLengthProp, _interpolation) : Mathf.Lerp(m_grappleLengthPropOld2, m_grappleLengthPropOld1, _interpolation + 1f));
			if (flag || num > 0f)
			{
				Vector3 vector = m_transform.TransformPoint(m_grappleGunOffset);
				bool flag2 = true;
				if (dynamicCamera != null)
				{
					Vector3 centre = (vector + m_grapplePointPos) * 0.5f;
					float radius = (vector - m_grapplePointPos).magnitude * 0.5f;
					flag2 = dynamicCamera.IsSphereInClipFrustum(centre, radius);
				}
				m_ropeLineGO.active = flag2;
				if (flag2)
				{
					if (m_rope != null)
					{
						m_rope.AddToLineMesh(m_ropeLine, _interpolation, vector.y, m_grapplePointPos.y, num, flag, vector, color);
					}
					else
					{
						m_ropeLine.SetLine(vector, m_grapplePointPos, 0.15f, color, Color.white);
					}
				}
				if (m_grappleSteamRenderer != null && m_grappleSteamRenderer.IsActive(m_vehicle.PlayerIndex))
				{
					Vector3 vector2 = vector;
					Vector3 normalized = (m_grapplePointPos - vector2).normalized;
					vector2.y += 0.2f;
					m_grappleSteamRenderer.SetData(m_vehicle.PlayerIndex, vector2, normalized, color);
				}
			}
			else
			{
				m_ropeLineGO.active = false;
			}
		}
		if (m_trail != null && m_trailDeployed)
		{
			Color color2 = ((!m_enableTrailColour) ? Color.white : m_vehicle.Colour);
			float num2 = 1f - m_trailEmissiveAmount;
			color2.r *= m_trailEmissiveAmount + color.r * num2;
			color2.g *= m_trailEmissiveAmount + color.g * num2;
			color2.b *= m_trailEmissiveAmount + color.b * num2;
			color2.a = m_trailAlpha;
			Vector3 forward = m_transform.forward;
			Vector3 point = m_transform.position + forward * m_trailOffset.y;
			point.y += m_trailOffset.x;
			Vector3 normalized2 = Vector3.Cross(Vector3.up, forward).normalized;
			m_trail.UpdateHeadSpatial(dynamicCamera, point, normalized2 * m_trailWidth, color2, m_trailSegmentSize);
			if (m_trailShadow != null)
			{
				point += m_trailShadowOffset;
				Color color3 = m_trailShadowBlend;
				if (m_vehicle.InnerEdgeType == TrackEdgeType.Air && m_vehicle.OuterEdgeType == TrackEdgeType.Air)
				{
					color3 = Color.black;
				}
				m_trailShadow.UpdateHeadSpatial(dynamicCamera, point, normalized2 * m_trailWidth, color3, m_trailSegmentSize);
			}
		}
		if (m_snowSpray != null)
		{
			Vector3 vector3 = m_transform.right;
			vector3.y = 0f;
			if (m_snowSpraySide == Wheel.FL)
			{
				vector3 = -vector3;
			}
			Vector3 wheelHitPosition = m_vehicle.GetWheelHitPosition(m_snowSpraySide);
			Vector3 point2 = wheelHitPosition + vector3 * 2f;
			point2.y += 0.1f;
			float num3 = m_alpha * m_snowSprayForce;
			bool flag3 = num3 > 0.01f;
			Color32 colour;
			if (flag3)
			{
				colour = color;
				colour.a = (byte)(int)(Mathf.Min(1f, num3) * 255f);
			}
			else
			{
				colour = default(Color32);
				vector3 = Vector3.zero;
			}
			if (m_skidMarkManager != null)
			{
				if ((m_currentSkidMark == null && num3 > 0.4f) || (m_currentSkidMark != null && num3 > 0.2f))
				{
					Vector3 point3 = wheelHitPosition;
					point3.y += 0.05f;
					if (m_currentSkidMark == null)
					{
						m_currentSkidMark = m_skidMarkManager.GetFreeSkidMark();
						if (m_currentSkidMark != null)
						{
							m_currentSkidMark.Begin(point3, vector3 * 0.4f, colour);
						}
					}
					else if (m_currentSkidMark.NextPoint(point3, vector3 * 0.4f, 1f, colour))
					{
						m_currentSkidMark.End(5f);
						m_currentSkidMark = null;
					}
				}
				else if (m_currentSkidMark != null)
				{
					m_currentSkidMark.End(5f);
					m_currentSkidMark = null;
				}
			}
			float deltaTime = Time.deltaTime;
			colour.a = (byte)(int)((float)(int)colour.a * 1.2f);
			if (flag3)
			{
				m_snowSpray.UpdateHeadTemporal(dynamicCamera, point2, vector3 * 1.75f, colour, 0.02f, deltaTime);
			}
			else
			{
				m_snowSpray.UpdateHeadTemporalNoWidth(dynamicCamera, point2, colour, 0.02f, deltaTime);
			}
		}
		if (m_currentOilSlickMark != null)
		{
			Vector3 right = m_transform.right;
			Vector3 wheelHitPosition2 = m_vehicle.GetWheelHitPosition(m_currentOilSlickSide);
			wheelHitPosition2.y += 0.05f;
			float num4 = Mathf.Clamp01(m_vehicle.OilSlickTime);
			bool flag4 = num4 == 0f;
			if (!m_currentOilSlickMark.IsActive)
			{
				m_currentOilSlickMark.Begin(wheelHitPosition2, right * 0.4f, new Color(0f, 0f, 0f, 0f));
			}
			else if (m_currentOilSlickMark.NextPoint(wheelHitPosition2, right * 0.4f, 1f, new Color(0f, 0f, 0f, num4)))
			{
				flag4 = true;
			}
			if (flag4)
			{
				m_currentOilSlickMark.End(5f);
				m_currentOilSlickMark = null;
			}
		}
		if (m_shadow != null)
		{
			m_shadow.SetTrackData(m_vehicle.CurrentTrackSpline, m_vehicle.CurrentT, m_vehicle.CurrentTrackHub, m_vehicle.CurrentHubSector);
			if (m_vehicle.InnerEdgeType == TrackEdgeType.Air && m_vehicle.OuterEdgeType == TrackEdgeType.Air)
			{
				m_shadow.RenderEnable = false;
			}
			else
			{
				m_shadow.RenderEnable = true;
			}
		}
		if (m_vehicle.EnableMiniRewards)
		{
			if (m_miniRewardTime > 0f)
			{
				float deltaTime2 = Time.deltaTime;
				m_miniRewardTime -= deltaTime2;
			}
			if (m_miniRewardParticleId != -1 && !SceneSettings.RenderSettings.SetMiniRewardParticleData(m_miniRewardParticleId, m_transform.position))
			{
				m_miniRewardParticleId = -1;
			}
			if (m_shadowBlob != null)
			{
				float glowData = 0f;
				if (m_miniRewardTime > 0f)
				{
					glowData = m_miniRewardTime / 0.75f;
					glowData = 1f - glowData;
					glowData = Mathf.Min(glowData * 8f, 1f - Mathf.Pow(glowData, 1.5f));
				}
				m_shadowBlob.SetGlowData(glowData);
			}
		}
		if (m_playerIdentifier != null)
		{
			float num5 = ((!(m_proximityAlpha > 0.5f)) ? 5f : 7.5f);
			float num6 = ((!(m_proximity < num5)) ? 0f : 1f);
			float num7 = m_proximityAlpha - num6;
			float num8 = Time.deltaTime * 5f;
			if (num7 > num8)
			{
				m_proximityAlpha -= num8;
			}
			else if (num7 < 0f - num8)
			{
				m_proximityAlpha += num8;
			}
			else
			{
				m_proximityAlpha = num6;
			}
			Transform transform = MainCamera.Transform;
			if (transform != null)
			{
				m_playerIdentifier.SetData(m_transform.position, transform.transform.up, -transform.transform.forward);
				float f = Vector3.Dot(m_vehicle.Forward, transform.transform.right);
				f = Mathf.Abs(f);
				float width = f * 0.5f + 4.5f;
				float height = (1f - f) * 0.5f + 4.5f;
				m_playerIdentifier.SetExtraData(m_proximityAlpha, width, height);
			}
			else
			{
				m_playerIdentifier.SetData(m_transform.position);
			}
		}
	}

	public void EmitLaunchParticles(float _emitTime)
	{
		if (m_launchSteamEmitter != null)
		{
			StartCoroutine(LaunchParticles(_emitTime));
		}
	}

	private IEnumerator LaunchParticles(float _emitTime)
	{
		while (_emitTime > 0f)
		{
			yield return YieldCache.WaitForFixedUpdate;
			float deltaTime = Time.deltaTime;
			_emitTime -= deltaTime;
			Vector3 vehiclePosition = m_vehicle.Position + m_vehicle.Forward * -2f;
			Vector3 vehicleVelocity = m_vehicle.Velocity;
			float vehicleSpeedScale = Mathf.Clamp01(vehicleVelocity.magnitude * 0.1f);
			Color colour = Color.white;
			if (m_lightmapSampler != null)
			{
				colour = m_lightmapSampler.CoarseLighting;
			}
			vehicleVelocity *= 0.75f;
			for (int i = 0; i < 1; i++)
			{
				float lerp = (float)i / 1f;
				Vector3 position = vehiclePosition;
				position += Random.insideUnitSphere * 0.05f;
				position += vehicleVelocity * (lerp * deltaTime);
				Vector3 velocity = Random.insideUnitSphere;
				velocity.y++;
				velocity *= vehicleSpeedScale * 1.5f;
				velocity -= vehicleVelocity;
				float size = Random.Range(m_launchSteamEmitter.minSize, m_launchSteamEmitter.maxSize);
				float energy = Random.Range(m_launchSteamEmitter.minEnergy, m_launchSteamEmitter.maxEnergy);
				m_launchSteamParticleSys.Emit(position, velocity, size, energy, colour, 0f, 0f);
			}
		}
	}

	public void StartGrapple(GrapplePoint _grapplePoint, Vector3 _pos0, Vector3 _vel0)
	{
		if (m_rope != null)
		{
			m_grapplePointPos = _grapplePoint.Position;
			m_grapplePointRadius = _grapplePoint.radius;
			m_grappleLengthProp = 0f;
			m_grappleLengthPropOld1 = m_grappleLengthProp;
			m_rope.Init(_pos0, _vel0, m_grapplePointPos - (m_grapplePointPos - _pos0).normalized * m_grapplePointRadius);
		}
	}

	private void OnVehicleCollision(Vector3 point, Vector3 normal, Vector3 velocity, bool isMajor, bool playAudio)
	{
		if (!RenderCollisions)
		{
			return;
		}
		Color color = Color.white;
		if (m_lightmapSampler != null)
		{
			color = m_lightmapSampler.CoarseLighting;
		}
		ColourPaletteComponent component = m_collisionParticleEmitter.GetComponent<ColourPaletteComponent>();
		Vector3 vector = velocity * 0.25f;
		int num = ((!isMajor) ? 5 : 10);
		for (int i = 0; i != num; i++)
		{
			Vector3 insideUnitSphere = RandomTables.InsideUnitSphere;
			Vector3 insideUnitSphere2 = RandomTables.InsideUnitSphere;
			float rotation = RandomTables.Value * 360f;
			float angularVelocity = RandomTables.Value * 90f;
			Color color2 = color;
			if (component != null)
			{
				color2 *= component.RandomColour;
			}
			m_collisionParticleEmitter.Emit(point + insideUnitSphere, vector + insideUnitSphere2, 1f, 1f, color2, rotation, angularVelocity);
		}
		if (isMajor)
		{
			Vector3 velocity2 = velocity * 0.8f;
			float rotation2 = MathUtils.AngleOnXZ(normal);
			m_sparksParticleEmitter.Emit(point, velocity2, 3f, 0.35f, new Color(1f, 1f, 1f, 0.75f), rotation2, 0f);
		}
		if (m_crashBank != null && playAudio)
		{
			m_audioPlayer.PlayRandomClip(m_crashBank, (!isMajor) ? (m_volumeScale * 0.5f) : m_volumeScale);
		}
	}

	private void OnSurroundCollision(bool isMajor)
	{
		if (m_wallCrashBank != null)
		{
			m_audioPlayer.PlayRandomClip(m_wallCrashBank, (!isMajor) ? (m_volumeScale * 0.5f) : m_volumeScale);
		}
	}

	private void OnTrackCollision()
	{
		if (m_crashBank != null)
		{
			m_audioPlayer.PlayRandomClip(m_crashBank, m_volumeScale * 0.5f);
		}
		Vector3 velocity = m_vehicle.Velocity;
		EmitSnowBlast(new Vector2(velocity.x, velocity.z));
	}

	public void DeployTrail(float _deployTime, float _deployAmount)
	{
		if (m_trail != null)
		{
			StartCoroutine(DeployTrailAnim(_deployTime, _deployAmount));
		}
	}

	private IEnumerator DeployTrailAnim(float _deployTime, float _deployAmount)
	{
		float timer = 0f;
		while (timer < _deployTime)
		{
			yield return null;
			timer += Time.deltaTime;
			float deployProp = timer * _deployAmount / _deployTime;
			Color colour = ((!m_enableTrailColour) ? Color.white : m_vehicle.Colour);
			if (m_lightmapSampler != null)
			{
				float trailLitAmount = 1f - m_trailEmissiveAmount;
				Color lightColour = m_lightmapSampler.CoarseLighting;
				colour.r *= m_trailEmissiveAmount + lightColour.r * trailLitAmount;
				colour.g *= m_trailEmissiveAmount + lightColour.g * trailLitAmount;
				colour.b *= m_trailEmissiveAmount + lightColour.b * trailLitAmount;
			}
			colour.a = 0f;
			Vector3 forward = m_transform.forward;
			Vector3 start = m_transform.position + forward * m_trailOffset.y;
			Vector3 end = m_transform.position + forward * (m_trailOffset.y - (float)m_trail.PointCount * m_trailSegmentSize * deployProp);
			start.y += m_trailOffset.x;
			end.y += m_trailOffset.x;
			m_trail.Set(start, end, m_trailWidth, colour);
			if (m_trailShadow != null)
			{
				start += m_trailShadowOffset;
				end += m_trailShadowOffset;
				m_trailShadow.Set(start, end, m_trailWidth, m_trailShadowBlend);
			}
		}
		m_trailDeployed = true;
	}

	private void ApplyTrackSnowParticles(Vector3 _position, Vector3 _velocity, Vector3 _trackPoint, Vector3 _trackTangent, Vector3 _trackNormal, float _trackWidth)
	{
		if (!IsVisible)
		{
			return;
		}
		Vector3 vector = Vector3.Dot(_velocity, _trackTangent) * _trackTangent;
		float magnitude = vector.magnitude;
		float num = 20f;
		float num2 = 40f;
		if (!(magnitude > num))
		{
			return;
		}
		Vector3 position = _trackPoint;
		Vector3 velocity = vector * 0.05f;
		float a = (magnitude - num) / (num2 - num);
		a = Mathf.Min(a, 1f);
		a *= 0.7f;
		float num3 = Vector3.Dot(_trackPoint, _trackNormal);
		float num4 = Vector3.Dot(_position, _trackNormal);
		float num5 = num4 - num3;
		float num6 = _trackWidth * 0.2f;
		float num7 = _trackWidth * 0.4f;
		TrackEdgeType trackEdgeType;
		if (num5 < 0f - num6)
		{
			position -= _trackNormal * _trackWidth;
			velocity += _trackNormal;
			a *= Mathf.Min((0f - num5 - num6) / (num7 - num6), 1f);
			trackEdgeType = m_vehicle.InnerEdgeType;
		}
		else if (num5 > num6)
		{
			position += _trackNormal * _trackWidth;
			velocity -= _trackNormal;
			a *= Mathf.Min((num5 - num6) / (num7 - num6), 1f);
			trackEdgeType = m_vehicle.OuterEdgeType;
		}
		else
		{
			trackEdgeType = TrackEdgeType.None;
		}
		if (trackEdgeType != TrackEdgeType.None && trackEdgeType != TrackEdgeType.Air && a > 0.1f)
		{
			velocity *= 4f;
			TrackEdgeType trackEdgeType2 = trackEdgeType;
			Color colour = ((trackEdgeType2 != TrackEdgeType.Snow && trackEdgeType2 == TrackEdgeType.Grass) ? Color.grey : Color.white);
			if (m_lightmapSampler != null)
			{
				colour *= m_lightmapSampler.CoarseLighting;
			}
			colour.a = a;
			float rotation = Random.Range(0f, 360f);
			RenderSettings renderSettings = SceneSettings.RenderSettings;
			renderSettings.EmitTrackSnowParticle(position, velocity, 3.5f, 1f, colour, rotation);
		}
	}

	private void OnCollectMiniReward()
	{
		m_numMiniRewards++;
		TriggerMiniReward((m_numMiniRewards <= 1) ? 0.75f : 1f, m_numMiniRewards > 1, false, true);
	}

	private void OnDeployMiniReward(bool _dischargeAll)
	{
		TriggerMiniReward(1f, true, true);
		m_miniRewardBoostReleaseTime = (float)m_numMiniRewards * 0.25f;
		m_numMiniRewards -= ((!_dischargeAll) ? 1 : m_numMiniRewards);
		if (m_miniRewardDeployedBank != null)
		{
			m_audioPlayer.PlayRandomClip(m_miniRewardDeployedBank, VolumeScale);
		}
	}

	private void TriggerMiniReward(float _strength = 1f, bool _sparks = false, bool _extra = false, bool _inwardSparks = false, bool _shockParticle = false)
	{
		if (m_miniRewardAcquiredBank != null)
		{
			m_audioPlayer.PlayRandomClip(m_miniRewardAcquiredBank, VolumeScale);
		}
		if (_extra)
		{
			m_miniRewardTime = 0.75f;
		}
		if (_shockParticle && m_miniRewardParticleId == -1)
		{
			float energy = 0.2f;
			float size = _strength * 3f;
			m_miniRewardParticleId = SceneSettings.RenderSettings.EmitMiniRewardParticle(m_vehicle.Position + m_vehicle.Up * -0.75f, Vector3.zero, size, energy);
		}
		if (_inwardSparks)
		{
			Vector3 position = m_vehicle.Position;
			Vector3 velocity = m_vehicle.Velocity;
			velocity *= 1f;
			Color color = c_miniRewardColour;
			for (int i = 0; i < 30; i++)
			{
				Vector2 normalized = RandomTables.InsideUnitCircle.normalized;
				Vector3 pos = position;
				pos.x += normalized.x * 4f;
				pos.z += normalized.y * 4f;
				Vector2 vector = normalized * -20f;
				Vector3 velocity2 = velocity;
				velocity2.x += vector.x;
				velocity2.z += vector.y;
				m_miniRewardSparksReversedParticleEmitter.Emit(pos, velocity2, 4f, 0.25f, color, RandomTables.Value * 360f, RandomTables.Value * 10f);
			}
		}
		if (_sparks)
		{
			Vector3 position2 = m_vehicle.Position;
			Vector3 velocity3 = m_vehicle.Velocity;
			velocity3 *= 0.8f;
			Color color2 = c_miniRewardColour;
			for (int j = 0; j < 10; j++)
			{
				Vector3 pos2 = position2;
				Vector2 vector2 = RandomTables.InsideUnitCircle * 5f;
				Vector3 velocity4 = velocity3;
				velocity4.x += vector2.x;
				velocity4.z += vector2.y;
				float rotation = Random.Range(0f, 360f);
				m_miniRewardSparksParticleEmitter.Emit(pos2, velocity4, 5f, 0.35f, color2, rotation, 0f);
			}
		}
	}

	private void MiniRewardUpdate()
	{
		if (m_numMiniRewards <= 0)
		{
			return;
		}
		Vector3 vector = m_vehicle.Position + m_vehicle.Forward * -1.5f;
		Vector3 velocity = m_vehicle.Velocity;
		velocity *= 0.2f;
		Color color = c_miniRewardColour;
		for (int i = 0; i < 4; i++)
		{
			if (m_numMiniRewards >= 2 || Random.Range(0, 2) < m_numMiniRewards)
			{
				Vector2 vector2 = RandomTables.InsideUnitCircle * 0.5f;
				Vector3 pos = vector;
				pos.x += vector2.x;
				pos.z += vector2.y;
				Vector2 vector3 = RandomTables.InsideUnitCircle * 1f;
				Vector3 velocity2 = velocity;
				velocity2.x += vector3.x;
				velocity2.z += vector3.y;
				float rotation = RandomTables.Value * 360f;
				m_miniRewardSparksParticleEmitter.Emit(pos, velocity2, 4f, 0.35f, color, rotation, 0f);
			}
		}
	}

	private void OnDropMiniReward()
	{
		if (m_numMiniRewards != 0)
		{
			float num = (float)m_numMiniRewards / 2f;
			m_numMiniRewards = 0;
			if (m_miniRewardReleasedBank != null)
			{
				m_audioPlayer.PlayRandomClip(m_miniRewardReleasedBank, VolumeScale);
			}
			Vector3 position = m_vehicle.Position;
			Vector3 velocity = m_vehicle.Velocity;
			velocity *= 0.4f;
			Color color = c_miniRewardColour;
			int num2 = Mathf.CeilToInt(40f * num);
			for (int i = 0; i < num2; i++)
			{
				Vector2 normalized = RandomTables.InsideUnitCircle.normalized;
				Vector3 pos = position;
				pos.x += normalized.x * 1.5f;
				pos.z += normalized.y * 1.5f;
				Vector2 vector = normalized * (RandomTables.Value * 3f + 1f);
				Vector3 velocity2 = velocity;
				velocity2.x += vector.x;
				velocity2.z += vector.y;
				m_miniRewardSparksParticleEmitter.Emit(pos, velocity2, 4f, 0.65f, color, RandomTables.Value * 360f, RandomTables.Value * 10f);
			}
		}
	}

	private void MiniRewardBoostUpdate()
	{
		Vector3 position = m_vehicle.Position;
		Vector3 velocity = m_vehicle.Velocity;
		velocity *= 0.1f;
		Color color = c_miniRewardColour;
		int num = 6;
		for (int i = 0; i < num; i++)
		{
			Vector3 pos = position;
			Vector3 velocity2 = velocity * (RandomTables.Value * 0.5f + 0.5f);
			m_miniRewardSparksParticleEmitter.Emit(pos, velocity2, 4f, 0.4f, color, RandomTables.Value * 360f, RandomTables.Value * 10f);
		}
	}

	public void EmitSnowBlast(Vector2 _vel2)
	{
		Vector3 position = m_vehicle.Position;
		position.y -= 0.5f;
		Color color = Color.white;
		if (m_lightmapSampler != null)
		{
			color = m_lightmapSampler.CoarseLighting;
		}
		for (int i = 0; i < 10; i++)
		{
			Vector2 vector = _vel2 * 0f + RandomTables.OnUnitCircle * (RandomTables.Value * 2.5f + 12.5f);
			Vector3 vel = new Vector3(vector.x, 2f, vector.y);
			Vector3 pos = position;
			pos.x += vel.x * 0.05f;
			pos.z += vel.z * 0.05f;
			Color colour = color;
			m_snowBlastParticleSys.Emit(pos, vel, 1.5f, 0.35f, colour, 0f, 0f);
		}
	}

	public void SetShadowAlpha(float _alpha)
	{
		if (m_shadow != null)
		{
			m_shadow.SetBlend(m_shadowBlend * _alpha);
		}
		if (m_shadowBlob != null)
		{
			m_shadowBlob.SetAlpha(_alpha);
		}
		if (m_trailShadow != null)
		{
			m_trailShadowBlend = m_shadowBlend * _alpha;
		}
	}

	public void SetTrailAlpha(float _alpha, bool _headOnly)
	{
		m_trailAlpha = _alpha;
		if (!_headOnly && m_trail != null)
		{
			m_trail.SetAlpha(_alpha);
		}
	}

	private void OnGrapple(GrapplePoint _grapplePoint)
	{
		if (m_grappleBank != null)
		{
			m_audioPlayer.PlayRandomClip(m_grappleBank, VolumeScale);
		}
		if (m_grappleSteamRenderer != null)
		{
			Vector3 vector = m_transform.TransformPoint(m_grappleGunOffset);
			Vector3 normalized = (m_grapplePointPos - vector).normalized;
			vector.y += 0.2f;
			m_grappleSteamRenderer.Emit(m_vehicle.PlayerIndex);
			m_grappleSteamRenderer.SetData(m_vehicle.PlayerIndex, vector, normalized, Color.white);
		}
	}

	private void OnGrappleRelease(GrapplePoint _grapplePoint)
	{
		if (m_recoilBank != null)
		{
			m_audioPlayer.PlayRandomClip(m_recoilBank, VolumeScale);
		}
	}

	private void OnOilSlick(OilSlick _oilSlick)
	{
		if (m_skidMarkManager != null)
		{
			Vector3 lhs = _oilSlick.transform.position - m_transform.position;
			m_currentOilSlickSide = ((!(Vector3.Dot(lhs, m_transform.right) > 0f)) ? Wheel.FL : Wheel.FR);
			m_currentOilSlickMark = m_skidMarkManager.GetFreeSkidMark();
		}
	}

	public void OnChildBecameVisible(Object _child)
	{
		IsVisible = true;
	}

	public void OnChildBecameInvisible(Object _child)
	{
		IsVisible = false;
	}

	public void EnterObscuredZone()
	{
		if (m_obscuredRenderer != null)
		{
			m_obscuredRenderer.EnterObscuredZone();
		}
	}

	public void ExitObscuredZone()
	{
		if (m_obscuredRenderer != null)
		{
			m_obscuredRenderer.ExitObscuredZone();
		}
	}

	public void RaceEvent_Launch()
	{
		DeployTrail(0.5f, 0.01f);
	}

	private void OnDeath(GameObject _vehicleGO)
	{
		if (m_explosionPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(m_explosionPrefab, m_transform.position, Quaternion.LookRotation(Vector3.up)) as GameObject;
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			Object.Destroy(gameObject, component.duration);
		}
		if (m_catchParticlePrefab != null && FleePursuerVisuals.s_this != null)
		{
			GameObject gameObject2 = Object.Instantiate(m_catchParticlePrefab, m_transform.position, Quaternion.identity) as GameObject;
			gameObject2.transform.parent = FleePursuerVisuals.s_this.transform;
			ParticleSystem component2 = gameObject2.GetComponent<ParticleSystem>();
			Object.Destroy(gameObject2, component2.duration + component2.startLifetime);
		}
		if (m_catchAudioBank != null)
		{
			m_audioPlayer.PlayRandomClip(m_catchAudioBank);
		}
	}

	private void OnShield()
	{
		if (m_shieldParticlePrefab != null)
		{
			GameObject gameObject = Object.Instantiate(m_shieldParticlePrefab, m_transform.position, m_transform.rotation) as GameObject;
			gameObject.transform.parent = m_transform;
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			Object.Destroy(gameObject, component.duration + component.startLifetime);
		}
	}

	private void OnBoost()
	{
		if (m_boostBank != null)
		{
			m_audioPlayer.PlayRandomClip(m_boostBank, VolumeScale);
		}
	}

	private void OnBatteringRam(Vector3 _point, Vector3 _direction)
	{
		RenderSettings renderSettings = SceneSettings.RenderSettings;
		Color white = Color.white;
		for (int i = 0; i < 16; i++)
		{
			Vector3 insideUnitSphere = RandomTables.InsideUnitSphere;
			Vector3 position = _point + insideUnitSphere;
			Vector3 velocity = _direction + insideUnitSphere * 0.5f;
			renderSettings.EmitSparkParticle(position, velocity, 0.15f, 0.25f, white);
		}
	}
}
