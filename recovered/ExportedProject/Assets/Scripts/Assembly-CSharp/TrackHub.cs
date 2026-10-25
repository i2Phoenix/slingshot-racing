using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Track/Track Hub")]
public class TrackHub : IndexedMonoBehaviour<TrackHub>
{
	private class Sector
	{
		public TrackSpline spline;

		public float angleToSpline;

		public float connectorDistance;

		public float divisionAngle;

		public float divisionSize;

		public bool isIncoming;

		public TrackNode Node
		{
			get
			{
				return (!isIncoming) ? spline.NodeAtHead : spline.NodeAtTail;
			}
		}

		public Vector3 ConnectorDir
		{
			get
			{
				return MathUtils.DirectionOnXZ(angleToSpline);
			}
		}

		public Vector3 DivisionDir
		{
			get
			{
				return MathUtils.DirectionOnXZ(divisionAngle);
			}
		}

		public Vector3 ConnectorNormal
		{
			get
			{
				return MathUtils.DirectionOnXZ(angleToSpline + 90f);
			}
		}

		public Vector3 Tangent
		{
			get
			{
				return (!isIncoming) ? ConnectorDir : (-ConnectorDir);
			}
		}

		public Vector3 Normal
		{
			get
			{
				return (!isIncoming) ? ConnectorNormal : (-ConnectorNormal);
			}
		}

		public static int CompareByAngle(Sector lhs, Sector rhs)
		{
			if (lhs.angleToSpline < rhs.angleToSpline)
			{
				return -1;
			}
			if (lhs.angleToSpline > rhs.angleToSpline)
			{
				return 1;
			}
			return 0;
		}
	}

	public const float c_defaultLightmapPadding = 6f;

	private TrackLighting m_lighting;

	private Collider m_surroundCollider;

	private Collider m_trackCollider;

	private List<Sector> sectors;

	public int TraverseStamp;

	public List<TrackSpline> incomingSplines;

	public List<TrackSpline> outgoingSplines;

	public TrackCrossSection crossSection;

	public Material material;

	public float m_bevel;

	public bool m_grappleContinuity;

	public bool disableRendering;

	public GrapplePoint m_allowGrapplePointStart;

	public int NumSectors
	{
		get
		{
			return (sectors != null) ? sectors.Count : 0;
		}
	}

	public TrackLighting Lighting
	{
		get
		{
			return m_lighting;
		}
	}

	public Collider SurroundCollider
	{
		get
		{
			return m_surroundCollider;
		}
	}

	public Collider TrackCollider
	{
		get
		{
			return m_trackCollider;
		}
	}

	public float Radius { get; private set; }

	private new void Awake()
	{
		base.Awake();
		if (incomingSplines == null)
		{
			incomingSplines = new List<TrackSpline>();
		}
		if (outgoingSplines == null)
		{
			outgoingSplines = new List<TrackSpline>();
		}
		m_lighting = GetComponent<TrackLighting>();
		BuildSectors();
		TraverseStamp = 0;
	}

	private new void Start()
	{
		base.Start();
		if (!disableRendering)
		{
			MeshFilter meshFilter = base.gameObject.AddComponent<MeshFilter>();
			GenerateTrackGeometry(meshFilter.mesh);
			MeshRenderer meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			if (material != null)
			{
				meshRenderer.sharedMaterial = material;
			}
			TrackLighting component = GetComponent<TrackLighting>();
			if (component != null && component.lightmapIndex < LightmapSettings.lightmaps.Length)
			{
				meshRenderer.lightmapIndex = component.lightmapIndex;
				meshRenderer.lightmapTilingOffset = component.lightmapTilingOffset;
			}
		}
		MeshCollider component2 = base.gameObject.GetComponent<MeshCollider>();
		if (component2 == null)
		{
			Mesh mesh = new Mesh();
			float minHeight = GenerateTrackCollisionGeometry(mesh);
			component2 = base.gameObject.AddComponent<MeshCollider>();
			component2.sharedMesh = mesh;
			component2.sharedMaterial = (PhysicMaterial)Resources.Load("PhysicsMaterials/Track");
			m_trackCollider = component2;
			GameObject gameObject = new GameObject("Surround");
			gameObject.layer = LayerMask.NameToLayer("TrackSurround");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			Mesh mesh2 = new Mesh();
			GenerateTrackSurroundCollisionGeometry(minHeight, mesh2);
			component2 = gameObject.AddComponent<MeshCollider>();
			component2.sharedMesh = mesh2;
			component2.sharedMaterial = (PhysicMaterial)Resources.Load("PhysicsMaterials/Surround");
			component2.smoothSphereCollisions = true;
			m_surroundCollider = component2;
			if (PhysicsVehicle.DebugSurroundColliders)
			{
				MeshFilter meshFilter2 = gameObject.AddComponent<MeshFilter>();
				meshFilter2.sharedMesh = mesh2;
				MeshRenderer meshRenderer2 = gameObject.AddComponent<MeshRenderer>();
				meshRenderer2.sharedMaterial = (Material)Resources.Load("Materials/DebugCollider");
			}
		}
	}

	public void ClosestInfoTo(Vector3 point, out float o_t, out int o_sectorIndex)
	{
		Vector3 vector = base.transform.InverseTransformPoint(point);
		float angle = MathUtils.AngleOnXZ(vector);
		int num = 0;
		float fromAngle = sectors[0].divisionAngle;
		for (int i = 1; i < sectors.Count; i++)
		{
			float divisionAngle = sectors[i].divisionAngle;
			if (MathUtils.AngleInRange(angle, fromAngle, divisionAngle))
			{
				num = i;
				break;
			}
			fromAngle = divisionAngle;
		}
		o_sectorIndex = num;
		o_t = ClosestTTo(vector, num);
	}

	private float ClosestTTo(Vector3 localPoint, int sectorIndex)
	{
		Sector sector = sectors[sectorIndex];
		return Vector3.Dot(sector.Tangent, localPoint);
	}

	public float MinSectorT(int sectorIndex)
	{
		Sector sector = sectors[sectorIndex];
		return (!sector.isIncoming) ? 0f : (0f - sector.connectorDistance);
	}

	public float MaxSectorT(int sectorIndex)
	{
		Sector sector = sectors[sectorIndex];
		return (!sector.isIncoming) ? sector.connectorDistance : 0f;
	}

	public float SectorLength(int sectorIndex)
	{
		return sectors[sectorIndex].connectorDistance;
	}

	public TrackSpline SplineAtSector(int sectorIndex)
	{
		return sectors[sectorIndex].spline;
	}

	public bool IsSectorOutgoing(int sectorIndex)
	{
		return !sectors[sectorIndex].isIncoming;
	}

	public Vector3 PointAt(float t, int sectorIndex)
	{
		Sector sector = sectors[sectorIndex];
		return base.transform.TransformPoint(sector.Tangent * t);
	}

	public Vector3 TangentAt(int sectorIndex)
	{
		Sector sector = sectors[sectorIndex];
		return base.transform.TransformDirection(sector.Tangent);
	}

	public Vector3 UpAt()
	{
		return base.transform.TransformDirection(Vector3.up);
	}

	public float HeightAt(float widthProp)
	{
		return (!(crossSection == null)) ? crossSection.Evaluate(widthProp) : 0f;
	}

	public float[] CrossSectionDistribution()
	{
		return (!(crossSection != null)) ? TrackUtils.DefaultCrossSectionDistribution() : crossSection.GetDistribution(11);
	}

	public void InfoAt(float t, int sectorIndex, out Vector3 o_point, out Vector3 o_tangent, out Vector3 o_normal, out float o_width)
	{
		Sector sector = sectors[sectorIndex];
		Vector3 tangent = sector.Tangent;
		Transform transform = base.transform;
		o_point = transform.TransformPoint(tangent * t);
		o_tangent = transform.TransformDirection(tangent);
		o_normal = transform.TransformDirection(sector.Normal);
		o_width = sector.Node.width;
	}

	private static void SectorGeometryRequirements(out int o_numVertices, out int o_numIndices)
	{
		o_numVertices = 22;
		o_numIndices = 60;
	}

	private static void SectorLightmapGeometryRequirements(out int o_numVertices, out int o_numIndices)
	{
		o_numVertices = 33;
		o_numIndices = 120;
	}

	private static void SectorCollisionGeometryRequirements(out int o_numVertices, out int o_numIndices)
	{
		o_numVertices = 22;
		o_numIndices = 60;
	}

	private static void SectorSurroundCollisionGeometryRequirements(out int o_numVertices, out int o_numIndices)
	{
		o_numVertices = 9;
		o_numIndices = 21;
	}

	private static void SectorGeometryRequirementsQuad(out int o_numVertices, out int o_numIndices)
	{
		o_numVertices = 22;
		o_numIndices = 40;
	}

	private void AppendSectorGeometry(bool _applyTransform, Sector sector, Sector prevSector, int vertexOffset, int indexOffset, Vector3[] o_vertices, Vector2[] o_uv, int[] o_triangles, out Vector2 o_minXZ, out Vector2 o_maxXZ)
	{
		FillVertexData(_applyTransform, sector, prevSector, vertexOffset, o_vertices, o_uv, out o_minXZ, out o_maxXZ);
		FillTris(1, vertexOffset, indexOffset, o_triangles);
	}

	private void AppendSectorLightmapGeometry(bool _applyTransform, Sector sector, Sector prevSector, int vertexOffset, int indexOffset, Vector3[] o_vertices, int[] o_triangles, out Vector2 o_minXZ, out Vector2 o_maxXZ)
	{
		FillLightmapVertexData(_applyTransform, sector, prevSector, vertexOffset, o_vertices, out o_minXZ, out o_maxXZ);
		FillTris(2, vertexOffset, indexOffset, o_triangles);
	}

	private void AppendSectorGeometryQuad(bool _applyTransform, Sector sector, Sector prevSector, int vertexOffset, int indexOffset, Vector3[] o_vertices, Vector2[] o_uv, int[] o_quads, out Vector2 o_minXZ, out Vector2 o_maxXZ)
	{
		FillVertexData(_applyTransform, sector, prevSector, vertexOffset, o_vertices, o_uv, out o_minXZ, out o_maxXZ);
		FillQuads(vertexOffset, indexOffset, o_quads);
	}

	private void FillVertexData(bool _applyTransform, Sector sector, Sector prevSector, int vertexOffset, Vector3[] o_vertices, Vector2[] o_uv, out Vector2 o_minXZ, out Vector2 o_maxXZ)
	{
		int num = 22;
		TrackNode node = sector.Node;
		Vector3 vector = base.transform.InverseTransformPoint(sector.spline.transform.TransformPoint(node.position));
		Vector3 vector2 = base.transform.InverseTransformDirection(sector.spline.transform.TransformDirection(node.right));
		float width = node.width;
		Vector3 divisionDir = sector.DivisionDir;
		Vector3 divisionDir2 = prevSector.DivisionDir;
		int num2 = vertexOffset;
		Vector2 vector3 = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 vector4 = new Vector2(float.MinValue, float.MinValue);
		float[] array = CrossSectionDistribution();
		for (int i = 0; i < 11; i++)
		{
			float num3 = array[i];
			float num4 = num3 * 0.5f + 0.5f;
			float num5 = num4 * 0.9f + 0.05f;
			if (!sector.isIncoming)
			{
				num3 = 0f - num3;
			}
			float widthProp = Mathf.Abs(num3);
			float num6 = HeightAt(widthProp);
			if (!sector.isIncoming)
			{
				num5 = 1f - num5;
			}
			Vector3 vector5 = vector + num3 * width * vector2 + Vector3.up * num6;
			vector3 = Vector2.Min(vector3, new Vector2(vector5.x, vector5.z));
			vector4 = Vector2.Max(vector4, new Vector2(vector5.x, vector5.z));
			if (o_vertices != null)
			{
				o_vertices[num2] = ((!_applyTransform) ? vector5 : base.transform.TransformPoint(vector5));
			}
			if (o_uv != null)
			{
				o_uv[num2] = new Vector2(num5, 0f);
			}
			num2++;
		}
		for (int j = 0; j < 11; j++)
		{
			float num7 = array[j];
			float num8 = num7 * 0.5f + 0.5f;
			float num9 = num8 * 0.9f + 0.05f;
			float num10 = Mathf.Abs(num7);
			float num11 = HeightAt(num10);
			if (!sector.isIncoming)
			{
				num9 = 1f - num9;
			}
			Vector3 vector6 = ((!(num7 >= 0f)) ? (divisionDir * num10 * sector.divisionSize + Vector3.up * num11) : (divisionDir2 * num10 * prevSector.divisionSize + Vector3.up * num11));
			vector3 = Vector2.Min(vector3, new Vector2(vector6.x, vector6.z));
			vector4 = Vector2.Max(vector4, new Vector2(vector6.x, vector6.z));
			float y = Mathf.Round((vector6 - o_vertices[num2 - 11]).magnitude * 0.2f);
			if (o_vertices != null)
			{
				o_vertices[num2] = ((!_applyTransform) ? vector6 : base.transform.TransformPoint(vector6));
			}
			if (o_uv != null)
			{
				o_uv[num2] = new Vector2(num9, y);
			}
			num2++;
		}
		o_minXZ = vector3;
		o_maxXZ = vector4;
	}

	private void FillLightmapVertexData(bool _applyTransform, Sector sector, Sector prevSector, int vertexOffset, Vector3[] o_vertices, out Vector2 o_minXZ, out Vector2 o_maxXZ)
	{
		int num = 33;
		TrackNode node = sector.Node;
		Vector3 vector = base.transform.InverseTransformPoint(sector.spline.transform.TransformPoint(node.position));
		Vector3 vector2 = base.transform.InverseTransformDirection(sector.spline.transform.TransformDirection(node.right));
		float width = node.width;
		Vector3 divisionDir = sector.DivisionDir;
		Vector3 divisionDir2 = prevSector.DivisionDir;
		int num2 = vertexOffset;
		float[] array = CrossSectionDistribution();
		for (int i = 0; i < 11; i++)
		{
			float num3 = array[i];
			if (!sector.isIncoming)
			{
				num3 = 0f - num3;
			}
			float widthProp = Mathf.Abs(num3);
			float num4 = HeightAt(widthProp);
			Vector3 vector3 = vector + num3 * width * vector2 + Vector3.up * num4;
			vector3 += sector.ConnectorDir * 6f;
			if (o_vertices != null)
			{
				o_vertices[num2] = ((!_applyTransform) ? vector3 : base.transform.TransformPoint(vector3));
			}
			num2++;
		}
		Vector2 vector4 = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 vector5 = new Vector2(float.MinValue, float.MinValue);
		for (int j = 0; j < 11; j++)
		{
			float num5 = array[j];
			if (!sector.isIncoming)
			{
				num5 = 0f - num5;
			}
			float widthProp2 = Mathf.Abs(num5);
			float num6 = HeightAt(widthProp2);
			Vector3 vector6 = vector + num5 * width * vector2 + Vector3.up * num6;
			vector4 = Vector2.Min(vector4, new Vector2(vector6.x, vector6.z));
			vector5 = Vector2.Max(vector5, new Vector2(vector6.x, vector6.z));
			if (o_vertices != null)
			{
				o_vertices[num2] = ((!_applyTransform) ? vector6 : base.transform.TransformPoint(vector6));
			}
			num2++;
		}
		for (int k = 0; k < 11; k++)
		{
			float num7 = array[k];
			float num8 = Mathf.Abs(num7);
			float num9 = HeightAt(num8);
			Vector3 vector7 = ((!(num7 >= 0f)) ? (divisionDir * num8 * sector.divisionSize + Vector3.up * num9) : (divisionDir2 * num8 * prevSector.divisionSize + Vector3.up * num9));
			vector4 = Vector2.Min(vector4, new Vector2(vector7.x, vector7.z));
			vector5 = Vector2.Max(vector5, new Vector2(vector7.x, vector7.z));
			if (o_vertices != null)
			{
				o_vertices[num2] = ((!_applyTransform) ? vector7 : base.transform.TransformPoint(vector7));
			}
			num2++;
		}
		o_minXZ = vector4;
		o_maxXZ = vector5;
	}

	private void FillTris(int numRows, int vertexOffset, int indexOffset, int[] o_triangles)
	{
		int num = numRows * 10 * 6;
		int num2 = indexOffset;
		for (int i = 0; i < numRows; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				o_triangles[num2++] = vertexOffset + j;
				o_triangles[num2++] = vertexOffset + j + 11;
				o_triangles[num2++] = vertexOffset + j + 1;
				o_triangles[num2++] = vertexOffset + j + 1;
				o_triangles[num2++] = vertexOffset + j + 11;
				o_triangles[num2++] = vertexOffset + j + 11 + 1;
			}
			vertexOffset += 11;
		}
	}

	private void FillQuads(int vertexOffset, int indexOffset, int[] o_quads)
	{
		int num = 40;
		int num2 = indexOffset;
		for (int i = 0; i < 10; i++)
		{
			o_quads[num2++] = vertexOffset + i;
			o_quads[num2++] = vertexOffset + i + 11;
			o_quads[num2++] = vertexOffset + i + 11 + 1;
			o_quads[num2++] = vertexOffset + i + 1;
		}
	}

	private float AppendSectorCollisionGeometry(Sector sector, Sector prevSector, int vertexOffset, int indexOffset, Vector3[] o_vertices, int[] o_triangles)
	{
		int num = 22;
		int num2 = 60;
		float num3 = float.MaxValue;
		TrackNode node = sector.Node;
		Vector3 vector = base.transform.InverseTransformPoint(sector.spline.transform.TransformPoint(node.position));
		Vector3 vector2 = base.transform.InverseTransformDirection(sector.spline.transform.TransformDirection(node.right));
		float width = node.width;
		Vector3 divisionDir = sector.DivisionDir;
		Vector3 divisionDir2 = prevSector.DivisionDir;
		int num4 = vertexOffset;
		float[] array = CrossSectionDistribution();
		for (int i = 0; i < 11; i++)
		{
			float num5 = array[i];
			if (!sector.isIncoming)
			{
				num5 = 0f - num5;
			}
			float widthProp = Mathf.Abs(num5);
			float num6 = HeightAt(widthProp);
			num3 = Mathf.Min(num3, num6);
			Vector3 vector3 = vector + num5 * width * vector2 + Vector3.up * num6;
			o_vertices[num4] = vector3;
			num4++;
		}
		for (int j = 0; j < 11; j++)
		{
			float num7 = array[j];
			float num8 = Mathf.Abs(num7);
			float num9 = HeightAt(num8);
			num3 = Mathf.Min(num3, num9);
			Vector3 vector4 = ((!(num7 >= 0f)) ? (divisionDir * num8 * sector.divisionSize + Vector3.up * num9) : (divisionDir2 * num8 * prevSector.divisionSize + Vector3.up * num9));
			o_vertices[num4] = vector4;
			num4++;
		}
		int num10 = indexOffset;
		int num11 = vertexOffset;
		for (int k = 0; k < 10; k++)
		{
			o_triangles[num10++] = num11 + k;
			o_triangles[num10++] = num11 + k + 11;
			o_triangles[num10++] = num11 + k + 1;
			o_triangles[num10++] = num11 + k + 1;
			o_triangles[num10++] = num11 + k + 11;
			o_triangles[num10++] = num11 + k + 11 + 1;
		}
		num11 += 11;
		return num3;
	}

	private void AppendSectorSurroundCollisionGeometry(float minHeight, Sector sector, Sector prevSector, int vertexOffset, int indexOffset, Vector3[] o_vertices, int[] o_triangles)
	{
		int num = 9;
		int num2 = 21;
		TrackNode node = sector.Node;
		Vector3 vector = base.transform.InverseTransformPoint(sector.spline.transform.TransformPoint(node.position));
		Vector3 vector2 = base.transform.InverseTransformDirection(sector.spline.transform.TransformDirection(node.right));
		float width = node.width;
		float roofHeight = node.m_roofHeight;
		bool roofFromMinHeight = sector.spline.roofFromMinHeight;
		Vector3 divisionDir = sector.DivisionDir;
		Vector3 divisionDir2 = prevSector.DivisionDir;
		int num3 = vertexOffset;
		float num4 = 0f;
		for (int i = 0; i < 2; i++)
		{
			float num5 = i;
			float num6 = num5 * 2f - 1f;
			if (!sector.isIncoming)
			{
				num6 = 0f - num6;
			}
			float widthProp = Mathf.Abs(num6);
			float num7 = HeightAt(widthProp);
			Vector3 vector3 = vector + num6 * width * vector2 + Vector3.up * minHeight;
			Vector3 vector4 = new Vector3(vector3.x, vector3.y + (roofFromMinHeight ? 0f : (num7 - minHeight)) + roofHeight, vector3.z);
			num4 = Mathf.Max(num4, vector4.y);
			if (i == 1)
			{
				o_vertices[num3] = vector4;
				num3++;
			}
			o_vertices[num3] = vector3;
			num3++;
			if (i == 0)
			{
				o_vertices[num3] = vector4;
				num3++;
			}
		}
		for (int j = 0; j < 2; j++)
		{
			float num8 = j;
			float num9 = num8 * 2f - 1f;
			float num10 = Mathf.Abs(num9);
			float num11 = HeightAt(num10);
			Vector3 vector5 = ((!(num9 >= 0f)) ? (divisionDir * sector.divisionSize) : (divisionDir2 * prevSector.divisionSize));
			vector5 = vector5 * num10 + Vector3.up * minHeight;
			Vector3 vector6 = new Vector3(vector5.x, vector5.y + (roofFromMinHeight ? 0f : (num11 - minHeight)) + roofHeight, vector5.z);
			num4 = Mathf.Max(num4, vector6.y);
			if (j == 1)
			{
				o_vertices[num3] = vector6;
				num3++;
			}
			o_vertices[num3] = vector5;
			num3++;
			if (j == 0)
			{
				o_vertices[num3] = vector6;
				num3++;
			}
		}
		o_vertices[num3] = new Vector3(0f, num4, 0f);
		num3++;
		int num12 = indexOffset;
		int num13 = vertexOffset;
		for (int k = 0; k < 3; k++)
		{
			o_triangles[num12++] = num13 + k;
			o_triangles[num12++] = num13 + k + 1;
			o_triangles[num12++] = num13 + k + 4;
			o_triangles[num12++] = num13 + k + 1;
			o_triangles[num12++] = num13 + k + 5;
			o_triangles[num12++] = num13 + k + 4;
		}
		num13 += 4;
		o_triangles[num12++] = num13 + 1;
		o_triangles[num12++] = num13 + 2;
		o_triangles[num12++] = num13 + 4;
	}

	public void GenerateTrackGeometry(Mesh mesh)
	{
		int num = 0;
		int num2 = 0;
		int o_numVertices;
		int o_numIndices;
		SectorGeometryRequirements(out o_numVertices, out o_numIndices);
		num += o_numVertices * NumSectors;
		num2 += o_numIndices * NumSectors;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		Vector2[] array3 = new Vector2[num];
		int[] array4 = new int[num2];
		AppendGeometry(false, 0, 0, array, array2, array3, array4);
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.uv2 = array3;
		mesh.triangles = array4;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
	}

	public float GenerateTrackCollisionGeometry(Mesh mesh)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < sectors.Count; i++)
		{
			int o_numVertices;
			int o_numIndices;
			SectorCollisionGeometryRequirements(out o_numVertices, out o_numIndices);
			num += o_numVertices;
			num2 += o_numIndices;
		}
		Vector3[] array = new Vector3[num];
		int[] array2 = new int[num2];
		int num3 = 0;
		int num4 = 0;
		float num5 = float.MaxValue;
		for (int j = 0; j < sectors.Count; j++)
		{
			int o_numVertices2;
			int o_numIndices2;
			SectorCollisionGeometryRequirements(out o_numVertices2, out o_numIndices2);
			int index = MathUtils.Repeat(j - 1, sectors.Count);
			float b = AppendSectorCollisionGeometry(sectors[j], sectors[index], num3, num4, array, array2);
			num5 = Mathf.Min(num5, b);
			num3 += o_numVertices2;
			num4 += o_numIndices2;
		}
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		return num5;
	}

	public void GenerateTrackSurroundCollisionGeometry(float minHeight, Mesh mesh)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < sectors.Count; i++)
		{
			int o_numVertices;
			int o_numIndices;
			SectorSurroundCollisionGeometryRequirements(out o_numVertices, out o_numIndices);
			num += o_numVertices;
			num2 += o_numIndices;
		}
		Vector3[] array = new Vector3[num];
		int[] array2 = new int[num2];
		int num3 = 0;
		int num4 = 0;
		for (int j = 0; j < sectors.Count; j++)
		{
			int o_numVertices2;
			int o_numIndices2;
			SectorSurroundCollisionGeometryRequirements(out o_numVertices2, out o_numIndices2);
			int index = MathUtils.Repeat(j - 1, sectors.Count);
			AppendSectorSurroundCollisionGeometry(minHeight, sectors[j], sectors[index], num3, num4, array, array2);
			num3 += o_numVertices2;
			num4 += o_numIndices2;
		}
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
	}

	public void GeometryRequirements(out int o_numVertices, out int o_numIndices)
	{
		int o_numVertices2;
		int o_numIndices2;
		SectorGeometryRequirements(out o_numVertices2, out o_numIndices2);
		o_numVertices = o_numVertices2 * NumSectors;
		o_numIndices = o_numIndices2 * NumSectors;
	}

	public void GeometryLightmapRequirements(out int o_numVertices, out int o_numIndices)
	{
		int o_numVertices2;
		int o_numIndices2;
		SectorLightmapGeometryRequirements(out o_numVertices2, out o_numIndices2);
		o_numVertices = o_numVertices2 * NumSectors;
		o_numIndices = o_numIndices2 * NumSectors;
	}

	public void GeometryRequirementsQuad(out int o_numVertices, out int o_numIndices)
	{
		int o_numVertices2;
		int o_numIndices2;
		SectorGeometryRequirementsQuad(out o_numVertices2, out o_numIndices2);
		o_numVertices = o_numVertices2 * NumSectors;
		o_numIndices = o_numIndices2 * NumSectors;
	}

	public void AppendGeometry(bool _applyTransform, int vertexOffset, int indexOffset, Vector3[] o_vertices, Vector2[] o_uv, Vector2[] o_uv2, int[] o_triangles)
	{
		int o_numVertices;
		int o_numIndices;
		SectorGeometryRequirements(out o_numVertices, out o_numIndices);
		Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
		int num = vertexOffset;
		int num2 = indexOffset;
		for (int i = 0; i < sectors.Count; i++)
		{
			int index = MathUtils.Repeat(i - 1, sectors.Count);
			Vector2 o_minXZ;
			Vector2 o_maxXZ;
			AppendSectorGeometry(_applyTransform, sectors[i], sectors[index], num, num2, o_vertices, o_uv, o_triangles, out o_minXZ, out o_maxXZ);
			vector = Vector2.Min(vector, o_minXZ);
			vector2 = Vector2.Max(vector2, o_maxXZ);
			num += o_numVertices;
			num2 += o_numIndices;
		}
		vector -= new Vector2(6f, 6f);
		vector2 += new Vector2(6f, 6f);
		int num3 = o_numVertices * sectors.Count;
		Vector2 vector3 = vector2 - vector;
		Vector2 b = new Vector2(1f / vector3.x, 1f / vector3.y);
		Vector2 vector4 = Vector2.Scale(-vector, b);
		if (o_uv2 != null)
		{
			int num4 = vertexOffset;
			for (int j = 0; j < num3; j++)
			{
				Vector2 a = new Vector2(o_vertices[num4].x, o_vertices[num4].z);
				o_uv2[num4] = Vector2.Scale(a, b) + vector4;
				o_uv2[num4].x = Mathf.Clamp01(o_uv2[num4].x);
				o_uv2[num4].y = Mathf.Clamp01(o_uv2[num4].y);
				num4++;
			}
		}
	}

	public void AppendLightmapGeometry(bool _applyTransform, int vertexOffset, int indexOffset, Vector3[] o_vertices, Vector2[] o_uv2, int[] o_triangles, out Vector4 o_uvScaleOffset)
	{
		int o_numVertices;
		int o_numIndices;
		SectorLightmapGeometryRequirements(out o_numVertices, out o_numIndices);
		Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
		int num = vertexOffset;
		int num2 = indexOffset;
		for (int i = 0; i < sectors.Count; i++)
		{
			int index = MathUtils.Repeat(i - 1, sectors.Count);
			Vector2 o_minXZ;
			Vector2 o_maxXZ;
			AppendSectorLightmapGeometry(_applyTransform, sectors[i], sectors[index], num, num2, o_vertices, o_triangles, out o_minXZ, out o_maxXZ);
			vector = Vector2.Min(vector, o_minXZ);
			vector2 = Vector2.Max(vector2, o_maxXZ);
			num += o_numVertices;
			num2 += o_numIndices;
		}
		vector -= new Vector2(6f, 6f);
		vector2 += new Vector2(6f, 6f);
		int num3 = o_numVertices * sectors.Count;
		Vector2 vector3 = vector2 - vector;
		Vector2 b = new Vector2(1f / vector3.x, 1f / vector3.y);
		Vector2 vector4 = Vector2.Scale(-vector, b);
		if (o_uv2 != null)
		{
			int num4 = vertexOffset;
			for (int j = 0; j < num3; j++)
			{
				Vector2 a = new Vector2(o_vertices[num4].x, o_vertices[num4].z);
				o_uv2[num4] = Vector2.Scale(a, b) + vector4;
				o_uv2[num4].x = Mathf.Clamp01(o_uv2[num4].x);
				o_uv2[num4].y = Mathf.Clamp01(o_uv2[num4].y);
				num4++;
			}
		}
		o_uvScaleOffset = new Vector4(b.x, b.y, vector4.x, vector4.y);
	}

	public void AppendGeometryQuad(bool _applyTransform, int vertexOffset, int indexOffset, Vector3[] o_vertices, Vector2[] o_uv, int[] o_quads)
	{
		int o_numVertices;
		int o_numIndices;
		SectorGeometryRequirementsQuad(out o_numVertices, out o_numIndices);
		for (int i = 0; i < sectors.Count; i++)
		{
			int index = MathUtils.Repeat(i - 1, sectors.Count);
			Vector2 o_minXZ;
			Vector2 o_maxXZ;
			AppendSectorGeometryQuad(_applyTransform, sectors[i], sectors[index], vertexOffset, indexOffset, o_vertices, o_uv, o_quads, out o_minXZ, out o_maxXZ);
			vertexOffset += o_numVertices;
			indexOffset += o_numIndices;
		}
	}

	public bool BuildSectors()
	{
		if (sectors == null)
		{
			sectors = new List<Sector>();
		}
		sectors.Clear();
		Radius = 0f;
		for (int i = 0; i < incomingSplines.Count; i++)
		{
			if (incomingSplines[i] != null)
			{
				Sector sector = new Sector();
				Vector3 point = base.transform.InverseTransformPoint(incomingSplines[i].PointAtTail);
				if (!(Mathf.Abs(point.x) < float.Epsilon) || !(Mathf.Abs(point.z) < float.Epsilon))
				{
					float angleToSpline = MathUtils.AngleOnXZ(point);
					sector.spline = incomingSplines[i];
					sector.connectorDistance = sector.Node.width * 0.5f;
					sector.angleToSpline = angleToSpline;
					sector.isIncoming = true;
					sectors.Add(sector);
				}
			}
		}
		for (int j = 0; j < outgoingSplines.Count; j++)
		{
			if (outgoingSplines[j] != null)
			{
				Sector sector2 = new Sector();
				Vector3 point2 = base.transform.InverseTransformPoint(outgoingSplines[j].PointAtHead);
				if (!(Mathf.Abs(point2.x) < float.Epsilon) || !(Mathf.Abs(point2.z) < float.Epsilon))
				{
					float angleToSpline2 = MathUtils.AngleOnXZ(point2);
					sector2.spline = outgoingSplines[j];
					sector2.connectorDistance = sector2.Node.width * 0.5f;
					sector2.angleToSpline = angleToSpline2;
					sector2.isIncoming = false;
					sectors.Add(sector2);
				}
			}
		}
		if (sectors.Count < 2)
		{
			sectors.Clear();
			return false;
		}
		sectors.Sort(Sector.CompareByAngle);
		for (int k = 0; k < sectors.Count; k++)
		{
			int index = MathUtils.Repeat(k + 1, sectors.Count);
			float angleToSpline3 = sectors[k].angleToSpline;
			float num = sectors[index].angleToSpline;
			if (num < angleToSpline3)
			{
				num += 360f;
			}
			float num2 = sectors[k].Node.width + m_bevel;
			float num3 = sectors[index].Node.width + m_bevel;
			float t = num2 / (num2 + num3);
			float num4 = Mathf.Lerp(angleToSpline3, num, t);
			sectors[k].divisionAngle = num4;
			float f = Mathf.DeltaAngle(angleToSpline3, num4) * ((float)Math.PI / 180f);
			float num5 = num2 / Mathf.Tan(f);
			sectors[k].connectorDistance = Mathf.Max(num5, sectors[k].connectorDistance);
			float a = Mathf.Sqrt(num2 * num2 + num5 * num5) - m_bevel;
			float f2 = Mathf.DeltaAngle(num4, num) * ((float)Math.PI / 180f);
			float num6 = num3 / Mathf.Tan(f2);
			sectors[index].connectorDistance = Mathf.Max(num6, sectors[index].connectorDistance);
			float b = Mathf.Sqrt(num3 * num3 + num6 * num6) - m_bevel;
			sectors[k].divisionSize = Mathf.Max(a, b);
		}
		for (int l = 0; l < sectors.Count; l++)
		{
			Sector sector3 = sectors[l];
			Quaternion quaternion = Quaternion.Inverse(sector3.spline.transform.rotation);
			if (sector3.isIncoming)
			{
				sector3.spline.ConnectHubToTail(this, l);
				Vector3 position = sector3.ConnectorDir * sector3.connectorDistance;
				Quaternion quaternion2 = Quaternion.LookRotation(sector3.Tangent);
				TrackNode node = sector3.Node;
				node.crossSection = crossSection;
				node.position = sector3.spline.transform.InverseTransformPoint(base.transform.TransformPoint(position));
				node.rotation = quaternion * base.transform.rotation * quaternion2;
			}
			else
			{
				sector3.spline.ConnectHubToHead(this, l);
				Vector3 position2 = sector3.ConnectorDir * sector3.connectorDistance;
				Quaternion quaternion3 = Quaternion.LookRotation(sector3.Tangent);
				TrackNode node2 = sector3.Node;
				node2.crossSection = crossSection;
				node2.position = sector3.spline.transform.InverseTransformPoint(base.transform.TransformPoint(position2));
				node2.rotation = quaternion * base.transform.rotation * quaternion3;
			}
		}
		for (int m = 0; m < sectors.Count; m++)
		{
			Radius = Mathf.Max(Radius, sectors[m].connectorDistance);
		}
		return true;
	}

	public void OnReverseTrack()
	{
		CoreUtils.Swap(ref incomingSplines, ref outgoingSplines);
	}

	public void DisconnectIncomingSpline(TrackSpline spline)
	{
		for (int i = 0; i < incomingSplines.Count; i++)
		{
			if (incomingSplines[i] == spline)
			{
				incomingSplines[i] = null;
			}
		}
	}

	public void DisconnectOutgoingSpline(TrackSpline spline)
	{
		for (int i = 0; i < outgoingSplines.Count; i++)
		{
			if (outgoingSplines[i] == spline)
			{
				outgoingSplines[i] = null;
			}
		}
	}
}
