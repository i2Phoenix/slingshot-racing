using UnityEngine;

[AddComponentMenu("Vehicle/Repulsor Upgrade")]
[RequireComponent(typeof(Collider))]
public class RepulsorUpgrade : MonoBehaviour, Vehicle.IUpgrade
{
	private Collider m_collider;

	private PhysicsVehicle m_physicsVehicle;

	private ParticleSystem m_particleSystem;

	private float m_charge = 1f;

	private float m_activatedTime;

	public bool m_manualActivation;

	public float m_force = 100f;

	public float m_chargeRate = 0.1f;

	public float m_activatedDuration = 3f;

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
			return m_charge >= 1f && m_manualActivation && !m_physicsVehicle.HandbrakeOn;
		}
	}

	public float Charge
	{
		get
		{
			return m_charge;
		}
	}

	private void Awake()
	{
		m_collider = base.collider;
		m_collider.enabled = !m_manualActivation;
		m_particleSystem = GetComponentInChildren<ParticleSystem>();
		if (m_particleSystem != null)
		{
			m_particleSystem.gameObject.SetActiveRecursively(!m_manualActivation);
		}
	}

	public void Initialise(PhysicsVehicle _physicsVehicle)
	{
		m_physicsVehicle = _physicsVehicle;
	}

	private void Update()
	{
		if (!m_manualActivation)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		m_charge = Mathf.Min(1f, m_charge + deltaTime * m_chargeRate);
		if (!(m_activatedTime > 0f))
		{
			return;
		}
		m_activatedTime -= deltaTime;
		if (m_activatedTime <= 0f)
		{
			m_activatedTime = 0f;
			m_collider.enabled = false;
			if (m_particleSystem != null)
			{
				m_particleSystem.gameObject.SetActiveRecursively(false);
			}
		}
	}

	public void Activate()
	{
		if (m_manualActivation)
		{
			m_activatedTime = m_activatedDuration;
			m_charge = 0f;
			m_collider.enabled = true;
			if (m_particleSystem != null)
			{
				m_particleSystem.gameObject.SetActiveRecursively(true);
			}
		}
	}

	private void OnTriggerEnter(Collider _collider)
	{
		DoRepulsion(_collider);
	}

	private void OnTriggerExit(Collider _collider)
	{
		DoRepulsion(_collider);
	}

	private void OnTriggerStay(Collider _collider)
	{
		DoRepulsion(_collider);
	}

	private void DoRepulsion(Collider _collider)
	{
		if (!(_collider.attachedRigidbody == null))
		{
			PhysicsVehicle component = _collider.attachedRigidbody.GetComponent<PhysicsVehicle>();
			if (component != null && component != m_physicsVehicle)
			{
				Vector3 vector = component.Position - m_physicsVehicle.Position;
				Vector3 forward = m_physicsVehicle.Forward;
				Vector3 normalized = (vector - Vector3.Project(vector, forward)).normalized;
				component.RigidBody.AddForce(normalized * m_force);
			}
		}
	}
}
