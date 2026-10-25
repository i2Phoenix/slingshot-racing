using System.Runtime.InteropServices;
using UnityEngine;

public class Rope
{
	private Vector2[] m_positions;

	private Vector2[] m_oldPositions;

	private int m_numParticles;

	private float m_restLength;

	public Rope(int _numParticles)
	{
		m_numParticles = _numParticles;
		m_positions = new Vector2[m_numParticles];
		m_oldPositions = new Vector2[m_numParticles];
	}

	public void Init(Vector3 _pos0, Vector3 _vel0, Vector3 _pos1)
	{
		Vector2 vector = new Vector2(_pos0.x, _pos0.z);
		Vector2 vector2 = new Vector2(_vel0.x, _vel0.z);
		Vector2 vector3 = new Vector2(_pos1.x, _pos1.z);
		Vector2 zero = Vector2.zero;
		float fixedDeltaTime = Time.fixedDeltaTime;
		vector2 *= fixedDeltaTime;
		zero *= fixedDeltaTime;
		Vector2 vector4 = vector3 - vector;
		m_restLength = (vector4 / (m_numParticles - 1)).magnitude * 0.8f;
		for (int i = 0; i < m_numParticles; i++)
		{
			float num = (float)i / (float)(m_numParticles - 1);
			Vector2 vector5 = vector + vector4 * num;
			Vector2 vector6 = Vector2.Lerp(vector2, zero, num);
			vector5.x += Random.Range(-1f, 1f) * m_restLength * 0.1f;
			vector5.y += Random.Range(-1f, 1f) * m_restLength * 0.1f;
			m_positions[i] = vector5;
			m_oldPositions[i] = vector5 - vector6;
		}
	}

	public void Update(float _grappleForce, Vector3 _pos0, ref Vector3 _pos1, float _radius1)
	{
		float straighten = Mathf.Clamp(_grappleForce * 0.00125f, 0f, 1f);
		if (m_numParticles > 0)
		{
			RopeUpdateParams _params = default(RopeUpdateParams);
			_params.m_numParticles = (uint)m_numParticles;
			_params.m_restLength = m_restLength;
			_params.m_straighten = straighten;
			_params.m_radius1 = _radius1;
			_params.m_pos0.x = _pos0.x;
			_params.m_pos0.y = _pos0.z;
			_params.m_pos1.x = _pos1.x;
			_params.m_pos1.y = _pos1.z;
			RopeUpdate(ref m_positions[0], ref m_oldPositions[0], ref _params);
			m_restLength = _params.m_restLength;
			Vector2[] positions = m_positions;
			m_positions = m_oldPositions;
			m_oldPositions = positions;
		}
	}

	private void Update_CSharp(float _straighten, ref Vector3 _pos0, ref Vector3 _pos1, float _radius1)
	{
		float num = 0.5f;
		for (int i = 0; i < m_numParticles; i++)
		{
			Vector2 vector = (m_positions[i] - m_oldPositions[i]) * 1f;
			m_oldPositions[i] = m_positions[i] + vector;
		}
		Vector2[] positions = m_positions;
		m_positions = m_oldPositions;
		m_oldPositions = positions;
		Vector2 vector2 = new Vector2(_pos0.x, _pos0.z);
		Vector2 vector3 = new Vector2(_pos1.x, _pos1.z);
		if (_straighten > 0f)
		{
			num = (1f - _straighten) * 0.5f;
			float num2 = 1f / (float)(m_numParticles - 1);
			Vector2 vector4 = (vector3 - vector2) * num2;
			float num3 = vector4.magnitude * 0.8f;
			if (num3 < m_restLength)
			{
				m_restLength = Mathf.Lerp(m_restLength, num3, _straighten);
			}
			float lerp = Mathf.Clamp01(_straighten);
			Vector2 b = vector2;
			Vector2 vector5 = vector2 - m_oldPositions[0];
			Vector2 vector6 = -vector5 * num2;
			for (int j = 0; j < m_numParticles; j++)
			{
				m_positions[j] = Vector2Utils.LerpNoClamp(m_positions[j], b, lerp);
				m_oldPositions[j] = Vector2Utils.LerpNoClamp(m_oldPositions[j], m_positions[j] - vector5, lerp);
				b += vector4;
				vector5 += vector6;
			}
		}
		float num4 = m_restLength * m_restLength;
		float num5 = num * -2f;
		for (int k = 0; k != 6; k++)
		{
			Vector2 vector7 = m_positions[0];
			int num6 = 0;
			for (int l = 1; l != m_numParticles; l++)
			{
				Vector2 vector8 = m_positions[l];
				float num7 = vector8.x - vector7.x;
				float num8 = vector8.y - vector7.y;
				float num9 = num7 * num7 + num8 * num8;
				float num10 = (num4 / (num9 + num4) - 0.5f) * num5;
				float num11 = num7 * num10;
				float num12 = num8 * num10;
				m_positions[num6].x = vector7.x + num11;
				m_positions[num6].y = vector7.y + num12;
				vector7.x = vector8.x - num11;
				vector7.y = vector8.y - num12;
				num6++;
			}
			m_positions[num6].x = vector7.x;
			m_positions[num6].y = vector7.y;
			m_positions[0] = vector2;
			if (_straighten <= 0f)
			{
				m_positions[num6] = m_oldPositions[num6];
				continue;
			}
			Vector2 vector9 = m_positions[num6] - vector3;
			float sqrMagnitude = vector9.sqrMagnitude;
			if (sqrMagnitude > 0.0001f)
			{
				m_positions[num6] = vector3 + vector9 * (_radius1 / Mathf.Sqrt(sqrMagnitude));
			}
		}
	}

	public void AddToLineMesh(LineMesh _lineMesh, float _lerp, float _y0, float _y1, float _lengthProp, bool _lengthPropIncreasing, Vector3 _renderPos0, Color _colour)
	{
		_lerp = Mathf.Clamp01(_lerp);
		Vector3[] array = _lineMesh.AccessVertices();
		Color32[] array2 = _lineMesh.AccessColours();
		float num = _lengthProp;
		num = ((!_lengthPropIncreasing) ? Mathf.Pow(_lengthProp, 2f) : (1f - Mathf.Pow(1f - _lengthProp, 2f)));
		RopeUpdateMeshParams job = default(RopeUpdateMeshParams);
		job.m_numParticles = (uint)m_numParticles;
		job.m_frameLerp = _lerp;
		job.m_y0 = _y0;
		job.m_y1 = _y1;
		job.m_lengthProp = num;
		job.m_renderPos0.x = _renderPos0.x;
		job.m_renderPos0.y = _renderPos0.z;
		job.m_colour = _colour;
		RopeUpdateMesh2(out array[0], out array2[0], ref m_positions[0], ref m_oldPositions[0], ref job);
	}

	private void UpdateMesh_CSharp(Vector3[] points, Color32[] colours, float _lerp, float _y0, float _y1, float _lengthProp, ref Vector3 _renderPos0, ref Color _colour)
	{
		Vector2 vector = m_positions[m_numParticles - 1];
		Vector2 vector2 = default(Vector2);
		vector2.x = vector.y - _renderPos0.z;
		vector2.y = _renderPos0.x - vector.x;
		vector2.Normalize();
		Vector3 vector3 = default(Vector3);
		vector3.x = vector2.x * 0.15f;
		vector3.y = 0f;
		vector3.z = vector2.y * 0.15f;
		Vector2 vector4 = default(Vector2);
		vector4.x = _renderPos0.x;
		vector4.y = _renderPos0.z;
		Vector3 vector5 = default(Vector3);
		vector5.x = vector4.x;
		vector5.y = _y0;
		vector5.z = vector4.y;
		Color color = _colour;
		Color32 color2 = _colour;
		points[0] = vector5 - vector3;
		points[1] = vector5 + vector3;
		colours[0] = color2;
		colours[1] = color2;
		Vector2 vector6 = vector4 - Vector2Utils.LerpNoClamp(m_oldPositions[0], m_positions[0], _lerp);
		float num = 1f / (float)(m_numParticles - 1);
		float num2 = (_y1 - _y0) * num;
		float num3 = (1f - _colour.r) * num;
		float num4 = (1f - _colour.g) * num;
		float num5 = (1f - _colour.b) * num;
		int num6 = Mathf.Min(Mathf.FloorToInt(_lengthProp * (float)m_numParticles), m_numParticles - 1);
		for (int i = 1; i < num6; i++)
		{
			vector5.y += num2;
			color.r += num3;
			color.g += num4;
			color.b += num5;
			float num7 = (float)i * num;
			Vector2 vector7 = Vector2Utils.LerpNoClamp(m_oldPositions[i], m_positions[i], _lerp);
			vector7 += vector6 * (1f - num7);
			vector5.x = vector7.x;
			vector5.z = vector7.y;
			color2 = color;
			points[i * 2] = vector5 - vector3;
			points[i * 2 + 1] = vector5 + vector3;
			colours[i * 2] = color2;
			colours[i * 2 + 1] = color2;
		}
		int num8 = num6;
		Vector3 b = vector5;
		Color b2 = color;
		b.y += num2;
		b2.r += num3;
		b2.g += num4;
		b2.b += num5;
		float num9 = (float)num8 * num;
		Vector2 vector8 = Vector2Utils.LerpNoClamp(m_oldPositions[num8], m_positions[num8], _lerp);
		vector8 += vector6 * (1f - num9);
		b.x = vector8.x;
		b.z = vector8.y;
		float num10 = _lengthProp * (float)m_numParticles - (float)num6;
		vector5 = Vector3Utils.LerpNoClamp(vector5, b, num10);
		color = Color.Lerp(color, b2, num10);
		color2 = color;
		points[num8 * 2] = vector5 - vector3;
		points[num8 * 2 + 1] = vector5 + vector3;
		colours[num8 * 2] = color2;
		colours[num8 * 2 + 1] = color2;
		for (int j = num6 + 1; j < m_numParticles; j++)
		{
			points[j * 2] = vector5;
			points[j * 2 + 1] = vector5;
			colours[j * 2] = color2;
			colours[j * 2 + 1] = color2;
		}
	}

	[DllImport("GrappleRacerNative")]
	private static extern void RopeUpdate(ref Vector2 particles, ref Vector2 oldParticles, ref RopeUpdateParams _params);

	[DllImport("GrappleRacerNative")]
	private static extern void RopeUpdateMesh2(out Vector3 vertices, out Color32 colours, ref Vector2 positions, ref Vector2 oldPositions, ref RopeUpdateMeshParams job);
}
