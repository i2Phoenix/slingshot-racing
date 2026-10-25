using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioPlayer))]
public class PipeGUI_StampEffect : MonoBehaviour
{
	private GameObject m_deleter;

	private EZTransition m_awardFanfare;

	private Vector3[] m_awardPositions;

	public TransitionStore m_transitions;

	public AudioBank m_awardFanfareAudioBank;

	public GameObject m_stampEffectPrefab1;

	public GameObject m_stampEffectPrefab2;

	public float m_stampAudioTime;

	public float m_stampParticleTime1;

	public float m_stampParticleTime2;

	private void Start()
	{
		if (m_transitions != null)
		{
			m_awardFanfare = m_transitions.FindTransition("AwardFanfare");
		}
		m_deleter = new GameObject();
		m_deleter.name = "Deleter";
		m_deleter.transform.parent = base.transform;
		m_deleter.transform.localPosition = Vector3.zero;
		m_deleter.transform.localRotation = Quaternion.identity;
		m_deleter.transform.localScale = Vector3.one;
	}

	public IEnumerator Play(GameObject _go)
	{
		_go.active = true;
		Transform goTransform = _go.transform;
		Vector3 originalPosition = goTransform.position;
		Vector3 fanfarePosition = originalPosition;
		fanfarePosition.z -= 3f;
		goTransform.position = fanfarePosition;
		m_awardFanfare.MainSubject = _go;
		m_awardFanfare.Start();
		StartCoroutine(StampEffect(goTransform));
		while (m_awardFanfare.IsRunning())
		{
			yield return null;
		}
		goTransform.position = originalPosition;
		m_awardFanfare.MainSubject = null;
		yield return YieldCache.WaitForSeconds(0.05f);
	}

	public void Stop()
	{
		StopAllCoroutines();
		if (m_awardFanfare != null)
		{
			if (m_awardFanfare.IsRunning())
			{
				m_awardFanfare.StopSafe();
			}
			m_awardFanfare.MainSubject = null;
			GameObjectUtils.DestroyAllChildren(m_deleter);
		}
	}

	protected IEnumerator StampEffect(Transform _transform)
	{
		GameObject stampEffectGO = null;
		GameObject stampEffect2GO = null;
		bool firstFrame = true;
		bool firstFrame2 = true;
		bool finished = false;
		bool finished2 = false;
		bool playedAudio = false;
		AudioPlayer ap = GetComponent<AudioPlayer>();
		float timeSpent = 0f;
		while (!finished || !finished2)
		{
			timeSpent += Time.deltaTime;
			if (timeSpent >= m_stampAudioTime && !playedAudio)
			{
				ap.PlayRandomGUIClip(m_awardFanfareAudioBank);
				playedAudio = true;
			}
			if (timeSpent >= m_stampParticleTime1 && stampEffectGO == null && !finished)
			{
				Vector3 stampEffectPos = _transform.position;
				stampEffectPos.z += 0.05f;
				stampEffectGO = Object.Instantiate(m_stampEffectPrefab1, stampEffectPos, Quaternion.identity) as GameObject;
				stampEffectGO.transform.parent = m_deleter.transform;
			}
			if (timeSpent >= m_stampParticleTime2 && stampEffect2GO == null && !finished2)
			{
				Vector3 stampEffectPos2 = _transform.position;
				stampEffectPos2.z += 0.05f;
				stampEffect2GO = Object.Instantiate(m_stampEffectPrefab2, stampEffectPos2, Quaternion.identity) as GameObject;
				stampEffect2GO.transform.parent = m_deleter.transform;
			}
			if (stampEffectGO != null)
			{
				if (!firstFrame)
				{
					ParticleEmitter emitter = stampEffectGO.GetComponent<ParticleEmitter>();
					emitter.emit = false;
					Vector3 stampEffectPos3 = _transform.position;
					stampEffectPos3.z += 0.15f;
					stampEffectGO.transform.position = stampEffectPos3;
					if (emitter.particleCount <= 0)
					{
						Object.Destroy(stampEffectGO);
						finished = true;
					}
				}
				if (firstFrame)
				{
					firstFrame = false;
				}
			}
			if (stampEffect2GO != null)
			{
				if (!firstFrame2)
				{
					ParticleEmitter emitter2 = stampEffect2GO.GetComponent<ParticleEmitter>();
					emitter2.emit = false;
					Vector3 stampEffectPos4 = _transform.position;
					stampEffectPos4.z += 0.1f;
					stampEffect2GO.transform.position = stampEffectPos4;
					if (emitter2.particleCount <= 0)
					{
						Object.Destroy(stampEffect2GO);
						finished2 = true;
					}
				}
				if (firstFrame2)
				{
					firstFrame2 = false;
				}
			}
			yield return null;
		}
		yield return null;
	}
}
