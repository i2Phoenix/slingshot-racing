using System.Collections;
using UnityEngine;

public class PipeGUI_InstructorSpeech : MonoBehaviour
{
	public enum State
	{
		Hide = 0,
		HideImmediate = 1,
		Show = 2
	}

	public delegate void StateChange(State _state);

	private bool m_active;

	private bool m_allowClickToClose;

	public SpriteText m_text;

	public SpriteText m_closePrompt;

	public UIButton m_optionalButton;

	public PipeGUI_Instructor m_instructor;

	public GameObject m_objectToScale;

	public bool m_dontActivateRecursively;

	public bool AllowClickToClose
	{
		set
		{
			m_allowClickToClose = value;
		}
	}

	public event StateChange OnStateChange;

	private void Start()
	{
		base.gameObject.SetActiveRecursively(false);
	}

	public IEnumerator DisplayAndWaitBreakLines(string _message, string _closePrompt, bool _showOptionalButton = false)
	{
		Enable(_showOptionalButton);
		string[] messages = _message.Split('\n');
		string[] array = messages;
		foreach (string message in array)
		{
			yield return StartCoroutine(DisplayAndWaitImpl(message, _closePrompt));
		}
		Disable();
	}

	public IEnumerator DisplayAndWaitBreakLines(LocString[] _messages, string _closePrompt, bool _showOptionalButton = false)
	{
		Enable(_showOptionalButton);
		foreach (LocString message in _messages)
		{
			yield return StartCoroutine(DisplayAndWaitBreakLines(message.Text, _closePrompt, _showOptionalButton));
		}
		Disable();
	}

	public IEnumerator DisplayAndWait(LocString[] _messages, string _closePrompt, bool _showOptionalButton = false)
	{
		Enable(_showOptionalButton);
		foreach (LocString message in _messages)
		{
			yield return StartCoroutine(DisplayAndWaitImpl(message.Text, _closePrompt));
		}
		Disable();
	}

	public IEnumerator DisplayAndWait(string _message, string _closePrompt, bool _showOptionalButton = false)
	{
		Enable(_showOptionalButton);
		yield return StartCoroutine(DisplayAndWaitImpl(_message, _closePrompt));
		Disable();
	}

	private void Enable(bool _showOptionalButton)
	{
		if (m_instructor != null)
		{
			m_instructor.ShowTalking();
		}
		if (m_dontActivateRecursively)
		{
			base.gameObject.active = true;
		}
		else
		{
			base.gameObject.SetActiveRecursively(true);
		}
		if (m_optionalButton != null)
		{
			m_optionalButton.gameObject.SetActiveRecursively(_showOptionalButton);
		}
	}

	private void Disable()
	{
		if (m_instructor != null)
		{
			m_instructor.Hide();
		}
		base.gameObject.SetActiveRecursively(false);
		GameObject gameObject = ((!(m_objectToScale != null)) ? base.gameObject : m_objectToScale);
		gameObject.transform.localScale = Vector3.one;
	}

	private IEnumerator DisplayAndWaitImpl(string _message, string _closePrompt)
	{
		if (OnStateChange != null)
		{
			OnStateChange(State.Show);
		}
		m_active = true;
		GameObject scaledObject = ((!(m_objectToScale != null)) ? base.gameObject : m_objectToScale);
		AnimateScale.Do(scaledObject, EZAnimation.ANIM_MODE.FromTo, Vector3.zero, Vector3.one, EZAnimation.cubicOut, 0.2f, 0f, null, null);
		if (m_text != null)
		{
			m_text.Text = _message;
		}
		if (m_instructor != null)
		{
			m_instructor.PlayTalkAnim();
		}
		m_allowClickToClose = _closePrompt.Length > 0;
		Color promptColour = Color.black;
		if (m_closePrompt != null)
		{
			m_closePrompt.Text = _closePrompt;
			promptColour = m_closePrompt.Color;
		}
		while (m_active)
		{
			promptColour.a = 0.2f + Mathf.PingPong(Time.timeSinceLevelLoad * 0.7f, 0.8f);
			if (m_allowClickToClose && m_closePrompt != null)
			{
				m_closePrompt.SetColor(promptColour);
			}
			if (m_optionalButton != null && m_optionalButton.spriteText != null)
			{
				m_optionalButton.spriteText.SetColor(promptColour);
			}
			yield return null;
		}
		if (OnStateChange != null)
		{
			OnStateChange(State.Hide);
		}
		AnimateScale.Do(scaledObject, EZAnimation.ANIM_MODE.To, Vector3.zero, EZAnimation.cubicOut, 0.2f, 0f, null, null);
		yield return YieldCache.WaitForSeconds(0.2f);
	}

	public void HideImmediate()
	{
		StopAllCoroutines();
		m_active = false;
		base.gameObject.SetActiveRecursively(false);
		if (m_instructor != null)
		{
			m_instructor.HideImmediate();
		}
		if (OnStateChange != null)
		{
			OnStateChange(State.HideImmediate);
		}
	}

	public void Hide()
	{
		m_active = false;
	}

	public void HideSmoothly()
	{
		Hide();
	}

	private void OnClicked()
	{
		if (Time.deltaTime > 0f && m_allowClickToClose)
		{
			m_active = false;
		}
	}
}
