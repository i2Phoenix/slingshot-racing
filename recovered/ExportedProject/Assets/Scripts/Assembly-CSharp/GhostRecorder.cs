using System.IO;
using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
	public const int c_initialBufferSize = 65536;

	private Rigidbody m_rigidbody;

	private Vehicle m_vehicle;

	private bool m_record;

	private int m_numFrames;

	private BitOutputStream m_bitStream;

	private GhostEncoder m_ghostEncoder;

	private bool m_onLapEvent;

	private bool m_onBoostEvent;

	private bool m_onDeployMiniReward;

	private bool m_onDropMiniReward;

	private bool m_onCollectMiniReward;

	private bool m_onDeathEvent;

	private bool m_onShieldEvent;

	private int m_onPickupIndex = -1;

	private int m_onObstacleHitIndex = -1;

	private float m_lapTime;

	private uint m_lastBitfield;

	private GhostSerialisation.VehicleCollisionParams[] m_vehicleCollisionParams = new GhostSerialisation.VehicleCollisionParams[4];

	private int m_numVehicleCollisions;

	private bool m_onTrackCollision;

	private bool m_onSurroundCollision;

	private bool m_onSurroundCollision_IsMajor;

	public string m_saveFile = "TestRecording.bytes";

	public int m_saveAfter;

	private void Awake()
	{
		m_rigidbody = base.rigidbody;
		m_vehicle = GetComponent<Vehicle>();
		m_vehicle.OnLapEvent += OnLapEvent;
		m_vehicle.OnBoostEvent += OnBoostEvent;
		m_vehicle.OnDeployMiniReward += OnDeployMiniReward;
		m_vehicle.OnDropMiniReward += OnDropMiniReward;
		m_vehicle.OnCollectMiniReward += OnCollectMiniReward;
		m_vehicle.OnPickupEvent += OnPickup;
		m_vehicle.OnObstacleHitEvent += OnObstacleHit;
		m_vehicle.OnSurroundCollisionEvent += OnSurroundCollision;
		m_vehicle.OnTrackCollisionEvent += OnTrackCollision;
		m_vehicle.OnVehicleCollisionEvent += OnVehicleCollision;
		m_vehicle.OnDeathEvent += OnDeathEvent;
		m_vehicle.OnShieldEvent += OnShield;
	}

	private void OnDestroy()
	{
		m_vehicle.OnLapEvent -= OnLapEvent;
		m_vehicle.OnBoostEvent -= OnBoostEvent;
		m_vehicle.OnDeployMiniReward -= OnDeployMiniReward;
		m_vehicle.OnDropMiniReward -= OnDropMiniReward;
		m_vehicle.OnCollectMiniReward -= OnCollectMiniReward;
		m_vehicle.OnPickupEvent -= OnPickup;
		m_vehicle.OnObstacleHitEvent -= OnObstacleHit;
		m_vehicle.OnSurroundCollisionEvent -= OnSurroundCollision;
		m_vehicle.OnTrackCollisionEvent -= OnTrackCollision;
		m_vehicle.OnVehicleCollisionEvent -= OnVehicleCollision;
		m_vehicle.OnDeathEvent -= OnDeathEvent;
		m_vehicle.OnShieldEvent -= OnShield;
	}

	private void Start()
	{
		if (m_saveAfter > 0)
		{
			BeginRecord();
		}
	}

	public void BeginRecord()
	{
		m_record = true;
		m_numFrames = 0;
		m_lastBitfield = 0u;
		m_onPickupIndex = -1;
		m_onObstacleHitIndex = -1;
		m_bitStream = new BitOutputStream(16384);
		m_bitStream.Write(8065u, 13u);
		m_ghostEncoder.Reset();
	}

	public void EndRecordIn(float _time)
	{
		Invoke("EndRecord", _time);
	}

	public void EndRecord()
	{
		m_record = false;
	}

	public byte[] ToByteArray()
	{
		return (m_bitStream == null) ? null : m_bitStream.ToByteArray();
	}

	private void FixedUpdate()
	{
		if (!m_record)
		{
			return;
		}
		GhostSerialisation.Bitfields bitfields = default(GhostSerialisation.Bitfields);
		GrapplePoint grapplePoint = m_vehicle.GrapplePoint;
		bitfields.grapplePointIndex = ((!(grapplePoint != null)) ? (-1) : grapplePoint.Index);
		bitfields.trackSplineIndex = GhostSerialisation.ToTrackSplineIndex(m_vehicle.CurrentTrackSpline, m_vehicle.CurrentTrackHub);
		bitfields.waypoint = m_vehicle.LastWaypoint;
		bitfields.onLapEvent = m_onLapEvent;
		bitfields.onBoostEvent = m_onBoostEvent;
		bitfields.onDeployMiniReward = m_onDeployMiniReward;
		bitfields.onDropMiniReward = m_onDropMiniReward;
		bitfields.onCollectMiniReward = m_onCollectMiniReward;
		bitfields.onPickupEvent = m_onPickupIndex >= 0;
		bitfields.onObstacleHit = m_onObstacleHitIndex >= 0;
		bitfields.onSurroundCollision = m_onSurroundCollision;
		bitfields.onTrackCollision = m_onTrackCollision;
		bitfields.onVehicleCollision = m_numVehicleCollisions > 0;
		bitfields.onDeathEvent = m_onDeathEvent;
		bitfields.onShieldEvent = m_onShieldEvent;
		bool flag = m_lastBitfield != bitfields.packed;
		m_bitStream.Write(flag);
		if (flag)
		{
			m_bitStream.Write(bitfields.packed, 23u);
			m_lastBitfield = bitfields.packed;
		}
		if (m_onLapEvent)
		{
			m_onLapEvent = false;
			m_bitStream.Write(m_lapTime);
		}
		if (m_onPickupIndex >= 0)
		{
			m_bitStream.Write(m_onPickupIndex, 8u);
			m_onPickupIndex = -1;
		}
		if (m_onObstacleHitIndex >= 0)
		{
			m_bitStream.Write(m_onObstacleHitIndex, 8u);
			m_onObstacleHitIndex = -1;
		}
		if (m_onSurroundCollision)
		{
			m_bitStream.Write(m_onSurroundCollision_IsMajor);
			m_onSurroundCollision = false;
			m_onSurroundCollision_IsMajor = false;
		}
		if (m_numVehicleCollisions > 0)
		{
			m_bitStream.Write((uint)(m_numVehicleCollisions - 1), 2u);
			for (int i = 0; i != m_numVehicleCollisions; i++)
			{
				m_bitStream.Write(m_vehicleCollisionParams[i].point.x);
				m_bitStream.Write(m_vehicleCollisionParams[i].point.y);
				m_bitStream.Write(m_vehicleCollisionParams[i].point.z);
				m_bitStream.Write(m_vehicleCollisionParams[i].normal.x);
				m_bitStream.Write(m_vehicleCollisionParams[i].normal.y);
				m_bitStream.Write(m_vehicleCollisionParams[i].normal.z);
				m_bitStream.Write(m_vehicleCollisionParams[i].velocity.x);
				m_bitStream.Write(m_vehicleCollisionParams[i].velocity.y);
				m_bitStream.Write(m_vehicleCollisionParams[i].velocity.z);
				m_bitStream.Write(m_vehicleCollisionParams[i].isMajor);
				m_bitStream.Write(m_vehicleCollisionParams[i].playAudio);
			}
			m_numVehicleCollisions = 0;
		}
		m_onBoostEvent = false;
		m_onDeployMiniReward = false;
		m_onDropMiniReward = false;
		m_onCollectMiniReward = false;
		m_onTrackCollision = false;
		m_onDeathEvent = false;
		m_onShieldEvent = false;
		bool flag2 = (m_numFrames & 1) == 0;
		if (flag2 && flag2)
		{
			m_ghostEncoder.WriteRotation(m_bitStream, m_rigidbody.rotation);
			m_ghostEncoder.WritePosition(m_bitStream, m_rigidbody.position);
			if (grapplePoint != null)
			{
				m_ghostEncoder.WriteGrappleForce(m_bitStream, m_vehicle.GrappleForce);
			}
			else
			{
				m_ghostEncoder.ResetGrappleForce();
			}
		}
		m_numFrames++;
		if (m_saveAfter <= 0 || --m_saveAfter != 0)
		{
			return;
		}
		EndRecord();
		using (FileStream fileStream = File.Open(m_saveFile, FileMode.OpenOrCreate, FileAccess.Write))
		{
			byte[] array = ToByteArray();
			fileStream.Write(array, 0, array.Length);
		}
	}

	private void OnLapEvent(Vehicle _vehicle, float _lapTime)
	{
		m_lapTime = _lapTime;
		m_onLapEvent = true;
	}

	private void OnBoostEvent()
	{
		m_onBoostEvent = true;
	}

	private void OnDeployMiniReward(bool _depleteAll)
	{
		m_onDeployMiniReward = true;
		m_onDropMiniReward = _depleteAll;
	}

	private void OnDropMiniReward()
	{
		m_onDropMiniReward = true;
	}

	private void OnCollectMiniReward()
	{
		m_onCollectMiniReward = true;
	}

	private void OnPickup(int _index)
	{
		m_onPickupIndex = _index;
	}

	private void OnObstacleHit(TrackObstacle _obstacle)
	{
		m_onObstacleHitIndex = _obstacle.Index;
	}

	private void OnVehicleCollision(Vector3 point, Vector3 normal, Vector3 velocity, bool isMajor, bool playAudio)
	{
		if (m_numVehicleCollisions < 4)
		{
			int num = m_numVehicleCollisions++;
			m_vehicleCollisionParams[num].point = point;
			m_vehicleCollisionParams[num].normal = normal;
			m_vehicleCollisionParams[num].velocity = velocity;
			m_vehicleCollisionParams[num].isMajor = isMajor;
			m_vehicleCollisionParams[num].playAudio = playAudio;
		}
	}

	private void OnSurroundCollision(bool isMajor)
	{
		m_onSurroundCollision = true;
		m_onSurroundCollision_IsMajor |= isMajor;
	}

	private void OnTrackCollision()
	{
		m_onTrackCollision = true;
	}

	private void OnDeathEvent(GameObject go)
	{
		m_onDeathEvent = true;
		FixedUpdate();
		FixedUpdate();
	}

	private void OnShield()
	{
		m_onShieldEvent = true;
	}
}
