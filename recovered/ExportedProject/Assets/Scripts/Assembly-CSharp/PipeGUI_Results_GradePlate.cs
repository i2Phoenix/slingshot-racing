using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioPlayer))]
public class PipeGUI_Results_GradePlate : MonoBehaviour
{
	private RaceManager m_raceManager;

	private RaceCampaignSave.AwardType m_award;

	private PipeGUI_StampEffect m_stampEffect;

	private Vector3[] m_awardPositions;

	private bool m_inFanfare;

	private Renderer[] m_challengeSuccess;

	private Renderer[] m_challengeFailure;

	public PackedSprite[] m_awards;

	public PackedSprite m_position;

	public GameObject m_stampEffectPrefab;

	public GameObject m_notifyOnFanfareComplete;

	public GameObject m_defaultGUI;

	public GameObject m_challengesGUI;

	private void Awake()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnFinalisedRaceStats += OnFinalisedRaceStats;
			m_raceManager.OnSaveUpdatedEvent += OnSaveUpdatedEvent;
			m_raceManager.OnDisplayResults += OnDisplayResults;
		}
		m_awardPositions = new Vector3[m_awards.Length];
		for (int i = 0; i < m_awards.Length; i++)
		{
			m_awardPositions[i] = m_awards[i].transform.localPosition;
		}
		if (m_stampEffectPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(m_stampEffectPrefab) as GameObject;
			gameObject.transform.parent = base.transform;
			m_stampEffect = gameObject.GetComponent<PipeGUI_StampEffect>();
		}
	}

	private void Start()
	{
		if (m_raceManager != null)
		{
			GameCenterChallenge currentChallenge = m_raceManager.CurrentChallenge;
			if (currentChallenge != null && currentChallenge.score != null)
			{
				Object.Destroy(m_defaultGUI);
				m_defaultGUI = null;
				m_challengeSuccess = m_challengesGUI.transform.FindChild("Success").GetComponentsInChildren<Renderer>();
				m_challengeFailure = m_challengesGUI.transform.FindChild("Failure").GetComponentsInChildren<Renderer>();
			}
			else
			{
				Object.Destroy(m_challengesGUI);
				m_challengesGUI = null;
			}
		}
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnFinalisedRaceStats -= OnFinalisedRaceStats;
			m_raceManager.OnSaveUpdatedEvent -= OnSaveUpdatedEvent;
			m_raceManager.OnDisplayResults -= OnDisplayResults;
		}
	}

	private void OnFinalisedRaceStats(RaceStats _raceStats)
	{
		GameCenterChallenge currentChallenge = m_raceManager.CurrentChallenge;
		m_award = _raceStats.m_award;
		if (m_awards != null)
		{
			PackedSprite[] awards = m_awards;
			foreach (PackedSprite packedSprite in awards)
			{
				packedSprite.renderer.enabled = false;
			}
		}
		if (currentChallenge == null && m_position != null)
		{
			if (_raceStats.Race.AwardPointsType == RaceEvent.PointsType.Position)
			{
				m_position.gameObject.active = true;
				m_position.renderer.enabled = true;
				m_position.PlayAnim(_raceStats.m_finishPosition - 1);
				m_position.defaultAnim = _raceStats.m_finishPosition - 1;
			}
			else
			{
				m_position.renderer.enabled = false;
			}
		}
		if (currentChallenge != null)
		{
			bool completedChallenge = m_raceManager.CompletedChallenge;
			Renderer[] challengeSuccess = m_challengeSuccess;
			foreach (Renderer renderer in challengeSuccess)
			{
				renderer.enabled = completedChallenge;
			}
			Renderer[] challengeFailure = m_challengeFailure;
			foreach (Renderer renderer2 in challengeFailure)
			{
				renderer2.enabled = !completedChallenge;
			}
		}
	}

	private void OnSaveUpdatedEvent(object _sender, RaceManager.SaveUpdatedArgs _args)
	{
	}

	private void OnDisplayResults(RaceManager.DisplayResults _display)
	{
		if (_display == RaceManager.DisplayResults.Medals)
		{
			if (m_stampEffect != null)
			{
				base.gameObject.active = true;
				StartCoroutine(PlayFanfare());
			}
			return;
		}
		StopAllCoroutines();
		if (m_stampEffect != null && m_inFanfare)
		{
			m_stampEffect.Stop();
		}
		int award = (int)m_award;
		for (int i = 0; i < m_awards.Length; i++)
		{
			bool active = award > i;
			m_awards[i].gameObject.active = active;
			m_awards[i].transform.localPosition = m_awardPositions[i];
		}
	}

	private IEnumerator PlayFanfare()
	{
		m_inFanfare = true;
		yield return YieldCache.WaitForOneSecond;
		m_stampEffect.gameObject.active = true;
		int awardType = (int)m_award;
		for (int index = 0; index < m_awards.Length; index++)
		{
			if (awardType > index)
			{
				m_awards[index].renderer.enabled = true;
				yield return StartCoroutine(m_stampEffect.Play(m_awards[index].gameObject));
			}
		}
		m_inFanfare = false;
		if (m_notifyOnFanfareComplete != null)
		{
			m_notifyOnFanfareComplete.active = true;
			m_notifyOnFanfareComplete.BroadcastMessage("OnFanfareComplete");
		}
	}
}
