using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/Title Screen GUI")]
public class TitleScreenGUI : MonoBehaviour
{
	private GameManager m_gameManager;

	private SocialManager m_socialManager;

	private TurnBasedMatchManager m_turnBasedMatchManager;

	private bool m_ignoreInput;

	public GameObject m_raceControlPrefab;

	public RaceCampaign m_singlePlayerCampaign;

	public RaceCampaign m_singlePlayerBonusCampaign;

	public RaceCampaign m_freePlayCampaign;

	public RaceCampaign m_turnBasedCampaign;

	public RaceCampaign m_singlePlayerCampaignDev;

	public RaceCampaign m_freePlayCampaignDev;

	public UIBistateInteractivePanel m_singlePlayerPanel;

	public UIBistateInteractivePanel m_freeplayPanel;

	public FrontendVehicleList m_vehicleList;

	public GameObject m_turnsAvailable;

	public SpriteText m_turnsAvailableText;

	public GameObject m_frameRateDisplayPrefab;

	public static bool SuppressFPS { get; set; }

	private void Awake()
	{
		base.gameObject.AddComponent<ModernTitleLayout>();
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject == null)
		{
			gameObject = Object.Instantiate(m_raceControlPrefab) as GameObject;
		}
		GameManager component = gameObject.GetComponent<GameManager>();
		m_gameManager = component;
		m_turnBasedMatchManager = gameObject.GetComponent<TurnBasedMatchManager>();
		if (m_turnBasedMatchManager != null)
		{
			m_turnBasedMatchManager.OnGotoMatchEvent += GotoTurnBasedMatch;
			m_turnBasedMatchManager.OnGotoMatchFailedEvent += GotoTurnBasedMatchFailed;
			m_turnBasedMatchManager.OnCountedAvailableTurns += OnCountedAvailableTurns;
		}
		m_socialManager = gameObject.GetComponent<SocialManager>();
		GameCenterManager.playerAuthenticated += OnPlayerAuthenticated;
	}

	private IEnumerator Start()
	{
		yield return null;
		m_turnsAvailable.SetActiveRecursively(false);
		if (m_turnBasedMatchManager == null || !m_turnBasedMatchManager.IsSupported)
		{
			Transform child = m_freeplayPanel.transform.Find("TurnBased");
			if (child != null)
			{
				Object.Destroy(child.gameObject);
			}
			Object.Destroy(m_turnsAvailable);
			m_turnsAvailable = null;
			m_turnsAvailableText = null;
		}
		else if (m_socialManager.IsPlayerAuthenticated)
		{
			OnPlayerAuthenticated();
		}
		AdManager.OnEnterState(AdManager.State.TitleScreen);
	}

	private void OnDestroy()
	{
		if (m_turnBasedMatchManager != null)
		{
			m_turnBasedMatchManager.OnGotoMatchEvent -= GotoTurnBasedMatch;
			m_turnBasedMatchManager.OnGotoMatchFailedEvent -= GotoTurnBasedMatchFailed;
			m_turnBasedMatchManager.OnCountedAvailableTurns -= OnCountedAvailableTurns;
		}
		GameCenterManager.playerAuthenticated -= OnPlayerAuthenticated;
	}

	private void OnApplicationPause(bool _pause)
	{
		if (!_pause)
		{
			if (m_turnsAvailable != null)
			{
				m_turnsAvailable.SetActiveRecursively(false);
			}
			if (m_socialManager != null && m_socialManager.IsPlayerAuthenticated)
			{
				OnPlayerAuthenticated();
			}
		}
	}

	private void OnPlayerAuthenticated()
	{
		if (m_turnBasedMatchManager != null && m_turnBasedMatchManager.IsSupported)
		{
			m_turnBasedMatchManager.CountAvailableTurns();
		}
	}

	private void OnCountedAvailableTurns(int _count)
	{
		if (m_turnsAvailable != null)
		{
			m_turnsAvailable.SetActiveRecursively(_count > 0);
			if (_count > 0)
			{
				m_turnsAvailableText.Text = _count.ToString();
			}
		}
	}

	private void ToggleSinglePanel()
	{
		m_singlePlayerPanel.ToggleState();
	}

	private void OnSinglePlayerSelected()
	{
		ToggleSinglePanel();
	}

	private void OnStandardSinglePlayerSelected()
	{
		ToggleSinglePanel();
		CampaignSelected((!Settings.DevFreeplay) ? m_singlePlayerCampaign : m_singlePlayerCampaignDev);
	}

	private void OnBonusSinglePlayerSelected()
	{
		ToggleSinglePanel();
		CampaignSelected(m_singlePlayerBonusCampaign);
	}

	private void ToggleFreeplayPanel()
	{
	}

	private void OnFreePlaySelected()
	{
		OnLocalFreeplaySelected();
	}

	private void OnLocalFreeplaySelected()
	{
		ToggleFreeplayPanel();
		RaceCampaign campaign = ((!Settings.DevFreeplay) ? m_freePlayCampaign : m_freePlayCampaignDev);
		CampaignSelected(campaign);
	}

	private void OnTurnBasedFreeplaySelected()
	{
		ToggleFreeplayPanel();
		CampaignSelected(m_turnBasedCampaign);
	}

	private void CampaignSelected(RaceCampaign _campaign)
	{
		if (!(m_gameManager != null) || m_ignoreInput)
		{
			return;
		}
		if (_campaign.m_type == RaceCampaign.Type.TurnBased)
		{
			if (m_turnBasedMatchManager != null)
			{
				m_ignoreInput = true;
				m_turnBasedMatchManager.FindMatch();
			}
		}
		else
		{
			m_ignoreInput = true;
			m_gameManager.OnCampaignSelected(_campaign);
			AdManager.OnLeaveState(AdManager.State.TitleScreen);
		}
	}

	private void OnOptionsSelected()
	{
		if (m_gameManager != null && !m_ignoreInput)
		{
			m_ignoreInput = true;
			m_gameManager.OnOptionsSelected();
			AdManager.OnLeaveState(AdManager.State.TitleScreen);
		}
	}

	private void GotoTurnBasedMatch()
	{
		if (m_gameManager != null)
		{
			m_gameManager.OnCampaignSelected(m_turnBasedCampaign);
			AdManager.OnLeaveState(AdManager.State.TitleScreen);
		}
		else
		{
			m_ignoreInput = false;
		}
	}

	private void GotoTurnBasedMatchFailed()
	{
		m_ignoreInput = false;
	}
}
