using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Track/Track Obstacle")]
[RequireComponent(typeof(Collider))]
public class TrackObstacle : IndexedMonoBehaviour<TrackObstacle>
{
	private RaceManager m_raceManager;

	private GameObject m_intactVisuals;

	private GameObject m_destroyedVisuals;

	private GameObject m_transitionEffect;

	private ParticleSystem m_particleSystem;

	private bool m_destroyed;

	private AudioPlayer m_audioPlayer;

	public AudioBank m_audioBank;

	public AudioBank m_whenDestroyedAudioBank;

	public GameObject m_intactVisualsPrefab;

	public GameObject m_destroyedVisualsPrefab;

	public GameObject m_transitionEffectPrefab;

	public float m_intactDragScalar = 60f;

	public float m_destroyedDragScalar = 60f;

	public bool m_moveTransitionEffect;

	private new void Awake()
	{
		base.Awake();
		m_audioPlayer = base.gameObject.GetComponent<AudioPlayer>();
		if (m_audioPlayer == null)
		{
			m_audioPlayer = base.gameObject.AddComponent<AudioPlayer>();
		}
	}

	private new void Start()
	{
		base.Start();
		m_intactVisuals = UnityEngine.Object.Instantiate(m_intactVisualsPrefab) as GameObject;
		SetAsChild(m_intactVisuals.transform);
		m_destroyedVisuals = UnityEngine.Object.Instantiate(m_destroyedVisualsPrefab) as GameObject;
		SetAsChild(m_destroyedVisuals.transform);
		m_destroyedVisuals.SetActiveRecursively(false);
		m_transitionEffect = UnityEngine.Object.Instantiate(m_transitionEffectPrefab) as GameObject;
		SetAsChild(m_transitionEffect.transform);
		m_transitionEffect.transform.localPosition = Vector3.up;
		m_particleSystem = m_transitionEffect.GetComponent<ParticleSystem>();
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceReplayEvent += OnReplay;
		}
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceReplayEvent -= OnReplay;
		}
	}

	private void OnReplay(object _sender, EventArgs _args)
	{
		StopCoroutine("PlayDestruction");
		m_intactVisuals.SetActiveRecursively(true);
		m_destroyedVisuals.SetActiveRecursively(false);
		m_destroyed = false;
	}

	private void SetAsChild(Transform _transform)
	{
		Quaternion rotation = _transform.rotation;
		_transform.parent = base.transform;
		_transform.localRotation = rotation;
		_transform.localPosition = Vector3.zero;
	}

	private void OnTriggerEnter(Collider collider)
	{
		PhysicsVehicle component = collider.transform.root.GetComponent<PhysicsVehicle>();
		if (component != null)
		{
			float dragScalar = ((!m_destroyed) ? m_intactDragScalar : m_destroyedDragScalar);
			component.TriggerObstacleHit(this, dragScalar);
			TriggerCollision(component);
		}
	}

	public void TriggerCollision(Vehicle vehicle)
	{
		bool flag = vehicle.Visuals != null;
		if (!m_destroyed)
		{
			m_destroyed = true;
			if (flag && m_audioBank != null)
			{
				m_audioPlayer.PlayRandomClip(m_audioBank, vehicle.Visuals.VolumeScale);
			}
			StartCoroutine("PlayDestruction");
		}
		else if (flag && m_whenDestroyedAudioBank != null)
		{
			m_audioPlayer.PlayRandomClip(m_whenDestroyedAudioBank, vehicle.Visuals.VolumeScale);
		}
		if (m_moveTransitionEffect)
		{
			m_transitionEffect.transform.position = vehicle.Position;
		}
		m_particleSystem.Play();
	}

	private IEnumerator PlayDestruction()
	{
		ParticleSystem particleSystem = m_transitionEffect.GetComponent<ParticleSystem>();
		particleSystem.Play();
		yield return YieldCache.WaitForSeconds(0.1f);
		m_intactVisuals.SetActiveRecursively(false);
		m_destroyedVisuals.SetActiveRecursively(true);
	}
}
