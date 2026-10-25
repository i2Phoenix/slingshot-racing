#if SLINGSHOT_LEGACY_NETWORK_TEST
using UnityEngine;

public class LocalNetworkGhost : MonoBehaviour
{
	private Rigidbody m_rigidbody;

	private Vehicle m_vehicle;

	private int m_frameIndex;

	private void Awake()
	{
		m_rigidbody = base.rigidbody;
		m_vehicle = GetComponent<Vehicle>();
	}

	private void FixedUpdate()
	{
		m_frameIndex++;
	}

	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		stream.Serialize(ref m_frameIndex);
		GhostSerialisation.Position position = m_rigidbody.position;
		stream.Serialize(ref position.x);
		stream.Serialize(ref position.y);
		stream.Serialize(ref position.z);
		GhostSerialisation.Rotation rotation = m_rigidbody.rotation;
		stream.Serialize(ref rotation.x);
		stream.Serialize(ref rotation.y);
		stream.Serialize(ref rotation.z);
		short value = (short)((!(m_vehicle.GrapplePoint != null)) ? (-1) : m_vehicle.GrapplePoint.Index);
		stream.Serialize(ref value);
		short value2 = (short)GhostSerialisation.ToTrackSplineIndex(m_vehicle.CurrentTrackSpline, m_vehicle.CurrentTrackHub);
		stream.Serialize(ref value2);
		if (value >= 0)
		{
			float value3 = m_vehicle.GrappleForce;
			stream.Serialize(ref value3);
		}
	}
}

#endif
