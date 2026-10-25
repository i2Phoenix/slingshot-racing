using UnityEngine;

[AddComponentMenu("Graphics/Skid Mark")]
public class SkidMark : LineMeshBase
{
	private const int c_numFadeOutSteps = 20;

	private const int c_fadeOutStepAmount = 12;

	private Vector3 m_position;

	private float m_vCoord;

	private int m_currentPointIndex;

	private int m_pointCount;

	private float m_fadeOutStepTime;

	private int m_fadeOutStep;

	public bool IsActive
	{
		get
		{
			return base.gameObject.active;
		}
	}

	public void Initialise(int _pointCount)
	{
		Initialise(_uvRect: new Rect(0f, 0f, 1f, 1f), _initialStart: Vector3.zero, _initialEnd: Vector3.zero, _halfWidth: Vector3.zero, _initialColour: Color.black, _pointCount: _pointCount, o_points: null);
		base.gameObject.active = false;
		m_pointCount = _pointCount;
	}

	public void Begin(Vector3 _point, Vector3 _halfWidth, Color32 _colour)
	{
		base.gameObject.active = true;
		_colour.a = 0;
		Vector3 vector = _point - _halfWidth;
		Vector3 vector2 = _point + _halfWidth;
		m_vertices[0] = vector;
		m_vertices[1] = vector2;
		m_colours[0] = _colour;
		m_colours[1] = _colour;
		m_uvs[0].y = 0f;
		m_uvs[1].y = 0f;
		Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
		for (int i = 2; i < m_vertices.Length; i++)
		{
			m_vertices[i] = _point;
			m_colours[i] = color;
		}
		MarkDirty(DirtyFlags.Mesh);
		Vector3 min = Vector3.Min(vector, vector2);
		Vector3 max = Vector3.Max(vector, vector2);
		m_mesh.Bounds = BoundsUtils.BoundsFromMinMax(min, max);
		m_position = _point;
		m_vCoord = 0f;
		m_currentPointIndex = 0;
	}

	public bool NextPoint(Vector3 _point, Vector3 _halfWidth, float _segmentSize, Color32 _colour)
	{
		int num = m_pointCount - 1;
		if (m_currentPointIndex == num)
		{
			return true;
		}
		if ((_point - m_position).sqrMagnitude > _segmentSize * _segmentSize)
		{
			m_currentPointIndex++;
			m_position = _point;
			m_vCoord += _segmentSize;
			int num2 = m_currentPointIndex * 2;
			Vector3 vector = _point - _halfWidth;
			Vector3 vector2 = _point + _halfWidth;
			m_vertices[num2] = vector;
			m_vertices[num2 + 1] = vector2;
			if (m_currentPointIndex < num)
			{
				m_vertices[num2 + 2] = _point;
				m_vertices[num2 + 3] = _point;
			}
			else
			{
				_colour.a = 0;
			}
			m_colours[num2] = _colour;
			m_colours[num2 + 1] = _colour;
			m_uvs[num2].y = m_vCoord;
			m_uvs[num2 + 1].y = m_vCoord;
			MarkDirty(DirtyFlags.Mesh);
			Bounds bounds = m_mesh.Bounds;
			bounds.Encapsulate(vector);
			bounds.Encapsulate(vector2);
			m_mesh.Bounds = bounds;
		}
		return m_currentPointIndex == num;
	}

	public void End(float _fadeOutTime)
	{
		m_fadeOutStepTime = _fadeOutTime / 20f;
		m_fadeOutStep = 0;
		FadeOutStep();
	}

	private void FadeOutStep()
	{
		if (m_fadeOutStep < 20)
		{
			for (int i = 0; i < m_colours.Length; i++)
			{
				m_colours[i].a = (byte)((m_colours[i].a > 12) ? ((uint)(m_colours[i].a - 12)) : 0u);
			}
			MarkDirty(DirtyFlags.Colours);
			m_fadeOutStep++;
			Invoke("FadeOutStep", m_fadeOutStepTime);
		}
		else
		{
			base.gameObject.active = false;
			if (base.transform.parent != null)
			{
				base.transform.parent.SendMessage("SkidMark_FadedOut", this, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
