using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/PipeGUI/Track Plate Widget")]
public class PipeGUI_TrackPlateWidget : PipeGUI_PlateWidget
{
	public delegate void OnRaceSelected(RaceEvent _race, RaceCampaignSave.RaceEventSaveData _saveData);

	public new delegate void OnInformationSelected(RaceEvent _race, RaceCampaignSave.RaceEventSaveData _saveData);

	private RaceEvent m_race;

	private RaceCampaignSave.RaceEventSaveData m_saveData;

	public PackedSprite m_raceType;

	public GameObject m_debugTextPrefab;

	protected override RaceCampaignSave.AwardType Award
	{
		get
		{
			return (m_saveData == null) ? RaceCampaignSave.AwardType.Platinum : m_saveData.Award;
		}
	}

	protected override bool Locked
	{
		get
		{
			return m_saveData != null && m_saveData.Locked;
		}
	}

	protected override string PlateID
	{
		get
		{
			return m_race.m_layout.ID;
		}
	}

	public event OnRaceSelected OnRaceSelectedEvent;

	public event OnInformationSelected OnInformationSelectedEvent;

	protected override void Start()
	{
		base.Start();
		// SetTour can run while the instantiated children are still receiving
		// Awake. Reapply once PackedSprite has built its atlas animations.
		if (m_race != null) Refresh();
	}

	public void SetRaceData(RaceEvent _race, RaceCampaignSave.RaceEventSaveData _saveData)
	{
		m_race = _race;
		m_saveData = _saveData;
		Refresh();
	}

	public bool NeedsFanfare(RaceCampaignSave.RaceEventSaveData _newSaveData)
	{
		if (_newSaveData.Award > m_saveData.Award || (!_newSaveData.Locked && m_saveData.Locked))
		{
			return true;
		}
		return false;
	}

	public IEnumerator PlayFanfare(RaceCampaignSave.RaceEventSaveData _newSaveData)
	{
		FanfareData data = PrepareFanfare();
		m_saveData = _newSaveData;
		yield return StartCoroutine(PlayFanfare(data));
	}

	protected override void Refresh()
	{
		base.Refresh();
		if (m_raceType != null)
		{
			UVAnimation anim = m_raceType.GetAnim(m_race.m_objective.ToString());
			if (anim != null)
			{
				m_raceType.PlayAnim(anim);
				m_raceType.defaultAnim = anim.index;
			}
			m_raceType.gameObject.active = !Locked;
		}
		if (Settings.DevFreeplay && m_debugTextPrefab != null)
		{
			Transform transform = base.transform.Find("DebugText");
			if (transform == null)
			{
				GameObject gameObject = Object.Instantiate(m_debugTextPrefab) as GameObject;
				gameObject.name = "DebugText";
				transform = gameObject.transform;
				transform.parent = base.transform;
				transform.localPosition = new Vector3(0f, 40f, -0.1f);
			}
			transform.GetComponent<SpriteText>().Text = m_race.name;
		}
	}

	protected override void OnSelectedImpl()
	{
		if (OnRaceSelectedEvent != null)
		{
			OnRaceSelectedEvent(m_race, m_saveData);
		}
		base.OnSelectedImpl();
	}

	protected override void OnInformationSelectedImpl()
	{
		base.OnInformationSelectedImpl();
		if (OnInformationSelectedEvent != null)
		{
			OnInformationSelectedEvent(m_race, m_saveData);
		}
	}

	protected override IEnumerator PlayUnlockFanfare()
	{
		yield return StartCoroutine(PlayUnlockFanfare(m_raceType.gameObject));
	}

	protected IEnumerator PlayUnlockFanfare(GameObject _gameObject)
	{
		_gameObject.active = true;
		Vector3 originalPosition = _gameObject.transform.localPosition;
		Vector3 fanfarePosition = originalPosition;
		fanfarePosition.z -= 3f;
		_gameObject.transform.localPosition = fanfarePosition;
		m_awardFanfare.MainSubject = _gameObject;
		m_awardFanfare.Start();
		Coroutine effects = StartCoroutine(StampEffect(_gameObject.transform));
		while (m_awardFanfare.IsRunning())
		{
			yield return null;
		}
		yield return effects;
		_gameObject.transform.localPosition = originalPosition;
		m_awardFanfare.MainSubject = null;
	}
}
