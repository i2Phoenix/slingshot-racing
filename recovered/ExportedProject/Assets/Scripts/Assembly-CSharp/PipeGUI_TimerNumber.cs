using System.Collections;
using UnityEngine;

public class PipeGUI_TimerNumber : MonoBehaviour
{
	private int m_lastDigit = -1;

	public SpriteText m_text;

	public MeshRenderer m_anim;

	public MeshRenderer m_static;

	private void Start()
	{
		if (m_anim != null)
		{
			m_anim.enabled = false;
		}
		if (m_static != null)
		{
			m_static.enabled = false;
		}
	}

	public void Set(int _digit, bool _immediate)
	{
		if (_digit != m_lastDigit)
		{
			if (_immediate || m_anim == null)
			{
				SetImmediate(_digit);
			}
			else
			{
				StartCoroutine(SetAndAnimate(_digit));
			}
		}
	}

	public void OnStop()
	{
		if (m_anim != null)
		{
			m_anim.enabled = false;
		}
		if (m_static != null)
		{
			m_static.enabled = false;
		}
	}

	public void OnStart()
	{
		if (m_anim != null)
		{
			m_anim.enabled = false;
		}
		if (m_static != null)
		{
			m_static.enabled = true;
		}
	}

	public void Hide(bool _hide)
	{
		if (_hide)
		{
			SetImmediate(0);
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				meshRenderer.enabled = false;
			}
			return;
		}
		MeshRenderer[] componentsInChildren2 = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer2 in componentsInChildren2)
		{
			meshRenderer2.enabled = true;
		}
		if (m_anim != null)
		{
			m_anim.enabled = false;
		}
		if (m_static != null)
		{
			m_static.enabled = false;
		}
	}

	private void SetImmediate(int _digit)
	{
		StopAllCoroutines();
		m_lastDigit = _digit;
		if (m_anim != null)
		{
			m_anim.enabled = false;
		}
		m_text.Text = _digit.ToString();
	}

	private IEnumerator SetAndAnimate(int _digit)
	{
		if (m_anim != null)
		{
			m_anim.enabled = true;
		}
		m_lastDigit = _digit;
		yield return YieldCache.WaitForSeconds(0.25f);
		if (m_anim != null)
		{
			m_anim.enabled = false;
		}
		m_text.Text = _digit.ToString();
	}
}
