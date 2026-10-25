using UnityEngine;

[AddComponentMenu("Vehicle/Harpoon Upgrade")]
public class HarpoonUpgrade : MonoBehaviour, Vehicle.IUpgrade
{
	private RaceManager m_raceManager;

	private PhysicsVehicle m_physicsVehicle;

	private PhysicsVehicle m_otherPhysicsVehicle;

	private LineMesh m_lineMesh;

	private GameObject m_lineMeshGO;

	private float m_duration;

	private float m_charge = 1f;

	public float m_radius = 7.5f;

	public float m_refPointDistance = 5f;

	public float m_rotateDistance = 7f;

	public float m_chargeRate = 0.25f;

	public float m_maxHarpoonDuration = 1f;

	public float m_harpoonAcceleration = 20f;

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

	private void Awake()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
	}

	private void OnDestroy()
	{
		if (m_lineMesh != null)
		{
			m_lineMesh.Destroy();
		}
	}

	public void Initialise(PhysicsVehicle _physicsVehicle)
	{
		m_physicsVehicle = _physicsVehicle;
		if (m_physicsVehicle.Visuals != null)
		{
			m_lineMesh = LineMesh.Create(m_physicsVehicle.Position, m_physicsVehicle.Visuals.m_grappleUVRect, Color.white, 2);
			m_lineMesh.GetComponent<Renderer>().sharedMaterial = (Material)Resources.Load("Materials/TrailGrapple");
			m_lineMeshGO = m_lineMesh.gameObject;
		}
	}

	private void LateUpdate()
	{
		if (m_otherPhysicsVehicle != null)
		{
			m_lineMesh.SetLine(m_otherPhysicsVehicle.Visuals.Position, m_physicsVehicle.Visuals.Position, 0.15f, m_physicsVehicle.Visuals.CoarseLighting);
		}
	}

	private void FixedUpdate()
	{
		float deltaTime = Time.deltaTime;
		m_charge += deltaTime * m_chargeRate;
		if (m_charge >= 1f)
		{
			m_charge = 1f;
		}
		if (!(m_otherPhysicsVehicle != null))
		{
			return;
		}
		if (m_duration < m_maxHarpoonDuration)
		{
			m_duration += deltaTime;
			Vector3 vector = m_otherPhysicsVehicle.Position - m_physicsVehicle.Position;
			float num = Vector3.Dot(m_physicsVehicle.TrackTangent, vector);
			if (num < 0f)
			{
				if (m_lineMesh != null)
				{
					m_lineMeshGO.active = false;
				}
				m_otherPhysicsVehicle = null;
				m_duration = 0f;
				return;
			}
			Vector3 vector2;
			if (num > m_rotateDistance)
			{
				vector2 = vector.normalized;
			}
			else
			{
				vector2 = Vector3.Cross(vector, m_physicsVehicle.Up).normalized;
				if (Vector3.Dot(m_physicsVehicle.TrackNormal, vector) < 0f)
				{
					vector2 = -vector2;
				}
			}
			m_physicsVehicle.RigidBody.AddForce(vector2 * m_harpoonAcceleration, ForceMode.Acceleration);
			m_otherPhysicsVehicle.RigidBody.AddForce(vector2 * (0f - m_harpoonAcceleration), ForceMode.Acceleration);
		}
		else
		{
			if (m_lineMesh != null)
			{
				m_lineMeshGO.active = false;
			}
			m_otherPhysicsVehicle = null;
			m_duration = 0f;
		}
	}

	public void Activate()
	{
		if (m_raceManager == null)
		{
			return;
		}
		Vector3 vector = m_physicsVehicle.Position + m_refPointDistance * m_physicsVehicle.TrackTangent;
		PhysicsVehicle physicsVehicle = null;
		float num = m_radius;
		for (int i = 0; i < m_raceManager.NumRacers; i++)
		{
			Vehicle vehicle = m_raceManager.GetVehicle(i);
			if (vehicle == null)
			{
				continue;
			}
			PhysicsVehicle component = vehicle.GetComponent<PhysicsVehicle>();
			if (!(component == null) && !(component == m_physicsVehicle) && !(Vector3.Dot(m_physicsVehicle.Forward, component.Position - m_physicsVehicle.Position) < 0f))
			{
				float magnitude = (component.Position - vector).magnitude;
				if (magnitude < num)
				{
					num = magnitude;
					physicsVehicle = component;
				}
			}
		}
		if (physicsVehicle != null)
		{
			m_otherPhysicsVehicle = physicsVehicle;
			m_duration = 0f;
			m_charge = 0f;
			if (m_lineMesh != null)
			{
				m_lineMeshGO.active = true;
			}
		}
	}
}
