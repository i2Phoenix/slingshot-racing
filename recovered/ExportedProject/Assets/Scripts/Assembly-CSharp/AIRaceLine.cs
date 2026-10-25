using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("AI/AI Race Line")]
[RequireComponent(typeof(TrackSpline))]
public class AIRaceLine : MonoBehaviour
{
	[Serializable]
	public class Node
	{
		public float m_prop;

		public float m_widthProp;

		public Node Clone()
		{
			Node node = new Node();
			node.m_prop = m_prop;
			node.m_widthProp = m_widthProp;
			return node;
		}

		public static int Compare(Node _lhs, Node _rhs)
		{
			if (_lhs.m_prop < _rhs.m_prop)
			{
				return -1;
			}
			if (_lhs.m_prop > _rhs.m_prop)
			{
				return 1;
			}
			return 0;
		}
	}

	[Serializable]
	private class PerDifficultyData
	{
		[NonSerialized]
		public Vector3[] m_points;

		[NonSerialized]
		public Vector3[] m_tangents;

		[NonSerialized]
		public bool m_splineGenerated;

		public Node[] m_nodes;
	}

	private const float c_debugRenderInterval = 0.05f;

	private const float c_minTimeDelta = 0.1f;

	private const float c_allowedBasicError = 0.5f;

	private const float c_basicCurvatureScale = 1f;

	private const float c_basicCurvaturePower = 1f;

	[SerializeField]
	private PerDifficultyData[] m_perDifficultyData;

	private TrackSpline m_cachedTrackSpline;

	public TrackSpline TargetTrackSpline
	{
		get
		{
			if (m_cachedTrackSpline == null)
			{
				m_cachedTrackSpline = GetComponent<TrackSpline>();
			}
			return m_cachedTrackSpline;
		}
	}

	public void EnsurePerDifficultyData()
	{
		if (m_perDifficultyData != null && m_perDifficultyData.Length == 4)
		{
			return;
		}
		m_perDifficultyData = new PerDifficultyData[4];
		for (int i = 0; i < 4; i++)
		{
			if (m_perDifficultyData[i] == null)
			{
				m_perDifficultyData[i] = new PerDifficultyData();
			}
		}
	}

	private void Awake()
	{
		EnsurePerDifficultyData();
	}

	private void Start()
	{
		Debug.LogWarning("AIRaceLine is obselete", this);
		for (int i = 0; i < 4; i++)
		{
			EnsureSpline((AIDifficulty)i);
		}
	}

	public Vector3 PointAt(float _prop, AIDifficulty _difficulty)
	{
		PerDifficultyData perDifficultyData = m_perDifficultyData[(int)_difficulty];
		if (perDifficultyData.m_nodes == null || perDifficultyData.m_nodes.Length == 0)
		{
			return Vector3.zero;
		}
		if (!perDifficultyData.m_splineGenerated)
		{
			EnsureSpline(_difficulty);
		}
		int i;
		for (i = 0; i < perDifficultyData.m_nodes.Length && !(perDifficultyData.m_nodes[i].m_prop >= _prop); i++)
		{
		}
		if (i <= 0)
		{
			return perDifficultyData.m_points[0];
		}
		if (i >= perDifficultyData.m_nodes.Length)
		{
			int num = perDifficultyData.m_nodes.Length - 1;
			float num2 = Mathf.Max(0f, _prop - perDifficultyData.m_nodes[num].m_prop);
			return perDifficultyData.m_points[num] + perDifficultyData.m_tangents[num] * num2;
		}
		int num3 = i - 1;
		int num4 = i;
		Node node = perDifficultyData.m_nodes[num3];
		Node node2 = perDifficultyData.m_nodes[num4];
		float t = Mathf.Clamp01(Mathf.InverseLerp(node.m_prop, node2.m_prop, _prop));
		return SplineUtils.CubicHermite(perDifficultyData.m_points[num3], perDifficultyData.m_tangents[num3], perDifficultyData.m_points[num4], perDifficultyData.m_tangents[num4], t);
	}

	private float BasicWidthPropScale(AIDifficulty _difficulty)
	{
		switch (_difficulty)
		{
		default:
			return -0.5f;
		case AIDifficulty.Medium:
			return -0.166667f;
		case AIDifficulty.Hard:
			return 0.166667f;
		case AIDifficulty.Optimal:
			return 0.5f;
		}
	}

	private float BasicWidthPropAtT(float _t, AIDifficulty _difficulty)
	{
		float f = TargetTrackSpline.CurveAtT(_t) * 1f;
		float num = Mathf.Sign(f);
		f = num * Mathf.Pow(Mathf.Abs(f), 1f);
		return Mathf.Clamp(f, -1f, 1f) * BasicWidthPropScale(_difficulty);
	}

	public void GenerateBasicNodes(AIDifficulty _difficulty)
	{
		List<Node> list = new List<Node>();
		int num = TargetTrackSpline.nodes.Length;
		for (int i = 0; i < num; i++)
		{
			float num2 = (float)i / (float)(num - 1) * TargetTrackSpline.MaxT;
			Node node = new Node();
			node.m_prop = num2;
			node.m_widthProp = BasicWidthPropAtT(num2, _difficulty);
			list.Add(node);
		}
		bool flag;
		do
		{
			flag = false;
			int count = list.Count;
			for (int j = 1; j < count; j++)
			{
				Node nodeBefore = ((j <= 1) ? null : list[j - 2]);
				Node node2 = list[j - 1];
				Node node3 = list[j];
				Node nodeAfter = ((j >= count - 1) ? null : list[j + 1]);
				if (!(node3.m_prop - node2.m_prop < 0.1f))
				{
					Vector3 p = NodePoint(node2);
					Vector3 m = NodeTangent(node2, nodeBefore, node3);
					Vector3 p2 = NodePoint(node3);
					Vector3 m2 = NodeTangent(node3, node2, nodeAfter);
					Vector3 vector = SplineUtils.CubicHermite(p, m, p2, m2, 0.5f);
					float num3 = (node2.m_prop + node3.m_prop) * 0.5f;
					float num4 = BasicWidthPropAtT(num3, _difficulty);
					Vector3 o_point;
					Vector3 o_tangent;
					Vector3 o_normal;
					float o_width;
					TargetTrackSpline.InfoAtT(num3, out o_point, out o_tangent, out o_normal, out o_width);
					Vector3 vector2 = o_point + o_normal * (o_width * num4);
					if ((vector2 - vector).magnitude > o_width * 0.5f)
					{
						Node node4 = new Node();
						node4.m_prop = num3;
						node4.m_widthProp = num4;
						list.Add(node4);
						flag = true;
					}
				}
			}
			list.Sort(Node.Compare);
		}
		while (flag);
		bool flag2;
		do
		{
			flag2 = false;
			int count2 = list.Count;
			for (int k = 1; k < count2 - 1; k++)
			{
				Node nodeBefore2 = ((k <= 1) ? null : list[k - 2]);
				Node node5 = list[k - 1];
				Node node6 = list[k];
				Node node7 = list[k + 1];
				Node nodeAfter2 = ((k >= count2 - 2) ? null : list[k + 2]);
				Vector3 p3 = NodePoint(node5);
				Vector3 m3 = NodeTangent(node5, nodeBefore2, node7);
				Vector3 p4 = NodePoint(node7);
				Vector3 m4 = NodeTangent(node7, node5, nodeAfter2);
				Vector3 vector3 = SplineUtils.CubicHermite(p3, m3, p4, m4, 0.5f);
				float t = (node5.m_prop + node7.m_prop) * 0.5f;
				float num5 = TargetTrackSpline.WidthAtT(t);
				Vector3 vector4 = NodePoint(node6);
				if ((vector4 - vector3).magnitude <= num5 * 0.5f)
				{
					list.RemoveAt(k);
					flag2 = true;
					break;
				}
			}
		}
		while (flag2);
		EnsurePerDifficultyData();
		PerDifficultyData perDifficultyData = m_perDifficultyData[(int)_difficulty];
		perDifficultyData.m_nodes = list.ToArray();
		perDifficultyData.m_points = null;
		perDifficultyData.m_tangents = null;
		perDifficultyData.m_splineGenerated = false;
	}

	private Vector3 NodePoint(Node _node)
	{
		Vector3 o_point;
		Vector3 o_tangent;
		Vector3 o_normal;
		float o_width;
		TargetTrackSpline.InfoAtT(_node.m_prop, out o_point, out o_tangent, out o_normal, out o_width);
		return o_point + o_normal * (o_width * _node.m_widthProp);
	}

	private Vector3 NodeTangent(Node _node, Node _nodeBefore, Node _nodeAfter)
	{
		if (_nodeBefore != null && _nodeAfter != null)
		{
			Vector3 vector = NodePoint(_nodeBefore);
			Vector3 vector2 = NodePoint(_node);
			Vector3 vector3 = NodePoint(_nodeAfter);
			return (vector3 - vector2) * 0.5f + (vector2 - vector) * 0.5f;
		}
		return TargetTrackSpline.TangentAtT(_node.m_prop);
	}

	public void EnsureSpline(AIDifficulty _difficulty)
	{
		EnsurePerDifficultyData();
		PerDifficultyData perDifficultyData = m_perDifficultyData[(int)_difficulty];
		if (perDifficultyData.m_splineGenerated)
		{
			return;
		}
		perDifficultyData.m_points = null;
		perDifficultyData.m_tangents = null;
		if (perDifficultyData.m_nodes == null || perDifficultyData.m_nodes.Length == 0)
		{
			GenerateBasicNodes(_difficulty);
		}
		int num = perDifficultyData.m_nodes.Length;
		if (num > 0)
		{
			perDifficultyData.m_points = new Vector3[num];
			for (int i = 0; i < num; i++)
			{
				perDifficultyData.m_points[i] = NodePoint(perDifficultyData.m_nodes[i]);
			}
			perDifficultyData.m_tangents = new Vector3[num];
			for (int j = 0; j < num; j++)
			{
				Node nodeBefore = ((j <= 0) ? null : perDifficultyData.m_nodes[j - 1]);
				Node nodeAfter = ((j >= num - 1) ? null : perDifficultyData.m_nodes[j + 1]);
				perDifficultyData.m_tangents[j] = NodeTangent(perDifficultyData.m_nodes[j], nodeBefore, nodeAfter);
			}
			perDifficultyData.m_splineGenerated = true;
		}
	}

	public Node[] GetNodes(AIDifficulty _difficulty)
	{
		return m_perDifficultyData[(int)_difficulty].m_nodes;
	}

	public void SetNodes(AIDifficulty _difficulty, Node[] _nodes)
	{
		m_perDifficultyData[(int)_difficulty].m_nodes = _nodes;
		m_perDifficultyData[(int)_difficulty].m_points = null;
		m_perDifficultyData[(int)_difficulty].m_tangents = null;
		m_perDifficultyData[(int)_difficulty].m_splineGenerated = false;
	}

	public void TrackNodeAdded(int _nodeIndex)
	{
		m_perDifficultyData = null;
	}

	public void TrackNodeDeleted(int _nodeIndex)
	{
		m_perDifficultyData = null;
	}
}
