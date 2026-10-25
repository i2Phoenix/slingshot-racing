using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/PipeGUI/Tour Plate Widget")]
public class PipeGUI_TourPlateWidget : PipeGUI_PlateWidget
{
	public delegate void OnTourSelected(RaceTour _Tour, RaceCampaignSave.RaceTourSaveData _saveData);

	public new delegate void OnInformationSelected(RaceTour _Tour, RaceCampaignSave.RaceTourSaveData _saveData);

	private RaceTour m_tour;

	private RaceCampaignSave.RaceTourSaveData m_saveData;

	public PackedSprite m_completed;

	public PackedSprite m_identifier;

	public PackedSprite m_newTour;

	public SpriteText m_label;

	public SpriteText m_unlockPoints;

	public SpriteText m_debugText;

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
			return m_saveData != null && (m_saveData.Locked || m_saveData.NewUnlock);
		}
	}

	protected override string PlateID
	{
		get
		{
			return "Plate" + m_tour.m_major;
		}
	}

	public event OnTourSelected OnTourSelectedEvent;

	public event OnInformationSelected OnInformationSelectedEvent;

	public void SetTourData(RaceTour _tour, RaceCampaignSave.RaceTourSaveData _saveData)
	{
		m_tour = _tour;
		m_saveData = _saveData;
		Refresh();
	}

	public bool NeedsFanfare(RaceCampaignSave.RaceTourSaveData _newSaveData)
	{
		if (_newSaveData.NewUnlock)
		{
			return true;
		}
		return false;
	}

	public IEnumerator PlayFanfare(RaceCampaignSave.RaceTourSaveData _newSaveData)
	{
		FanfareData data = PrepareFanfare();
		m_saveData = _newSaveData;
		m_saveData.NewUnlock = false;
		yield return StartCoroutine(PlayFanfare(data));
	}

	protected override void Refresh()
	{
		base.Refresh();
		if (m_completed != null)
		{
			bool activeRecursively = m_saveData == null || m_saveData.Award == RaceCampaignSave.AwardType.Gold;
			m_completed.gameObject.SetActiveRecursively(activeRecursively);
		}
		if (m_identifier != null)
		{
			if (m_tour.m_major > 0 && m_tour.m_minor > 0)
			{
				m_identifier.gameObject.SetActiveRecursively(true);
				UVAnimation anim = m_identifier.GetAnim(string.Format("{0}_{1}", m_tour.m_major, m_tour.m_minor));
				if (anim != null)
				{
					m_identifier.PlayAnim(anim);
					m_identifier.defaultAnim = anim.index;
				}
			}
			else
			{
				m_identifier.gameObject.SetActiveRecursively(false);
			}
		}
		if (m_label != null)
		{
			m_label.Text = m_tour.DisplayName.ToUpper();
		}
		if (m_unlockPoints != null && Locked)
		{
			m_unlockPoints.Text = m_tour.m_pointsToUnlock.ToString();
		}
		if (m_newTour != null)
		{
			if (m_saveData != null && !m_saveData.Locked && !m_saveData.Played)
			{
				m_newTour.gameObject.SetActiveRecursively(true);
			}
			else
			{
				m_newTour.gameObject.SetActiveRecursively(false);
			}
		}
		if (!(m_debugText != null))
		{
			return;
		}
		if (Settings.DevFreeplay)
		{
			m_debugText.gameObject.active = true;
			string text = string.Empty;
			RaceEvent[] races = m_tour.m_races;
			foreach (RaceEvent raceEvent in races)
			{
				text = text + raceEvent.name + "\n";
			}
			m_debugText.Text = text;
		}
		else
		{
			m_debugText.gameObject.active = false;
		}
	}

	protected override void OnSelectedImpl()
	{
		if (OnTourSelectedEvent != null)
		{
			OnTourSelectedEvent(m_tour, m_saveData);
		}
		base.OnSelectedImpl();
	}

	protected override void OnSnappedImpl()
	{
		base.OnSnappedImpl();
	}

	protected override void OnInformationSelectedImpl()
	{
		base.OnInformationSelectedImpl();
		if (OnInformationSelectedEvent != null)
		{
			OnInformationSelectedEvent(m_tour, m_saveData);
		}
	}
}
