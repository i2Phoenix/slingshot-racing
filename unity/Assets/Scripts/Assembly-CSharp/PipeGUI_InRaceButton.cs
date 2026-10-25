using System;
using UnityEngine;

public class PipeGUI_InRaceButton : MonoBehaviour
{
	private GameObject m_raceControl;

	private RaceManager m_raceManager;

	private bool m_editMode;

	private int m_numUpgrades;

	private Vehicle.Upgrade m_upgradeType;

	private Vehicle.IUpgrade m_upgrade;

	private float m_charge;

	public SpriteText m_label;

	public SpriteText m_chargeText;

	public Vehicle.Upgrade m_defaultUpgradeType;

	private void Start()
	{
		m_raceControl = GameObject.FindGameObjectWithTag("RaceControl");
		if (m_raceControl != null)
		{
			m_raceManager = m_raceControl.GetComponent<RaceManager>();
			if (m_raceManager != null)
			{
				m_raceManager.OnRaceStartCountdownEvent += OnRaceStartCountdown;
				m_raceManager.OnRacePrepared += OnRacePrepared;
				m_raceManager.OnRaceCompleteEvent += OnRaceComplete;
			}
		}
		m_editMode = true;
		m_numUpgrades = Enum.GetValues(typeof(Vehicle.Upgrade)).Length;
		OnChangeUpgradeType();
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceStartCountdownEvent -= OnRaceStartCountdown;
			m_raceManager.OnRacePrepared -= OnRacePrepared;
			m_raceManager.OnRaceCompleteEvent -= OnRaceComplete;
		}
	}

	private void FixedUpdate()
	{
		if (m_upgrade == null)
		{
			return;
		}
		if (m_upgrade.Expended)
		{
			base.gameObject.SetActiveRecursively(false);
			m_upgrade = null;
			if (m_raceManager != null)
			{
				m_raceManager.BroadcastMessage("Event_ConfigureCutout", TouchInputManager.Cutout.Disabled);
			}
		}
		else
		{
			float charge = m_upgrade.Charge;
			if (charge != m_charge)
			{
				m_charge = charge;
				m_chargeText.Text = ((int)(charge * 100f)).ToString();
			}
		}
	}

	private void OnRacePrepared()
	{
		if (m_raceManager.CurrentRace.NoUpgrades)
		{
			base.gameObject.SetActiveRecursively(false);
		}
	}

	private void OnRaceStartCountdown(object _sender, EventArgs _args)
	{
		m_editMode = false;
		m_upgrade = null;
		m_charge = -1f;
		if (!m_raceManager.CurrentRace.NoUpgrades)
		{
			Vehicle vehicle = m_raceManager.GetVehicle(0);
			if (vehicle != null)
			{
				m_upgrade = vehicle.SetUpgrade(0, m_upgradeType);
			}
			if (m_upgrade != null && m_raceManager != null)
			{
				m_raceManager.BroadcastMessage("Event_ConfigureCutout", (!Settings.RaceButtonOnRight) ? TouchInputManager.Cutout.BottomLeft : TouchInputManager.Cutout.BottomRight);
			}
		}
		base.gameObject.SetActiveRecursively(m_upgrade != null);
	}

	private void OnRaceComplete(object _sender, EventArgs _args)
	{
		m_upgrade = null;
		base.gameObject.SetActiveRecursively(false);
	}

	private void OnButtonPressed()
	{
		if (m_editMode)
		{
			m_upgradeType++;
			if ((int)m_upgradeType >= m_numUpgrades)
			{
				m_upgradeType = Vehicle.Upgrade.None;
			}
			OnChangeUpgradeType();
		}
		else if (m_upgrade != null && m_upgrade.CanBeActivated)
		{
			m_upgrade.Activate();
		}
	}

	private void OnChangeUpgradeType()
	{
		m_label.Text = m_upgradeType.ToString();
	}
}
