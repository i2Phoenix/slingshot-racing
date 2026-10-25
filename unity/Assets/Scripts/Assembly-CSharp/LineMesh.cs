using UnityEngine;

[AddComponentMenu("Graphics/Line Mesh")]
public class LineMesh : LineMeshBase
{
	private int m_numPoints;

	public int PointCount
	{
		get
		{
			return m_numPoints;
		}
	}

	public static LineMesh Create(Vector3 _initialPoint, Rect _uvRect, Color _initialColour, int _particleCount)
	{
		return Create(_initialPoint, _initialPoint, Vector3.zero, _uvRect, _initialColour, _particleCount);
	}

	public static LineMesh Create(Vector3 _initialStart, Vector3 _initialEnd, Vector3 _halfWidth, Rect _uvRect, Color _initialColour, int _numPoints)
	{
		GameObject gameObject = new GameObject("LineMesh");
		LineMesh lineMesh = gameObject.AddComponent<LineMesh>();
		lineMesh.Initialise(_initialStart, _initialEnd, _halfWidth, _uvRect, _initialColour, _numPoints, null);
		lineMesh.m_numPoints = _numPoints;
		return lineMesh;
	}

	public Vector3[] AccessVertices()
	{
		MarkDirty((DirtyFlags)3);
		return m_vertices;
	}

	public Color32[] AccessColours()
	{
		MarkDirty(DirtyFlags.Colours);
		return m_colours;
	}

	public void SetPoint(int _pointIndex, Vector3 _point, Color32 _colour)
	{
		int num = _pointIndex * 2;
		m_vertices[num] = _point;
		m_vertices[num + 1] = _point;
		m_colours[num] = _colour;
		m_colours[num + 1] = _colour;
		MarkDirty((DirtyFlags)7);
	}

	public void SetPoint(int _pointIndex, Vector3 _point, Vector3 _halfWidth, Color32 _colour)
	{
		int num = _pointIndex * 2;
		m_vertices[num] = _point - _halfWidth;
		m_vertices[num + 1] = _point + _halfWidth;
		m_colours[num] = _colour;
		m_colours[num + 1] = _colour;
		MarkDirty((DirtyFlags)7);
	}

	public void SetAllVertices(Vector3 _point, Color32 _colour)
	{
		int num = 0;
		for (int i = 0; i < m_numPoints; i++)
		{
			m_vertices[num] = _point;
			m_colours[num] = _colour;
			num++;
			m_vertices[num] = _point;
			m_colours[num] = _colour;
			num++;
		}
		MarkDirty((DirtyFlags)7);
	}

	public void SetLine(Vector3 _start, Vector3 _end, float _halfWidth, Color32 _colour)
	{
		Vector3 rhs = _end - _start;
		Vector3 vector = Vector3.Cross(Vector3.up, rhs).normalized * _halfWidth;
		int num = 0;
		for (int i = 0; i < m_numPoints; i++)
		{
			float t = (float)i / (float)(m_numPoints - 1);
			Vector3 vector2 = Vector3.Lerp(_start, _end, t);
			m_vertices[num] = vector2 - vector;
			m_colours[num] = _colour;
			num++;
			m_vertices[num] = vector2 + vector;
			m_colours[num] = _colour;
			num++;
		}
		MarkDirty((DirtyFlags)7);
	}

	public void SetLine(Vector3 _start, Vector3 _end, float _halfWidth, Color _startColour, Color _endColour)
	{
		Vector3 rhs = _end - _start;
		Vector3 vector = Vector3.Cross(Vector3.up, rhs).normalized * _halfWidth;
		int num = 0;
		for (int i = 0; i < m_numPoints; i++)
		{
			float t = (float)i / (float)(m_numPoints - 1);
			Vector3 vector2 = Vector3.Lerp(_start, _end, t);
			Color32 color = Color.Lerp(_startColour, _endColour, t);
			m_vertices[num] = vector2 - vector;
			m_colours[num] = color;
			num++;
			m_vertices[num] = vector2 + vector;
			m_colours[num] = color;
			num++;
		}
		MarkDirty((DirtyFlags)7);
	}
}
