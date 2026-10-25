using UnityEngine;

[RequireComponent(typeof(SpriteText))]
[AddComponentMenu("Debug/In Game FPS Display")]
public class InGameFPSDisplay : MonoBehaviour
{
	public float updateInterval = 1f;

	private SpriteText m_text;

	private float startTime;

	private int frames;

	private void Start()
	{
		m_text = GetComponent<SpriteText>();
		startTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		if (UIManager.Exists() && UIManager.instance.uiCameras.Length > 0)
		{
			Camera camera = UIManager.instance.uiCameras[0].camera;
			if (camera != m_text.RenderCamera)
			{
				m_text.RenderCamera = camera;
			}
		}
		frames++;
		if (Time.realtimeSinceStartup - startTime > updateInterval)
		{
			float num = Time.realtimeSinceStartup - startTime;
			float num2 = (float)frames / num;
			string text = string.Format("{0:F1}", num2);
			if (num2 < 30f)
			{
				m_text.color = Color.yellow;
			}
			else if (num2 < 10f)
			{
				m_text.color = Color.red;
			}
			else
			{
				m_text.color = Color.green;
			}
			m_text.Text = text;
			frames = 0;
			startTime = Time.realtimeSinceStartup;
		}
	}
}
