using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{
	private static Camera s_camera;

	private static Transform s_transform;

	public static Camera Get
	{
		get
		{
			if (s_camera == null)
			{
				s_camera = Camera.main;
				if (s_camera != null)
				{
					s_transform = s_camera.transform;
				}
			}
			return s_camera;
		}
	}

	public static Transform Transform
	{
		get
		{
			if (s_camera == null)
			{
				s_camera = Camera.main;
				if (s_camera != null)
				{
					s_transform = s_camera.transform;
				}
			}
			return s_transform;
		}
	}

	protected void Awake()
	{
		s_camera = base.camera;
		s_transform = s_camera.transform;
	}

	protected void OnDestroy()
	{
		if (s_camera == this)
		{
			s_camera = null;
			s_transform = null;
		}
	}

	protected void Refresh()
	{
		s_camera = null;
		s_transform = null;
	}
}
