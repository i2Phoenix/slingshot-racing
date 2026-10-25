#if SLINGSHOT_LEGACY_NETWORK_TEST
using UnityEngine;

public class RemoteNetworkGhost : MonoBehaviour
{
	private struct Frame
	{
		public Vector3 position;

		public Quaternion rotation;

		public float grappleForce;

		public short grapplePointIndex;

		public short trackSplineIndex;
	}

	private const int c_bufferSize = 32;

	private const int c_discontinuity = 8;

	private const int c_delayTarget = 4;

	private const int c_extrapolateLimit = 15;

	private GhostVehicle m_vehicle;

	private Frame m_relayFrame;

	private int m_relayFrameIndex;

	private bool m_reset = true;

	private int m_framesOffTarget;

	private int m_tailFrameIndex;

	private int m_tailFrame;

	private int m_numFrames;

	private int m_numExtrapolatedFrames;

	private Frame[] m_frames = new Frame[32];

	private void Start()
	{
		m_vehicle = GetComponent<GhostVehicle>();
	}

	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			GhostSerialisation.Position position = m_relayFrame.position;
			GhostSerialisation.Rotation rotation = m_relayFrame.rotation;
			stream.Serialize(ref m_relayFrameIndex);
			stream.Serialize(ref position.x);
			stream.Serialize(ref position.y);
			stream.Serialize(ref position.z);
			stream.Serialize(ref rotation.x);
			stream.Serialize(ref rotation.y);
			stream.Serialize(ref rotation.z);
			stream.Serialize(ref m_relayFrame.grapplePointIndex);
			stream.Serialize(ref m_relayFrame.trackSplineIndex);
			if (m_relayFrame.grapplePointIndex >= 0)
			{
				stream.Serialize(ref m_relayFrame.grappleForce);
			}
			return;
		}
		int value = 0;
		Frame frame = default(Frame);
		GhostSerialisation.Position position2 = default(GhostSerialisation.Position);
		GhostSerialisation.Rotation rotation2 = default(GhostSerialisation.Rotation);
		stream.Serialize(ref value);
		stream.Serialize(ref position2.x);
		stream.Serialize(ref position2.y);
		stream.Serialize(ref position2.z);
		stream.Serialize(ref rotation2.x);
		stream.Serialize(ref rotation2.y);
		stream.Serialize(ref rotation2.z);
		stream.Serialize(ref frame.grapplePointIndex);
		stream.Serialize(ref frame.trackSplineIndex);
		if (frame.grapplePointIndex >= 0)
		{
			stream.Serialize(ref frame.grappleForce);
		}
		if (value <= 0)
		{
			return;
		}
		frame.position = position2;
		frame.rotation = rotation2;
		if (value > m_tailFrameIndex)
		{
			if (m_tailFrameIndex == 0 || value - m_tailFrameIndex >= 8)
			{
				m_reset = true;
				m_tailFrame = 0;
				m_numExtrapolatedFrames = 0;
				m_numFrames = 1;
				m_frames[0] = frame;
			}
			else
			{
				int num = value - m_tailFrameIndex;
				int tailFrame = m_tailFrame;
				int num2 = 1;
				while (true)
				{
					if (++m_tailFrame == 32)
					{
						m_tailFrame = 0;
					}
					if (++m_numFrames >= 32)
					{
						m_numFrames = 31;
					}
					if (num2 == num)
					{
						break;
					}
					float t = (float)num2 / (float)num;
					m_frames[m_tailFrame].position = Vector3.Lerp(m_frames[tailFrame].position, frame.position, t);
					m_frames[m_tailFrame].rotation = Quaternion.Slerp(m_frames[tailFrame].rotation, frame.rotation, t);
					m_frames[m_tailFrame].grappleForce = Mathf.Lerp(m_frames[tailFrame].grappleForce, frame.grappleForce, t);
					m_frames[m_tailFrame].grapplePointIndex = m_frames[tailFrame].grapplePointIndex;
					m_frames[m_tailFrame].trackSplineIndex = m_frames[tailFrame].trackSplineIndex;
					num2++;
				}
				m_frames[m_tailFrame] = frame;
			}
			m_tailFrameIndex = value;
			m_numExtrapolatedFrames = 0;
			return;
		}
		int num3 = m_tailFrameIndex - value;
		if (num3 >= 32)
		{
			return;
		}
		int num4 = m_tailFrame - num3;
		if (num4 < 0)
		{
			num4 += 32;
		}
		m_frames[num4] = frame;
		if (m_numExtrapolatedFrames > num3)
		{
			int num5 = m_numExtrapolatedFrames - num3;
			m_numExtrapolatedFrames = 0;
			if ((m_tailFrame -= num5) < 0)
			{
				m_tailFrame += 32;
			}
			for (int i = 0; i != num5; i++)
			{
				ExtrapolateFrame();
			}
		}
	}

	private void ExtrapolateFrame()
	{
		int tailFrame = m_tailFrame;
		int num = ((m_tailFrame <= 0) ? 32 : m_tailFrame) - 1;
		if (++m_tailFrame == 32)
		{
			m_tailFrame = 0;
		}
		if (m_numExtrapolatedFrames < 15)
		{
			m_frames[m_tailFrame].position = m_frames[tailFrame].position * 2f - m_frames[num].position;
			m_frames[m_tailFrame].rotation = m_frames[tailFrame].rotation * Quaternion.Inverse(m_frames[num].rotation) * m_frames[tailFrame].rotation;
		}
		else
		{
			m_frames[m_tailFrame].position = m_frames[tailFrame].position;
			m_frames[m_tailFrame].rotation = m_frames[tailFrame].rotation;
		}
		m_frames[m_tailFrame].grappleForce = m_frames[tailFrame].grappleForce;
		m_frames[m_tailFrame].grapplePointIndex = m_frames[tailFrame].grapplePointIndex;
		m_frames[m_tailFrame].trackSplineIndex = m_frames[tailFrame].trackSplineIndex;
		m_numExtrapolatedFrames++;
	}

	private void FixedUpdate()
	{
		if (m_reset && m_numFrames < 4)
		{
			m_framesOffTarget = 0;
			m_vehicle.RepeatFrame();
			return;
		}
		if (m_numFrames > 4)
		{
			if (m_framesOffTarget < 0)
			{
				m_framesOffTarget = 0;
			}
			m_framesOffTarget++;
			if (m_framesOffTarget > 30)
			{
				Debug.Log("Overflow");
				m_framesOffTarget = 0;
				m_numFrames--;
			}
		}
		else if (m_numFrames < 4)
		{
			if (m_framesOffTarget > 0)
			{
				m_framesOffTarget = 0;
			}
			m_framesOffTarget--;
			if (m_framesOffTarget < -30)
			{
				Debug.Log("underflow");
				m_framesOffTarget = 0;
				m_vehicle.RepeatFrame();
				return;
			}
		}
		else
		{
			m_framesOffTarget = 0;
		}
		if (m_numFrames == 0)
		{
			ExtrapolateFrame();
			m_tailFrameIndex++;
			m_numFrames++;
		}
		int num = m_tailFrame - m_numFrames + 1;
		if (num < 0)
		{
			num += 32;
		}
		int num2 = m_tailFrame - m_numFrames;
		if (num2 < 0)
		{
			num2 += 32;
		}
		m_relayFrame = m_frames[num];
		m_relayFrameIndex = m_tailFrameIndex - m_numFrames + 1;
		Vector3 velocity = (m_frames[num].position - m_frames[num2].position) * (1f / Time.fixedDeltaTime);
		GrapplePoint grapplePoint = ((m_frames[num].grapplePointIndex < 0) ? null : IndexedMonoBehaviour<GrapplePoint>.All[m_frames[num].grapplePointIndex]);
		TrackSpline ts;
		TrackHub th;
		GhostSerialisation.FromTrackSplineIndex(out ts, out th, m_frames[num].trackSplineIndex);
		m_vehicle.NewFrame(m_frames[num].position, m_frames[num].rotation, velocity, grapplePoint, m_frames[num].grappleForce, ts, th, 0, m_reset);
		m_reset = false;
		m_numFrames--;
	}
}

#endif
