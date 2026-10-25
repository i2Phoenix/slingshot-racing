using System.Collections.Generic;
using UnityEngine;

public class TrackUtils : TrackUtilsBase
{
	private const float c_lengthIntegralStep = 0.01f;

	public const float c_splineIntervalT = 0.05f;

	public const float c_splineIntervalW = 0.2f;

	public const float c_vScale = 0.2f;

	public const float c_defaultLightmapPadding = 6f;

	public const float c_maxDivisionError = 0.025f;

	private const float c_maxDivisionT = 0.25f;

	public const int c_numDivisionsT = 20;

	public const int c_numDivisionsW = 11;

	private const int c_defaultTraverseDepth = 2;

	private static float[] s_defaultCrossSectionDistribution = null;

	private static float[] s_crossSectionDistribution = new float[11];

	public static float SplineHeight(TrackCrossSection c0, TrackCrossSection c1, float widthProp, float unitT, bool _reversedTrack)
	{
		float widthProp2 = ((!_reversedTrack) ? widthProp : (0f - widthProp));
		float num = ((!(c0 == null)) ? c0.Evaluate(widthProp2) : 0f);
		if (c1 != c0)
		{
			float num2 = ((!(c1 == null)) ? c1.Evaluate(widthProp2) : 0f);
			num += (num2 - num) * unitT;
		}
		return num;
	}

	public static float SplineLength(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1, float unitTMin, float unitTMax)
	{
		float num = 0f;
		for (float num2 = unitTMin; num2 < unitTMax; num2 += 0.01f)
		{
			float num3 = TrackUtilsBase.SplineDsDt(p0, m0, p1, m1, num2) * 0.01f;
			num += num3;
		}
		return num;
	}

	public static float SplineLength(TrackNode[] _nodes)
	{
		float num = 0f;
		for (int i = 1; i < _nodes.Length; i++)
		{
			TrackNode trackNode = _nodes[i - 1];
			TrackNode trackNode2 = _nodes[i];
			num += SplineLength(trackNode.position, trackNode.tangent, trackNode2.position, trackNode2.tangent, 0f, 1f);
		}
		return num;
	}

	public static float MaxSplineWidth(TrackNode[] _nodes)
	{
		float num = 0f;
		for (int i = 0; i < _nodes.Length; i++)
		{
			num = Mathf.Max(num, _nodes[i].width);
		}
		return num;
	}

	private static float DVal(TrackNode[] nodes, Vector3 point, float t)
	{
		float num = nodes.Length - 1;
		Vector3 b;
		if (t < 0f)
		{
			b = nodes[0].position + nodes[0].tangent * t;
		}
		else if (t > num)
		{
			int num2 = nodes.Length - 1;
			b = nodes[num2].position + nodes[num2].tangent * (t - num);
		}
		else
		{
			b = PointAtT(nodes, t);
		}
		return Vector3Utils.DistanceSqr(point, b);
	}

	private static float DValDT(TrackNode[] nodes, Vector3 point, float t)
	{
		TrackNode o_node;
		TrackNode o_node2;
		float o_unitT;
		NodeInfoAtT(nodes, t, out o_node, out o_node2, out o_unitT);
		Vector3 position = o_node.position;
		Vector3 tangent = o_node.tangent;
		Vector3 position2 = o_node2.position;
		Vector3 tangent2 = o_node2.tangent;
		Vector3 a = SplineUtils.CubicHermite(position, tangent, position2, tangent2, o_unitT);
		Vector3 b = SplineUtils.CubicHermiteDT(position, tangent, position2, tangent2, o_unitT);
		Vector3 vector = Vector3.Scale(a, b) - Vector3.Scale(point, b);
		return 2f * (vector.x + vector.y + vector.z);
	}

	private static void DValDT_DValDT2(TrackNode[] nodes, Vector3 point, float t, out float o_dValDT, out float o_dValDT2)
	{
		TrackNode o_node;
		TrackNode o_node2;
		float o_unitT;
		NodeInfoAtT(nodes, t, out o_node, out o_node2, out o_unitT);
		Vector3 position = o_node.position;
		Vector3 tangent = o_node.tangent;
		Vector3 position2 = o_node2.position;
		Vector3 tangent2 = o_node2.tangent;
		Vector3 a = SplineUtils.CubicHermite(position, tangent, position2, tangent2, o_unitT);
		Vector3 vector = SplineUtils.CubicHermiteDT(position, tangent, position2, tangent2, o_unitT);
		Vector3 b = SplineUtils.CubicHermiteDT2(position, tangent, position2, tangent2, o_unitT);
		Vector3 vector2 = Vector3.Scale(a, vector) - Vector3.Scale(point, vector);
		Vector3 vector3 = Vector3.Scale(vector, vector) + Vector3.Scale(a, b) - Vector3.Scale(point, b);
		o_dValDT = 2f * (vector2.x + vector2.y + vector2.z);
		o_dValDT2 = 2f * (vector3.x + vector3.y + vector3.z);
	}

	private static float NextSNewton(TrackNode[] nodes, CPData cpData0, Vector3 point)
	{
		float t = cpData0.t;
		float o_dValDT;
		float o_dValDT2;
		DValDT_DValDT2(nodes, point, t, out o_dValDT, out o_dValDT2);
		return t - o_dValDT / o_dValDT2;
	}

	public static float ClosestTToPoint(TrackNode[] nodes, Vector3 point, float tHint, float _tHintSpread, float tLowerBound, float tUpperBound)
	{
		CPData3 cpData = default(CPData3);
		float t = Mathf.Clamp(tHint - _tHintSpread, tLowerBound, tUpperBound);
		float t2 = Mathf.Clamp(tHint + _tHintSpread, tLowerBound, tUpperBound);
		float d = DVal(nodes, point, t);
		float d2 = DVal(nodes, point, tHint);
		float d3 = DVal(nodes, point, t2);
		cpData.m_0 = new CPData(t, d);
		cpData.m_1 = new CPData(tHint, d2);
		cpData.m_2 = new CPData(t2, d3);
		cpData.Sort();
		int num = 0;
		while (num < 16)
		{
			num++;
			float value = TrackUtilsBase.NextSQuadratic(cpData);
			value = Mathf.Clamp(value, tLowerBound, tUpperBound);
			float num2 = DVal(nodes, point, value);
			if (num2 < cpData.m_2.d - 0.01f)
			{
				cpData.m_2 = new CPData(value, num2);
				cpData.Sort();
				continue;
			}
			break;
		}
		while (num < 16)
		{
			num++;
			float value2 = NextSNewton(nodes, cpData.m_0, point);
			value2 = Mathf.Clamp(value2, tLowerBound, tUpperBound);
			float num3 = DVal(nodes, point, value2);
			if (num3 < cpData.m_0.d - 0.01f)
			{
				cpData.m_0 = new CPData(value2, num3);
				continue;
			}
			break;
		}
		float num4 = ((!(DValDT(nodes, point, cpData.m_0.t) > 0f)) ? 0.01f : (-0.01f));
		while (num < 16)
		{
			num++;
			float value3 = cpData.m_0.t + num4;
			value3 = Mathf.Clamp(value3, tLowerBound, tUpperBound);
			float num5 = DVal(nodes, point, value3);
			if (num5 < cpData.m_0.d - 0.01f)
			{
				cpData.m_0 = new CPData(value3, num5);
				continue;
			}
			break;
		}
		return cpData.m_0.t;
	}

	public static void NodeInfoAtT(TrackNode[] nodes, float t, out TrackNode o_node0, out TrackNode o_node1, out float o_unitT)
	{
		int i = (int)t;
		int num = MathUtils.Clamp(i, 0, nodes.Length - 2);
		o_unitT = Mathf.Clamp01(t - (float)num);
		o_node0 = nodes[num];
		o_node1 = nodes[num + 1];
	}

	public static Vector3 PointAtT(TrackNode[] nodes, float t)
	{
		TrackNode o_node;
		TrackNode o_node2;
		float o_unitT;
		NodeInfoAtT(nodes, t, out o_node, out o_node2, out o_unitT);
		Vector3 position = o_node.position;
		Vector3 tangent = o_node.tangent;
		Vector3 position2 = o_node2.position;
		Vector3 tangent2 = o_node2.tangent;
		return SplineUtils.CubicHermite(position, tangent, position2, tangent2, o_unitT);
	}

	public static Vector3 TangentAtT(TrackNode[] nodes, float t)
	{
		TrackNode o_node;
		TrackNode o_node2;
		float o_unitT;
		NodeInfoAtT(nodes, t, out o_node, out o_node2, out o_unitT);
		Vector3 position = o_node.position;
		Vector3 tangent = o_node.tangent;
		Vector3 position2 = o_node2.position;
		Vector3 tangent2 = o_node2.tangent;
		return SplineUtils.CubicHermiteDT(position, tangent, position2, tangent2, o_unitT);
	}

	public static Vector3 TangentDTAtT(TrackNode[] nodes, float t)
	{
		TrackNode o_node;
		TrackNode o_node2;
		float o_unitT;
		NodeInfoAtT(nodes, t, out o_node, out o_node2, out o_unitT);
		Vector3 position = o_node.position;
		Vector3 tangent = o_node.tangent;
		Vector3 position2 = o_node2.position;
		Vector3 tangent2 = o_node2.tangent;
		return SplineUtils.CubicHermiteDT2(position, tangent, position2, tangent2, o_unitT);
	}

	public static Vector3 UpAtT(TrackNode[] nodes, float t)
	{
		TrackNode o_node;
		TrackNode o_node2;
		float o_unitT;
		NodeInfoAtT(nodes, t, out o_node, out o_node2, out o_unitT);
		return TrackUtilsBase.SplineUp(o_node.up, o_node2.up, o_unitT);
	}

	public static Vector3 NormalAtT(TrackNode[] nodes, float t)
	{
		TrackNode o_node;
		TrackNode o_node2;
		float o_unitT;
		NodeInfoAtT(nodes, t, out o_node, out o_node2, out o_unitT);
		Vector3 position = o_node.position;
		Vector3 tangent = o_node.tangent;
		Vector3 up = o_node.up;
		Vector3 position2 = o_node2.position;
		Vector3 tangent2 = o_node2.tangent;
		Vector3 up2 = o_node2.up;
		return TrackUtilsBase.SplineNormal(position, tangent, up, position2, tangent2, up2, o_unitT);
	}

	public static float WidthAtT(TrackNode[] _nodes, float _t)
	{
		TrackNode o_node;
		TrackNode o_node2;
		float o_unitT;
		NodeInfoAtT(_nodes, _t, out o_node, out o_node2, out o_unitT);
		return TrackUtilsBase.SplineWidth(o_node.width, o_node2.width, o_unitT);
	}

	public static float HeightAtT(TrackNode[] nodes, float t, float widthProp, bool _reversedTrack)
	{
		TrackNode o_node;
		TrackNode o_node2;
		float o_unitT;
		NodeInfoAtT(nodes, t, out o_node, out o_node2, out o_unitT);
		return SplineHeight(o_node.crossSection, o_node2.crossSection, widthProp, o_unitT, _reversedTrack);
	}

	public static float[] DefaultCrossSectionDistribution()
	{
		if (s_defaultCrossSectionDistribution == null)
		{
			s_defaultCrossSectionDistribution = new float[11];
			float num = 0.2f;
			for (int i = 0; i != 11; i++)
			{
				s_defaultCrossSectionDistribution[i] = (float)i * num - 1f;
			}
		}
		return s_defaultCrossSectionDistribution;
	}

	public static float[] CrossSectionDistributionAtT(TrackNode[] nodes, float t)
	{
		TrackNode o_node;
		TrackNode o_node2;
		float o_unitT;
		NodeInfoAtT(nodes, t, out o_node, out o_node2, out o_unitT);
		float[] array = ((!(o_node.crossSection != null)) ? DefaultCrossSectionDistribution() : o_node.crossSection.GetDistribution(11));
		float[] array2 = ((!(o_node2.crossSection != null)) ? DefaultCrossSectionDistribution() : o_node2.crossSection.GetDistribution(11));
		for (int i = 0; i != 11; i++)
		{
			s_crossSectionDistribution[i] = Mathf.Lerp(array[i], array2[i], o_unitT);
		}
		return s_crossSectionDistribution;
	}

	public static float DsDtAtT(TrackNode[] nodes, float t)
	{
		TrackNode o_node;
		TrackNode o_node2;
		float o_unitT;
		NodeInfoAtT(nodes, t, out o_node, out o_node2, out o_unitT);
		return TrackUtilsBase.SplineDsDt(o_node.position, o_node.tangent, o_node2.position, o_node2.tangent, o_unitT);
	}

	public static float CurveAtT(TrackNode[] nodes, float t)
	{
		TrackNode o_node;
		TrackNode o_node2;
		float o_unitT;
		NodeInfoAtT(nodes, t, out o_node, out o_node2, out o_unitT);
		return TrackUtilsBase.SplineCurve(o_node.position, o_node.tangent, o_node.up, o_node2.position, o_node2.tangent, o_node2.up, o_unitT);
	}

	public static void InfoAtT(TrackNode[] nodes, float t, out Vector3 o_point, out Vector3 o_tangent, out Vector3 o_normal, out float o_width)
	{
		TrackNode o_node;
		TrackNode o_node2;
		float o_unitT;
		NodeInfoAtT(nodes, t, out o_node, out o_node2, out o_unitT);
		Vector3 position = o_node.position;
		Vector3 tangent = o_node.tangent;
		Vector3 up = o_node.up;
		Vector3 position2 = o_node2.position;
		Vector3 tangent2 = o_node2.tangent;
		Vector3 up2 = o_node2.up;
		o_point = SplineUtils.CubicHermite(position, tangent, position2, tangent2, o_unitT);
		Vector3 vector = SplineUtils.CubicHermiteDT(position, tangent, position2, tangent2, o_unitT);
		Vector3 up3 = TrackUtilsBase.SplineUp(up, up2, o_unitT);
		o_tangent = vector;
		o_normal = TrackUtilsBase.SplineNormal(vector, up3);
		o_width = TrackUtilsBase.SplineWidth(o_node.width, o_node2.width, o_unitT);
	}

	public static void InfoAtT(TrackNode[] nodes, float t, out Vector3 o_point, out Vector3 o_tangent, out Vector3 o_normal, out float o_width, out float o_roofHeight)
	{
		TrackNode o_node;
		TrackNode o_node2;
		float o_unitT;
		NodeInfoAtT(nodes, t, out o_node, out o_node2, out o_unitT);
		Vector3 position = o_node.position;
		Vector3 tangent = o_node.tangent;
		Vector3 up = o_node.up;
		Vector3 position2 = o_node2.position;
		Vector3 tangent2 = o_node2.tangent;
		Vector3 up2 = o_node2.up;
		o_point = SplineUtils.CubicHermite(position, tangent, position2, tangent2, o_unitT);
		Vector3 vector = SplineUtils.CubicHermiteDT(position, tangent, position2, tangent2, o_unitT);
		Vector3 up3 = TrackUtilsBase.SplineUp(up, up2, o_unitT);
		o_tangent = vector;
		o_normal = TrackUtilsBase.SplineNormal(vector, up3);
		o_width = TrackUtilsBase.SplineWidth(o_node.width, o_node2.width, o_unitT);
		o_roofHeight = TrackUtilsBase.SplineRoofHeight(o_node.m_roofHeight, o_node2.m_roofHeight, o_unitT);
	}

	public static float[] SplineTangentialDivisions(TrackNode[] nodes, TrackEdgeData _trackEdgeData = null)
	{
		return SplineTangentialDivisions(nodes, 0.025f, _trackEdgeData);
	}

	public static float[] SplineTangentialDivisions(TrackNode[] nodes, float _maxDivisionError, TrackEdgeData _trackEdgeData = null)
	{
		List<float> list = new List<float>();
		for (int i = 0; i < nodes.Length - 1; i++)
		{
			float num = i;
			for (int j = 0; j < 20; j++)
			{
				float item = num + (float)j / 20f;
				list.Add(item);
			}
		}
		list.Add(nodes.Length - 1);
		int num2 = 1;
		while (num2 < list.Count - 1)
		{
			float num3 = list[num2 - 1];
			float num4 = list[num2 + 1];
			if (num4 - num3 > 0.25f)
			{
				num2++;
				continue;
			}
			if (_trackEdgeData != null)
			{
				bool flag = _trackEdgeData.GetInnerWallType(num3) == TrackEdgeType.Air && _trackEdgeData.GetOuterWallType(num3) == TrackEdgeType.Air;
				bool flag2 = _trackEdgeData.GetInnerWallType(num4) == TrackEdgeType.Air && _trackEdgeData.GetOuterWallType(num4) == TrackEdgeType.Air;
				if (flag != flag2)
				{
					num2++;
					continue;
				}
			}
			float t = list[num2];
			Vector3 vector = PointAtT(nodes, num3);
			Vector3 vector2 = PointAtT(nodes, t);
			Vector3 vector3 = PointAtT(nodes, num4);
			Vector3 normalized = (vector3 - vector).normalized;
			float num5 = Vector3.Dot(normalized, vector);
			float num6 = Vector3.Dot(normalized, vector2);
			Vector3 a = vector + normalized * (num6 - num5);
			float num7 = Vector3.Distance(a, vector2);
			if (num7 < _maxDivisionError)
			{
				list.RemoveAt(num2);
			}
			else
			{
				num2++;
			}
		}
		return list.ToArray();
	}

	public static void SplineGeometryRequirements(float[] tangentialDivisions, out int o_numVertices, out int o_numIndices)
	{
		o_numVertices = tangentialDivisions.Length * 11;
		o_numIndices = (tangentialDivisions.Length - 1) * 10 * 6;
	}

	public static void SplineLightmapGeometryRequirements(float[] tangentialDivisions, out int o_numVertices, out int o_numIndices)
	{
		o_numVertices = (tangentialDivisions.Length + 2) * 11;
		o_numIndices = (tangentialDivisions.Length + 1) * 10 * 6;
	}

	public static void SplineCollisionGeometryRequirements(float[] tangentialDivisions, out int o_numVertices, out int o_numIndices)
	{
		o_numVertices = tangentialDivisions.Length * 11;
		o_numIndices = (tangentialDivisions.Length - 1) * 10 * 6;
	}

	public static void SplineSurroundCollisionGeometryRequirements(float[] tangentialDivisions, out int o_numVertices, out int o_numIndices)
	{
		o_numVertices = tangentialDivisions.Length * 4;
		o_numIndices = (tangentialDivisions.Length - 1) * 3 * 6;
	}

	public static void SplineGeometryRequirementsQuads(float[] tangentialDivisions, out int o_numVertices, out int o_numIndices)
	{
		o_numVertices = tangentialDivisions.Length * 11;
		o_numIndices = (tangentialDivisions.Length - 1) * 10 * 4;
	}

	public static void AppendSplineGeometry(Transform transform, TrackNode[] nodes, float[] tangentialDivisions, int vertexOffset, int indexOffset, Vector3[] o_vertices, Vector2[] o_uv, Vector2[] o_uv2, int[] o_triangles, bool _reversedTrack)
	{
		FillVertexData(transform, nodes, tangentialDivisions, vertexOffset, o_vertices, o_uv, o_uv2, _reversedTrack);
		FillTris(nodes, tangentialDivisions.Length, vertexOffset, indexOffset, o_triangles);
	}

	public static void AppendSplineGeometry(Transform transform, TrackNode[] nodes, TrackEdgeData _trackEdgeData, float[] tangentialDivisions, int vertexOffset, int indexOffset, Vector3[] o_vertices, Vector2[] o_uv, Vector2[] o_uv2, int[] o_triangles, bool _reversedTrack)
	{
		FillVertexData(transform, nodes, tangentialDivisions, vertexOffset, o_vertices, o_uv, o_uv2, _reversedTrack);
		FillTris(nodes, _trackEdgeData, tangentialDivisions, tangentialDivisions.Length, vertexOffset, indexOffset, o_triangles);
	}

	public static void AppendSplineLightmapGeometry(TrackNode[] nodes, float[] tangentialDivisions, TrackLightingTweak _lightingTweak, Vector3[] o_vertices, Vector2[] o_uv2, int[] o_triangles, out Vector2 o_uvBounds)
	{
		FillLightmapVertexData(nodes, tangentialDivisions, _lightingTweak, o_vertices, o_uv2, out o_uvBounds);
		FillTris(nodes, tangentialDivisions.Length + 2, 0, 0, o_triangles);
	}

	public static void AppendSplineGeometryQuad(Transform transform, TrackNode[] nodes, float[] tangentialDivisions, int vertexOffset, int indexOffset, Vector3[] o_vertices, Vector2[] o_uv, Vector2[] o_uv2, int[] o_quads, bool _reversedTrack)
	{
		FillVertexData(transform, nodes, tangentialDivisions, vertexOffset, o_vertices, o_uv, o_uv2, _reversedTrack);
		FillQuads(nodes, tangentialDivisions.Length, vertexOffset, indexOffset, o_quads);
	}

	private static void FillVertexData(Transform transform, TrackNode[] nodes, float[] tangentialDivisions, int vertexOffset, Vector3[] o_vertices, Vector2[] o_uv, Vector2[] o_uv2, bool _reversedTrack)
	{
		int num = tangentialDivisions.Length * 11;
		float num2 = 0f;
		float num3 = 6f;
		float num4 = 0f;
		int num5 = vertexOffset;
		Vector3 vector = nodes[0].position;
		foreach (float t in tangentialDivisions)
		{
			Vector3 o_point;
			Vector3 o_tangent;
			Vector3 o_normal;
			float o_width;
			InfoAtT(nodes, t, out o_point, out o_tangent, out o_normal, out o_width);
			Vector3 vector2 = UpAtT(nodes, t);
			num4 = Mathf.Max(num4, o_width);
			Vector3 vector3 = o_point;
			float magnitude = (vector3 - vector).magnitude;
			num2 += magnitude;
			num3 += magnitude;
			vector = vector3;
			float[] array = CrossSectionDistributionAtT(nodes, t);
			for (int j = 0; j < 11; j++)
			{
				float num6 = array[j];
				float num7 = num6 * 0.5f + 0.5f;
				float x = num7 * 0.9f + 0.05f;
				float num8 = HeightAtT(nodes, t, num6, _reversedTrack);
				Vector3 vector4 = o_point + num6 * o_width * o_normal + vector2 * num8;
				if (o_vertices != null)
				{
					o_vertices[num5] = ((!(transform == null)) ? transform.TransformPoint(vector4) : vector4);
				}
				if (o_uv != null)
				{
					o_uv[num5] = new Vector2(x, num2);
				}
				if (o_uv2 != null)
				{
					o_uv2[num5] = new Vector2(num7, num3);
				}
				num5++;
			}
		}
		num3 += 6f;
		if (o_uv != null)
		{
			int num9 = vertexOffset;
			float num10 = Mathf.Max(1f, Mathf.Round(num2 * 0.2f));
			for (int k = 0; k < num; k++)
			{
				o_uv[num9].y = o_uv[num9].y / num2 * num10;
				num9++;
			}
		}
		if (o_uv2 != null)
		{
			int num11 = vertexOffset;
			float num12 = num4 * 2f;
			float num13 = Mathf.Max(num12, num3);
			float num14 = num12 / num13;
			float num15 = 1f / num13;
			for (int l = 0; l < num; l++)
			{
				o_uv2[num11].x = Mathf.Clamp01(o_uv2[num11].x * num14);
				o_uv2[num11].y = Mathf.Clamp01(o_uv2[num11].y * num15);
				num11++;
			}
		}
	}

	private static void FillLightmapVertexData(TrackNode[] nodes, float[] tangentialDivisions, TrackLightingTweak _lightingTweak, Vector3[] o_vertices, Vector2[] o_uv2, out Vector2 o_uvBounds)
	{
		int num = (tangentialDivisions.Length + 2) * 11;
		float num2 = 0f;
		float num3 = 0f;
		int num4 = 0;
		float t = 0f;
		Vector3 o_point;
		Vector3 o_tangent;
		Vector3 o_normal;
		float o_width;
		InfoAtT(nodes, t, out o_point, out o_tangent, out o_normal, out o_width);
		Vector3 vector = UpAtT(nodes, t);
		float num5 = ((!(_lightingTweak == null)) ? _lightingTweak.HeightAt(t) : 0f);
		o_point -= o_tangent.normalized * 6f;
		float[] array = CrossSectionDistributionAtT(nodes, t);
		for (int i = 0; i < 11; i++)
		{
			float num6 = array[i];
			float x = num6 * 0.5f + 0.5f;
			float num7 = HeightAtT(nodes, t, num6, false) + num5;
			Vector3 vector2 = o_point + num6 * o_width * o_normal + vector * num7;
			if (o_vertices != null)
			{
				o_vertices[num4] = vector2;
			}
			if (o_uv2 != null)
			{
				o_uv2[num4] = new Vector2(x, num2);
			}
			num4++;
		}
		num2 += 6f;
		Vector3 vector3 = nodes[0].position;
		foreach (float t2 in tangentialDivisions)
		{
			Vector3 o_point2;
			Vector3 o_tangent2;
			Vector3 o_normal2;
			float o_width2;
			InfoAtT(nodes, t2, out o_point2, out o_tangent2, out o_normal2, out o_width2);
			Vector3 vector4 = UpAtT(nodes, t2);
			float num8 = ((!(_lightingTweak == null)) ? _lightingTweak.HeightAt(t2) : 0f);
			num3 = Mathf.Max(num3, o_width2);
			Vector3 vector5 = o_point2;
			float magnitude = (vector5 - vector3).magnitude;
			num2 += magnitude;
			vector3 = vector5;
			float[] array2 = CrossSectionDistributionAtT(nodes, t2);
			for (int k = 0; k < 11; k++)
			{
				float num9 = array2[k];
				float x2 = num9 * 0.5f + 0.5f;
				float num10 = HeightAtT(nodes, t2, num9, false) + num8;
				Vector3 vector6 = o_point2 + num9 * o_width2 * o_normal2 + vector4 * num10;
				if (o_vertices != null)
				{
					o_vertices[num4] = vector6;
				}
				if (o_uv2 != null)
				{
					o_uv2[num4] = new Vector2(x2, num2);
				}
				num4++;
			}
		}
		num2 += 6f;
		float t3 = nodes.Length - 1;
		Vector3 o_point3;
		Vector3 o_tangent3;
		Vector3 o_normal3;
		float o_width3;
		InfoAtT(nodes, t3, out o_point3, out o_tangent3, out o_normal3, out o_width3);
		Vector3 vector7 = UpAtT(nodes, t3);
		float num11 = ((!(_lightingTweak == null)) ? _lightingTweak.HeightAt(t3) : 0f);
		o_point3 += o_tangent3.normalized * 6f;
		float[] array3 = CrossSectionDistributionAtT(nodes, t3);
		for (int l = 0; l < 11; l++)
		{
			float num12 = array3[l];
			float x3 = num12 * 0.5f + 0.5f;
			float num13 = HeightAtT(nodes, t3, num12, false) + num11;
			Vector3 vector8 = o_point3 + num12 * o_width3 * o_normal3 + vector7 * num13;
			if (o_vertices != null)
			{
				o_vertices[num4] = vector8;
			}
			if (o_uv2 != null)
			{
				o_uv2[num4] = new Vector2(x3, num2);
			}
			num4++;
		}
		int num14 = 0;
		float num15 = num3 * 2f;
		float num16 = Mathf.Max(num15, num2);
		float num17 = num15 / num16;
		float num18 = 1f / num16;
		for (int m = 0; m < num; m++)
		{
			o_uv2[num14].x = Mathf.Clamp01(o_uv2[num14].x * num17);
			o_uv2[num14].y = Mathf.Clamp01(o_uv2[num14].y * num18);
			num14++;
		}
		o_uvBounds = new Vector2(num15 / num16, num2 / num16);
	}

	private static void FillTris(TrackNode[] nodes, int numTangentialDivisions, int vertexOffset, int indexOffset, int[] o_triangles)
	{
		int num = (numTangentialDivisions - 1) * 10 * 6;
		int num2 = indexOffset;
		int num3 = vertexOffset;
		for (int i = 0; i < numTangentialDivisions - 1; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				o_triangles[num2++] = num3 + j;
				o_triangles[num2++] = num3 + j + 11;
				o_triangles[num2++] = num3 + j + 1;
				o_triangles[num2++] = num3 + j + 1;
				o_triangles[num2++] = num3 + j + 11;
				o_triangles[num2++] = num3 + j + 11 + 1;
			}
			num3 += 11;
		}
	}

	private static void FillTris(TrackNode[] nodes, TrackEdgeData _trackEdgeData, float[] tangentialDivisions, int numTangentialDivisions, int vertexOffset, int indexOffset, int[] o_triangles)
	{
		int num = (numTangentialDivisions - 1) * 10 * 6;
		int num2 = indexOffset;
		int num3 = vertexOffset;
		for (int i = 0; i < numTangentialDivisions - 1; i++)
		{
			float t = tangentialDivisions[i];
			if (_trackEdgeData.GetInnerWallType(t) != TrackEdgeType.Air || _trackEdgeData.GetOuterWallType(t) != TrackEdgeType.Air)
			{
				for (int j = 0; j < 10; j++)
				{
					o_triangles[num2++] = num3 + j;
					o_triangles[num2++] = num3 + j + 11;
					o_triangles[num2++] = num3 + j + 1;
					o_triangles[num2++] = num3 + j + 1;
					o_triangles[num2++] = num3 + j + 11;
					o_triangles[num2++] = num3 + j + 11 + 1;
				}
			}
			num3 += 11;
		}
		for (int k = num2; k < indexOffset + num; k++)
		{
			o_triangles[num2++] = 0;
		}
	}

	private static void FillQuads(TrackNode[] nodes, int numTangentialDivisions, int vertexOffset, int indexOffset, int[] o_quads)
	{
		int num = (numTangentialDivisions - 1) * 10 * 4;
		int num2 = indexOffset;
		int num3 = vertexOffset;
		for (int i = 0; i < numTangentialDivisions - 1; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				o_quads[num2++] = num3 + j;
				o_quads[num2++] = num3 + j + 11;
				o_quads[num2++] = num3 + j + 11 + 1;
				o_quads[num2++] = num3 + j + 1;
			}
			num3 += 11;
		}
	}

	public static float AppendSplineCollisionGeometry(TrackNode[] nodes, float[] tangentialDivisions, int vertexOffset, int indexOffset, Vector3[] o_vertices, int[] o_triangles, bool _reversedTrack)
	{
		int num = tangentialDivisions.Length * 11;
		int num2 = (tangentialDivisions.Length - 1) * 10 * 6;
		float num3 = float.MaxValue;
		int num4 = vertexOffset;
		foreach (float t in tangentialDivisions)
		{
			Vector3 o_point;
			Vector3 o_tangent;
			Vector3 o_normal;
			float o_width;
			float o_roofHeight;
			InfoAtT(nodes, t, out o_point, out o_tangent, out o_normal, out o_width, out o_roofHeight);
			Vector3 vector = UpAtT(nodes, t);
			float[] array = CrossSectionDistributionAtT(nodes, t);
			for (int j = 0; j < 11; j++)
			{
				float num5 = array[j];
				float num6 = HeightAtT(nodes, t, num5, _reversedTrack);
				num3 = Mathf.Min(num3, num6);
				switch (j)
				{
				case 0:
				{
					float num10 = array[1];
					float num11 = HeightAtT(nodes, t, num10, _reversedTrack);
					float num12 = (num11 - num6) / (num10 - num5);
					num5 -= 0.05f;
					num6 -= 0.05f * num12;
					break;
				}
				case 10:
				{
					float num7 = array[9];
					float num8 = HeightAtT(nodes, t, num7, _reversedTrack);
					float num9 = (num8 - num6) / (num7 - num5);
					num5 += 0.05f;
					num6 += 0.05f * num9;
					break;
				}
				}
				Vector3 vector2 = o_point + num5 * o_width * o_normal + vector * num6;
				o_vertices[num4] = vector2;
				num4++;
			}
		}
		int num13 = indexOffset;
		int num14 = vertexOffset;
		for (int k = 0; k < tangentialDivisions.Length - 1; k++)
		{
			for (int l = 0; l < 10; l++)
			{
				o_triangles[num13++] = num14 + l;
				o_triangles[num13++] = num14 + l + 11;
				o_triangles[num13++] = num14 + l + 1;
				o_triangles[num13++] = num14 + l + 1;
				o_triangles[num13++] = num14 + l + 11;
				o_triangles[num13++] = num14 + l + 11 + 1;
			}
			num14 += 11;
		}
		return num3;
	}

	public static void AppendSplineSurroundCollisionGeometry(bool roofFromMinHeight, float minHeight, TrackNode[] nodes, float[] tangentialDivisions, int vertexOffset, int indexOffset, Vector3[] o_vertices, int[] o_triangles, bool _reversedTrack)
	{
		int num = tangentialDivisions.Length * 4;
		int num2 = (tangentialDivisions.Length - 1) * 3 * 6;
		int num3 = vertexOffset;
		foreach (float t in tangentialDivisions)
		{
			Vector3 o_point;
			Vector3 o_tangent;
			Vector3 o_normal;
			float o_width;
			float o_roofHeight;
			InfoAtT(nodes, t, out o_point, out o_tangent, out o_normal, out o_width, out o_roofHeight);
			Vector3 vector = UpAtT(nodes, t);
			for (int j = 0; j < 2; j++)
			{
				float num4 = j;
				float num5 = num4 * 2f - 1f;
				float num6 = HeightAtT(nodes, t, num5, _reversedTrack);
				Vector3 vector2 = o_point + num5 * o_width * o_normal + vector * minHeight;
				Vector3 vector3 = new Vector3(vector2.x, vector2.y + (roofFromMinHeight ? 0f : (num6 - minHeight)) + o_roofHeight, vector2.z);
				if (j == 1)
				{
					o_vertices[num3] = vector3;
					num3++;
				}
				o_vertices[num3] = vector2;
				num3++;
				if (j == 0)
				{
					o_vertices[num3] = vector3;
					num3++;
				}
			}
		}
		int num7 = indexOffset;
		int num8 = vertexOffset;
		for (int k = 0; k < tangentialDivisions.Length - 1; k++)
		{
			for (int l = 0; l < 3; l++)
			{
				o_triangles[num7++] = num8 + l;
				o_triangles[num7++] = num8 + l + 1;
				o_triangles[num7++] = num8 + l + 4;
				o_triangles[num7++] = num8 + l + 1;
				o_triangles[num7++] = num8 + l + 5;
				o_triangles[num7++] = num8 + l + 4;
			}
			num8 += 4;
		}
	}

	public static float TrackLength(TrackLocation _location)
	{
		return TrackLength(_location, 2);
	}

	public static float TrackLength(TrackLocation _location, int _maxDepth)
	{
		if (_location.m_spline == null && _location.m_hub == null)
		{
			return float.NaN;
		}
		if (_location.m_spline != null)
		{
			float num = _location.m_spline.ReParamTtoS(_location.m_t);
			float num2 = _location.m_spline.Length - num;
			if (_location.m_spline.HubAtTail == null)
			{
				return num2 + ForwardDistance(_location.m_spline, _location, _maxDepth);
			}
			return num2 + ForwardDistance(_location.m_spline.HubAtTail, _location.m_spline.HubSectorAtTail, _location, _maxDepth);
		}
		if (_location.m_hub != null)
		{
			if (_location.m_hub.IsSectorOutgoing(_location.m_sector))
			{
				float num3 = _location.m_hub.MaxSectorT(_location.m_sector) - _location.m_t;
				return num3 + ForwardDistance(_location.m_hub.SplineAtSector(_location.m_sector), _location, _maxDepth);
			}
			float num4 = _location.m_t - _location.m_hub.MinSectorT(_location.m_sector);
			return ForwardDistance(_location.m_hub, _location.m_sector, _location, _maxDepth) - num4;
		}
		return float.NaN;
	}

	public static float MinDistance(TrackLocation _from, TrackLocation _to)
	{
		return MinDistance(_from, _to, 2);
	}

	public static float MinDistance(TrackLocation _from, TrackLocation _to, int _maxDepth)
	{
		float num = ForwardDistance(_from, _to, _maxDepth);
		float num2 = BackwardDistance(_from, _to, _maxDepth);
		if (float.IsNaN(num))
		{
			return 0f - num2;
		}
		if (float.IsNaN(num2))
		{
			return num;
		}
		return (!(num <= num2)) ? (0f - num2) : num;
	}

	public static float ForwardDistance(TrackLocation _from, TrackLocation _to)
	{
		return ForwardDistance(_from, _to, 2);
	}

	public static float ForwardDistance(TrackLocation _from, TrackLocation _to, int _maxDepth)
	{
		if (_from.m_spline == null && _from.m_hub == null)
		{
			return float.NaN;
		}
		if (_to.m_spline == null && _to.m_hub == null)
		{
			return float.NaN;
		}
		if (_from.m_spline != null)
		{
			float num = _from.m_spline.ReParamTtoS(_from.m_t);
			if (_to.m_spline == _from.m_spline && _to.m_t >= _from.m_t)
			{
				float num2 = _to.m_spline.ReParamTtoS(_to.m_t);
				float b = num2 - num;
				return Mathf.Max(0f, b);
			}
			if (_from.m_spline.HubAtTail == null)
			{
				float num3 = _from.m_spline.Length - num;
				return num3 + ForwardDistance(_from.m_spline, _to, _maxDepth);
			}
			float num4 = _from.m_spline.Length - num;
			return num4 + ForwardDistance(_from.m_spline.HubAtTail, _from.m_spline.HubSectorAtTail, _to, _maxDepth);
		}
		if (_from.m_hub != null)
		{
			if (_to.m_hub == _from.m_hub && _to.m_sector == _from.m_sector && _to.m_t >= _from.m_t)
			{
				return _to.m_t - _from.m_t;
			}
			if (_from.m_hub.IsSectorOutgoing(_from.m_sector))
			{
				float num5 = _from.m_hub.MaxSectorT(_from.m_sector) - _from.m_t;
				return num5 + ForwardDistance(_from.m_hub.SplineAtSector(_from.m_sector), _to, _maxDepth);
			}
			float num6 = _from.m_t - _from.m_hub.MinSectorT(_from.m_sector);
			return ForwardDistance(_from.m_hub, _from.m_sector, _to, _maxDepth) - num6;
		}
		return float.NaN;
	}

	public static float ForwardDistance(TrackHub _hub, int _sector, TrackLocation _to, int _maxDepth)
	{
		if (_to.m_hub == _hub)
		{
			if (_to.m_sector == _sector)
			{
				return _to.m_t - _hub.MinSectorT(_sector);
			}
			if (_hub.IsSectorOutgoing(_to.m_sector))
			{
				float num = _hub.SectorLength(_sector);
				float num2 = _to.m_t - _hub.MinSectorT(_to.m_sector);
				return num + num2;
			}
		}
		else if (_to.m_spline != null)
		{
			for (int i = 0; i < _hub.NumSectors; i++)
			{
				if (_hub.IsSectorOutgoing(i) && _hub.SplineAtSector(i) == _to.m_spline)
				{
					float num3 = _hub.SectorLength(_sector);
					float num4 = _hub.SectorLength(i);
					float num5 = num3 + num4;
					return num5 + ForwardDistance(_hub.SplineAtSector(i), _to, _maxDepth);
				}
			}
		}
		float num6 = float.NaN;
		if (_maxDepth > 0)
		{
			for (int j = 0; j < _hub.NumSectors; j++)
			{
				if (_hub.IsSectorOutgoing(j))
				{
					float num7 = _hub.SectorLength(_sector);
					float num8 = _hub.SectorLength(j);
					float num9 = num7 + num8;
					num9 += ForwardDistance(_hub.SplineAtSector(j), _to, _maxDepth - 1);
					if (float.IsNaN(num6) || num9 < num6)
					{
						num6 = num9;
					}
				}
			}
		}
		return num6;
	}

	public static float ForwardDistance(TrackSpline _spline, TrackLocation _to, int _maxDepth)
	{
		if (_to.m_spline == _spline)
		{
			return _spline.ReParamTtoS(_to.m_t);
		}
		float length = _spline.Length;
		return length + ForwardDistance(_spline.HubAtTail, _spline.HubSectorAtTail, _to, _maxDepth);
	}

	public static float BackwardDistance(TrackLocation _from, TrackLocation _to)
	{
		return ForwardDistance(_to, _from, 2);
	}

	public static float BackwardDistance(TrackLocation _from, TrackLocation _to, int _maxDepth)
	{
		return ForwardDistance(_to, _from, _maxDepth);
	}

	public static void ReverseTrack()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("TrackHub");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.BroadcastMessage("OnReverseTrack");
		}
		GameObject[] array3 = GameObject.FindGameObjectsWithTag("TrackSpline");
		GameObject[] array4 = array3;
		foreach (GameObject gameObject2 in array4)
		{
			gameObject2.BroadcastMessage("OnReverseTrack");
		}
		GameObject[] array5 = GameObject.FindGameObjectsWithTag("Finish");
		GameObject[] array6 = array5;
		foreach (GameObject gameObject3 in array6)
		{
			gameObject3.BroadcastMessage("OnReverseTrack");
		}
		GameObject[] array7 = GameObject.FindGameObjectsWithTag("Reversible");
		GameObject[] array8 = array7;
		foreach (GameObject gameObject4 in array8)
		{
			gameObject4.BroadcastMessage("OnReverseTrack");
		}
	}

	public static Color SampleTrackLighting(Vector3 _samplePoint)
	{
		return SampleTrackLighting(_samplePoint, float.MaxValue);
	}

	public static Color SampleTrackLighting(Vector3 _samplePoint, float _yRange)
	{
		TrackSpline trackSpline = TrackSpline.FindClosestToPoint(_samplePoint, _yRange);
		if (trackSpline == null)
		{
			return Color.white;
		}
		TrackHub trackHub = null;
		float o_t = trackSpline.ClosestTTo(_samplePoint);
		int o_sectorIndex = 0;
		if (o_t <= 0f && trackSpline.HubAtHead != null)
		{
			trackHub = trackSpline.HubAtHead;
			trackSpline = null;
			trackHub.ClosestInfoTo(_samplePoint, out o_t, out o_sectorIndex);
		}
		else if (o_t >= trackSpline.MaxT && trackSpline.HubAtTail != null)
		{
			trackHub = trackSpline.HubAtTail;
			trackSpline = null;
			trackHub.ClosestInfoTo(_samplePoint, out o_t, out o_sectorIndex);
		}
		if (trackSpline != null)
		{
			TrackLighting lighting = trackSpline.Lighting;
			if (lighting == null || !lighting.HasCoarseSamples)
			{
				return Color.white;
			}
			float num = trackSpline.ReParamTtoS(o_t) + lighting.m_lightmapPadding;
			float num2 = 1f / (trackSpline.Length + 2f * lighting.m_lightmapPadding);
			float num3 = num * num2;
			Vector3 o_point;
			Vector3 o_tangent;
			Vector3 o_normal;
			float o_width;
			trackSpline.InfoAtT(o_t, out o_point, out o_tangent, out o_normal, out o_width);
			if (lighting.Reversed)
			{
				num3 = 1f - num3;
				o_normal = -o_normal;
			}
			Vector3 lhs = _samplePoint - o_point;
			float num4 = Vector3.Dot(lhs, o_normal) / o_width;
			float u = Mathf.Clamp01(num4 * 0.5f + 0.5f);
			float v = Mathf.Clamp01(num3);
			return lighting.CoarseSample(u, v);
		}
		TrackLighting lighting2 = trackHub.Lighting;
		if (lighting2 == null || !lighting2.HasCoarseSamples)
		{
			return Color.white;
		}
		Vector4 trackUVScaleOffset = lighting2.trackUVScaleOffset;
		Vector3 vector = trackHub.transform.InverseTransformPoint(_samplePoint);
		float u2 = Mathf.Clamp01(vector.x * trackUVScaleOffset.x + trackUVScaleOffset.z);
		float v2 = Mathf.Clamp01(vector.z * trackUVScaleOffset.y + trackUVScaleOffset.w);
		return lighting2.CoarseSample(u2, v2);
	}
}
