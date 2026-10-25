using System;
using UnityEngine;

[AddComponentMenu("Camera/Debug Camera")]
public class DebugCamera : MainCamera
{
	public float m_normalSpeed = 5f;

	public float m_fastSpeed = 10f;

	public float m_mouseSpeed = 0.01f;

	private Vector3 m_prevMousePosition;

	private Camera m_camera;

	private GameObject m_prevCameraGO;

	private bool m_enabled;

	private void Start()
	{
		m_camera = GetComponent<Camera>();
		m_camera.enabled = false;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftShift))
		{
			if (m_enabled)
			{
				m_camera.enabled = false;
				m_prevCameraGO.active = true;
				m_prevCameraGO = null;
				m_enabled = false;
				Refresh();
			}
			else
			{
				m_prevCameraGO = Camera.main.gameObject;
				m_camera.enabled = true;
				m_camera.backgroundColor = m_prevCameraGO.GetComponent<Camera>().backgroundColor;
				base.transform.position = m_prevCameraGO.transform.position;
				base.transform.rotation = m_prevCameraGO.transform.rotation;
				m_prevCameraGO.active = false;
				m_enabled = true;
				m_prevMousePosition = Input.mousePosition;
				Refresh();
			}
		}
		if (m_enabled)
		{
			float num = ((!Input.GetKey(KeyCode.LeftShift)) ? m_normalSpeed : m_fastSpeed);
			if (Time.timeScale > float.Epsilon)
			{
				num /= Time.timeScale;
			}
			if (Input.GetKey(KeyCode.W))
			{
				base.transform.position += base.transform.forward * num * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.S))
			{
				base.transform.position -= base.transform.forward * num * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.D))
			{
				base.transform.position += base.transform.right * num * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.A))
			{
				base.transform.position -= base.transform.right * num * Time.deltaTime;
			}
			Vector3 mousePosition = Input.mousePosition;
			if (Input.GetMouseButton(1))
			{
				Vector3 vector = (mousePosition - m_prevMousePosition) * m_mouseSpeed;
				base.transform.RotateAround(Vector3.up, vector.x);
				base.transform.RotateAround(base.transform.right, 0f - vector.y);
			}
			m_prevMousePosition = mousePosition;
			if (Input.GetKeyDown(KeyCode.O) && Input.GetKey(KeyCode.LeftShift))
			{
				string text = Application.persistentDataPath + string.Format("/capture-{0:yyyy-MM-dd_hh-mm-ss-tt}.png", DateTime.Now);
				UnityEngine.ScreenCapture.CaptureScreenshot(text);
				Debug.Log(text);
			}
		}
	}
}
