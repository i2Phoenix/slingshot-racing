using UnityEngine;

public static class Vector2Utils
{
	public static Vector2 LerpNoClamp(Vector2 _a, Vector2 _b, float _lerp)
	{
		Vector2 result = default(Vector2);
		result.x = _a.x + _lerp * (_b.x - _a.x);
		result.y = _a.y + _lerp * (_b.y - _a.y);
		return result;
	}

	public static float DistanceSqr(Vector2 _a, Vector2 _b)
	{
		float num = _a.x - _b.x;
		float num2 = _a.y - _b.y;
		return num * num + num2 * num2;
	}
}
