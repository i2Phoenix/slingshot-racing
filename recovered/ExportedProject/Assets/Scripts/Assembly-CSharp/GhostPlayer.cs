using System;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
	private class PerFrameData
	{
		public GrapplePoint grapplePoint;

		public TrackSpline trackSpline;

		public TrackHub trackHub;

		public GhostSerialisation.Bitfields bitfields;

		public float lapTime;

		public int onPickupIndex;

		public int onObstacleHitIndex;

		public bool onSurroundCollision_IsMajor;

		public GhostSerialisation.VehicleCollisionParams[] vehicleCollisionParams = new GhostSerialisation.VehicleCollisionParams[4];

		public int numVehicleCollisions;
	}

	private GhostVehicle m_vehicle;

	private BitInputStream m_bitStream;

	private GhostEncoder m_ghostEncoder;

	private float m_rcpDeltaTime;

	private int m_numFrames;

	private int m_version;

	private Vector3 m_position0;

	private Vector3 m_position1;

	private Quaternion m_rotation0;

	private Quaternion m_rotation1;

	private float m_grappleForce0;

	private float m_grappleForce1;

	private Vector3 m_velocity;

	private PerFrameData m_nextFrameData;

	private PerFrameData m_currFrameData;

	public bool Play(byte[] data)
	{
		m_bitStream = new BitInputStream(data);
		uint num = m_bitStream.ReadUInt(13u);
		if ((num & 0x1F80) == 8064)
		{
			m_version = (int)num & -8065;
		}
		else
		{
			m_bitStream.Reset();
			m_version = 0;
		}
		m_numFrames = 0;
		m_currFrameData = new PerFrameData();
		m_nextFrameData = new PerFrameData();
		m_ghostEncoder.Reset();
		try
		{
			ReadPerFrameData();
			ReadSubSampledData(true);
			return true;
		}
		catch (IndexOutOfRangeException)
		{
			m_bitStream = null;
			return false;
		}
	}

	public void Stop()
	{
		m_vehicle.RepeatFrame();
		m_bitStream = null;
	}

	private void Start()
	{
		m_vehicle = GetComponent<GhostVehicle>();
		m_rcpDeltaTime = 0.5f / Time.fixedDeltaTime;
	}

	private void ReadPerFrameData()
	{
		PerFrameData currFrameData = m_currFrameData;
		m_currFrameData = m_nextFrameData;
		m_nextFrameData = currFrameData;
		if (m_bitStream.ReadBool())
		{
			m_nextFrameData.bitfields.packed = m_bitStream.ReadUInt(GhostSerialisation.Bitfields.c_numBitsToRead[m_version]);
			int grapplePointIndex = m_nextFrameData.bitfields.grapplePointIndex;
			m_nextFrameData.grapplePoint = ((grapplePointIndex < 0) ? null : IndexedMonoBehaviour<GrapplePoint>.All[grapplePointIndex]);
			GhostSerialisation.FromTrackSplineIndex(out m_nextFrameData.trackSpline, out m_nextFrameData.trackHub, m_nextFrameData.bitfields.trackSplineIndex);
		}
		else
		{
			m_nextFrameData.bitfields.packed = m_currFrameData.bitfields.packed;
			m_nextFrameData.grapplePoint = m_currFrameData.grapplePoint;
			m_nextFrameData.trackSpline = m_currFrameData.trackSpline;
			m_nextFrameData.trackHub = m_currFrameData.trackHub;
		}
		if (m_nextFrameData.bitfields.onLapEvent)
		{
			m_nextFrameData.lapTime = m_bitStream.ReadFloat();
		}
		if (m_nextFrameData.bitfields.onPickupEvent)
		{
			m_nextFrameData.onPickupIndex = m_bitStream.ReadInt(8u);
		}
		if (m_nextFrameData.bitfields.onObstacleHit)
		{
			m_nextFrameData.onObstacleHitIndex = m_bitStream.ReadInt(8u);
		}
		if (m_nextFrameData.bitfields.onSurroundCollision)
		{
			m_nextFrameData.onSurroundCollision_IsMajor = m_bitStream.ReadBool();
		}
		m_nextFrameData.numVehicleCollisions = 0;
		if (m_nextFrameData.bitfields.onVehicleCollision)
		{
			int num = (int)(m_bitStream.ReadUInt(2u) + 1);
			m_nextFrameData.numVehicleCollisions = num;
			for (int i = 0; i != num; i++)
			{
				m_nextFrameData.vehicleCollisionParams[i].point.x = m_bitStream.ReadFloat();
				m_nextFrameData.vehicleCollisionParams[i].point.y = m_bitStream.ReadFloat();
				m_nextFrameData.vehicleCollisionParams[i].point.z = m_bitStream.ReadFloat();
				m_nextFrameData.vehicleCollisionParams[i].normal.x = m_bitStream.ReadFloat();
				m_nextFrameData.vehicleCollisionParams[i].normal.y = m_bitStream.ReadFloat();
				m_nextFrameData.vehicleCollisionParams[i].normal.z = m_bitStream.ReadFloat();
				m_nextFrameData.vehicleCollisionParams[i].velocity.x = m_bitStream.ReadFloat();
				m_nextFrameData.vehicleCollisionParams[i].velocity.y = m_bitStream.ReadFloat();
				m_nextFrameData.vehicleCollisionParams[i].velocity.z = m_bitStream.ReadFloat();
				m_nextFrameData.vehicleCollisionParams[i].isMajor = m_bitStream.ReadBool();
				m_nextFrameData.vehicleCollisionParams[i].playAudio = m_bitStream.ReadBool();
			}
		}
	}

	private void ReadSubSampledData(bool _isFirst)
	{
		if (!_isFirst)
		{
			m_position0 = m_position1;
			m_rotation0 = m_rotation1;
			m_grappleForce0 = m_grappleForce1;
		}
		m_rotation1 = m_ghostEncoder.ReadRotation(m_bitStream);
		m_position1 = m_ghostEncoder.ReadPosition(m_bitStream);
		if (m_nextFrameData.grapplePoint != null)
		{
			m_grappleForce1 = m_ghostEncoder.ReadGrappleForce(m_bitStream);
		}
		else
		{
			m_ghostEncoder.ResetGrappleForce();
			m_grappleForce1 = 0f;
		}
		if (_isFirst)
		{
			m_position0 = m_position1;
			m_rotation0 = m_rotation1;
			m_grappleForce0 = m_grappleForce1;
		}
		m_velocity = (m_position1 - m_position0) * m_rcpDeltaTime;
	}

	private void FixedUpdate()
	{
		if (m_bitStream == null)
		{
			return;
		}
		try
		{
			ReadPerFrameData();
			if ((m_numFrames & 1) == 1)
			{
				ReadSubSampledData(false);
				m_vehicle.NewFrame(Vector3.Lerp(m_position0, m_position1, 0.5f), Quaternion.Lerp(m_rotation0, m_rotation1, 0.5f), m_velocity, m_currFrameData.grapplePoint, (!(m_currFrameData.grapplePoint != null)) ? 0f : Mathf.Lerp(m_grappleForce0, m_grappleForce1, 0.5f), m_currFrameData.trackSpline, m_currFrameData.trackHub, m_currFrameData.bitfields.waypoint, false);
			}
			else
			{
				m_vehicle.NewFrame(m_position1, m_rotation1, m_velocity, m_currFrameData.grapplePoint, m_grappleForce1, m_currFrameData.trackSpline, m_currFrameData.trackHub, m_currFrameData.bitfields.waypoint, m_numFrames == 0);
			}
			if (m_currFrameData.bitfields.onLapEvent)
			{
				m_vehicle.SendLapEvent(m_currFrameData.lapTime);
			}
			if (m_currFrameData.bitfields.onBoostEvent)
			{
				m_vehicle.SendBoostEvent();
			}
			if (m_currFrameData.bitfields.onCollectMiniReward)
			{
				m_vehicle.SendCollectMiniRewardEvent();
			}
			if (m_currFrameData.bitfields.onDeployMiniReward)
			{
				m_vehicle.SendDeployMiniRewardEvent(m_currFrameData.bitfields.onDropMiniReward);
			}
			else if (m_currFrameData.bitfields.onDropMiniReward)
			{
				m_vehicle.SendDropMiniRewardEvent();
			}
			if (m_currFrameData.bitfields.onPickupEvent)
			{
				PickupManager get = PickupManager.Get;
				if (get != null)
				{
					get.TriggerCollection(m_currFrameData.onPickupIndex);
				}
			}
			if (m_currFrameData.bitfields.onObstacleHit)
			{
				IndexedMonoBehaviour<TrackObstacle>.All[m_currFrameData.onObstacleHitIndex].TriggerCollision(m_vehicle);
			}
			if (m_currFrameData.bitfields.onSurroundCollision)
			{
				m_vehicle.SendSurroundCollisionEvent(m_currFrameData.onSurroundCollision_IsMajor);
			}
			if (m_currFrameData.bitfields.onTrackCollision)
			{
				m_vehicle.SendTrackCollisionEvent();
			}
			for (int i = 0; i != m_currFrameData.numVehicleCollisions; i++)
			{
				m_vehicle.SendVehicleCollisionEvent(m_currFrameData.vehicleCollisionParams[i].point, m_currFrameData.vehicleCollisionParams[i].normal, m_currFrameData.vehicleCollisionParams[i].velocity, m_currFrameData.vehicleCollisionParams[i].isMajor, m_currFrameData.vehicleCollisionParams[i].playAudio);
			}
			if (m_currFrameData.bitfields.onDeathEvent)
			{
				m_vehicle.SendDeathEvent();
			}
			m_numFrames++;
		}
		catch (IndexOutOfRangeException)
		{
			m_vehicle.RepeatFrame();
			m_bitStream = null;
		}
	}
}
