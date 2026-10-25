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
		m_hitRect = new Rect(_region.xMin * (float)Screen.width, _region.yMin * (float)Screen.height, _region.xMax * (float)Screen.width, _region.yMax * (float)Screen.height);
		m_isDownThisFrame = false;
		m_isDownLastFrame = false;
	}

	public void SetCutout(Rect _region)
	{
		m_cutoutRect = new Rect(_region.xMin * (float)Screen.width, _region.yMin * (float)Screen.height, _region.xMax * (float)Screen.width, _region.yMax * (float)Screen.height);
		m_hasCutout = true;
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
		if (!m_isDownThisFrame && m_hitRect.Contains(position) && (!m_hasCutout || !m_cutoutRect.Contains(position)))
		{
			m_isDownThisFrame = true;
		}
	}

	public bool Touch2(Vector3 position)
	{
		if (m_hitRect.Contains(position) && (!m_hasCutout || !m_cutoutRect.Contains(position)))
		{
			m_isDownThisFrame = true;
			return true;
		}
		return false;
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
