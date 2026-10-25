using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
public class BatchedLineRendererBase : MonoBehaviour
{
	public class LineBase
	{
		internal int m_vertexOffset;

		internal int m_indexOffset;

		internal int m_pointCount;

		public virtual void OnDestroy()
		{
		}
	}

	protected MeshFilter m_meshFilter;

	protected MeshRenderer m_meshRenderer;

	protected Mesh m_mesh;

	protected List<LineBase> m_activeLines;

	protected Vector3[] m_vertices;

	protected Vector2[] m_uvs;

	protected Color[] m_colours;

	protected int[] m_triangles;

	protected bool m_rebuildMesh;

	protected bool m_verticesDirty;

	public Material m_material;

	private void Awake()
	{
		m_meshFilter = base.gameObject.AddComponent<MeshFilter>();
		m_meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		m_meshRenderer.castShadows = false;
		m_meshRenderer.receiveShadows = false;
		m_mesh = m_meshFilter.mesh;
		m_activeLines = new List<LineBase>();
	}

	private void Start()
	{
		m_meshRenderer.sharedMaterial = m_material;
	}

	private void LateUpdate()
	{
		if (m_rebuildMesh)
		{
			m_mesh.Clear();
			if (m_vertices != null && m_triangles != null)
			{
				m_mesh.vertices = m_vertices;
				m_mesh.uv = m_uvs;
				m_mesh.colors = m_colours;
				m_mesh.triangles = m_triangles;
			}
			m_rebuildMesh = false;
			m_verticesDirty = false;
		}
		if (m_verticesDirty)
		{
			m_mesh.vertices = m_vertices;
			m_mesh.colors = m_colours;
			m_mesh.RecalculateBounds();
			m_verticesDirty = false;
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < m_activeLines.Count; i++)
		{
			LineBase lineBase = m_activeLines[i];
			lineBase.OnDestroy();
			lineBase.m_vertexOffset = -1;
			lineBase.m_indexOffset = -1;
			lineBase.m_pointCount = 0;
		}
		m_activeLines.Clear();
		m_activeLines = null;
		m_vertices = null;
		m_uvs = null;
		m_colours = null;
		m_triangles = null;
		m_meshFilter = null;
		m_meshRenderer = null;
		m_mesh.Clear();
		m_mesh = null;
		m_material = null;
	}

	protected void InitialiseLine(LineBase _line, Vector3 _initialStart, Vector3 _initialEnd, Vector3 _halfWidth, Rect _uvRect, Color _initialColour, int _pointCount, Vector3[] o_points)
	{
		int num = ((m_vertices != null) ? m_vertices.Length : 0);
		int num2 = ((m_triangles != null) ? m_triangles.Length : 0);
		_line.m_vertexOffset = num;
		_line.m_indexOffset = num2;
		_line.m_pointCount = _pointCount;
		m_activeLines.Add(_line);
		int num3 = VertexCount(_pointCount);
		int num4 = IndexCount(_pointCount);
		num += num3;
		num2 += num4;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		Color[] array3 = new Color[num];
		int[] array4 = new int[num2];
		if (m_vertices != null)
		{
			m_vertices.CopyTo(array, 0);
		}
		if (m_uvs != null)
		{
			m_uvs.CopyTo(array2, 0);
		}
		if (m_colours != null)
		{
			m_colours.CopyTo(array3, 0);
		}
		if (m_triangles != null)
		{
			m_triangles.CopyTo(array4, 0);
		}
		int num5 = _line.m_vertexOffset;
		for (int i = 0; i < _pointCount; i++)
		{
			float num6 = 1f - (float)i / (float)(_pointCount - 1);
			Vector3 vector = Vector3.Lerp(_initialStart, _initialEnd, num6);
			if (o_points != null)
			{
				o_points[i] = vector;
			}
			array[num5] = vector - _halfWidth;
			array2[num5] = new Vector2(_uvRect.x, _uvRect.y + _uvRect.height * num6);
			array3[num5] = _initialColour;
			num5++;
			array[num5] = vector + _halfWidth;
			array2[num5] = new Vector2(_uvRect.x + _uvRect.width, _uvRect.y + _uvRect.height * num6);
			array3[num5] = _initialColour;
			num5++;
		}
		int num7 = _line.m_vertexOffset;
		int num8 = _line.m_indexOffset;
		for (int j = 0; j < _pointCount - 1; j++)
		{
			array4[num8] = num7;
			array4[num8 + 1] = num7 + 1;
			array4[num8 + 2] = num7 + 2;
			array4[num8 + 3] = num7 + 2;
			array4[num8 + 4] = num7 + 1;
			array4[num8 + 5] = num7 + 3;
			num7 += 2;
			num8 += 6;
		}
		m_vertices = array;
		m_uvs = array2;
		m_colours = array3;
		m_triangles = array4;
		m_rebuildMesh = true;
	}

	protected void DestroyLine(LineBase _line)
	{
		int num = m_vertices.Length - VertexCount(_line.m_pointCount);
		int num2 = m_triangles.Length - IndexCount(_line.m_pointCount);
		int index = m_activeLines.IndexOf(_line);
		_line.OnDestroy();
		_line.m_vertexOffset = -1;
		_line.m_indexOffset = -1;
		_line.m_pointCount = 0;
		m_activeLines.RemoveAt(index);
		if (m_activeLines.Count <= 0)
		{
			m_vertices = null;
			m_uvs = null;
			m_colours = null;
			m_triangles = null;
			m_rebuildMesh = true;
			return;
		}
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		Color[] array3 = new Color[num];
		int[] array4 = new int[num2];
		int num3 = 0;
		int indexOffset = 0;
		for (int i = 0; i < m_activeLines.Count; i++)
		{
			LineBase lineBase = m_activeLines[i];
			int num4 = lineBase.m_vertexOffset;
			int indexOffset2 = lineBase.m_indexOffset;
			lineBase.m_vertexOffset = num3;
			lineBase.m_indexOffset = indexOffset;
			int num5 = VertexCount(lineBase.m_pointCount);
			int num6 = IndexCount(lineBase.m_pointCount);
			for (int j = 0; j < num5; j++)
			{
				array[num3] = m_vertices[num4];
				array2[num3] = m_uvs[num4];
				array3[num3] = m_colours[num4];
				num3++;
				num4++;
			}
			for (int k = 0; k < num6; k++)
			{
				array4[indexOffset++] = m_triangles[indexOffset2++];
			}
		}
		m_vertices = array;
		m_uvs = array2;
		m_colours = array3;
		m_triangles = array4;
		m_rebuildMesh = true;
	}

	private static int VertexCount(int _pointCount)
	{
		return _pointCount * 2;
	}

	private static int IndexCount(int _pointCount)
	{
		return (_pointCount - 1) * 6;
	}
}
