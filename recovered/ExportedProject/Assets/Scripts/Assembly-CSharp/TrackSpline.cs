using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Track/Track Spline")]
public class TrackSpline : IndexedMonoBehaviour<TrackSpline>
{
	public const int c_minNodes = 2;

	private const float c_reParamNodeFrequency = 10f;

	private TrackLighting m_lighting;

	private TrackEdgeData m_edgeData;

	private ReParamNode[] m_reParamNodes;

	private GrapplePoint[] m_grapplePoints;

	private Collider[] m_surroundColliders;

	private Collider[] m_trackColliders;

	private TrackNodeCache[] m_nodeCacheWS;

	private bool m_reversedTrack;

	public TrackNode[] nodes;

	public Material material;

	public float rumbleAreaProp;

	public float rumbleAvoidProp;

	public bool disableRendering;

	public bool roofFromMinHeight;

	public float MaxT
	{
		get
		{
			return nodes.Length - 1;
		}
	}

	public TrackNode NodeAtHead
	{
		get
		{
			return nodes[0];
		}
	}

	public Vector3 PointAtHead
	{
		get
		{
			return base.transform.TransformPoint(nodes[0].position);
		}
	}

	public Vector3 TangentAtHead
	{
		get
		{
			return base.transform.TransformDirection(nodes[0].tangent);
		}
	}

	public TrackNode NodeAtTail
	{
		get
		{
			return nodes[nodes.Length - 1];
		}
	}

	public Vector3 PointAtTail
	{
		get
		{
			return base.transform.TransformPoint(nodes[nodes.Length - 1].position);
		}
	}

	public Vector3 TangentAtTail
	{
		get
		{
			return base.transform.TransformDirection(nodes[nodes.Length - 1].tangent);
		}
	}

	public TrackLighting Lighting
	{
		get
		{
			return m_lighting;
		}
	}

	public TrackEdgeData EdgeData
	{
		get
		{
			return m_edgeData;
		}
	}

	public int Waypoint { get; set; }

	public float Length { get; private set; }

	public TrackHub HubAtHead { get; private set; }

	public int HubSectorAtHead { get; private set; }

	public TrackHub HubAtTail { get; private set; }

	public int HubSectorAtTail { get; private set; }

	public Collider[] SurroundColliders
	{
		get
		{
			return m_surroundColliders;
		}
	}

	public Collider[] TrackColliders
	{
		get
		{
			return m_trackColliders;
		}
	}

	private void Init()
	{
		if (nodes == null)
		{
			nodes = new TrackNode[2];
			nodes[0] = new TrackNode();
			nodes[0].position = Vector3.forward * -50f;
			nodes[0].forward = Vector3.forward;
			nodes[1] = new TrackNode();
			nodes[1].position = Vector3.forward * 50f;
			nodes[1].forward = Vector3.forward;
		}
		m_lighting = GetComponent<TrackLighting>();
		m_edgeData = GetComponent<TrackEdgeData>();
		SetupLengthAndReParam();
	}

	public void SetupLengthAndReParam()
	{
		Length = 0f;
		List<ReParamNode> list = new List<ReParamNode>();
		for (int i = 0; i < nodes.Length; i++)
		{
			if (i < nodes.Length - 1)
			{
				TrackNode trackNode = nodes[i];
				TrackNode trackNode2 = nodes[i + 1];
				Vector3 position = trackNode.position;
				Vector3 tangent = trackNode.tangent;
				Vector3 position2 = trackNode2.position;
				Vector3 tangent2 = trackNode2.tangent;
				float num = TrackUtils.SplineLength(position, tangent, position2, tangent2, 0f, 1f);
				float num2 = Length;
				float num3 = 0f;
				int num4 = Mathf.CeilToInt(num / 10f);
				float num5 = 1f / (float)num4;
				for (int j = 0; j < num4; j++)
				{
					list.Add(new ReParamNode
					{
						s = num2,
						t = num3 + (float)i
					});
					float num6 = TrackUtils.SplineLength(position, tangent, position2, tangent2, num3, num3 + num5);
					num2 += num6;
					num3 += num5;
				}
				Length = num2;
			}
			else
			{
				list.Add(new ReParamNode
				{
					s = Length,
					t = i
				});
			}
		}
		int val = list.Count - 1;
		for (int k = 0; k < list.Count; k++)
		{
			int index = Math.Max(0, k - 1);
			int index2 = Math.Min(val, k + 1);
			ReParamNode reParamNode = list[index];
			ReParamNode reParamNode2 = list[index2];
			float num7 = reParamNode2.s - reParamNode.s;
			float num8 = reParamNode2.t - reParamNode.t;
			ReParamNode value = list[k];
			value.dsdt = num7 / num8;
			value.dtds = num8 / num7;
			list[k] = value;
		}
		m_reParamNodes = list.ToArray();
	}

	private new void Awake()
	{
		base.Awake();
		Init();
	}

	private void Reset()
	{
		Init();
	}

	private new void Start()
	{
		base.Start();
		GetGrapplePoints();
		InitSplineCache();
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
		InitCollisionGeometry();
	}

	public bool IsInGrappleArea(GrapplePoint gp, float t)
	{
		if (gp == null)
		{
			return false;
		}
		return t >= gp.startProp && t <= gp.endProp;
	}

	private void GetGrapplePoints()
	{
		m_grapplePoints = GetComponentsInChildren<GrapplePoint>();
	}

	private void InitSplineCache()
	{
		Transform transform = base.transform;
		m_nodeCacheWS = new TrackNodeCache[nodes.Length];
		for (int i = 0; i < m_nodeCacheWS.Length; i++)
		{
			m_nodeCacheWS[i] = new TrackNodeCache(transform, nodes[i].position, nodes[i].tangent, nodes[i].up, nodes[i].width);
		}
	}

	public void GenerateTrackGeometry(Mesh mesh)
	{
		TrackEdgeData edgeData = EdgeData;
		float[] tangentialDivisions = TrackUtils.SplineTangentialDivisions(nodes, edgeData);
		int o_numVertices;
		int o_numIndices;
		TrackUtils.SplineGeometryRequirements(tangentialDivisions, out o_numVertices, out o_numIndices);
		Vector3[] array = new Vector3[o_numVertices];
		Vector2[] array2 = new Vector2[o_numVertices];
		Vector2[] array3 = new Vector2[o_numVertices];
		int[] array4 = new int[o_numIndices];
		if (edgeData != null)
		{
			TrackUtils.AppendSplineGeometry(null, nodes, edgeData, tangentialDivisions, 0, 0, array, array2, array3, array4, m_reversedTrack);
		}
		else
		{
			TrackUtils.AppendSplineGeometry(null, nodes, tangentialDivisions, 0, 0, array, array2, array3, array4, m_reversedTrack);
		}
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.uv2 = array3;
		mesh.triangles = array4;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
	}

	private void InitCollisionGeometry()
	{
		MeshCollider component = base.gameObject.GetComponent<MeshCollider>();
		if (!(component == null))
		{
			return;
		}
		float num = float.MaxValue;
		m_trackColliders = new Collider[nodes.Length - 1];
		int layer = LayerMask.NameToLayer("Track");
		PhysicMaterial sharedMaterial = (PhysicMaterial)Resources.Load("PhysicsMaterials/Track");
		for (int i = 0; i != nodes.Length - 1; i++)
		{
			GameObject gameObject = new GameObject("TrackSegment" + i);
			gameObject.layer = layer;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			Mesh mesh = new Mesh();
			num = Mathf.Min(num, GenerateTrackCollisionGeometry(i, i + 1, mesh));
			component = gameObject.AddComponent<MeshCollider>();
			component.sharedMesh = mesh;
			component.sharedMaterial = sharedMaterial;
			m_trackColliders[i] = component;
		}
		m_surroundColliders = new Collider[nodes.Length - 1];
		int layer2 = LayerMask.NameToLayer("TrackSurround");
		PhysicMaterial sharedMaterial2 = (PhysicMaterial)Resources.Load("PhysicsMaterials/Surround");
		for (int j = 0; j != nodes.Length - 1; j++)
		{
			GameObject gameObject2 = new GameObject("SurroundSegment" + j);
			gameObject2.layer = layer2;
			gameObject2.transform.parent = base.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject2.transform.localScale = Vector3.one;
			Mesh mesh2 = new Mesh();
			GenerateTrackSurroundCollisionGeometry(num, j, j + 1, mesh2);
			component = gameObject2.AddComponent<MeshCollider>();
			component.sharedMesh = mesh2;
			component.sharedMaterial = sharedMaterial2;
			component.smoothSphereCollisions = true;
			m_surroundColliders[j] = component;
			if (PhysicsVehicle.DebugSurroundColliders)
			{
				MeshFilter meshFilter = gameObject2.AddComponent<MeshFilter>();
				meshFilter.sharedMesh = mesh2;
				MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = (Material)Resources.Load("Materials/DebugCollider");
			}
		}
	}

	public float GenerateTrackCollisionGeometry(int startNode, int endNode, Mesh mesh)
	{
		TrackNode[] array = new TrackNode[1 + endNode - startNode];
		Array.Copy(nodes, startNode, array, 0, array.Length);
		float maxDivisionError = 0.05f;
		float[] tangentialDivisions = TrackUtils.SplineTangentialDivisions(array, maxDivisionError);
		int o_numVertices;
		int o_numIndices;
		TrackUtils.SplineCollisionGeometryRequirements(tangentialDivisions, out o_numVertices, out o_numIndices);
		Vector3[] array2 = new Vector3[o_numVertices];
		int[] array3 = new int[o_numIndices];
		float result = TrackUtils.AppendSplineCollisionGeometry(array, tangentialDivisions, 0, 0, array2, array3, m_reversedTrack);
		mesh.vertices = array2;
		mesh.triangles = array3;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		return result;
	}

	public void GenerateTrackSurroundCollisionGeometry(float minHeight, int startNode, int endNode, Mesh mesh)
	{
		TrackNode[] array = new TrackNode[1 + endNode - startNode];
		Array.Copy(nodes, startNode, array, 0, array.Length);
		float maxDivisionError = 0.05f;
		float[] tangentialDivisions = TrackUtils.SplineTangentialDivisions(array, maxDivisionError);
		int o_numVertices;
		int o_numIndices;
		TrackUtils.SplineSurroundCollisionGeometryRequirements(tangentialDivisions, out o_numVertices, out o_numIndices);
		Vector3[] array2 = new Vector3[o_numVertices];
		int[] array3 = new int[o_numIndices];
		TrackUtils.AppendSplineSurroundCollisionGeometry(roofFromMinHeight, minHeight, array, tangentialDivisions, 0, 0, array2, array3, m_reversedTrack);
		mesh.vertices = array2;
		mesh.triangles = array3;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
	}

	public GrapplePoint GrapplePointAt(float t)
	{
		GrapplePoint result = null;
		float num = float.MaxValue;
		for (int i = 0; i < m_grapplePoints.Length; i++)
		{
			GrapplePoint grapplePoint = m_grapplePoints[i];
			if (IsInGrappleArea(grapplePoint, t))
			{
				float num2 = t - grapplePoint.startProp;
				if (num2 < num)
				{
					result = grapplePoint;
					num = num2;
				}
			}
		}
		return result;
	}

	public float ReParamStoT(float s)
	{
		return TrackUtilsBase.ReParamStoT(m_reParamNodes, Mathf.Clamp(s, 0f, Length));
	}

	public float ReParamTtoS(float t)
	{
		return TrackUtilsBase.ReParamTtoS(m_reParamNodes, Mathf.Clamp(t, 0f, MaxT));
	}

	public float ClosestTTo(Vector3 point)
	{
		float value = 0f;
		float num = float.MaxValue;
		for (int i = 0; i < nodes.Length - 1; i++)
		{
			TrackNode trackNode = nodes[i];
			TrackNode trackNode2 = nodes[i + 1];
			Vector3 vector = base.transform.TransformPoint(trackNode.position);
			Vector3 vector2 = base.transform.TransformPoint(trackNode2.position);
			Vector3 normalized = (vector2 - vector).normalized;
			float num2 = Vector3.Dot(normalized, vector);
			float to = Vector3.Dot(normalized, vector2);
			float value2 = Vector3.Dot(normalized, point);
			float num3 = Mathf.Clamp01(Mathf.InverseLerp(num2, to, value2));
			float num4 = (float)i + num3;
			Vector3 vector3 = PointAtT(num4);
			float sqrMagnitude = (point - vector3).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				value = num4;
				num = sqrMagnitude;
			}
		}
		value = Mathf.Clamp(value, 0.01f, MaxT - 0.01f);
		return ClosestTTo(point, value, 1f, 0f, MaxT);
	}

	public float ClosestTTo(Vector3 _point, float _tHint, float _tHintSpread, float lowerBound, float upperBound)
	{
		if (m_nodeCacheWS != null)
		{
			return TrackUtilsBase.ClosestTToPoint(m_nodeCacheWS, _point, _tHint, _tHintSpread, lowerBound, upperBound);
		}
		Vector3 point = base.transform.InverseTransformPoint(_point);
		return TrackUtils.ClosestTToPoint(nodes, point, _tHint, _tHintSpread, lowerBound, upperBound);
	}

	public Vector3 PointAtT(float t)
	{
		if (m_nodeCacheWS != null)
		{
			return TrackUtilsBase.PointAtT(m_nodeCacheWS, t);
		}
		return base.transform.TransformPoint(TrackUtils.PointAtT(nodes, t));
	}

	public Vector3 PointAtT(float _t, float _wOffset)
	{
		if (m_nodeCacheWS != null)
		{
			Vector3 o_point;
			Vector3 o_tangent;
			Vector3 o_normal;
			float o_width;
			TrackUtilsBase.InfoAtT(m_nodeCacheWS, _t, out o_point, out o_tangent, out o_normal, out o_width);
			return o_point + o_normal * (o_width * _wOffset);
		}
		Vector3 o_point2;
		Vector3 o_tangent2;
		Vector3 o_normal2;
		float o_width2;
		TrackUtils.InfoAtT(nodes, _t, out o_point2, out o_tangent2, out o_normal2, out o_width2);
		return base.transform.TransformPoint(o_point2 + o_normal2 * (o_width2 * _wOffset));
	}

	public Vector3 TangentAtT(float t)
	{
		if (m_nodeCacheWS != null)
		{
			return TrackUtilsBase.TangentAtT(m_nodeCacheWS, t);
		}
		return base.transform.TransformDirection(TrackUtils.TangentAtT(nodes, t));
	}

	public Vector3 TangentDTAtT(float t)
	{
		if (m_nodeCacheWS != null)
		{
			return TrackUtilsBase.TangentDTAtT(m_nodeCacheWS, t);
		}
		return base.transform.TransformDirection(TrackUtils.TangentDTAtT(nodes, t));
	}

	public Vector3 UpAtT(float t)
	{
		if (m_nodeCacheWS != null)
		{
			return TrackUtilsBase.UpAtT(m_nodeCacheWS, t);
		}
		return base.transform.TransformDirection(TrackUtils.UpAtT(nodes, t));
	}

	public Vector3 NormalAtT(float t)
	{
		if (m_nodeCacheWS != null)
		{
			return TrackUtilsBase.NormalAtT(m_nodeCacheWS, t);
		}
		return base.transform.TransformDirection(TrackUtils.NormalAtT(nodes, t));
	}

	public float WidthAtT(float _t)
	{
		return TrackUtils.WidthAtT(nodes, _t);
	}

	public float HeightAt(float t, float widthProp)
	{
		return TrackUtils.HeightAtT(nodes, t, widthProp, m_reversedTrack);
	}

	public float DsDtAtT(float t)
	{
		if (m_nodeCacheWS != null)
		{
			return TrackUtilsBase.DsDtAtT(m_nodeCacheWS, t);
		}
		return TrackUtils.DsDtAtT(nodes, t);
	}

	public float CurveAtT(float _t)
	{
		if (m_nodeCacheWS != null)
		{
			return TrackUtilsBase.CurveAtT(m_nodeCacheWS, _t);
		}
		return TrackUtils.CurveAtT(nodes, _t);
	}

	public void InfoAtT(float t, out Vector3 o_point, out Vector3 o_tangent, out Vector3 o_normal, out float o_width)
	{
		if (m_nodeCacheWS != null)
		{
			TrackUtilsBase.InfoAtT(m_nodeCacheWS, t, out o_point, out o_tangent, out o_normal, out o_width);
			return;
		}
		TrackUtils.InfoAtT(nodes, t, out o_point, out o_tangent, out o_normal, out o_width);
		Transform transform = base.transform;
		o_point = transform.TransformPoint(o_point);
		o_tangent = transform.TransformDirection(o_tangent);
		o_normal = transform.TransformDirection(o_normal);
	}

	public void OnReverseTrack()
	{
		int num = nodes.Length;
		TrackNode[] array = new TrackNode[num];
		for (int i = 0; i < num; i++)
		{
			TrackNode trackNode = nodes[num - (i + 1)];
			trackNode.rotation *= Quaternion.LookRotation(-Vector3.forward);
			array[i] = trackNode;
		}
		nodes = array;
		m_reversedTrack = !m_reversedTrack;
		SetupLengthAndReParam();
	}

	public void ConnectHubToHead(TrackHub hub, int hubSector)
	{
		if (HubAtHead != null && HubAtHead != hub)
		{
			HubAtHead.DisconnectOutgoingSpline(this);
		}
		HubAtHead = hub;
		HubSectorAtHead = hubSector;
	}

	public void ConnectHubToTail(TrackHub hub, int hubSector)
	{
		if (HubAtTail != null && HubAtTail != hub)
		{
			HubAtTail.DisconnectIncomingSpline(this);
		}
		HubAtTail = hub;
		HubSectorAtTail = hubSector;
	}

	public int ClosestNodeIndex(float t)
	{
		return MathUtils.Clamp(Mathf.RoundToInt(t), 0, nodes.Length - 1);
	}

	public int BeforeNodeIndex(float t)
	{
		return MathUtils.Clamp(Mathf.FloorToInt(t), 0, nodes.Length - 2);
	}

	public int AfterNodeIndex(float t)
	{
		return MathUtils.Clamp(Mathf.CeilToInt(t), 1, nodes.Length - 1);
	}

	public static TrackSpline FindClosestToPoint(Vector3 _point)
	{
		return FindClosestToPoint(_point, float.MaxValue);
	}

	public static TrackSpline FindClosestToPoint(Vector3 _point, float _yRange)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("TrackSpline");
		float num = float.MaxValue;
		TrackSpline result = null;
		for (int i = 0; i < array.Length; i++)
		{
			TrackSpline component = array[i].GetComponent<TrackSpline>();
			if (component != null)
			{
				float t = component.ClosestTTo(_point);
				Vector3 vector = component.PointAtT(t);
				Vector3 vector2 = vector - _point;
				float magnitude = vector2.magnitude;
				if (magnitude < num && Mathf.Abs(vector2.y) <= _yRange)
				{
					num = magnitude;
					result = component;
				}
			}
		}
		return result;
	}
}
