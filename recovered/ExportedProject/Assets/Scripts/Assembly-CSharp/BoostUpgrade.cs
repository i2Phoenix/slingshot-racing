using UnityEngine;

[AddComponentMenu("Vehicle/Boost Upgrade")]
public class BoostUpgrade : MonoBehaviour, Vehicle.IUpgrade
{
	private PhysicsVehicle m_physicsVehicle;

	private float m_charge = 1f;

	public float m_impluse = 12f;

	public float m_chargeRate = 0.25f;

	public bool Expended
	{
		get
		{
			return false;
		}
	}

	public bool CanBeActivated
	{
		get
		{
			return m_charge >= 1f && !m_physicsVehicle.HandbrakeOn;
		}
	}

	public float Charge
	{
		get
		{
			return m_charge;
		}
	}

	public void Initialise(PhysicsVehicle _physicsVehicle)
	{
		m_physicsVehicle = _physicsVehicle;
	}

	private void FixedUpdate()
	{
		float deltaTime = Time.deltaTime;
		m_charge += deltaTime * m_chargeRate;
		if (m_charge >= 1f)
		{
			m_charge = 1f;
			base.enabled = false;
		}
	}

	public void Activate()
	{
		if (m_physicsVehicle.TriggerBoost(m_impluse))
		{
			m_charge = 0f;
			base.enabled = true;
		}
	}
}
