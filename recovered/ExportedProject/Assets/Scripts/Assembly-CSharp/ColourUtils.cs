using UnityEngine;

public static class ColourUtils
{
	public static Color LerpNoClamp(Color _a, Color _b, float _lerp)
	{
		Color result = default(Color);
		result.r = _a.r + _lerp * (_b.r - _a.r);
		result.g = _a.g + _lerp * (_b.g - _a.g);
		result.b = _a.b + _lerp * (_b.b - _a.b);
		result.a = _a.a + _lerp * (_b.a - _a.a);
		return result;
	}
}
