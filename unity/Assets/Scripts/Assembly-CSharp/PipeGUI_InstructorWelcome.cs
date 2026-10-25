using System.Collections;
using UnityEngine;

public class PipeGUI_InstructorWelcome : MonoBehaviour
{
	private static bool s_openedThisPlaythrough;

	private bool m_isOpen;

	public PipeGUI_InstructorSpeech m_speech;

	public LocString[] m_messages;

	private void Start()
	{
		if (UIGamepadManager.IsGamepadActive)
		{
			m_messages[1].ID += AppSettings.LocSuffixGamepad;
		}
		if (!s_openedThisPlaythrough)
		{
			Invoke("CheckForOpen", 0.5f);
		}
	}

	private void CheckForOpen()
	{
		if (!s_openedThisPlaythrough && Settings.PlayedCount <= 1)
		{
			s_openedThisPlaythrough = true;
			OpenDialog();
		}
	}

	private void OpenDialog()
	{
		if (!m_isOpen)
		{
			m_isOpen = true;
			StartCoroutine(OpenDialogAndWait());
		}
	}

	private IEnumerator OpenDialogAndWait()
	{
		yield return StartCoroutine(m_speech.DisplayAndWait(m_messages, Language.Get("ID_Tap")));
		m_isOpen = false;
	}

	private IEnumerator OpenDialogAndWait(string _text)
	{
		yield return StartCoroutine(m_speech.DisplayAndWait(_text, Language.Get("ID_Tap")));
		m_isOpen = false;
	}

	public void OpenDialog(string _text)
	{
		if (!m_isOpen)
		{
			m_isOpen = true;
			StartCoroutine(OpenDialogAndWait(_text));
		}
	}
}
