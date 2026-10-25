using UnityEngine;

[AddComponentMenu("Graphics/Batched Trail Renderer")]
public class BatchedTrailRenderer : BatchedLineRendererBase
{
	public class Trail : LineBase
	{
		private Vector3 m_position;

		private BatchedTrailRenderer m_renderer;

		public Vector3 Position
		{
			get
			{
				return m_position;
			}
		}

		internal Trail(BatchedTrailRenderer _renderer, Vector3 _position)
		{
			m_position = _position;
			m_renderer = _renderer;
		}

		public void AddParticle(Vector3 _point, Vector3 _halfWidth, Color _colour)
		{
			m_position = _point;
			m_renderer.AddParticle(this, _point, _halfWidth, _colour);
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
		}
	}

	public Trail CreateTrail(Vector3 _initialPoint, Rect _uvRect, Color _initialColour, int _particleCount)
	{
		return CreateTrail(_initialPoint, _initialPoint, Vector3.zero, _uvRect, _initialColour, _particleCount);
	}

	public Trail CreateTrail(Vector3 _initialStart, Vector3 _initialEnd, Vector3 _halfWidth, Rect _uvRect, Color _initialColour, int _particleCount)
	{
		Trail trail = new Trail(this, _initialEnd);
		InitialiseLine(trail, _initialStart, _initialEnd, _halfWidth, _uvRect, _initialColour, _particleCount, null);
		return trail;
	}

	private void AddParticle(Trail _trail, Vector3 _point, Vector3 _halfWidth, Color _colour)
	{
		int vertexOffset = _trail.m_vertexOffset;
		int pointCount = _trail.m_pointCount;
		for (int num = pointCount - 1; num > 0; num--)
		{
			int num2 = vertexOffset + num * 2;
			int num3 = vertexOffset + num * 2 + 1;
			m_vertices[num2] = m_vertices[num2 - 2];
			m_vertices[num3] = m_vertices[num3 - 2];
			m_colours[num2] = m_colours[num2 - 2];
			m_colours[num3] = m_colours[num3 - 2];
		}
		m_vertices[vertexOffset] = _point - _halfWidth;
		m_vertices[vertexOffset + 1] = _point + _halfWidth;
		m_colours[vertexOffset] = _colour;
		m_colours[vertexOffset + 1] = _colour;
		m_verticesDirty = true;
	}
}
