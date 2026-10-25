using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleStats : MonoBehaviour
{
	private PhysicsVehicle m_vehicle;

	private int m_trackLayer;

	private int m_numOtherCollisions;

	private int m_numOtherCollisionsThisLap;

	private int m_numVehicleCollisions;

	private int m_numVehicleCollisionsThisLap;

	private int m_numBoostsHit;

	private int m_numBoostsHitThisLap;

	private int m_numGrapplesFired;

	private int m_numGrapplesFiredThisLap;

	private int m_grapplePointsUsed;

	private int m_grapplePointsUsedThisLap;

	private int m_numOilSlicksHit;

	private int m_numOilSlicksHitThisLap;

	private float m_distanceTravelled;

	private float m_distanceTravelledThisLap;

	private float m_distanceTravelledInRumbleArea;

	private float m_distanceTravelledInRumbleAreaThisLap;

	private float m_maxSpeed;

	private float m_maxSpeedThisLap;

	public int NumOtherCollisions
	{
		get
		{
			return m_numOtherCollisions + m_numOtherCollisionsThisLap;
		}
	}

	public int NumOtherCollisionsThisLap
	{
		get
		{
			return m_numOtherCollisionsThisLap;
		}
	}

	public int NumVehicleCollisions
	{
		get
		{
			return m_numVehicleCollisions + m_numVehicleCollisionsThisLap;
		}
	}

	public int NumVehicleCollisionsThisLap
	{
		get
		{
			return m_numVehicleCollisionsThisLap;
		}
	}

	public int NumBoostsHit
	{
		get
		{
			return m_numBoostsHit + m_numBoostsHitThisLap;
		}
	}

	public int NumBoostsHitThisLap
	{
		get
		{
			return m_numBoostsHitThisLap;
		}
	}

	public int NumGrapplesFired
	{
		get
		{
			return m_numGrapplesFired + m_numGrapplesFiredThisLap;
		}
	}

	public int NumGrapplesFiredThisLap
	{
		get
		{
			return m_numGrapplesFiredThisLap;
		}
	}

	public int GrapplePointsUsedMask
	{
		get
		{
			return m_grapplePointsUsed | m_grapplePointsUsedThisLap;
		}
	}

	public int GrapplePointsUsedMaskThisLap
	{
		get
		{
			return m_grapplePointsUsedThisLap;
		}
	}

	public int NumGrapplePointsUsed
	{
		get
		{
			return BitCount(GrapplePointsUsedMask);
		}
	}

	public int NumGrapplePointsUsedThisLap
	{
		get
		{
			return BitCount(GrapplePointsUsedMaskThisLap);
		}
	}

	public int NumOilSlicksHit
	{
		get
		{
			return m_numOilSlicksHit + m_numOilSlicksHitThisLap;
		}
	}

	public int NumOilSlicksHitThisLap
	{
		get
		{
			return m_numOilSlicksHitThisLap;
		}
	}

	public float DistanceTravelled
	{
		get
		{
			return m_distanceTravelled + m_distanceTravelledThisLap;
		}
	}

	public float DistanceTravelledThisLap
	{
		get
		{
			return m_distanceTravelledThisLap;
		}
	}

	public float DistanceTravelledInRumbleArea
	{
		get
		{
			return m_distanceTravelledInRumbleArea + m_distanceTravelledInRumbleAreaThisLap;
		}
	}

	public float DistanceTravelledInRumbleAreaThisLap
	{
		get
		{
			return m_distanceTravelledInRumbleAreaThisLap;
		}
	}

	public float MaxSpeed
	{
		get
		{
			return m_maxSpeed + m_maxSpeedThisLap;
		}
	}

	public float MaxSpeedThisLap
	{
		get
		{
			return m_maxSpeedThisLap;
		}
	}

	private static int BitCount(int n)
	{
		int num = 0;
		while (n != 0)
		{
			num++;
			n &= n - 1;
		}
		return num;
	}

	private void Awake()
	{
		m_vehicle = base.gameObject.GetComponent<PhysicsVehicle>();
		m_vehicle.OnMovementEvent += OnMovement;
		m_vehicle.OnNewLapEvent += OnNewLap;
		m_vehicle.OnBoostEvent += OnBoost;
		m_vehicle.OnGrappleEvent += OnGrapple;
		m_vehicle.OnOilSlickEvent += OnOilSlick;
		m_trackLayer = LayerMask.NameToLayer("Track");
	}

	private void OnDestroy()
	{
		PhysicsVehicle component = base.gameObject.GetComponent<PhysicsVehicle>();
		if ((bool)component)
		{
			component.OnMovementEvent -= OnMovement;
			component.OnNewLapEvent -= OnNewLap;
			component.OnBoostEvent -= OnBoost;
			component.OnGrappleEvent -= OnGrapple;
			component.OnOilSlickEvent -= OnOilSlick;
		}
	}

	private void OnMovement(Vector3 _oldPos, Vector3 _newPos, float _time)
	{
		float magnitude = (_newPos - _oldPos).magnitude;
		m_distanceTravelledThisLap += magnitude;
		if (m_vehicle.InRumbleArea)
		{
			m_distanceTravelledInRumbleAreaThisLap += magnitude;
		}
		m_maxSpeedThisLap = Mathf.Max(m_maxSpeed, magnitude / _time);
	}

	private void OnGrapple(GrapplePoint _grapplePoint)
	{
		m_numGrapplesFiredThisLap++;
		m_grapplePointsUsedThisLap |= 1 << _grapplePoint.GlobalIndex;
	}

	private void OnBoost()
	{
		m_numBoostsHitThisLap++;
	}

	private void OnCollisionEnter(Collision _collision)
	{
		GameObject gameObject = _collision.gameObject;
		if (gameObject.GetComponent<Vehicle>() != null)
		{
			m_numVehicleCollisionsThisLap++;
		}
		else if (gameObject.layer != m_trackLayer)
		{
			m_numOtherCollisionsThisLap++;
		}
	}

	private void OnNewLap()
	{
		m_numOtherCollisions += m_numOtherCollisionsThisLap;
		m_numOtherCollisionsThisLap = 0;
		m_numVehicleCollisions += m_numVehicleCollisionsThisLap;
		m_numVehicleCollisionsThisLap = 0;
		m_numBoostsHit += m_numBoostsHitThisLap;
		m_numBoostsHitThisLap = 0;
		m_numGrapplesFired += m_numGrapplesFiredThisLap;
		m_numGrapplesFiredThisLap = 0;
		m_grapplePointsUsed |= m_grapplePointsUsedThisLap;
		m_grapplePointsUsedThisLap = 0;
		m_numOilSlicksHit += m_numOilSlicksHitThisLap;
		m_numOilSlicksHitThisLap = 0;
		m_distanceTravelled += m_distanceTravelledThisLap;
		m_distanceTravelledThisLap = 0f;
		m_distanceTravelledInRumbleArea += m_distanceTravelledInRumbleAreaThisLap;
		m_distanceTravelledInRumbleAreaThisLap = 0f;
		m_maxSpeed = Mathf.Max(m_maxSpeed, m_maxSpeedThisLap);
		m_maxSpeedThisLap = 0f;
	}

	private void OnOilSlick(OilSlick _oilSlick)
	{
		m_numOilSlicksHitThisLap++;
	}
}
