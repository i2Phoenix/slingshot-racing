using System;
using UnityEngine;

public static class CameraUtils
{
	public static float CalculateDistanceToBounds(Bounds _bounds, Vector3 _viewOffset, Camera _camera, float _minHeight, float _maxHeight)
	{
		float f = (float)Math.PI / 180f * _camera.fieldOfView * 0.5f;
		float aspect = _camera.aspect;
		float a = _bounds.extents.z / Mathf.Tan(f);
		float b = _bounds.extents.x / (Mathf.Tan(f) * aspect);
		float value = Mathf.Max(a, b);
		value = Mathf.Clamp(value, _minHeight, _maxHeight);
		value += _bounds.extents.y + 0.25f;
		Vector3 normalized = _viewOffset.normalized;
		float f2 = Vector3.Angle(Vector3.up, normalized) * ((float)Math.PI / 180f);
		return value / Mathf.Cos(f2);
	}
}
