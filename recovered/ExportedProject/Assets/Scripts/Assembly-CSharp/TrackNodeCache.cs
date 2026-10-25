using UnityEngine;

public struct TrackNodeCache
{
	public readonly Vector3 m_position;

	public readonly Vector3 m_tangent;

	public readonly Vector3 m_up;

	public readonly float m_width;

	public TrackNodeCache(Vector3 _position, Vector3 _tangent, Vector3 _up, float _width)
	{
		m_position = _position;
		m_tangent = _tangent;
		m_up = _up;
		m_width = _width;
	}

	public TrackNodeCache(Transform _transform, Vector3 _position, Vector3 _tangent, Vector3 _up, float _width)
	{
		m_position = _transform.TransformPoint(_position);
		m_tangent = _transform.TransformDirection(_tangent);
		m_up = _transform.TransformDirection(_up);
		m_width = _width;
	}
}
