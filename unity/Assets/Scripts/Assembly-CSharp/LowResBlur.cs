using UnityEngine;

[AddComponentMenu("Graphics/Low Res Blur")]
public class LowResBlur : MonoBehaviour
{
	public enum Depth
	{
		None = 0,
		Depth16 = 1,
		Depth24 = 2
	}

	public int m_iterations = 3;

	public float m_blurSpread = 0.6f;

	public Shader m_blurShader;

	private static Material m_blurMaterial;

	private RenderTexture m_renderTexture;

	private RenderTexture m_finalTexture;

	public Renderer m_presentRenderer;

	public Depth m_depth;

	public float m_resolutionScale = 0.25f;

	public bool m_downsampleBeforeBlur = true;

	public int m_downsampleMinWidth = 512;

	private Material BlurMaterial
	{
		get
		{
			if (m_blurMaterial == null)
			{
				m_blurMaterial = new Material(m_blurShader);
				m_blurMaterial.hideFlags = HideFlags.DontSave;
			}
			return m_blurMaterial;
		}
	}

	private bool ShouldDownsample
	{
		get
		{
			return m_downsampleBeforeBlur && Screen.width >= m_downsampleMinWidth;
		}
	}

	private void OnDisable()
	{
		if ((bool)m_blurMaterial)
		{
			Object.DestroyImmediate(m_blurMaterial);
		}
	}

	private void Awake()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			base.enabled = false;
			if (m_presentRenderer != null)
			{
				m_presentRenderer.gameObject.active = false;
			}
			return;
		}
		if (!m_blurShader || !BlurMaterial.shader.isSupported)
		{
			base.enabled = false;
			if (m_presentRenderer != null)
			{
				m_presentRenderer.gameObject.active = false;
			}
			return;
		}
		int num = Mathf.RoundToInt((float)Screen.width * m_resolutionScale);
		int num2 = Mathf.RoundToInt((float)Screen.height * m_resolutionScale);
		if (m_renderTexture == null)
		{
			int depth;
			switch (m_depth)
			{
			default:
				depth = 0;
				break;
			case Depth.Depth16:
				depth = 16;
				break;
			case Depth.Depth24:
				depth = 24;
				break;
			}
			m_renderTexture = new RenderTexture(num, num2, depth);
			m_renderTexture.name = "Low Res Render";
			base.GetComponent<Camera>().targetTexture = m_renderTexture;
		}
		if (m_finalTexture == null)
		{
			int num3 = (ShouldDownsample ? 1 : 0);
			int width = num >> num3;
			int height = num2 >> num3;
			m_finalTexture = new RenderTexture(width, height, 0);
			m_finalTexture.name = "Low Res Blur";
		}
		if (m_presentRenderer != null)
		{
			m_presentRenderer.gameObject.active = true;
			m_presentRenderer.material.mainTexture = m_finalTexture;
		}
	}

	private void OnDestroy()
	{
		if (m_renderTexture != null)
		{
			Object.Destroy(m_renderTexture);
			m_renderTexture = null;
		}
		if (m_finalTexture != null)
		{
			Object.Destroy(m_finalTexture);
			m_finalTexture = null;
		}
	}

	public void FourTapCone(RenderTexture _source, RenderTexture _dest, int _iteration)
	{
		float num = 0.5f + (float)_iteration * m_blurSpread;
		Graphics.BlitMultiTap(_source, _dest, BlurMaterial, new Vector2(0f - num, 0f - num), new Vector2(0f - num, num), new Vector2(num, num), new Vector2(num, 0f - num));
	}

	private void DownSample(RenderTexture _source, RenderTexture _dest)
	{
		float num = 1f;
		Graphics.BlitMultiTap(_source, _dest, BlurMaterial, new Vector2(0f - num, 0f - num), new Vector2(0f - num, num), new Vector2(num, num), new Vector2(num, 0f - num));
	}

	private void OnPostRender()
	{
		RenderTexture renderTexture;
		if (ShouldDownsample)
		{
			DownSample(m_renderTexture, m_finalTexture);
			renderTexture = m_finalTexture;
		}
		else
		{
			renderTexture = m_renderTexture;
		}
		RenderTexture temporary = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, 0);
		bool flag = true;
		int i;
		for (i = 0; i < m_iterations - 1; i++)
		{
			if (flag)
			{
				FourTapCone(renderTexture, temporary, i);
			}
			else
			{
				FourTapCone(temporary, renderTexture, i);
			}
			flag = !flag;
		}
		if (flag)
		{
			if (renderTexture == m_finalTexture)
			{
				FourTapCone(renderTexture, temporary, i);
				Graphics.Blit(temporary, m_finalTexture);
			}
			else
			{
				FourTapCone(renderTexture, m_finalTexture, i);
			}
		}
		else
		{
			FourTapCone(temporary, m_finalTexture, i);
		}
		RenderTexture.ReleaseTemporary(temporary);
	}
}
