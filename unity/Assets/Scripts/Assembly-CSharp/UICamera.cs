using UnityEngine;

[AddComponentMenu("GUI/UI Camera")]
[RequireComponent(typeof(Camera))]
[DefaultExecutionOrder(-31000)]
public class UICamera : MonoBehaviour
{
	private Camera m_camera;
	private Vector3 m_designPosition;
	private float m_designSize;

	private void Awake()
	{
		m_camera = GetComponent<Camera>();
		m_designPosition = transform.position;
		m_designSize = m_camera.orthographicSize;
		FitSafeArea();
	}

	private void LateUpdate() { FitSafeArea(); }

	private void FitSafeArea()
	{
		// Fixed-coordinate panels share the safe centre with edge-anchored EZ GUI.
		// Keep the viewport full size so scene backgrounds still cover the display.
		if (!m_camera.orthographic || m_camera.targetTexture != null) return;
		Rect safe = Screen.safeArea;
		if (safe.width <= 0 || safe.height <= 0 || m_camera.pixelHeight <= 0) return;
		// Original panels occupy a 4:3 canvas. A Fold's inner display can be
		// narrower than that: fit the complete canvas instead of clipping its sides.
		m_camera.orthographicSize = m_designSize * Mathf.Max(
			m_camera.pixelHeight / safe.height, (4f / 3f) * m_camera.pixelHeight / safe.width);
		Vector2 offset = safe.center - m_camera.pixelRect.center;
		float unitsPerPixel = 2f * m_camera.orthographicSize / m_camera.pixelHeight;
		transform.position = m_designPosition - unitsPerPixel *
			(transform.right * offset.x + transform.up * offset.y);
	}

	private void Start()
	{
		if (SystemInfo.supportsImageEffects)
		{
			base.gameObject.AddComponent<ScreenCapture>();
		}
		if (Camera.allCameras.Length <= 1)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("MainCamera");
		if (array == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].GetComponent<Camera>().targetTexture == null)
			{
				num++;
			}
		}
		if (num > 1)
		{
			base.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
		}
		if (array.Length > 1)
		{
			base.gameObject.tag = "Untagged";
		}
	}
}
