using UnityEngine;

[AddComponentMenu("Camera/Static Camera")]
public class StaticCamera : MainCamera
{
	private PlayableArea m_playableArea;

	private float m_lastAspect;

	private void Start()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("PlayableArea");
		if (gameObject != null)
		{
			m_playableArea = gameObject.GetComponent<PlayableArea>();
		}
	}

	private void Update()
	{
		if (m_playableArea != null)
		{
			Camera camera = base.camera;
			if (m_lastAspect != camera.aspect)
			{
				Transform transform = base.transform;
				Bounds bounds = new Bounds(m_playableArea.transform.position, m_playableArea.transform.localScale);
				float num = CameraUtils.CalculateDistanceToBounds(bounds, Vector3.up, camera, 1f, 1000f);
				transform.position = bounds.center + Vector3.up * num;
				transform.LookAt(bounds.center, Vector3.forward);
				transform.RotateAround(Vector3.up, m_playableArea.transform.eulerAngles.y);
				m_lastAspect = camera.aspect;
			}
		}
	}
}
