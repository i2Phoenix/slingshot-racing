using System;
using UnityEngine;

public static class MathUtils
{
	public enum Intersects
	{
		Inside = 0,
		Outside = 1,
		Intersects = 2
	}

	public static int Repeat(int i, int length)
	{
		int num = i % length;
		return (num >= 0) ? num : (length + num);
	}

	public static Intersects TestPlanesAABB(Plane[] _planes, Bounds _bounds)
	{
		Vector3 min = _bounds.min;
		Vector3 max = _bounds.max;
		Vector3[] array = new Vector3[8]
		{
			new Vector3(min.x, min.y, min.z),
			new Vector3(max.x, min.y, min.z),
			new Vector3(max.x, max.y, min.z),
			new Vector3(min.x, max.y, min.z),
			new Vector3(min.x, min.y, max.z),
			new Vector3(max.x, min.y, max.z),
			new Vector3(max.x, max.y, max.z),
			new Vector3(min.x, max.y, max.z)
		};
		bool flag = true;
		for (int i = 0; i < _planes.Length; i++)
		{
			Plane plane = _planes[i];
			bool flag2 = false;
			Vector3[] array2 = array;
			foreach (Vector3 inPt in array2)
			{
				bool side = plane.GetSide(inPt);
				flag &= side;
				flag2 |= side;
			}
			if (!flag2)
			{
				return Intersects.Outside;
			}
		}
		return (!flag) ? Intersects.Intersects : Intersects.Inside;
	}

	public static int Clamp(int i, int min, int max)
	{
		return (i < min) ? min : ((i <= max) ? i : max);
	}

	public static float WrapAngle(float angle)
	{
		return Mathf.Repeat(angle, 360f);
	}

	public static float AngleOnXZ(Vector3 point)
	{
		point.y = 0f;
		float num = Vector3.Angle(Vector3.forward, point);
		if (Vector3.Dot(Vector3.right, point) < 0f)
		{
			num = WrapAngle(360f - num);
		}
		return num;
	}

	public static Vector3 DirectionOnXZ(float angle)
	{
		float f = angle * ((float)Math.PI / 180f);
		float num = Mathf.Cos(f);
		float num2 = Mathf.Sin(f);
		return Vector3.forward * num + Vector3.right * num2;
	}

	public static bool AngleInRange(float angle, float fromAngle, float toAngle)
	{
		float num = WrapAngle(angle);
		float num2 = WrapAngle(fromAngle);
		float num3 = WrapAngle(toAngle);
		if (num2 <= num3)
		{
			return num >= num2 && num <= num3;
		}
		return num >= num2 || num <= num3;
	}

	public static float Gauss(float _x, float _sigma)
	{
		float num = _sigma * _sigma;
		float num2 = 1f / Mathf.Sqrt((float)Math.PI * 2f * num);
		float num3 = _x * _x;
		float num4 = Mathf.Exp((0f - num3) / (2f * num));
		return num2 * num4;
	}

	public static float Gauss(float _x, float _mu, float _sigma)
	{
		return Gauss(_x - _mu, _sigma);
	}

	public static float LerpNoClamp(float _a, float _b, float _lerp)
	{
		return _a + _lerp * (_b - _a);
	}

	public static float Frac(float _f)
	{
		return _f - (float)Math.Truncate(_f);
	}
}
