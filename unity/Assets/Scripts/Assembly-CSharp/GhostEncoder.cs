using UnityEngine;

public struct GhostEncoder
{
	private const float c_rotPackScale = 16f / 45f;

	private const float c_rotUnpackScale = 2.8125f;

	private const float c_posPackScale = 16f;

	private const float c_posUnpackScale = 0.0625f;

	private const float c_forcePackScale = 0.25f;

	private const float c_forceUnpackScale = 4f;

	private const uint c_rotK = 0u;

	private const uint c_posKxz = 1u;

	private const uint c_posKy = 0u;

	private int m_prevRx;

	private int m_prevRy;

	private int m_prevRz;

	private Quaternion m_prevRot1;

	private Vector3 m_prevPos1;

	private Vector3 m_prevPos2;

	private float m_prevForce;

	public void Reset()
	{
		m_prevRot1 = Quaternion.identity;
		m_prevPos1 = Vector3.zero;
		m_prevPos2 = Vector3.zero;
		m_prevRx = 0;
		m_prevRy = 0;
		m_prevRz = 0;
		m_prevForce = 0f;
	}

	public void WriteRotation(BitOutputStream bitstream, Quaternion rot)
	{
		float angle;
		Vector3 axis;
		(Quaternion.Inverse(m_prevRot1) * rot).ToAngleAxis(out angle, out axis);
		if (angle > 180f)
		{
			angle -= 360f;
		}
		float num = angle * (16f / 45f);
		int num2 = Mathf.RoundToInt(axis.x * num);
		int num3 = Mathf.RoundToInt(axis.y * num);
		int num4 = Mathf.RoundToInt(axis.z * num);
		EntropyEncode(bitstream, num2 - m_prevRx, 0u);
		EntropyEncode(bitstream, num3 - m_prevRy, 0u);
		EntropyEncode(bitstream, num4 - m_prevRz, 0u);
		m_prevRx = num2;
		m_prevRy = num3;
		m_prevRz = num4;
		UnpackRotation(num2, num3, num4);
	}

	public Quaternion ReadRotation(BitInputStream bitstream)
	{
		int num = m_prevRx + EntropyDecode(bitstream, 0u);
		int num2 = m_prevRy + EntropyDecode(bitstream, 0u);
		int num3 = m_prevRz + EntropyDecode(bitstream, 0u);
		m_prevRx = num;
		m_prevRy = num2;
		m_prevRz = num3;
		UnpackRotation(num, num2, num3);
		return m_prevRot1;
	}

	public void WritePosition(BitOutputStream bitstream, Vector3 pos)
	{
		Vector3 vector = m_prevPos1 * 2f - m_prevPos2;
		Vector3 vector2 = pos - vector;
		int num = Mathf.RoundToInt(vector2.x * 16f);
		int num2 = Mathf.RoundToInt(vector2.y * 16f);
		int num3 = Mathf.RoundToInt(vector2.z * 16f);
		EntropyEncode(bitstream, num, 1u);
		EntropyEncode(bitstream, num2, 0u);
		EntropyEncode(bitstream, num3, 1u);
		UnpackPosition(num, num2, num3, vector);
	}

	public Vector3 ReadPosition(BitInputStream bitStream)
	{
		int px = EntropyDecode(bitStream, 1u);
		int py = EntropyDecode(bitStream, 0u);
		int pz = EntropyDecode(bitStream, 1u);
		UnpackPosition(px, py, pz, m_prevPos1 * 2f - m_prevPos2);
		return m_prevPos1;
	}

	public void WriteGrappleForce(BitOutputStream bitstream, float force)
	{
		int num = Mathf.RoundToInt((Mathf.Min(255f, force) - m_prevForce) * 0.25f);
		EntropyEncode(bitstream, num, 0u);
		UnpackForce(num);
	}

	public float ReadGrappleForce(BitInputStream bitstream)
	{
		int force = EntropyDecode(bitstream, 0u);
		UnpackForce(force);
		return m_prevForce;
	}

	public void ResetGrappleForce()
	{
		m_prevForce = 0f;
	}

	private void UnpackRotation(int rx, int ry, int rz)
	{
		if ((rx | ry | rz) != 0)
		{
			Vector3 vector = default(Vector3);
			vector.x = rx;
			vector.y = ry;
			vector.z = rz;
			float magnitude = vector.magnitude;
			Quaternion quaternion = Quaternion.AngleAxis(magnitude * 2.8125f, vector * (1f / magnitude));
			m_prevRot1 = QuaternionUtils.Normalise(m_prevRot1 * quaternion);
		}
	}

	private void UnpackPosition(int px, int py, int pz, Vector3 predict)
	{
		Vector3 vector = default(Vector3);
		vector.x = (float)px * 0.0625f;
		vector.y = (float)py * 0.0625f;
		vector.z = (float)pz * 0.0625f;
		m_prevPos2 = m_prevPos1;
		m_prevPos1 = predict + vector;
	}

	private void UnpackForce(int force)
	{
		m_prevForce += (float)force * 4f;
	}

	private void EntropyEncode(BitOutputStream bitstream, int val, uint k)
	{
		uint val2 = 0u;
		uint num = 1u;
		if (val != 0)
		{
			uint num2 = 0u;
			if (val < 0)
			{
				val = -val;
				num2 = 1u;
			}
			val--;
			val2 = 1u;
			uint num3 = (uint)(1 << (int)k);
			while (val >= num3)
			{
				val -= (int)num3;
				k++;
				num++;
				num3 <<= 1;
			}
			num += 2;
			val2 = (uint)((1 << (int)num) - 4) | num2;
			num += k;
			val2 = (val2 << (int)k) | (uint)val;
		}
		bitstream.Write(val2, num);
	}

	private int EntropyDecode(BitInputStream bitstream, uint k)
	{
		if (bitstream.ReadUInt(1u) != 0)
		{
			uint num = 1u;
			uint num2 = (uint)(1 << (int)k);
			while (bitstream.ReadUInt(1u) != 0)
			{
				num += num2;
				k++;
				num2 <<= 1;
			}
			uint num3 = bitstream.ReadUInt(1u);
			if (k != 0)
			{
				num += bitstream.ReadUInt(k);
			}
			return (int)((num3 != 0) ? (0 - num) : num);
		}
		return 0;
	}
}
