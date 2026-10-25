using UnityEngine;

[AddComponentMenu("Graphics/Obscured Renderer")]
[RequireComponent(typeof(Renderer))]
public class ObscuredRenderer : MonoBehaviour
{
	private Renderer m_renderer;

	private int m_obscuredZoneCount;

	private void Awake()
	{
		m_renderer = base.renderer;
		m_renderer.enabled = m_obscuredZoneCount > 0;
	}

	public void EnterObscuredZone()
	{
		m_renderer.enabled = ++m_obscuredZoneCount > 0;
	}

	public void ExitObscuredZone()
	{
		m_renderer.enabled = --m_obscuredZoneCount > 0;
	}
}
