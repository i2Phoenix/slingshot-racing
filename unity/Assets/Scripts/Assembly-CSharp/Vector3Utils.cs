using UnityEngine;

public static class Vector3Utils
{
	public static Vector3 LerpNoClamp(Vector3 _a, Vector3 _b, float _lerp)
	{
		Vector3 result = default(Vector3);
		result.x = _a.x + _lerp * (_b.x - _a.x);
		result.y = _a.y + _lerp * (_b.y - _a.y);
		result.z = _a.z + _lerp * (_b.z - _a.z);
		return result;
	}

	public static float DistanceSqr(Vector3 _a, Vector3 _b)
	{
		float num = _a.x - _b.x;
		float num2 = _a.y - _b.y;
		float num3 = _a.z - _b.z;
		return num * num + num2 * num2 + num3 * num3;
	}

	public static Vector3 Min(Vector3 _a, Vector3 _b, Vector3 _c, Vector3 _d)
	{
		float num = ((!(_a.x < _b.x)) ? _b.x : _a.x);
		float num2 = ((!(_c.x < _d.x)) ? _d.x : _c.x);
		Vector3 result = default(Vector3);
		result.x = ((!(num < num2)) ? num2 : num);
		num = ((!(_a.y < _b.y)) ? _b.y : _a.y);
		num2 = ((!(_c.y < _d.y)) ? _d.y : _c.y);
		result.y = ((!(num < num2)) ? num2 : num);
		num = ((!(_a.z < _b.z)) ? _b.z : _a.z);
		num2 = ((!(_c.z < _d.z)) ? _d.z : _c.z);
		result.z = ((!(num < num2)) ? num2 : num);
		return result;
	}

	public static Vector3 Max(Vector3 _a, Vector3 _b, Vector3 _c, Vector3 _d)
	{
		float num = ((!(_a.x > _b.x)) ? _b.x : _a.x);
		float num2 = ((!(_c.x > _d.x)) ? _d.x : _c.x);
		Vector3 result = default(Vector3);
		result.x = ((!(num > num2)) ? num2 : num);
		num = ((!(_a.y > _b.y)) ? _b.y : _a.y);
		num2 = ((!(_c.y > _d.y)) ? _d.y : _c.y);
		result.y = ((!(num > num2)) ? num2 : num);
		num = ((!(_a.z > _b.z)) ? _b.z : _a.z);
		num2 = ((!(_c.z > _d.z)) ? _d.z : _c.z);
		result.z = ((!(num > num2)) ? num2 : num);
		return result;
	}

	public static Vector3 Normalise(Vector3 _v)
	{
		float magnitude = _v.magnitude;
		Vector3 result = default(Vector3);
		if (magnitude > 1E-05f)
		{
			magnitude = 1f / magnitude;
			result.x = _v.x * magnitude;
			result.y = _v.y * magnitude;
			result.z = _v.z * magnitude;
		}
		else
		{
			result.x = 0f;
			result.y = 0f;
			result.z = 0f;
		}
		return result;
	}

	public static void Normalise(ref Vector3 _v)
	{
		float magnitude = _v.magnitude;
		if (magnitude > 1E-05f)
		{
			magnitude = 1f / magnitude;
			_v.x *= magnitude;
			_v.y *= magnitude;
			_v.z *= magnitude;
		}
		else
		{
			_v.x = 0f;
			_v.y = 0f;
			_v.z = 0f;
		}
	}

	public static void NormaliseAndMagnitude(Vector3 _v, out Vector3 o_normalised, out float o_magnitude)
	{
		float num = (o_magnitude = _v.magnitude);
		if (num > 1E-05f)
		{
			num = 1f / num;
			o_normalised.x = _v.x * num;
			o_normalised.y = _v.y * num;
			o_normalised.z = _v.z * num;
		}
		else
		{
			o_normalised.x = 0f;
			o_normalised.y = 0f;
			o_normalised.z = 0f;
		}
	}

	public static Vector3 RandomRange(Vector3 _min, Vector3 _max)
	{
		return new Vector3(Random.Range(_min.x, _max.x), Random.Range(_min.y, _max.y), Random.Range(_min.z, _max.z));
	}
}
