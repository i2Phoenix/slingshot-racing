using System;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleController : MonoBehaviour
{
	private GlobalAI m_globalAI;

	private PhysicsVehicle m_vehicle;

	private bool m_suppressControls;

	public TouchRegion touch;

	public string buttonName;

	public bool SuppressControls
	{
		get
		{
			return m_suppressControls;
		}
		set
		{
			m_suppressControls = value;
		}
	}

	public event EventHandler OnButtonDownEvent;

	public event EventHandler OnButtonUpEvent;

	private void Awake()
	{
		touch = new TouchRegion(new Rect(0f, 0f, 1f, 1f));
		m_vehicle = base.gameObject.GetComponent<PhysicsVehicle>();
	}

	private void Start()
	{
		m_globalAI = GlobalAI.Get();
		if (m_globalAI != null)
		{
			m_globalAI.RegisterHuman(m_vehicle);
		}
	}

	private void OnDestroy()
	{
		if (m_globalAI != null)
		{
			m_globalAI.UnregisterHuman(m_vehicle);
		}
	}

	private void Update()
	{
		if (touch.GetTouchDown() || Input.GetButtonDown(buttonName))
		{
			if (!m_suppressControls)
			{
				m_vehicle.ButtonDown();
			}
			if (OnButtonDownEvent != null)
			{
				OnButtonDownEvent(this, EventArgs.Empty);
			}
		}
		if (touch.GetTouchUp() || Input.GetButtonUp(buttonName))
		{
			if (!m_suppressControls)
			{
				m_vehicle.ButtonUp();
			}
			if (OnButtonUpEvent != null)
			{
				OnButtonUpEvent(this, EventArgs.Empty);
			}
		}
	}
}
