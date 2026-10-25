using UnityEngine;

public class TouchRegion
{
	private Rect m_hitRect;

	private Rect m_cutoutRect;

	private bool m_hasCutout;

	private bool m_isDownThisFrame;

	private bool m_isDownLastFrame;

	public TouchRegion(Rect _region)
	{
		m_hitRect = _region;
		m_isDownThisFrame = false;
		m_isDownLastFrame = false;
	}

	public void SetCutout(Rect _region)
	{
		m_cutoutRect = _region;
		m_hasCutout = true;
	}

	public void SetRegion(Rect region)
	{
		// Controllers retain this object for the entire race. Resizing must only
		// change its geometry, retaining the press/release state as well.
		m_hitRect = region;
	}

	public void ClearCutout()
	{
		m_hasCutout = false;
	}

	public void PreUpdate()
	{
		m_isDownLastFrame = m_isDownThisFrame;
		m_isDownThisFrame = false;
	}

	public void Touch(Vector3 position)
	{
		position = Normalize(position);
		if (!m_isDownThisFrame && m_hitRect.Contains(position) && (!m_hasCutout || !m_cutoutRect.Contains(position)))
		{
			m_isDownThisFrame = true;
		}
	}

	public bool Touch2(Vector3 position)
	{
		position = Normalize(position);
		if (m_hitRect.Contains(position) && (!m_hasCutout || !m_cutoutRect.Contains(position)))
		{
			m_isDownThisFrame = true;
			return true;
		}
		return false;
	}

	private static Vector3 Normalize(Vector3 position)
	{
		// Regions are stored as normalized rectangles, including their width/height.
		// Convert against the current display instead of caching the launch resolution.
		return new Vector3(position.x / Screen.width, position.y / Screen.height, 0f);
	}

	public bool GetTouchDown()
	{
		return m_isDownThisFrame && !m_isDownLastFrame;
	}

	public bool GetTouchUp()
	{
		return !m_isDownThisFrame && m_isDownLastFrame;
	}
}
