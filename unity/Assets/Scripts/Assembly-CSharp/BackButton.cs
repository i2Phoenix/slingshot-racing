using UnityEngine;

public class BackButton : MonoBehaviour
{
	public static bool Disabled;

	private UIButton m_button;

	private UIButton3D m_button3D;

	private bool m_escapeDown;

	private bool m_isQuitting;

	public bool m_quitApp;

	public bool m_gamepadExclusive;

	private void Start()
	{
		m_button = GetComponent<UIButton>();
		m_button3D = GetComponent<UIButton3D>();
		base.enabled = m_quitApp || m_button != null || m_button3D != null;
	}

	private void Update()
	{
		if ((m_gamepadExclusive && UIGamepadManager.IsGamepadActive) || UIManager.instance == null || !UIManager.instance.enabled || UIManager.instance.blockInput || Disabled)
		{
			return;
		}
		bool keyDown = Input.GetKeyDown(KeyCode.Escape);
		bool flag = m_escapeDown && !Input.GetKey(KeyCode.Escape);
		if (keyDown || flag)
		{
			POINTER_INFO ptr = new POINTER_INFO
			{
				evt = (keyDown ? POINTER_INFO.INPUT_EVENT.PRESS : POINTER_INFO.INPUT_EVENT.TAP)
			};
			if (m_button != null)
			{
				m_button.OnInput(ptr);
			}
			if (m_button3D != null)
			{
				m_button3D.OnInput(ptr);
			}
			m_escapeDown = keyDown;
		}
		if (m_quitApp && keyDown)
		{
			Time.timeScale = 0f;
			base.enabled = false;
			ShowQuitDialog();
		}
	}

	private void OnApplicationPause(bool _pause)
	{
		if (_pause && m_isQuitting)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			androidJavaObject.Call("finish");
		}
	}

	private void ShowQuitDialog()
	{
		if (!Platform.ShowDialog(Language.Get("ID_QuitGame_Title"), Language.Get("ID_QuitGame_Desc"), Language.Get("ID_CancelLC"), Language.Get("ID_OK"), OnAlertClosed))
		{
			OnAlertClosed(true);
		}
	}

	private void OnAlertClosed(bool _confirmed)
	{
		float timeScale = 1f;
		if (_confirmed)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			m_isQuitting |= androidJavaObject.Call<bool>("moveTaskToBack", new object[1] { true });
			timeScale = 0f;
		}
		Time.timeScale = timeScale;
		base.enabled = true;
	}
}
