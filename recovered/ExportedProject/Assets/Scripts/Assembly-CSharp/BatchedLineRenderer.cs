using System;
using UnityEngine;

[AddComponentMenu("Graphics/Batched Line Renderer")]
public class BatchedLineRenderer : BatchedLineRendererBase
{
	public class Line : LineBase
	{
		private BatchedLineRenderer m_renderer;

		internal Vector3[] m_points;

		public int PointCount
		{
			get
			{
				return m_pointCount;
			}
		}

		internal Line(BatchedLineRenderer _renderer, Vector3[] _points)
		{
			m_renderer = _renderer;
			m_points = _points;
		}

		public void SetPoint(int _pointIndex, Vector3 _point, Vector3 _halfWidth, Color _colour)
		{
			m_renderer.SetPoint(this, _pointIndex, _point, _halfWidth, _colour);
		}

		public void SetPoint(int _pointIndex, Vector3 _point, float _halfWidth, Color _colour)
		{
			m_renderer.SetPoint(this, _pointIndex, _point, _halfWidth, _colour);
		}

		public void SetAllVertices(Vector3 _point, Color _colour)
		{
			m_renderer.SetAllVertices(this, _point, _colour);
		}

		public void SetLine(Vector3 _start, Vector3 _end, float _halfWidth, Color _colour)
		{
			m_renderer.SetLine(this, _start, _end, _halfWidth, _colour);
		}

		public Vector3 GetPoint(int _pointIndex)
		{
			return m_points[_pointIndex];
		}

		public void Destroy()
		{
			if (m_renderer != null)
			{
				m_renderer.DestroyLine(this);
			}
		}

		public override void OnDestroy()
		{
			m_renderer = null;
			m_points = null;
		}
	}

	public Line CreateLine(Vector3 _initialPoint, Rect _uvRect, Color _initialColour, int _particleCount)
	{
		return CreateLine(_initialPoint, _initialPoint, Vector3.zero, _uvRect, _initialColour, _particleCount);
	}

	public Line CreateLine(Vector3 _initialStart, Vector3 _initialEnd, Vector3 _halfWidth, Rect _uvRect, Color _initialColour, int _pointCount)
	{
		Vector3[] array = new Vector3[_pointCount];
		Line line = new Line(this, array);
		InitialiseLine(line, _initialStart, _initialEnd, _halfWidth, _uvRect, _initialColour, _pointCount, array);
		return line;
	}

	private void SetPoint(Line _line, int _pointIndex, Vector3 _point, Vector3 _halfWidth, Color _colour)
	{
		_line.m_points[_pointIndex] = _point;
		int num = _line.m_vertexOffset + _pointIndex * 2;
		m_vertices[num] = _point - _halfWidth;
		m_vertices[num + 1] = _point + _halfWidth;
		m_colours[num] = _colour;
		m_colours[num + 1] = _colour;
		m_verticesDirty = true;
	}

	private void SetPoint(Line _line, int _pointIndex, Vector3 _point, float _halfWidth, Color _colour)
	{
		_line.m_points[_pointIndex] = _point;
		int num = _line.m_vertexOffset + _pointIndex * 2;
		int pointIndex = Math.Max(0, _pointIndex - 1);
		int pointIndex2 = Math.Min(_line.PointCount - 1, _pointIndex + 1);
		Vector3 rhs = _line.GetPoint(pointIndex2) - _line.GetPoint(pointIndex);
		Vector3 vector = Vector3.Cross(Vector3.up, rhs).normalized * _halfWidth;
		m_vertices[num] = _point - vector;
		m_vertices[num + 1] = _point + vector;
		m_colours[num] = _colour;
		m_colours[num + 1] = _colour;
		m_verticesDirty = true;
	}

	private void SetAllVertices(Line _line, Vector3 _point, Color _colour)
	{
		int num = _line.m_vertexOffset;
		for (int i = 0; i < _line.PointCount; i++)
		{
			_line.m_points[i] = _point;
			m_vertices[num] = _point;
			m_colours[num] = _colour;
			num++;
			m_vertices[num] = _point;
			m_colours[num] = _colour;
			num++;
		}
		m_verticesDirty = true;
	}

	private void SetLine(Line _line, Vector3 _start, Vector3 _end, float _halfWidth, Color _colour)
	{
		Vector3 rhs = _end - _start;
		Vector3 vector = Vector3.Cross(Vector3.up, rhs).normalized * _halfWidth;
		int num = _line.m_vertexOffset;
		for (int i = 0; i < _line.PointCount; i++)
		{
			float t = 1f - (float)i / (float)(_line.PointCount - 1);
			Vector3 vector2 = Vector3.Lerp(_start, _end, t);
			_line.m_points[i] = vector2;
			m_vertices[num] = vector2 - vector;
			m_colours[num] = _colour;
			num++;
			m_vertices[num] = vector2 + vector;
			m_colours[num] = _colour;
			num++;
		}
		m_verticesDirty = true;
	}
}
