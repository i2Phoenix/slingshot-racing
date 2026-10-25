using System.Runtime.InteropServices;
using UnityEngine;

public class TrackUtilsBase
{
	public struct NodeInfo
	{
		public int i0;

		public float unitT;
	}

	protected struct CPData
	{
		public float t;

		public float d;

		public CPData(float _t, float _d)
		{
			t = _t;
			d = _d;
		}
	}

	protected struct CPData3
	{
		public CPData m_0;

		public CPData m_1;

		public CPData m_2;

		public void Sort()
		{
			if (CPLess(m_1, m_0))
			{
				CPData cPData = m_0;
				m_0 = m_1;
				m_1 = cPData;
			}
			if (CPLess(m_2, m_1))
			{
				CPData cPData = m_1;
				m_1 = m_2;
				m_2 = cPData;
				if (CPLess(m_1, m_0))
				{
					cPData = m_0;
					m_0 = m_1;
					m_1 = cPData;
				}
			}
		}
	}

	public const float c_searchStep = 0.01f;

	protected const int c_maxIterations = 16;

	protected const float c_distThreshold = 0.01f;

	protected const float c_splineCurveThreshold = 0.0001f;

	protected static bool CPLess(CPData lhs, CPData rhs)
	{
		return lhs.d < rhs.d;
	}

	protected static float NextSQuadratic(CPData3 cpData)
	{
		float num = cpData.m_0.t * cpData.m_0.t;
		float num2 = cpData.m_1.t * cpData.m_1.t;
		float num3 = cpData.m_2.t * cpData.m_2.t;
		float num4 = num2 - num3;
		float num5 = num3 - num;
		float num6 = num - num2;
		float num7 = cpData.m_1.t - cpData.m_2.t;
		float num8 = cpData.m_2.t - cpData.m_0.t;
		float num9 = cpData.m_0.t - cpData.m_1.t;
		float num10 = num4 * cpData.m_0.d + num5 * cpData.m_1.d + num6 * cpData.m_2.d;
		float num11 = num7 * cpData.m_0.d + num8 * cpData.m_1.d + num9 * cpData.m_2.d;
		return 0.5f * (num10 / num11);
	}

	[DllImport("GrappleRacerNative")]
	public static extern void TrackUtils_SplinePoint(out Vector3 o_ret, ref Vector3 p0, ref Vector3 m0, ref Vector3 p1, ref Vector3 m1, ref float unitT);

	[DllImport("GrappleRacerNative")]
	public static extern void TrackUtils_SplineTangent(out Vector3 o_ret, ref Vector3 p0, ref Vector3 m0, ref Vector3 p1, ref Vector3 m1, ref float unitT);

	[DllImport("GrappleRacerNative")]
	public static extern void TrackUtils_SplineTangentDT(out Vector3 o_ret, ref Vector3 p0, ref Vector3 m0, ref Vector3 p1, ref Vector3 m1, ref float unitT);

	[DllImport("GrappleRacerNative")]
	public static extern void TrackUtils_SplineUp(out Vector3 o_ret, ref Vector3 u0, ref Vector3 u1, ref float unitT);

	[DllImport("GrappleRacerNative")]
	public static extern void TrackUtils_SplineNormal(out Vector3 o_ret, ref Vector3 p0, ref Vector3 m0, ref Vector3 u0, ref Vector3 p1, ref Vector3 m1, ref Vector3 u1, ref float unitT);

	[DllImport("GrappleRacerNative")]
	public static extern void TrackUtils_SplineNormal2(out Vector3 o_ret, ref Vector3 tangent, ref Vector3 up);

	[DllImport("GrappleRacerNative")]
	public static extern void TrackUtils_SplineWidth(out float o_ret, ref float w0, ref float w1, ref float unitT);

	[DllImport("GrappleRacerNative")]
	public static extern void TrackUtils_SplineRoofHeight(out float o_ret, ref float h0, ref float h1, ref float unitT);

	[DllImport("GrappleRacerNative")]
	public static extern void TrackUtils_SplineDsDt(out float o_ret, ref Vector3 p0, ref Vector3 m0, ref Vector3 p1, ref Vector3 m1, ref float unitT);

	[DllImport("GrappleRacerNative")]
	public static extern void TrackUtils_SplineCurve(out float o_ret, ref Vector3 p0, ref Vector3 m0, ref Vector3 u0, ref Vector3 p1, ref Vector3 m1, ref Vector3 u1, ref float unitT);

	[DllImport("GrappleRacerNative")]
	private static extern void TrackUtils_PointAtT(out Vector3 ret, ref TrackNodeCache nodes, int numNodes, float t);

	[DllImport("GrappleRacerNative")]
	private static extern void TrackUtils_TangentAtT(out Vector3 ret, ref TrackNodeCache nodes, int numNodes, float t);

	[DllImport("GrappleRacerNative")]
	private static extern void TrackUtils_TangentDTAtT(out Vector3 ret, ref TrackNodeCache nodes, int numNodes, float t);

	[DllImport("GrappleRacerNative")]
	private static extern void TrackUtils_UpAtT(out Vector3 ret, ref TrackNodeCache nodes, int numNodes, float t);

	[DllImport("GrappleRacerNative")]
	private static extern void TrackUtils_NormalAtT(out Vector3 ret, ref TrackNodeCache nodes, int numNodes, float t);

	[DllImport("GrappleRacerNative")]
	private static extern void TrackUtils_DsDtAtT(out float ret, ref TrackNodeCache nodes, int numNodes, float t);

	[DllImport("GrappleRacerNative")]
	private static extern void TrackUtils_CurveAtT(out float ret, ref TrackNodeCache nodes, int numNodes, float t);

	[DllImport("GrappleRacerNative")]
	private static extern void TrackUtils_InfoAtT(ref TrackNodeCache _nodes, int _numNodes, float _t, out Vector3 o_point, out Vector3 o_tangent, out Vector3 o_normal, out float o_width);

	[DllImport("GrappleRacerNative")]
	private static extern float TrackUtils_ClosestTToPoint(ref TrackNodeCache nodes, int numNodes, ref Vector3 point, float tHint, float tHintSpread, float tLowerBound, float tUpperBound);

	[DllImport("GrappleRacerNative")]
	private static extern float TrackUtils_ReParamStoT(ref ReParamNode nodes, float clampedS);

	[DllImport("GrappleRacerNative")]
	private static extern float TrackUtils_ReParamTtoS(ref ReParamNode nodes, float clampedT);

	public static Vector3 SplinePoint(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1, float unitT)
	{
		Vector3 o_ret;
		TrackUtils_SplinePoint(out o_ret, ref p0, ref m0, ref p1, ref m1, ref unitT);
		return o_ret;
	}

	public static Vector3 SplineTangent(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1, float unitT)
	{
		Vector3 o_ret;
		TrackUtils_SplineTangent(out o_ret, ref p0, ref m0, ref p1, ref m1, ref unitT);
		return o_ret;
	}

	public static Vector3 SplineTangentDT(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1, float unitT)
	{
		Vector3 o_ret;
		TrackUtils_SplineTangentDT(out o_ret, ref p0, ref m0, ref p1, ref m1, ref unitT);
		return o_ret;
	}

	public static Vector3 SplineUp(Vector3 u0, Vector3 u1, float unitT)
	{
		Vector3 o_ret;
		TrackUtils_SplineUp(out o_ret, ref u0, ref u1, ref unitT);
		return o_ret;
	}

	public static Vector3 SplineNormal(Vector3 p0, Vector3 m0, Vector3 u0, Vector3 p1, Vector3 m1, Vector3 u1, float unitT)
	{
		Vector3 o_ret;
		TrackUtils_SplineNormal(out o_ret, ref p0, ref m0, ref u0, ref p1, ref m1, ref u1, ref unitT);
		return o_ret;
	}

	public static Vector3 SplineNormal(Vector3 tangent, Vector3 up)
	{
		Vector3 o_ret;
		TrackUtils_SplineNormal2(out o_ret, ref tangent, ref up);
		return o_ret;
	}

	public static float SplineWidth(float w0, float w1, float unitT)
	{
		float o_ret;
		TrackUtils_SplineWidth(out o_ret, ref w0, ref w1, ref unitT);
		return o_ret;
	}

	public static float SplineRoofHeight(float h0, float h1, float unitT)
	{
		float o_ret;
		TrackUtils_SplineRoofHeight(out o_ret, ref h0, ref h1, ref unitT);
		return o_ret;
	}

	public static float SplineDsDt(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1, float unitT)
	{
		float o_ret;
		TrackUtils_SplineDsDt(out o_ret, ref p0, ref m0, ref p1, ref m1, ref unitT);
		return o_ret;
	}

	public static float SplineCurve(Vector3 p0, Vector3 m0, Vector3 u0, Vector3 p1, Vector3 m1, Vector3 u1, float unitT)
	{
		float o_ret;
		TrackUtils_SplineCurve(out o_ret, ref p0, ref m0, ref u0, ref p1, ref m1, ref u1, ref unitT);
		return o_ret;
	}

	public static Vector3 PointAtT(TrackNodeCache[] nodes, float t)
	{
		Vector3 ret;
		TrackUtils_PointAtT(out ret, ref nodes[0], nodes.Length, t);
		return ret;
	}

	public static Vector3 TangentAtT(TrackNodeCache[] nodes, float t)
	{
		Vector3 ret;
		TrackUtils_TangentAtT(out ret, ref nodes[0], nodes.Length, t);
		return ret;
	}

	public static Vector3 TangentDTAtT(TrackNodeCache[] nodes, float t)
	{
		Vector3 ret;
		TrackUtils_TangentDTAtT(out ret, ref nodes[0], nodes.Length, t);
		return ret;
	}

	public static Vector3 UpAtT(TrackNodeCache[] nodes, float t)
	{
		Vector3 ret;
		TrackUtils_UpAtT(out ret, ref nodes[0], nodes.Length, t);
		return ret;
	}

	public static Vector3 NormalAtT(TrackNodeCache[] nodes, float t)
	{
		Vector3 ret;
		TrackUtils_NormalAtT(out ret, ref nodes[0], nodes.Length, t);
		return ret;
	}

	public static float DsDtAtT(TrackNodeCache[] nodes, float t)
	{
		float ret;
		TrackUtils_DsDtAtT(out ret, ref nodes[0], nodes.Length, t);
		return ret;
	}

	public static float CurveAtT(TrackNodeCache[] nodes, float t)
	{
		float ret;
		TrackUtils_CurveAtT(out ret, ref nodes[0], nodes.Length, t);
		return ret;
	}

	public static void InfoAtT(TrackNodeCache[] nodes, float _t, out Vector3 o_point, out Vector3 o_tangent, out Vector3 o_normal, out float o_width)
	{
		TrackUtils_InfoAtT(ref nodes[0], nodes.Length, _t, out o_point, out o_tangent, out o_normal, out o_width);
	}

	public static float ClosestTToPoint(TrackNodeCache[] nodes, Vector3 _point, float _tHint, float _tHintSpread, float _tLowerBound, float _tUpperBound)
	{
		return TrackUtils_ClosestTToPoint(ref nodes[0], nodes.Length, ref _point, _tHint, _tHintSpread, _tLowerBound, _tUpperBound);
	}

	public static float ReParamStoT(ReParamNode[] nodes, float clampedS)
	{
		return TrackUtils_ReParamStoT(ref nodes[0], clampedS);
	}

	public static float ReParamTtoS(ReParamNode[] nodes, float clampedT)
	{
		return TrackUtils_ReParamTtoS(ref nodes[0], clampedT);
	}
}
