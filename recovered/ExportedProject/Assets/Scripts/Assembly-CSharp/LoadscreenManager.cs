using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[AddComponentMenu("GUI/Loadscreen Manager")]
[RequireComponent(typeof(MeshFilter))]
public class LoadscreenManager : MonoBehaviour
{
	private float m_alpha;

	public Color m_fadeColour;

	public Material m_loadingScreen;

	public Material m_solidColour;

	public MeshRenderer m_backgroundRenderer;

	public MeshRenderer m_foregroundRenderer;

	public MeshRenderer[] m_otherRenderers;

	public GameObject m_inputDisabler;

	public BusyIcon m_busyIcon;

	public void Start()
	{
		base.gameObject.SetActiveRecursively(false);
	}

	public IEnumerator ShowLoadingScreen(bool _show, float _fadeTime, bool _fadeToColour, float _delay)
	{
		m_inputDisabler.active = true;
		BackButton.Disabled = true;
		m_backgroundRenderer.material.color = m_fadeColour;
		SetAlpha(m_alpha);
		yield return new WaitForSeconds(_delay);
		base.gameObject.SetActiveRecursively(true);
		m_backgroundRenderer.gameObject.active = _fadeToColour;
		m_foregroundRenderer.gameObject.active = !_fadeToColour;
		if (_show)
		{
			yield return null;
		}
		else
		{
			yield return Resources.UnloadUnusedAssets();
			yield return null;
			GC.Collect();
		}
		float endVolume = ((!_show) ? Settings.SoundVolume : 0f);
		if (_fadeTime > 0f)
		{
			float startVolume = AudioListener.volume;
			float fadeTimeRecip = 1f / _fadeTime;
			float audioFade = 0f;
			if (_show)
			{
				while (m_alpha < 1f)
				{
					float fadeAmount = Time.deltaTime * fadeTimeRecip;
					m_alpha += fadeAmount;
					SetAlpha(m_alpha);
					audioFade = Mathf.Clamp01(audioFade + fadeAmount);
					AudioListener.volume = Mathf.SmoothStep(startVolume, endVolume, audioFade);
					yield return null;
				}
				if (m_busyIcon != null)
				{
					m_busyIcon.HideImmediate();
				}
			}
			else
			{
				while (m_alpha > 0f)
				{
					float fadeAmount2 = Time.deltaTime * fadeTimeRecip;
					m_alpha -= fadeAmount2;
					SetAlpha(m_alpha);
					audioFade = Mathf.Clamp01(audioFade + fadeAmount2);
					AudioListener.volume = Mathf.SmoothStep(startVolume, endVolume, audioFade);
					yield return null;
				}
			}
		}
		else
		{
			if (_show && m_busyIcon != null)
			{
				m_busyIcon.HideImmediate();
			}
			m_alpha = ((!_show) ? 0f : 1f);
			SetAlpha(m_alpha);
		}
		AudioListener.volume = endVolume;
		m_alpha = Mathf.Clamp(m_alpha, 0f, 1f);
		SetAlpha(m_alpha);
		base.gameObject.SetActiveRecursively(_show);
		m_foregroundRenderer.gameObject.active = _show && !_fadeToColour;
		m_backgroundRenderer.gameObject.active = _show && _fadeToColour;
		m_inputDisabler.active = _show;
		BackButton.Disabled = _show;
		if (_show)
		{
			PlatformSettings.ApplySettings();
		}
	}

	public IEnumerator ShowSystemBusy(bool _show, float _fadeTime)
	{
		m_inputDisabler.active = true;
		BackButton.Disabled = true;
		m_backgroundRenderer.material.color = m_fadeColour;
		SetAlpha(m_alpha);
		base.gameObject.SetActiveRecursively(true);
		m_foregroundRenderer.gameObject.active = false;
		yield return null;
		float targetAlpha = ((!_show) ? 0f : 0.85f);
		if (_fadeTime > 0f)
		{
			float fadeTimeRecip = 1f / _fadeTime;
			if (_show)
			{
				while (m_alpha < targetAlpha)
				{
					float fadeAmount = Time.deltaTime * fadeTimeRecip;
					m_alpha += fadeAmount;
					SetAlpha(m_alpha);
					yield return null;
				}
			}
			else
			{
				while (m_alpha > targetAlpha)
				{
					float fadeAmount2 = Time.deltaTime * fadeTimeRecip;
					m_alpha -= fadeAmount2;
					SetAlpha(m_alpha);
					yield return null;
				}
			}
		}
		else
		{
			m_alpha = targetAlpha;
			SetAlpha(m_alpha);
		}
		m_alpha = Mathf.Clamp(m_alpha, 0f, targetAlpha);
		SetAlpha(m_alpha);
		base.gameObject.SetActiveRecursively(_show);
		m_foregroundRenderer.gameObject.active = false;
		m_inputDisabler.active = _show;
		BackButton.Disabled = _show;
	}

	private void SetAlpha(float _alpha)
	{
		SetAlpha(m_backgroundRenderer, _alpha);
		SetAlpha(m_foregroundRenderer, _alpha);
		if (m_otherRenderers != null)
		{
			MeshRenderer[] otherRenderers = m_otherRenderers;
			foreach (MeshRenderer meshRenderer in otherRenderers)
			{
				SetAlpha(meshRenderer, _alpha);
			}
		}
	}

	private static void SetAlpha(MeshRenderer _renderer, float alpha)
	{
		Color color = _renderer.material.color;
		color.a = alpha;
		_renderer.material.color = color;
	}
}
