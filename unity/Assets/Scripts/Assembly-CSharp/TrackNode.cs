using System;
using UnityEngine;

[Serializable]
public class TrackNode
{
	[Flags]
	public enum Flags
	{
		CatmulRom = 1
	}

	public const float c_defaultWidth = 3f;

	public const float c_defaultTangentScale = 15f;

	public const float c_defaultRoofHeight = 2f;

	public TrackCrossSection crossSection;

	public Quaternion rotation;

	public Vector3 position;

	public float width;

	public float tangentScale;

	public float m_roofHeight;

	public Flags flags;

	public Vector3 forward
	{
		get
		{
			return QuaternionUtils.Mul00Z(rotation, 1f);
		}
		set
		{
			rotation = Quaternion.LookRotation(value);
		}
	}

	public Vector3 tangent
	{
		get
		{
			return QuaternionUtils.Mul00Z(rotation, tangentScale);
		}
	}

	public Vector3 right
	{
		get
		{
			return QuaternionUtils.MulX00(rotation, 1f);
		}
	}

	public Vector3 up
	{
		get
		{
			return QuaternionUtils.Mul0Y0(rotation, 1f);
		}
	}

	public bool CatmulRom
	{
		get
		{
			return (flags & Flags.CatmulRom) != 0;
		}
		set
		{
			if (value)
			{
				flags |= Flags.CatmulRom;
			}
			else
			{
				flags &= ~Flags.CatmulRom;
			}
		}
	}

	public TrackNode()
	{
		rotation = Quaternion.identity;
		width = 3f;
		tangentScale = 15f;
		m_roofHeight = 2f;
		flags = (Flags)0;
	}
}
