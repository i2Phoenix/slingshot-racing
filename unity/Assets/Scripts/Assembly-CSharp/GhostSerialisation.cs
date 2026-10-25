using UnityEngine;

public static class GhostSerialisation
{
	public struct Bitfields
	{
		public const uint c_numBitsToWrite = 23u;

		public static uint[] c_numBitsToRead = new uint[2] { 13u, 23u };

		public uint packed;

		public int grapplePointIndex
		{
			get
			{
				return (int)((packed & 0xF) - 1);
			}
			set
			{
				packed = (packed & 0xF) | (uint)(value + 1);
			}
		}

		public int trackSplineIndex
		{
			get
			{
				return (int)(((packed & 0x70) >> 4) - 3);
			}
			set
			{
				packed = (uint)((packed & -113) | (uint)(value + 3 << 4));
			}
		}

		public bool onLapEvent
		{
			get
			{
				return (packed & 0x80) != 0;
			}
			set
			{
				packed = (uint)((!value) ? (packed & -129) : (packed | 0x80));
			}
		}

		public int waypoint
		{
			get
			{
				return (int)(((packed & 0xF00) >> 8) - 2);
			}
			set
			{
				packed = (uint)((packed & -3841) | (uint)(value + 2 << 8));
			}
		}

		public bool onBoostEvent
		{
			get
			{
				return (packed & 0x1000) != 0;
			}
			set
			{
				packed = (uint)((!value) ? (packed & -4097) : (packed | 0x1000));
			}
		}

		public bool onDeployMiniReward
		{
			get
			{
				return (packed & 0x2000) != 0;
			}
			set
			{
				packed = (uint)((!value) ? (packed & -8193) : (packed | 0x2000));
			}
		}

		public bool onDropMiniReward
		{
			get
			{
				return (packed & 0x4000) != 0;
			}
			set
			{
				packed = (uint)((!value) ? (packed & -16385) : (packed | 0x4000));
			}
		}

		public bool onCollectMiniReward
		{
			get
			{
				return (packed & 0x8000) != 0;
			}
			set
			{
				packed = (uint)((!value) ? (packed & -32769) : (packed | 0x8000));
			}
		}

		public bool onPickupEvent
		{
			get
			{
				return (packed & 0x10000) != 0;
			}
			set
			{
				packed = (uint)((!value) ? (packed & -65537) : (packed | 0x10000));
			}
		}

		public bool onObstacleHit
		{
			get
			{
				return (packed & 0x20000) != 0;
			}
			set
			{
				packed = (uint)((!value) ? (packed & -131073) : (packed | 0x20000));
			}
		}

		public bool onSurroundCollision
		{
			get
			{
				return (packed & 0x40000) != 0;
			}
			set
			{
				packed = (uint)((!value) ? (packed & -262145) : (packed | 0x40000));
			}
		}

		public bool onTrackCollision
		{
			get
			{
				return (packed & 0x80000) != 0;
			}
			set
			{
				packed = (uint)((!value) ? (packed & -524289) : (packed | 0x80000));
			}
		}

		public bool onVehicleCollision
		{
			get
			{
				return (packed & 0x100000) != 0;
			}
			set
			{
				packed = (uint)((!value) ? (packed & -1048577) : (packed | 0x100000));
			}
		}

		public bool onDeathEvent
		{
			get
			{
				return (packed & 0x200000) != 0;
			}
			set
			{
				packed = (uint)((!value) ? (packed & -2097153) : (packed | 0x200000));
			}
		}

		public bool onShieldEvent
		{
			get
			{
				return (packed & 0x400000) != 0;
			}
			set
			{
				packed = (uint)((!value) ? (packed & -4194305) : (packed | 0x400000));
			}
		}
	}

	public struct VehicleCollisionParams
	{
		public Vector3 point;

		public Vector3 normal;

		public Vector3 velocity;

		public bool isMajor;

		public bool playAudio;
	}

	public struct Position
	{
		private const float c_packScale = 256f;

		private const float c_unpackScale = 1f / 256f;

		public short x;

		public short y;

		public short z;

		public static implicit operator Position(Vector3 v)
		{
			Position result = default(Position);
			result.x = (short)(v.x * 256f);
			result.y = (short)(v.y * 256f);
			result.z = (short)(v.z * 256f);
			return result;
		}

		public static implicit operator Vector3(Position p)
		{
			Vector3 result = default(Vector3);
			result.x = (float)p.x * (1f / 256f);
			result.y = (float)p.y * (1f / 256f);
			result.z = (float)p.z * (1f / 256f);
			return result;
		}
	}

	public struct Rotation
	{
		private const float c_packScale = 182.0389f;

		private const float c_unpackScale = 180f / 32767f;

		public short x;

		public short y;

		public short z;

		public static implicit operator Rotation(Quaternion q)
		{
			float angle;
			Vector3 axis;
			q.ToAngleAxis(out angle, out axis);
			if (angle > 180f)
			{
				angle -= 360f;
			}
			axis *= angle * 182.0389f;
			axis.x = Mathf.Clamp(axis.x, -32767f, 32767f);
			axis.y = Mathf.Clamp(axis.y, -32767f, 32767f);
			axis.z = Mathf.Clamp(axis.z, -32767f, 32767f);
			Rotation result = default(Rotation);
			result.x = (short)axis.x;
			result.y = (short)axis.y;
			result.z = (short)axis.z;
			return result;
		}

		public static implicit operator Quaternion(Rotation r)
		{
			Vector3 axis = default(Vector3);
			axis.x = r.x;
			axis.y = r.y;
			axis.z = r.z;
			Quaternion result = Quaternion.identity;
			float magnitude = axis.magnitude;
			if (magnitude > 0f)
			{
				axis /= magnitude;
				magnitude *= 180f / 32767f;
				result = Quaternion.AngleAxis(magnitude, axis);
			}
			return result;
		}
	}

	public const int c_numVersionBits = 13;

	public const uint c_versionTag = 8064u;

	public const uint c_version = 8065u;

	public const int c_maxVehicleCollisions = 4;

	public static TrackSpline s_theTrackSpline;

	public static TrackHub s_theTrackHub;

	public static int ToTrackSplineIndex(TrackSpline ts, TrackHub th)
	{
		int result = 0;
		if (ts != null)
		{
			result = ts.Index + 1;
		}
		else if (th != null)
		{
			result = -(th.Index + 1);
		}
		return result;
	}

	public static void FromTrackSplineIndex(out TrackSpline ts, out TrackHub th, int trackSplineIndex)
	{
		ts = ((trackSplineIndex <= 0) ? null : IndexedMonoBehaviour<TrackSpline>.All[trackSplineIndex - 1]);
		th = ((trackSplineIndex >= 0) ? null : IndexedMonoBehaviour<TrackHub>.All[-trackSplineIndex - 1]);
	}
}
