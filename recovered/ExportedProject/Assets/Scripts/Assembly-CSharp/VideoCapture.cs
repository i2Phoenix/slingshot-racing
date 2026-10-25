using System;
using System.IO;
using UnityEngine;

[AddComponentMenu("Graphics/Video Capture")]
public class VideoCapture : MonoBehaviour
{
	private GUIContent m_copyrightNotice;

	private GUIContent m_recordInfo;

	private int m_frameIndex;

	public string m_name;

	public int m_framerate;

	private void Start()
	{
		string text = string.Format("{0}/{1}/", Application.persistentDataPath, m_name);
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		Debug.Log("Starting capture: " + text);
		m_copyrightNotice = new GUIContent(string.Format("Copyright {0} Entropy Flux LTD", DateTime.Now.Year));
		m_recordInfo = new GUIContent("Press backspace to toggle recording");
	}

	private void OnDestroy()
	{
		Time.captureFramerate = 0;
	}

	private void OnGUI()
	{
		GUIStyle gUIStyle = GUI.skin.FindStyle("label");
		if (Time.captureFramerate == 0)
		{
			GUILayout.BeginVertical();
			GUI.color = Color.red;
			GUILayout.Label(m_recordInfo, gUIStyle);
			if (GUILayout.Button("Record"))
			{
				Time.timeScale = 1f;
				Time.captureFramerate = m_framerate;
			}
			if (Time.timeScale == 0f)
			{
				if (GUILayout.Button("Play"))
				{
					Time.timeScale = 1f;
				}
			}
			else if (GUILayout.Button("Pause"))
			{
				Time.timeScale = 0f;
			}
			GUILayout.EndVertical();
		}
		Vector2 vector = gUIStyle.CalcSize(m_copyrightNotice);
		Vector2 vector2 = new Vector2((float)Screen.width - (vector.x + 5f), (float)Screen.height - (vector.y + 5f));
		GUI.color = Color.black;
		GUI.Label(new Rect(vector2.x + 1f, vector2.y + 1f, vector.x, vector.y), m_copyrightNotice, gUIStyle);
		GUI.color = Color.red;
		GUI.Label(new Rect(vector2.x, vector2.y, vector.x, vector.y), m_copyrightNotice, gUIStyle);
	}

	private void LateUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			if (Time.captureFramerate == 0)
			{
				Time.timeScale = 1f;
				Time.captureFramerate = m_framerate;
			}
			else
			{
				Time.captureFramerate = 0;
			}
		}
		else if (Time.captureFramerate > 0)
		{
			string filename = string.Format("{0}/{1}/Frame{2:000000}.png", Application.persistentDataPath, m_name, m_frameIndex);
			Application.CaptureScreenshot(filename);
			m_frameIndex++;
		}
	}
}
