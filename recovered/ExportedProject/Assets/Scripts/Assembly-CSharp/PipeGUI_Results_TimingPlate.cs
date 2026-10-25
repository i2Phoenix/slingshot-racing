using System;
using System.Collections;
using UnityEngine;

public class PipeGUI_Results_TimingPlate : MonoBehaviour
{
	[Serializable]
	public class Timing
	{
		private Vector3 m_improvedPos;

		public SpriteText m_text;

		public GameObject m_improved;

		public float Time
		{
			set
			{
				if (IsDeltaTime)
				{
					m_text.Text = string.Format("{0}{1:0.000}", (!(value < 0f)) ? "+" : "-", Mathf.Abs(value));
				}
				else if (value > 0f)
				{
					TimeSpan timeSpan = TimeSpan.FromSeconds(value);
					m_text.Text = string.Format("{0:D1}:{1:D2}.{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
				}
				else
				{
					m_text.Text = "--:--.---";
				}
			}
		}

		public bool Improved { get; set; }

		public bool IsDeltaTime { get; set; }

		public void UpdateImprovedGUI(bool _forceOff)
		{
			m_improved.transform.localPosition = m_improvedPos;
			m_improved.renderer.enabled = !_forceOff && Improved;
		}

		public void Start()
		{
			m_improvedPos = m_improved.transform.localPosition;
		}
	}

	private RaceManager m_raceManager;

	private PipeGUI_StampEffect m_stampEffect;

	private Timing[] m_timings;

	private bool m_inFanfare;

	public Timing m_totalTimeGUI;

	public Timing m_bestLapTimeGUI;

	public GameObject m_stampEffectPrefab;

	public GameObject m_notifyOnFanfareComplete;

	public PackedSprite m_lapIcon;

	private bool UseBestLapTime
	{
		get
		{
			RaceEvent.PointsType awardPointsType = m_raceManager.CurrentRace.AwardPointsType;
			if (awardPointsType == RaceEvent.PointsType.Position || awardPointsType == RaceEvent.PointsType.BestLap)
			{
				return true;
			}
			return false;
		}
	}

	private void Start()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnFinalisedRaceStats += OnFinalisedRaceStats;
			m_raceManager.OnSaveUpdatedEvent += OnSaveUpdatedEvent;
			m_raceManager.OnDisplayResults += OnDisplayResults;
		}
		if (m_stampEffectPrefab != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_stampEffectPrefab) as GameObject;
			gameObject.transform.parent = base.transform;
			m_stampEffect = gameObject.GetComponent<PipeGUI_StampEffect>();
		}
		m_timings = new Timing[2];
		m_timings[0] = m_totalTimeGUI;
		m_timings[1] = m_bestLapTimeGUI;
		Timing[] timings = m_timings;
		foreach (Timing timing in timings)
		{
			timing.Start();
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
		m_totalTimeGUI.Time = _raceStats.m_raceTime;
		if (UseBestLapTime)
		{
			m_bestLapTimeGUI.IsDeltaTime = false;
			m_bestLapTimeGUI.Time = _raceStats.m_bestLapTime;
			return;
		}
		RaceEvent currentRace = m_raceManager.CurrentRace;
		float num = 0f;
		GameCenterChallenge currentChallenge = m_raceManager.CurrentChallenge;
		if (currentChallenge != null && currentChallenge.score != null)
		{
			num = (float)currentChallenge.score.value / 1000f;
		}
		else
		{
			switch (_raceStats.m_award)
			{
			case RaceCampaignSave.AwardType.None:
				num = currentRace.m_times[2];
				break;
			case RaceCampaignSave.AwardType.Bronze:
				num = currentRace.m_times[1];
				break;
			default:
				num = currentRace.m_times[0];
				break;
			}
		}
		float num2 = _raceStats.m_raceTime - num;
		if (currentRace.InvTotalTime)
		{
			num2 = 0f - num2;
		}
		m_bestLapTimeGUI.IsDeltaTime = true;
		m_bestLapTimeGUI.Time = num2;
	}

	private void OnSaveUpdatedEvent(object _sender, RaceManager.SaveUpdatedArgs _args)
	{
		bool useBestLapTime = UseBestLapTime;
		m_totalTimeGUI.Improved = _args.Improvements.ImprovedTotalTime;
		m_bestLapTimeGUI.Improved = ((!useBestLapTime) ? _args.Improvements.ImprovedTotalTime : _args.Improvements.ImprovedBestLap);
		int num = ((!useBestLapTime) ? 1 : 0);
		m_lapIcon.gameObject.active = true;
		m_lapIcon.PlayAnim(num);
		m_lapIcon.defaultAnim = num;
		Timing[] timings = m_timings;
		foreach (Timing timing in timings)
		{
			timing.UpdateImprovedGUI(true);
		}
	}

	private void OnDisplayResults(RaceManager.DisplayResults _display)
	{
		if (_display != RaceManager.DisplayResults.Medals)
		{
			StopAllCoroutines();
			if (m_stampEffect != null && m_inFanfare)
			{
				m_stampEffect.Stop();
			}
			Timing[] timings = m_timings;
			foreach (Timing timing in timings)
			{
				timing.UpdateImprovedGUI(false);
			}
		}
	}

	private void OnFanfareComplete()
	{
		if (m_stampEffect != null)
		{
			StartCoroutine(PlayFanfare());
		}
	}

	private IEnumerator PlayFanfare()
	{
		m_inFanfare = true;
		yield return YieldCache.WaitForSeconds(0.1f);
		m_stampEffect.gameObject.active = true;
		Timing[] timings = m_timings;
		foreach (Timing timing in timings)
		{
			if (timing.Improved)
			{
				timing.m_improved.renderer.enabled = true;
				yield return StartCoroutine(m_stampEffect.Play(timing.m_improved));
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
