using System.Collections;
using UnityEngine;

public class UIGamepadHideVisuals : MonoBehaviour
{
	private Renderer[] m_renderers;

	public bool m_continuous;

	public int m_gamepadLayer;

	public bool m_includeChildren;

	private IEnumerator Start()
	{
		if (m_includeChildren)
		{
			yield return null;
			m_renderers = base.gameObject.GetComponentsInChildren<Renderer>(true);
		}
		else
		{
			m_renderers = new Renderer[1];
			m_renderers[0] = base.renderer;
		}
		if (UIGamepadManager.IsGamepadActive && m_gamepadLayer == 0)
		{
			EnableRenderers(false);
		}
		m_continuous = m_gamepadLayer > 0 || m_continuous;
		base.enabled = m_continuous;
	}

	private void LateUpdate()
	{
		if (m_continuous && UIGamepadManager.IsGamepadActive)
		{
			if (m_gamepadLayer == 0)
			{
				EnableRenderers(false);
			}
			else
			{
				EnableRenderers(UIGamepadManager.ActiveLayer < m_gamepadLayer);
			}
		}
	}

	private void EnableRenderers(bool _enable)
	{
		for (int i = 0; i < m_renderers.Length; i++)
		{
			m_renderers[i].enabled = _enable;
		}
	}
}
