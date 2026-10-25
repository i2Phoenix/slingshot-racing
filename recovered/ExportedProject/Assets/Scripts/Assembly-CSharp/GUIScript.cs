using System;
using UnityEngine;

[ExecuteInEditMode]
public class GUIScript : MonoBehaviour
{
	public delegate void DoGUI();

	private GUIContent m_menuButtonText;

	public GUISkin guiSkin;

	public event DoGUI DoGUIEvent;

	private void Awake()
	{
		base.useGUILayout = false;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Menu))
		{
			Application.LoadLevel("Menu");
		}
	}

	private void OnDestroy()
	{
		DoGUIEvent = null;
		guiSkin = null;
	}

	private void OnGUI()
	{
		if (!(guiSkin == null))
		{
			GUI.skin = guiSkin;
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3((float)Screen.width / 1920f, (float)Screen.height / 1200f, 1f));
			if (DoGUIEvent != null)
			{
				DoGUIEvent();
			}
		}
	}

	public static GUIScript[] Get()
	{
		return UnityEngine.Object.FindObjectsOfType(typeof(GUIScript)) as GUIScript[];
	}

	public static void Register(DoGUI _func)
	{
		GUIScript[] array = Get();
		if (array != null)
		{
			foreach (GUIScript obj in array)
			{
				obj.DoGUIEvent = (DoGUI)Delegate.Combine(obj.DoGUIEvent, _func);
			}
		}
	}

	public static void Unregister(DoGUI _func)
	{
		GUIScript[] array = Get();
		if (array != null)
		{
			foreach (GUIScript obj in array)
			{
				obj.DoGUIEvent = (DoGUI)Delegate.Remove(obj.DoGUIEvent, _func);
			}
		}
	}
}
