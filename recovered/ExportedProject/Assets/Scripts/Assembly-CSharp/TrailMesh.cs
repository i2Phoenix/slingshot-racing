using System;
using UnityEngine;

[AddComponentMenu("Graphics/Trail Mesh")]
public class TrailMesh : LineMeshBase
{
	private Vector3 m_position;

	private Vector3 m_headPoint0;

	private Vector3 m_headPoint1;

	private Color32 m_headColour;

	private float m_time;

	private int m_pointCount;

	private int m_noWidthThreshold;

	public Vector3 Position
	{
		get
		{
			return m_position;
		}
	}

	public int PointCount
	{
		get
		{
			return m_pointCount;
		}
	}

	public static TrailMesh Create(Vector3 _initialPoint, Rect _uvRect, Color _initialColour, int _pointCount)
	{
		return Create(_initialPoint, _initialPoint, Vector3.zero, _uvRect, _initialColour, _pointCount);
	}

	public static TrailMesh Create(Vector3 _initialHead, Vector3 _initialTail, Vector3 _halfWidth, Rect _uvRect, Color _initialColour, int _pointCount)
	{
		GameObject gameObject = new GameObject("TrailMesh");
		TrailMesh trailMesh = gameObject.AddComponent<TrailMesh>();
		trailMesh.m_position = _initialHead;
		trailMesh.m_pointCount = _pointCount;
		trailMesh.Initialise(_initialHead, _initialTail, _halfWidth, _uvRect, _initialColour, _pointCount, null);
		trailMesh.m_headPoint0 = trailMesh.m_vertices[0];
		trailMesh.m_headPoint1 = trailMesh.m_vertices[1];
		trailMesh.m_headColour = trailMesh.m_colours[0];
		return trailMesh;
	}

	public void UpdateHeadSpatial(DynamicCamera _camera, Vector3 _point, Vector3 _halfWidth, Color32 _colour, float _segmentSize)
	{
		Vector3 position = m_position;
		Vector3 vector = _point - position;
		float magnitude = vector.magnitude;
		float uVSlide;
		if (magnitude > _segmentSize)
		{
			float num = 1f / magnitude;
			float num2 = magnitude / _segmentSize;
			int num3 = Mathf.FloorToInt(num2);
			int num4 = Math.Max(1, num3 - m_pointCount);
			for (int i = num4; i <= num3; i++)
			{
				float num5 = (float)i * _segmentSize * num;
				AddParticle(position + vector * num5, _halfWidth, _colour);
			}
			uVSlide = num2 - (float)num3;
		}
		else
		{
			uVSlide = magnitude / _segmentSize;
		}
		bool flag = IsVisible(ref _point, _camera);
		m_gameObject.active = flag;
		if (flag)
		{
			SetUVSlide(uVSlide);
			TweakHead(ref _point, ref _halfWidth, _colour);
		}
	}

	public void UpdateHeadSpatialNoWidth(DynamicCamera _camera, Vector3 _point, Color32 _colour, float _segmentSize)
	{
		if (m_noWidthThreshold <= m_pointCount)
		{
			Vector3 position = m_position;
			Vector3 vector = _point - position;
			float magnitude = vector.magnitude;
			float uVSlide;
			if (magnitude > _segmentSize)
			{
				float num = 1f / magnitude;
				float num2 = magnitude / _segmentSize;
				int num3 = Mathf.FloorToInt(num2);
				int num4 = Math.Max(1, num3 - m_pointCount);
				for (int i = num4; i <= num3; i++)
				{
					float num5 = (float)i * _segmentSize * num;
					AddParticleNoWidth(position + vector * num5, _colour);
				}
				uVSlide = num2 - (float)num3;
			}
			else
			{
				uVSlide = magnitude / _segmentSize;
			}
			bool flag = IsVisible(ref _point, _camera);
			m_gameObject.active = flag;
			if (flag)
			{
				SetUVSlide(uVSlide);
				TweakHeadNoWidth(ref _point, _colour);
			}
		}
		else
		{
			m_headPoint0 = _point;
			m_headPoint1 = _point;
			m_headColour = _colour;
			m_position = _point;
			m_gameObject.active = false;
		}
	}

	public void UpdateHeadTemporal(DynamicCamera _camera, Vector3 _point, Vector3 _halfWidth, Color32 _colour, float _segmentTime, float _deltaTime)
	{
		Vector3 position = m_position;
		m_time += _deltaTime;
		float uVSlide;
		if (m_time > _segmentTime)
		{
			Vector3 vector = _point - position;
			float num = 1f / m_time;
			float num2 = m_time / _segmentTime;
			int num3 = Mathf.FloorToInt(num2);
			int num4 = Math.Max(1, num3 - m_pointCount);
			for (int i = num4; i <= num3; i++)
			{
				float num5 = (float)i * _segmentTime * num;
				AddParticle(position + vector * num5, _halfWidth, _colour);
			}
			m_time -= (float)num3 * _segmentTime;
			uVSlide = num2 - (float)num3;
		}
		else
		{
			uVSlide = m_time / _segmentTime;
		}
		bool flag = IsVisible(ref _point, _camera);
		m_gameObject.active = flag;
		if (flag)
		{
			SetUVSlide(uVSlide);
			TweakHead(ref _point, ref _halfWidth, _colour);
		}
	}

	public void UpdateHeadTemporalNoWidth(DynamicCamera _camera, Vector3 _point, Color32 _colour, float _segmentTime, float _deltaTime)
	{
		if (m_noWidthThreshold <= m_pointCount)
		{
			Vector3 position = m_position;
			m_time += _deltaTime;
			float uVSlide;
			if (m_time > _segmentTime)
			{
				Vector3 vector = _point - position;
				float num = 1f / m_time;
				float num2 = m_time / _segmentTime;
				int num3 = Mathf.FloorToInt(num2);
				int num4 = Math.Max(1, num3 - m_pointCount);
				for (int i = num4; i <= num3; i++)
				{
					float num5 = (float)i * _segmentTime * num;
					AddParticleNoWidth(position + vector * num5, _colour);
				}
				m_time -= (float)num3 * _segmentTime;
				uVSlide = num2 - (float)num3;
			}
			else
			{
				uVSlide = m_time / _segmentTime;
			}
			bool flag = IsVisible(ref _point, _camera);
			m_gameObject.active = flag;
			if (flag)
			{
				SetUVSlide(uVSlide);
				TweakHeadNoWidth(ref _point, _colour);
			}
		}
		else
		{
			m_headPoint0 = _point;
			m_headPoint1 = _point;
			m_headColour = _colour;
			m_position = _point;
			m_gameObject.active = false;
		}
	}

	private void AddParticle(Vector3 _point, Vector3 _halfWidth, Color32 _colour)
	{
		if (m_noWidthThreshold > m_pointCount)
		{
			int num = m_pointCount * 2;
			for (int i = 2; i != num; i++)
			{
				m_vertices[i] = _point;
				m_colours[i] = _colour;
			}
		}
		else
		{
			BubbleVertices();
		}
		m_noWidthThreshold = 0;
		m_headPoint0 = _point - _halfWidth;
		m_headPoint1 = _point + _halfWidth;
		m_headColour = _colour;
		m_vertices[0] = m_headPoint0;
		m_vertices[1] = m_headPoint1;
		m_colours[0] = m_headColour;
		m_colours[1] = m_headColour;
		MarkDirty((DirtyFlags)7);
		m_position = _point;
	}

	private void AddParticleNoWidth(Vector3 _point, Color32 _colour)
	{
		if (++m_noWidthThreshold <= m_pointCount)
		{
			BubbleVertices();
			m_headPoint0 = _point;
			m_headPoint1 = _point;
			m_headColour = _colour;
			m_vertices[0] = m_headPoint0;
			m_vertices[1] = m_headPoint1;
			m_colours[0] = m_headColour;
			m_colours[1] = m_headColour;
			MarkDirty((DirtyFlags)6);
		}
		m_position = _point;
	}

	private void BubbleVertices()
	{
		int num = m_pointCount * 2;
		int num2 = num;
		while (--num2 >= 4)
		{
			m_vertices[num2] = m_vertices[num2 - 2];
			m_colours[num2] = m_colours[num2 - 2];
		}
		m_vertices[2] = m_headPoint0;
		m_vertices[3] = m_headPoint1;
		m_colours[2] = m_headColour;
		m_colours[3] = m_headColour;
	}

	private void TweakHead(ref Vector3 _point, ref Vector3 _halfWidth, Color32 _colour)
	{
		m_vertices[0] = _point - _halfWidth;
		m_vertices[1] = _point + _halfWidth;
		m_colours[0] = _colour;
		m_colours[1] = _colour;
		MarkDirty((DirtyFlags)6);
	}

	private void TweakHeadNoWidth(ref Vector3 _point, Color32 _colour)
	{
		m_vertices[0] = _point;
		m_vertices[1] = _point;
		m_colours[0] = _colour;
		m_colours[1] = _colour;
		MarkDirty((DirtyFlags)6);
	}

	public void SetAlpha(float _alpha)
	{
		int num = m_pointCount * 2;
		byte a = (byte)(int)(_alpha * 255f);
		for (int i = 0; i < num; i++)
		{
			m_colours[i].a = a;
		}
		MarkDirty(DirtyFlags.Colours);
	}

	public void Set(Vector3 _head, Vector3 _tail, float _halfWidth, Color32 _colour)
	{
		Vector3 rhs = _head - _tail;
		Vector3 vector = Vector3.Cross(Vector3.up, rhs).normalized * _halfWidth;
		int num = 0;
		for (int i = 0; i < m_pointCount; i++)
		{
			float t = (float)i / (float)(m_pointCount - 1);
			Vector3 vector2 = Vector3.Lerp(_head, _tail, t);
			m_vertices[num] = vector2 - vector;
			m_colours[num] = _colour;
			num++;
			m_vertices[num] = vector2 + vector;
			m_colours[num] = _colour;
			num++;
		}
		m_headPoint0 = m_vertices[0];
		m_headPoint1 = m_vertices[1];
		m_headColour = m_colours[0];
		m_position = _head;
		MarkDirty((DirtyFlags)7);
	}

	private bool IsVisible(ref Vector3 _head, DynamicCamera _camera)
	{
		if (_camera != null)
		{
			Vector3 vector = (m_vertices[m_pointCount - 2] + m_vertices[m_pointCount - 1]) * 0.5f;
			Vector3 centre = (_head + vector) * 0.5f;
			float magnitude = (_head - vector).magnitude;
			return _camera.IsSphereInClipFrustum(centre, magnitude);
		}
		return true;
	}
}
