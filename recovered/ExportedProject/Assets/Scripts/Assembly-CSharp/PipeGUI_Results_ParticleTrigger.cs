using System.Collections;
using UnityEngine;

public class PipeGUI_Results_ParticleTrigger : MonoBehaviour
{
	private RaceManager m_raceManager;

	private GameObject m_deleter;

	public GameObject m_effectPrefab1;

	public GameObject m_effectPrefab2;

	public GameObject m_effectPrefab3;

	public GameObject m_streamersPrefab;

	private void Start()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnDisplayResults += OnDisplayResults;
			m_raceManager.OnRacePrepared += OnRacePrepared;
		}
		m_deleter = new GameObject();
		m_deleter.name = "ParticleDeleter";
		m_deleter.transform.parent = base.transform;
		m_deleter.transform.localPosition = Vector3.zero;
		m_deleter.transform.localRotation = Quaternion.identity;
		m_deleter.transform.localScale = Vector3.one;
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnDisplayResults -= OnDisplayResults;
			m_raceManager.OnRacePrepared -= OnRacePrepared;
		}
	}

	private void OnDisplayResults(RaceManager.DisplayResults _display)
	{
		if (_display == RaceManager.DisplayResults.Medals)
		{
			base.gameObject.active = true;
			StartCoroutine(OnDisplayParticles());
		}
		else
		{
			StopAllCoroutines();
			GameObjectUtils.DestroyAllChildren(m_deleter);
		}
	}

	private void OnRacePrepared()
	{
		GameObjectUtils.DestroyAllChildren(m_deleter);
	}

	private IEnumerator OnDisplayParticles()
	{
		yield return YieldCache.WaitForSeconds(0.75f);
		FireParticles();
	}

	public void TriggerForCapture()
	{
		StopAllCoroutines();
		GameObjectUtils.DestroyAllChildren(m_deleter);
		FireParticles();
	}

	private void FireParticles()
	{
		if (!(m_raceManager == null) && (m_raceManager.CurrentChallenge == null || m_raceManager.CompletedChallenge))
		{
			StartCoroutine(StartEffect());
		}
	}

	protected IEnumerator StartEffect()
	{
		yield return null;
		Vector3 pos = base.transform.position;
		float time1 = 0.1f;
		Vector3 pos2 = pos;
		GameObject effectGO1 = null;
		bool firstFrame1 = true;
		bool finished1 = false;
		float time2 = 0.1f;
		Vector3 pos3 = pos;
		Quaternion rotation2 = Quaternion.Euler(-90f, 0f, 0f);
		pos3.z -= 12f;
		GameObject effectGO2 = null;
		bool firstFrame2 = true;
		bool finished2 = false;
		Vector3 pos4 = pos;
		pos4.z--;
		GameObject effectGO3 = Object.Instantiate(m_effectPrefab3, pos4, Quaternion.identity) as GameObject;
		effectGO3.transform.parent = m_deleter.transform;
		bool firstFrame3 = true;
		bool finished3 = false;
		float time4 = 0.1f;
		float endTime4 = 2f;
		Vector3 pos5 = pos;
		Quaternion rotation4 = Quaternion.identity;
		GameObject effectGO4 = null;
		bool finished4 = ((m_streamersPrefab == null) ? true : false);
		float timeSpent = 0f;
		while (!finished1 || !finished2 || !finished3)
		{
			timeSpent += Time.deltaTime;
			if (timeSpent >= time1 && effectGO1 == null && !finished1)
			{
				effectGO1 = Object.Instantiate(m_effectPrefab1, pos2, Quaternion.identity) as GameObject;
				effectGO1.transform.parent = m_deleter.transform;
			}
			if (timeSpent >= time2 && effectGO2 == null && !finished2)
			{
				effectGO2 = Object.Instantiate(m_effectPrefab2, pos3, rotation2) as GameObject;
				effectGO2.transform.parent = m_deleter.transform;
				ParticleSystem particleSystem = effectGO2.GetComponent<ParticleSystem>();
				particleSystem.Emit(20);
			}
			if (timeSpent >= time4 && effectGO4 == null && !finished4)
			{
				effectGO4 = Object.Instantiate(m_streamersPrefab, pos5, rotation4) as GameObject;
				effectGO4.transform.parent = m_deleter.transform;
			}
			if (effectGO1 != null)
			{
				if (!firstFrame1)
				{
					ParticleEmitter emitter = effectGO1.GetComponent<ParticleEmitter>();
					if (emitter.particleCount <= 0)
					{
						Object.Destroy(effectGO1);
						finished1 = true;
					}
				}
				if (firstFrame1)
				{
					firstFrame1 = false;
				}
			}
			if (effectGO2 != null)
			{
				if (!firstFrame2)
				{
					ParticleSystem particleSystem2 = effectGO2.GetComponent<ParticleSystem>();
					if (particleSystem2.particleCount <= 0)
					{
						Object.Destroy(effectGO2);
						finished2 = true;
					}
				}
				if (firstFrame2)
				{
					firstFrame2 = false;
				}
			}
			if (effectGO3 != null)
			{
				if (!firstFrame3)
				{
					ParticleEmitter emitter2 = effectGO3.GetComponent<ParticleEmitter>();
					if (emitter2.particleCount <= 0)
					{
						Object.Destroy(effectGO3);
						finished3 = true;
					}
				}
				if (firstFrame3)
				{
					firstFrame3 = false;
				}
			}
			if (effectGO4 != null && timeSpent >= endTime4)
			{
				finished4 = true;
			}
			yield return null;
		}
		yield return YieldCache.WaitForHalfSecond;
		GameObjectUtils.DestroyAllChildren(m_deleter);
	}
}
