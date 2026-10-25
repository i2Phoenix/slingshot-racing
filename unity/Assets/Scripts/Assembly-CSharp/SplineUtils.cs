using System.Runtime.InteropServices;
using UnityEngine;

public class SplineUtils
{
	public static Vector3 CatmullRomTangent(Vector3 _pointBefore, Vector3 _pointAfter)
	{
		return (_pointAfter - _pointBefore) * 0.5f;
	}

	public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		Vector3 m = CatmullRomTangent(p0, p2);
		Vector3 m2 = CatmullRomTangent(p1, p3);
		return CubicHermite(p1, m, p2, m2, t);
	}

	public static Vector3 CatmullRomDT(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		Vector3 m = CatmullRomTangent(p0, p2);
		Vector3 m2 = CatmullRomTangent(p1, p3);
		return CubicHermiteDT(p1, m, p2, m2, t);
	}

	public static Vector3 CatmullRomDT2(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		Vector3 m = CatmullRomTangent(p0, p2);
		Vector3 m2 = CatmullRomTangent(p1, p3);
		return CubicHermiteDT2(p1, m, p2, m2, t);
	}

	[DllImport("GrappleRacerNative")]
	public static extern void CubicHermite1D(out float ret, float p0, float m0, float p1, float m1, float t);

	[DllImport("GrappleRacerNative")]
	public static extern void CubicHermite(out Vector3 ret, ref Vector3 p0, ref Vector3 m0, ref Vector3 p1, ref Vector3 m1, float t);

	[DllImport("GrappleRacerNative")]
	public static extern void CubicHermiteDT(out Vector3 ret, ref Vector3 p0, ref Vector3 m0, ref Vector3 p1, ref Vector3 m1, float t);

	[DllImport("GrappleRacerNative")]
	public static extern void CubicHermiteDT2(out Vector3 ret, ref Vector3 p0, ref Vector3 m0, ref Vector3 p1, ref Vector3 m1, float t);

	public static float CubicHermite1D(float p0, float m0, float p1, float m1, float t)
	{
		float ret;
		CubicHermite1D(out ret, p0, m0, p1, m1, t);
		return ret;
	}

	public static Vector3 CubicHermite(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1, float t)
	{
		Vector3 ret;
		CubicHermite(out ret, ref p0, ref m0, ref p1, ref m1, t);
		return ret;
	}

	public static Vector3 CubicHermiteDT(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1, float t)
	{
		Vector3 ret;
		CubicHermiteDT(out ret, ref p0, ref m0, ref p1, ref m1, t);
		return ret;
	}

	public static Vector3 CubicHermiteDT2(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1, float t)
	{
		Vector3 ret;
		CubicHermiteDT2(out ret, ref p0, ref m0, ref p1, ref m1, t);
		return ret;
	}
}
