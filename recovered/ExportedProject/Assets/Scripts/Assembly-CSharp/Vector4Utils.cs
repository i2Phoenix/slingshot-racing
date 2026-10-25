using UnityEngine;

public static class Vector4Utils
{
	public static Vector4 LerpNoClamp(Vector4 _a, Vector4 _b, float _lerp)
	{
		Vector4 result = default(Vector4);
		result.x = _a.x + _lerp * (_b.x - _a.x);
		result.y = _a.y + _lerp * (_b.y - _a.y);
		result.z = _a.z + _lerp * (_b.z - _a.z);
		result.w = _a.w + _lerp * (_b.w - _a.w);
		return result;
	}

	public static float DistanceSqr(Vector4 _a, Vector4 _b)
	{
		float num = _a.x - _b.x;
		float num2 = _a.y - _b.y;
		float num3 = _a.z - _b.z;
		float num4 = _a.w - _b.w;
		return num * num + num2 * num2 + num3 * num3 + num4 * num4;
	}

	public static Vector4 Direction(Vector3 _dir)
	{
		Vector4 result = default(Vector4);
		result.x = _dir.x;
		result.y = _dir.y;
		result.z = _dir.z;
		result.w = 0f;
		return result;
	}

	public static Vector4 Position(Vector3 _pos)
	{
		Vector4 result = default(Vector4);
		result.x = _pos.x;
		result.y = _pos.y;
		result.z = _pos.z;
		result.w = 1f;
		return result;
	}
}
