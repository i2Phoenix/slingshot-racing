using System.Runtime.InteropServices;
using UnityEngine;

public class QuaternionUtils
{
	public static void QuaternionMul(ref Vector3 v, ref Quaternion q)
	{
		QuaternionMul(ref v.x, ref q);
	}

	[DllImport("GrappleRacerNative")]
	public static extern void QuaternionMul(ref float v, ref Quaternion q);

	public static Quaternion Normalise(Quaternion _q)
	{
		float num = _q.x * _q.x + _q.y * _q.y + _q.z * _q.z + _q.w * _q.w;
		if (num <= float.Epsilon)
		{
			return Quaternion.identity;
		}
		float num2 = 1f / Mathf.Sqrt(num);
		Quaternion result = default(Quaternion);
		result.x = _q.x * num2;
		result.y = _q.y * num2;
		result.z = _q.z * num2;
		result.w = _q.w * num2;
		return result;
	}

	public static Vector3 MulX00(Quaternion _q, float _x)
	{
		float num = _q.y * 2f;
		float num2 = _q.z * 2f;
		float num3 = _q.y * num;
		float num4 = _q.z * num2;
		float num5 = _q.x * num;
		float num6 = _q.x * num2;
		float num7 = _q.w * num;
		float num8 = _q.w * num2;
		Vector3 result = default(Vector3);
		result.x = (1f - (num3 + num4)) * _x;
		result.y = (num5 + num8) * _x;
		result.z = (num6 - num7) * _x;
		return result;
	}

	public static Vector3 Mul0Y0(Quaternion _q, float _y)
	{
		float num = _q.x * 2f;
		float num2 = _q.y * 2f;
		float num3 = _q.z * 2f;
		float num4 = _q.x * num;
		float num5 = _q.z * num3;
		float num6 = _q.x * num2;
		float num7 = _q.y * num3;
		float num8 = _q.w * num;
		float num9 = _q.w * num3;
		Vector3 result = default(Vector3);
		result.x = (num6 - num9) * _y;
		result.y = (1f - (num4 + num5)) * _y;
		result.z = (num7 + num8) * _y;
		return result;
	}

	public static Vector3 Mul00Z(Quaternion _q, float _z)
	{
		float num = _q.x * 2f;
		float num2 = _q.y * 2f;
		float num3 = _q.z * 2f;
		float num4 = _q.x * num;
		float num5 = _q.y * num2;
		float num6 = _q.x * num3;
		float num7 = _q.y * num3;
		float num8 = _q.w * num;
		float num9 = _q.w * num2;
		Vector3 result = default(Vector3);
		result.x = (num6 + num9) * _z;
		result.y = (num7 - num8) * _z;
		result.z = (1f - (num4 + num5)) * _z;
		return result;
	}
}
