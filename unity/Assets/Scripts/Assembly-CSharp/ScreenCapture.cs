using System;
using UnityEngine;

public class ScreenCapture : MonoBehaviour
{
	private static ScreenCapture s_screenCapture;

	private Material m_downsampleMaterial;

	private Action<Texture2D> m_callback;

	private int m_width;

	private int m_height;

	private bool m_includeAlpha;

	public Shader m_downsampleShader;

	private Material DownsampleMaterial
	{
		get
		{
			if (m_downsampleMaterial == null)
			{
				m_downsampleMaterial = new Material(m_downsampleShader);
				m_downsampleMaterial.hideFlags = HideFlags.DontSave;
			}
			return m_downsampleMaterial;
		}
	}

	public static ScreenCapture Get
	{
		get
		{
			return s_screenCapture;
		}
	}

	private void Awake()
	{
		if (s_screenCapture == null)
		{
			s_screenCapture = this;
		}
		if (m_downsampleShader == null)
		{
			m_downsampleShader = Shader.Find("Hidden/BlurEffectConeTap");
		}
		base.enabled = false;
	}

	private void OnDestroy()
	{
		if (s_screenCapture == this)
		{
			s_screenCapture = null;
		}
		if (m_downsampleMaterial != null)
		{
			UnityEngine.Object.Destroy(m_downsampleMaterial);
		}
	}

	public bool PerformCapture(int _width, int _height, bool _includeAlpha, Action<Texture2D> _callback)
	{
		if (!SystemInfo.supportsImageEffects)
		{
			return false;
		}
		if (m_downsampleShader == null || !DownsampleMaterial.shader.isSupported)
		{
			return false;
		}
		if (_width < 1)
		{
			return false;
		}
		if (_height < 1)
		{
			return false;
		}
		if (_callback == null)
		{
			return false;
		}
		if (m_callback != null)
		{
			return false;
		}
		m_callback = _callback;
		m_width = _width;
		m_height = _height;
		m_includeAlpha = _includeAlpha;
		base.enabled = true;
		return true;
	}

	private void OnRenderImage(RenderTexture _source, RenderTexture _destination)
	{
		if (m_callback != null)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(m_width, m_height);
			if (m_width <= _source.width / 2 && m_height <= _source.height / 2)
			{
				Graphics.BlitMultiTap(_source, temporary, DownsampleMaterial, new Vector2(-1f, -1f), new Vector2(-1f, 1f), new Vector2(1f, 1f), new Vector2(1f, -1f));
			}
			else
			{
				Graphics.Blit(_source, temporary);
			}
			TextureFormat format = ((!m_includeAlpha) ? TextureFormat.RGB24 : TextureFormat.ARGB32);
			Texture2D texture2D = new Texture2D(m_width, m_height, format, false);
			RenderTexture.active = temporary;
			texture2D.ReadPixels(new Rect(0f, 0f, m_width, m_height), 0, 0);
			texture2D.Apply();
			RenderTexture.active = _destination;
			RenderTexture.ReleaseTemporary(temporary);
			m_callback(texture2D);
			m_callback = null;
		}
		Graphics.Blit(_source, _destination);
		base.enabled = false;
	}
}
