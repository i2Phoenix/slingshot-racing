using UnityEngine;

[RequireComponent(typeof(SpriteText))]
public class PositionPopup : MonoBehaviour
{
	private GameObject m_gameObject;

	private SpriteText m_spriteText;

	private Transform m_transform;

	private Camera m_camera;

	private Camera m_customCamera;

	private Transform m_cameraTransform;

	private Vehicle m_target;

	private Vector3 m_offsetVec;

	private Vector3 m_camForward;

	private Vector3 m_position;

	private Vector3 m_velocity;

	private float m_ttl;

	private float m_scale;

	private string m_pendingText;

	private float m_fadeRateScale;

	public float m_fadeInRate = 4f;

	public float m_fadeOutRate = 4f;

	public float m_duration = 1f;

	public float m_offset = 4f;

	public bool m_trackTarget = true;

	public float m_drag = 0.25f;

	public float m_velocityScale = 0.8f;

	public float m_upVelocityScale;

	public float m_depth = 2f;

	public Vehicle Target
	{
		set
		{
			m_target = value;
		}
	}

	public Color Colour
	{
		set
		{
			m_spriteText.Color = value;
		}
	}

	public Camera CustomCamera
	{
		set
		{
			m_customCamera = value;
		}
	}

	private void Awake()
	{
		m_gameObject = base.gameObject;
		m_transform = base.transform;
		m_spriteText = GetComponent<SpriteText>();
		m_gameObject.active = false;
	}

	private void LateUpdate()
	{
		float smoothDeltaTime = Time.smoothDeltaTime;
		if (m_pendingText != null || m_ttl < 0f)
		{
			m_scale -= m_fadeRateScale * m_fadeOutRate * smoothDeltaTime;
			if (m_scale <= 0f)
			{
				m_scale = 0f;
				if (m_pendingText == null)
				{
					m_gameObject.active = false;
					return;
				}
				m_spriteText.Text = m_pendingText;
				m_pendingText = null;
			}
		}
		else if (m_scale < 1f)
		{
			m_scale += m_fadeRateScale * m_fadeInRate * smoothDeltaTime;
			if (m_scale > 1f)
			{
				m_scale = 1f;
				m_fadeRateScale = 1f;
			}
		}
		else
		{
			m_ttl -= smoothDeltaTime;
		}
		if (m_trackTarget)
		{
			if (m_target != null)
			{
				m_position = m_target.Visuals.transform.position;
			}
		}
		else
		{
			m_position += m_velocity * smoothDeltaTime;
			m_velocity *= Mathf.Pow(m_drag, smoothDeltaTime);
		}
		if ((bool)m_cameraTransform)
		{
			float num = m_depth / Mathf.Max(m_scale, 0.001f);
			Vector3 position = m_cameraTransform.position;
			Vector3 vector = m_position - position;
			Vector3 position2 = position + m_offsetVec * num + vector * (num / Vector3.Dot(vector, m_camForward));
			m_transform.position = position2;
		}
	}

	public void Popup(string _text)
	{
		m_gameObject.active = true;
		if (m_camera == null)
		{
			m_camera = ((!(m_customCamera != null)) ? MainCamera.Get : m_customCamera);
			m_cameraTransform = m_camera.transform;
			m_camForward = m_cameraTransform.forward;
			Quaternion rotation = m_cameraTransform.rotation;
			m_offsetVec = rotation * Vector3.up * m_offset;
			m_transform.rotation = rotation;
			m_transform.position = m_cameraTransform.position + m_camForward * m_depth;
			m_spriteText.renderCamera = m_camera;
			m_spriteText.UpdateCamera();
		}
		if (m_scale > 0f && m_spriteText.Text != _text)
		{
			m_fadeRateScale = 2f;
			m_pendingText = _text;
		}
		else
		{
			m_fadeRateScale = 1f;
			m_spriteText.Text = _text;
			m_pendingText = null;
		}
		if (!m_trackTarget)
		{
			Vector3 up = m_cameraTransform.up;
			Vector3 velocity = m_target.Velocity;
			m_velocity = up * (m_upVelocityScale * velocity.magnitude) + velocity * m_velocityScale;
			m_position = m_target.Visuals.transform.position;
		}
		m_ttl = m_duration;
	}
}
