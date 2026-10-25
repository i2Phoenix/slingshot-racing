using UnityEngine;

[AddComponentMenu("Gameplay/Flee/Pursuer Visuals")]
[RequireComponent(typeof(AudioSource))]
public class FleePursuerVisuals : MonoBehaviour
{
	public static FleePursuerVisuals s_this;

	private Transform m_transform;

	private AudioSource m_audioSource;

	private LightmapSampler m_lightmapSampler;

	private ParticleSystem m_particleSystem;

	private Animation m_animation;

	private bool m_adjustPitch = true;

	private FleePursuer m_pursuer;

	public AudioBank m_catchPersonalAudioBank;

	public float m_enginePitchScale;

	public float m_enginePitchOffset;

	public Transform Transform
	{
		get
		{
			return m_transform;
		}
	}

	private void Awake()
	{
		m_transform = base.transform;
		m_audioSource = base.GetComponent<AudioSource>();
		m_lightmapSampler = base.gameObject.AddComponent<LightmapSampler>();
		m_particleSystem = GameObjectUtils.GetComponentInChildren<ParticleSystem>(base.gameObject);
		m_animation = GameObjectUtils.GetComponentInChildren<Animation>(base.gameObject);
		if (s_this == null)
		{
			s_this = this;
		}
	}

	private void OnDestroy()
	{
		if (s_this == this)
		{
			s_this = null;
		}
	}

	public void Init(FleePursuer _pursuer)
	{
		m_pursuer = _pursuer;
	}

	public void PursuerUpdate()
	{
		if (m_pursuer.CurrentTrackSpline != null)
		{
			m_lightmapSampler.UpdateForTrackSpline(m_pursuer.CurrentTrackSpline, m_pursuer.CurrentT, m_pursuer.TrackPoint, m_pursuer.TrackNormal, m_pursuer.TrackTangent, m_pursuer.TrackWidth);
		}
		else if (m_pursuer.CurrentTrackHub != null)
		{
			m_lightmapSampler.UpdateForTrackHub(m_pursuer.CurrentTrackHub);
		}
		m_particleSystem.startColor = m_lightmapSampler.CoarseLighting;
		if (m_adjustPitch)
		{
			m_audioSource.pitch = m_pursuer.EnginePitch * m_enginePitchScale + m_enginePitchOffset;
		}
	}

	public void FleeManager_Go()
	{
		m_particleSystem.Play();
	}

	public void FleeManager_StopPursuit()
	{
		m_audioSource.Stop();
		if (m_catchPersonalAudioBank != null)
		{
			m_audioSource.loop = false;
			m_audioSource.pitch = 1f;
			m_catchPersonalAudioBank.PlayRandomClip(m_audioSource);
			m_adjustPitch = false;
		}
		m_particleSystem.Stop();
		m_animation.Stop();
	}
}
