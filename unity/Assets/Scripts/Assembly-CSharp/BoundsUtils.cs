using UnityEngine;

public static class BoundsUtils
{
	public static Bounds BoundsFromMinMax(Vector3 _min, Vector3 _max)
	{
		Vector3 vector = (_max - _min) * 0.5f;
		return new Bounds
		{
			extents = vector,
			center = _min + vector
		};
	}

	public static void BoundsFromMinMax(Vector3 _min, Vector3 _max, out Bounds o_bounds)
	{
		Vector3 vector = (_max - _min) * 0.5f;
		o_bounds = default(Bounds);
		o_bounds.extents = vector;
		o_bounds.center = _min + vector;
	}
}
