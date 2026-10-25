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

	private bool m_showPosition;

	private SpriteText m_localisedPosition;

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
#if DEVELOPMENT_BUILD || UNITY_EDITOR
		Debug.Log("QA_GRADE_AWAKE manager=" + (m_raceManager != null) + " object=" + name);
#endif
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
				m_challengeSuccess = m_challengesGUI.transform.Find("Success").GetComponentsInChildren<Renderer>();
				m_challengeFailure = m_challengesGUI.transform.Find("Failure").GetComponentsInChildren<Renderer>();
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
#if DEVELOPMENT_BUILD || UNITY_EDITOR
		Debug.Log("QA_GRADE_STATS position=" + _raceStats.m_finishPosition + " award=" + _raceStats.m_award);
#endif
		GameCenterChallenge currentChallenge = m_raceManager.CurrentChallenge;
		m_award = _raceStats.m_award;
		m_showPosition = currentChallenge == null && _raceStats.Race.AwardPointsType == RaceEvent.PointsType.Position;
		if (m_localisedPosition != null)
			m_localisedPosition.gameObject.SetActive(false);
		if (m_awards != null)
		{
			PackedSprite[] awards = m_awards;
			foreach (PackedSprite packedSprite in awards)
			{
				packedSprite.GetComponent<Renderer>().enabled = false;
			}
		}
		if (currentChallenge == null && m_position != null)
		{
			if (m_showPosition)
			{
				m_position.gameObject.active = true;
				m_position.GetComponent<Renderer>().enabled = true;
				m_position.PlayAnim(_raceStats.m_finishPosition - 1);
				m_position.defaultAnim = _raceStats.m_finishPosition - 1;
			}
			else
			{
				m_position.GetComponent<Renderer>().enabled = false;
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
#if DEVELOPMENT_BUILD || UNITY_EDITOR
		Debug.Log("QA_GRADE_DISPLAY display=" + _display + " sprite=" + ((m_position == null) ? "none" : "default=" + m_position.defaultAnim + " current=" + m_position.CurAnimIndex + " active=" + m_position.gameObject.activeInHierarchy));
#endif
		if (_display == RaceManager.DisplayResults.Medals)
		{
			// Result data can arrive while the Results panel is inactive. Restore
			// its ancestors before applying the saved finishing-position frame.
			for (Transform parent = transform; parent != null; parent = parent.parent)
				parent.gameObject.SetActive(true);
			if (m_position != null && m_showPosition)
			{
				// Plate/Default below this controller was also deactivated by EZ GUI.
				for (Transform parent = m_position.transform; parent != null; parent = parent.parent)
					parent.gameObject.SetActive(true);
				m_position.PlayAnim(m_position.defaultAnim);
				UpdatePositionLabel();
			}
			if (m_stampEffect != null)
			{
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

	private void UpdatePositionLabel()
	{
		// The original ordinal (1st, 2nd...) is baked into the sprite atlas.
		// Keep that artwork for existing languages; Russian uses the font atlas.
		Renderer original = m_position.GetComponent<Renderer>();
		bool useRussian = Language.CurrentLanguage() == LanguageCode.RU;
		original.enabled = !useRussian;
		if (!useRussian)
		{
			if (m_localisedPosition != null)
				m_localisedPosition.gameObject.SetActive(false);
			return;
		}
		if (m_localisedPosition == null)
		{
			GameObject label = new GameObject("LocalisedPosition");
			label.layer = m_position.gameObject.layer;
			label.transform.SetParent(m_position.transform, false);
			m_localisedPosition = label.AddComponent<SpriteText>();
			m_localisedPosition.anchor = SpriteText.Anchor_Pos.Middle_Center;
			m_localisedPosition.alignment = SpriteText.Alignment_Type.Center;
			m_localisedPosition.multiline = false;
			m_localisedPosition.SetFont(UIManager.instance.defaultFont, UIManager.instance.defaultFontMaterial);
			m_localisedPosition.SetCamera(m_position.renderCamera);
		}
		m_localisedPosition.gameObject.SetActive(true);
		m_localisedPosition.transform.localScale = Vector3.one;
		m_localisedPosition.SetCharacterSize(22f);
		m_localisedPosition.Text = Language.Get("ID_Position" + (m_position.defaultAnim + 1));
		// Use the sprite's actual mesh bounds: its atlas pivot includes an offset.
		Bounds bounds = m_position.GetComponent<MeshFilter>().sharedMesh.bounds;
		Bounds textBounds = m_localisedPosition.GetComponent<MeshFilter>().sharedMesh.bounds;
		float scale = Mathf.Min(1f, bounds.size.x / Mathf.Max(0.01f, textBounds.size.x));
		m_localisedPosition.transform.localScale = Vector3.one * scale;
		m_localisedPosition.transform.localPosition = bounds.center - textBounds.center * scale;
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
				m_awards[index].GetComponent<Renderer>().enabled = true;
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
