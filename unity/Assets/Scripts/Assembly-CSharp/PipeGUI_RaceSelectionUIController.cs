using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeGUI_RaceSelectionUIController : MonoBehaviour
{
	private GameManager m_gameManager;

	private RaceCampaign m_campaign;

	private SocialManager m_socialManager;

	private bool m_showLeaderboardOnUpdate;

	public UIScrollList m_scrollList;

	public GameObject m_listItemPrefab;

	public SpriteText m_tourName;

	public RaceCampaign m_testCampaign;

	public RaceTour m_testTour;

	public bool m_testFanfare;

	private bool Standalone
	{
		get
		{
			return m_gameManager == null;
		}
	}

	private IEnumerator Start()
	{
		GameCenterManager.reportScoreFinished += OnReportScore;
		GPGManager.submitScoreSucceededEvent += OnSubmitScoreSucceeded;
		m_gameManager = GameObjectUtils.FindComponentWithTag<GameManager>("RaceControl");
		if (m_gameManager != null)
		{
			m_socialManager = m_gameManager.GetComponent<SocialManager>();
		}
		yield return null;
		if (Standalone && m_testCampaign != null)
		{
			RaceCampaignSave saveData = base.gameObject.AddComponent<RaceCampaignSave>();
			saveData.SetCampaign(m_testCampaign, null, !m_testFanfare);
			RaceCampaignSave.RaceTourSaveData tourSaveData = null;
			if (m_testTour != null)
			{
				tourSaveData = saveData.GetSaveData(m_testTour);
			}
			SetCampaign(m_testCampaign, saveData, m_testTour, tourSaveData);
			if (m_testFanfare && m_testTour != null)
			{
				PlayFanfare(tourSaveData, null, m_testTour);
			}
		}
	}

	private void OnDestroy()
	{
		if (m_campaign != null)
		{
			m_scrollList.RemoveItemSnappedDelegate(OnItemSnapped);
		}
		GameCenterManager.reportScoreFinished -= OnReportScore;
		GPGManager.submitScoreSucceededEvent -= OnSubmitScoreSucceeded;
	}

	public void SetCampaign(RaceCampaign _campaign, RaceCampaignSave _saveData, RaceTour _selected, RaceCampaignSave.RaceTourSaveData _selectedSaveData)
	{
		if (m_campaign != null)
		{
			m_scrollList.RemoveItemSnappedDelegate(OnItemSnapped);
		}
		m_campaign = _campaign;
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
		UIScrollList scrollList = m_scrollList;
		for (int k = 0; k < m_campaign.m_tours.Length; k++)
		{
			IUIListObject iUIListObject = scrollList.CreateItem(m_listItemPrefab);
			PipeGUI_RacePlateSelection component = iUIListObject.gameObject.GetComponent<PipeGUI_RacePlateSelection>();
			if (component != null)
			{
				RaceTour raceTour = m_campaign.m_tours[k];
				RaceCampaignSave.RaceTourSaveData raceTourSaveData = null;
				if (raceTour == _selected)
				{
					raceTourSaveData = _selectedSaveData;
				}
				if (raceTourSaveData == null)
				{
					raceTourSaveData = ((!(_saveData != null)) ? null : _saveData.GetSaveData(raceTour));
				}
				component.SetTour(m_campaign.m_tours[k], raceTourSaveData);
			}
		}
		m_scrollList.AddItemSnappedDelegate(OnItemSnapped);
		scrollList.ScrollToItem(index, 0f);
	}

	public void PlayFanfare(RaceCampaignSave.RaceTourSaveData _saveData, BusyIcon _busyIcon, RaceTour _unlockedTour)
	{
		StartCoroutine(PlayFanfareImpl(_saveData, _busyIcon, _unlockedTour));
	}

	private IEnumerator PlayFanfareImpl(RaceCampaignSave.RaceTourSaveData _saveData, BusyIcon _busyIcon, RaceTour _unlockedTour)
	{
		PipeGUI_RacePlateSelection widget = m_scrollList.SnappedItem.gameObject.GetComponent<PipeGUI_RacePlateSelection>();
		if ((bool)widget && widget.Tour == _saveData.Tour)
		{
			widget.m_testFanfare = m_testFanfare;
			m_scrollList.controlIsEnabled = false;
			yield return StartCoroutine(widget.PlayFanfareImpl(_saveData, _busyIcon, _unlockedTour));
			m_scrollList.controlIsEnabled = true;
		}
	}

	private void OnItemSnapped(IUIListObject _item)
	{
		PipeGUI_RacePlateSelection component = _item.gameObject.GetComponent<PipeGUI_RacePlateSelection>();
		if (m_tourName != null)
		{
			m_tourName.Text = component.Tour.DisplayName.ToUpper();
		}
		if (m_gameManager != null)
		{
			m_gameManager.UpdateTourAchievementsForTour(component.Tour);
			m_socialManager.FlushUpdates();
		}
	}

	private void OnBackSelected()
	{
		if (m_gameManager != null)
		{
			m_gameManager.GoBack();
			m_gameManager = null;
		}
	}

	private void OnJumpToTour()
	{
		if (m_gameManager != null)
		{
			m_gameManager.GotoUnlockedTour();
			m_gameManager = null;
		}
	}

	private void OnLeaderboardToggle()
	{
		if (!(PipeGUI_RacePlateSelection.Leaderboard == null) && !(m_socialManager == null))
		{
			if (m_socialManager.IsUpdating(PipeGUI_RacePlateSelection.Leaderboard))
			{
				m_showLeaderboardOnUpdate = true;
			}
			else
			{
				m_socialManager.ShowLeaderboard(PipeGUI_RacePlateSelection.Leaderboard);
			}
		}
	}

	private void OnReportScore(string _leaderboardID)
	{
		if (!(PipeGUI_RacePlateSelection.Leaderboard == null) && m_showLeaderboardOnUpdate && _leaderboardID == PipeGUI_RacePlateSelection.Leaderboard.ID)
		{
			m_showLeaderboardOnUpdate = false;
			if (!(m_socialManager == null))
			{
				m_socialManager.ShowLeaderboard(PipeGUI_RacePlateSelection.Leaderboard);
			}
		}
	}

	private void OnSubmitScoreSucceeded(string _leaderboardId, Dictionary<string, object> _scoreReport)
	{
		OnReportScore(_leaderboardId);
	}
}
