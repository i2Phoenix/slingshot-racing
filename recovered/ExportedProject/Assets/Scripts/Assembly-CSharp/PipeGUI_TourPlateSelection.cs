using System.Collections;
using UnityEngine;

public class PipeGUI_TourPlateSelection : MonoBehaviour
{
	private AudioSource m_audioSource;

	private GameManager m_gameManager;

	private RaceCampaign m_campaign;

	private RaceCampaignSave m_saveData;

	public GameObject m_tourList;

	public GameObject m_listItemPrefab;

	public AudioBank m_lockedAudioBank;

	public AudioBank m_selectedAudioBank;

	public PipeGUI_InstructorSpeech m_tourInfo;

	public PipeGUI_InstructorIAP m_IAP;

	public RaceCampaign m_testCampaign;

	public bool m_testFanfare;

	private bool Standalone
	{
		get
		{
			return m_gameManager == null;
		}
	}

	private bool EnableInput
	{
		set
		{
			UIScrollList component = m_tourList.GetComponent<UIScrollList>();
			for (int i = 0; i < component.Count; i++)
			{
				IUIListObject item = component.GetItem(i);
				PipeGUI_TourPlateWidget component2 = item.gameObject.GetComponent<PipeGUI_TourPlateWidget>();
				if (component2 != null)
				{
					component2.EnableInput = value;
				}
			}
			component.controlIsEnabled = value;
		}
	}

	private void Awake()
	{
		m_audioSource = base.gameObject.AddComponent<AudioSource>();
		m_audioSource.rolloffMode = AudioRolloffMode.Linear;
		m_audioSource.minDistance = 0f;
		m_audioSource.maxDistance = 200f;
	}

	private void Start()
	{
		m_gameManager = GameObjectUtils.FindComponentWithTag<GameManager>("RaceControl");
		if (Standalone && m_testCampaign != null)
		{
			RaceCampaignSave raceCampaignSave = base.gameObject.AddComponent<RaceCampaignSave>();
			raceCampaignSave.SetCampaign(m_testCampaign, null, !m_testFanfare);
			SetCampaign(m_testCampaign, raceCampaignSave, null);
			if (m_testFanfare)
			{
				PlayFanfare(raceCampaignSave, null);
			}
		}
	}

	public void SetCampaign(RaceCampaign _campaign, RaceCampaignSave _saveData, RaceTour _selected)
	{
		m_campaign = _campaign;
		m_saveData = _saveData;
		int index = 0;
		if (_selected != null)
		{
			for (int i = 0; i < m_campaign.m_tours.Length; i++)
			{
				if (m_campaign.m_tours[i] == _selected)
				{
					index = i;
					break;
				}
			}
		}
		else if (_saveData != null && _saveData.PointsScored > 0)
		{
			for (int j = 0; j < m_campaign.m_tours.Length; j++)
			{
				RaceCampaignSave.RaceTourSaveData saveData = _saveData.GetSaveData(m_campaign.m_tours[j]);
				if (!saveData.Locked)
				{
					index = j;
					if (!saveData.Played)
					{
						break;
					}
				}
			}
		}
		PipeGUI_PulseSync component = GetComponent<PipeGUI_PulseSync>();
		UIScrollList component2 = m_tourList.GetComponent<UIScrollList>();
		for (int k = 0; k < m_campaign.m_tours.Length; k++)
		{
			IUIListObject iUIListObject = component2.CreateItem(m_listItemPrefab);
			PipeGUI_TourPlateWidget component3 = iUIListObject.gameObject.GetComponent<PipeGUI_TourPlateWidget>();
			if (!(component3 != null))
			{
				continue;
			}
			RaceCampaignSave.RaceTourSaveData saveData2 = ((!(_saveData != null)) ? null : _saveData.GetSaveData(m_campaign.m_tours[k]));
			component3.SetTourData(m_campaign.m_tours[k], saveData2);
			component3.OnTourSelectedEvent += OnTourSelected;
			component3.OnInformationSelectedEvent += OnInformationSelected;
			component3.AutoScroll = false;
			if (!(component != null))
			{
				continue;
			}
			PipeGUI_Pulse[] componentsInChildren = component3.GetComponentsInChildren<PipeGUI_Pulse>();
			if (componentsInChildren != null)
			{
				PipeGUI_Pulse[] array = componentsInChildren;
				foreach (PipeGUI_Pulse pipeGUI_Pulse in array)
				{
					pipeGUI_Pulse.SetSync(component, k);
				}
			}
		}
		component2.ScrollToItem(index, 0f);
	}

	private void OnBackSelected()
	{
		if (m_gameManager != null)
		{
			m_gameManager.GoBack();
			m_gameManager = null;
		}
	}

	public void OnTourSelected(RaceTour _tour, RaceCampaignSave.RaceTourSaveData _saveData)
	{
		if (_saveData != null && _saveData.Locked)
		{
			if (m_lockedAudioBank != null)
			{
				m_lockedAudioBank.PlayRandomClip(m_audioSource);
			}
			StartCoroutine(DisplayLocked(_tour, _saveData));
			return;
		}
		if (m_selectedAudioBank != null)
		{
			m_selectedAudioBank.PlayRandomClip(m_audioSource);
		}
		if (m_gameManager != null)
		{
			StartCoroutine(RunTour(m_gameManager, _tour));
			m_gameManager = null;
		}
	}

	private IEnumerator RunTour(GameManager _gameManager, RaceTour _tour)
	{
		yield return YieldCache.WaitForHalfSecond;
		_gameManager.TourSelected(_tour);
	}

	public void OnInformationSelected(RaceTour _tour, RaceCampaignSave.RaceTourSaveData _saveData)
	{
		StartCoroutine(DisplayInformation(_tour, _saveData));
	}

	private IEnumerator DisplayInformation(RaceTour _tour, RaceCampaignSave.RaceTourSaveData _saveData)
	{
		EnableInput = false;
		yield return YieldCache.WaitForSeconds(0.25f);
		bool locked = _saveData != null && _saveData.Locked;
		string text = _tour.GetFullDescription(locked);
		yield return StartCoroutine(m_tourInfo.DisplayAndWait(text, Language.Get("ID_TapToClose")));
		EnableInput = true;
	}

	private IEnumerator DisplayLocked(RaceTour _tour, RaceCampaignSave.RaceTourSaveData _saveData)
	{
		EnableInput = false;
		yield return YieldCache.WaitForSeconds(0.25f);
		string messageForIAP = string.Format(Language.Get("ID_Description_LockedTourIAP"), _tour.m_pointsToUnlock);
		string messageForNoIAP = string.Format(Language.Get("ID_Description_LockedTour"), _tour.m_pointsToUnlock);
		yield return StartCoroutine(m_IAP.DisplayAndWait(messageForIAP, messageForNoIAP));
		EnableInput = true;
	}

	public void PlayFanfare(RaceCampaignSave _saveData, BusyIcon _busyIcon)
	{
		StartCoroutine(PlayFanfareImpl(_saveData, _busyIcon));
	}

	private IEnumerator PlayFanfareImpl(RaceCampaignSave _saveData, BusyIcon _busyIcon)
	{
		EnableInput = false;
		if (_busyIcon != null)
		{
			_busyIcon.ShowIcon();
		}
		yield return YieldCache.WaitForHalfSecond;
		UIScrollList list = m_tourList.GetComponent<UIScrollList>();
		for (int index = 0; index < list.Count; index++)
		{
			IUIListObject item = list.GetItem(index);
			PipeGUI_TourPlateWidget widget = item.gameObject.GetComponent<PipeGUI_TourPlateWidget>();
			if (!(widget != null))
			{
				continue;
			}
			RaceCampaignSave.RaceTourSaveData oldSaveData = ((!(m_saveData != null)) ? null : m_saveData.GetSaveData(m_campaign.m_tours[index]));
			RaceCampaignSave.RaceTourSaveData newSaveData = ((!(_saveData != null)) ? null : _saveData.GetSaveData(m_campaign.m_tours[index]));
			if (oldSaveData != null && newSaveData != null)
			{
				if (widget.NeedsFanfare(newSaveData) || m_testFanfare)
				{
					list.ScrollToItem(item, 1f);
					yield return YieldCache.WaitForOneSecond;
					yield return StartCoroutine(widget.PlayFanfare(newSaveData));
				}
				else
				{
					widget.SetTourData(newSaveData.Tour, newSaveData);
				}
			}
		}
		if (_busyIcon != null)
		{
			_busyIcon.HideIcon();
		}
		EnableInput = true;
	}
}
