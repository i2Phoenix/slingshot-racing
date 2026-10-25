using UnityEngine;

public class SaveManager : MonoBehaviour, RaceCampaignSave.IManager
{
	private const string c_saveManagerName = "__SaveManager";

	private const string c_saveManagerFind = "/__SaveManager";

	private MapList<string, RaceCampaignSave> m_saves;

	private bool m_isDeleting;

	public RaceCampaign[] m_campaigns;

	public RaceCampaignSave[] Saves
	{
		get
		{
			RaceCampaignSave[] array = new RaceCampaignSave[m_saves.Count];
			for (int i = 0; i < m_saves.Count; i++)
			{
				array[i] = m_saves.GetValueByIndex(i);
			}
			return array;
		}
	}

	private RaceCampaignSave MainSave { get; set; }

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		base.gameObject.name = "__SaveManager";
		m_saves = new MapList<string, RaceCampaignSave>();
		for (int i = 0; i < m_campaigns.Length; i++)
		{
			GameObject gameObject = new GameObject(m_campaigns[i].name);
			gameObject.transform.parent = base.transform;
			RaceCampaignSave raceCampaignSave = gameObject.AddComponent<RaceCampaignSave>();
			if (i == 0)
			{
				MainSave = raceCampaignSave;
			}
			raceCampaignSave.SetCampaign(m_campaigns[i], this, true);
			m_saves.Add(m_campaigns[i].name, raceCampaignSave);
		}
		foreach (MapList<string, RaceCampaignSave>.Item safe in m_saves)
		{
			safe.Value.UpdateSaveDataManaged(true);
		}
	}

	private void Start()
	{
		GlobalSave.StartCloudDownload();
		for (int i = 0; i < m_campaigns.Length; i++)
		{
			RaceCampaignSave.StartCloudDownloadForCampaign(m_campaigns[i]);
		}
	}

	public RaceCampaignSave GetSave(RaceCampaign _campaign)
	{
		RaceCampaignSave o_value = null;
		m_saves.TryGetValue(_campaign.name, out o_value);
		return o_value;
	}

	public void UpdateTotalNuts(long _numNuts)
	{
		RaceCampaignSave mainSave = MainSave;
		if (mainSave.UpdateTotalNuts(_numNuts) && mainSave.Campaign.m_totalNutsLeaderboard != null)
		{
			SocialManager socialManager = GameObjectUtils.FindComponentWithTag<SocialManager>("RaceControl");
			if (socialManager != null)
			{
				socialManager.UpdateScore(mainSave.Campaign.m_totalNutsLeaderboard, mainSave.TotalNuts, false);
			}
		}
	}

	public void OnUpdateSaveData(RaceCampaignSave _save)
	{
		if (m_isDeleting)
		{
			return;
		}
		foreach (MapList<string, RaceCampaignSave>.Item safe in m_saves)
		{
			if (safe.Value != _save)
			{
				safe.Value.UpdateSaveDataManaged(true);
			}
		}
	}

	public int GetTotalPointsScored(RaceCampaignSave _save)
	{
		if (_save.Campaign.m_type == RaceCampaign.Type.SinglePlayer)
		{
			int num = 0;
			{
				foreach (MapList<string, RaceCampaignSave>.Item safe in m_saves)
				{
					if (safe.Value.Campaign.m_type == RaceCampaign.Type.SinglePlayer)
					{
						num += safe.Value.PointsScored;
					}
				}
				return num;
			}
		}
		return _save.PointsScored;
	}

	public int GetTotalPointsAvailable(RaceCampaignSave _save)
	{
		if (_save.Campaign.m_type == RaceCampaign.Type.SinglePlayer)
		{
			int num = 0;
			{
				foreach (MapList<string, RaceCampaignSave>.Item safe in m_saves)
				{
					if (safe.Value.Campaign.m_type == RaceCampaign.Type.SinglePlayer)
					{
						num += safe.Value.PointsAvailable;
					}
				}
				return num;
			}
		}
		return _save.PointsAvailable;
	}

	public void OnAccessAllAreas()
	{
		foreach (MapList<string, RaceCampaignSave>.Item safe in m_saves)
		{
			if (safe.Value.Campaign.m_type == RaceCampaign.Type.SinglePlayer)
			{
				safe.Value.LocksDisabled = true;
				safe.Value.UpdateSaveDataManaged(true);
			}
		}
	}

	public void OnRevokeAccessAllAreas()
	{
		foreach (MapList<string, RaceCampaignSave>.Item safe in m_saves)
		{
			if (safe.Value.Campaign.m_type == RaceCampaign.Type.SinglePlayer)
			{
				safe.Value.LocksDisabled = false;
				safe.Value.UpdateSaveDataManaged(true);
			}
		}
	}

	public void ResetSaveData()
	{
		m_isDeleting = true;
		for (int i = 0; i < m_campaigns.Length; i++)
		{
			RaceCampaignSave.DeleteSaveForCampaign(m_campaigns[i]);
		}
		foreach (MapList<string, RaceCampaignSave>.Item safe in m_saves)
		{
			safe.Value.Reload();
		}
		m_isDeleting = false;
	}

	public static SaveManager Find()
	{
		GameObject gameObject = GameObject.Find("/__SaveManager");
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent<SaveManager>();
	}
}
