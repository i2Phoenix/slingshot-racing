using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PipeGUI_InstructorSpeech))]
public class PipeGUI_InstructorYesNo : MonoBehaviour
{
	public delegate void DialogDelegate(Object _dialog);

	private PipeGUI_InstructorSpeech m_speech;

	private DialogDelegate m_confirm;

	private DialogDelegate m_cancel;

	public PipeGUI_Instructor m_instructor;

	private void Awake()
	{
		m_speech = GetComponent<PipeGUI_InstructorSpeech>();
	}

	public IEnumerator DisplayAndWait(string _message, DialogDelegate _confirm, DialogDelegate _cancel)
	{
		base.gameObject.active = true;
		DisableControls();
		m_confirm = _confirm;
		m_cancel = _cancel;
		if (m_instructor != null)
		{
			m_instructor.SetTalkingStance();
			m_instructor.PlayTalkAnim();
		}
		yield return StartCoroutine(m_speech.DisplayAndWait(_message, string.Empty));
		if (m_instructor != null)
		{
			m_instructor.SetDefaultStance();
		}
		m_confirm = null;
		m_cancel = null;
		EnableControls();
	}

	private void OnConfirmSelected()
	{
		if (m_confirm != null)
		{
			m_confirm(this);
		}
		m_speech.Hide();
	}

	private void OnCancelSelected()
	{
		if (m_cancel != null)
		{
			m_cancel(this);
		}
		m_speech.Hide();
	}

	private void DisableControls()
	{
		UIManager instance = UIManager.instance;
		int num = 1 << LayerMask.NameToLayer("GUI");
		for (int i = 0; i < instance.uiCameras.Length; i++)
		{
			EZCameraSettings eZCameraSettings = instance.uiCameras[i];
			eZCameraSettings.mask = (int)eZCameraSettings.mask & ~num;
			instance.ReplaceCamera(i, eZCameraSettings.camera);
		}
	}

	private void EnableControls()
	{
		UIManager instance = UIManager.instance;
		int num = 1 << LayerMask.NameToLayer("GUI");
		for (int i = 0; i < instance.uiCameras.Length; i++)
		{
			EZCameraSettings eZCameraSettings = instance.uiCameras[i];
			eZCameraSettings.mask = (int)eZCameraSettings.mask | num;
			instance.ReplaceCamera(i, eZCameraSettings.camera);
		}
	}
}
