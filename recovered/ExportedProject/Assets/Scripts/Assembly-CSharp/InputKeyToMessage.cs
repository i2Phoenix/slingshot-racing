using System;
using UnityEngine;

[AddComponentMenu("Debug/Input Key To Message")]
public class InputKeyToMessage : MonoBehaviour
{
	[Serializable]
	public class InputMessagePair
	{
		public KeyCode m_key;

		public KeyCode m_modifier;

		public string m_messageName;
	}

	public InputMessagePair[] m_messages;

	public GameObject m_targetObject;

	public string m_targetObjectTag;

	private void Start()
	{
		if (m_targetObjectTag.Length > 0)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag(m_targetObjectTag);
			if (gameObject != null)
			{
				m_targetObject = gameObject;
			}
		}
	}

	private void Update()
	{
		if (m_messages == null)
		{
			return;
		}
		GameObject gameObject = ((!(m_targetObject != null)) ? base.gameObject : m_targetObject);
		InputMessagePair[] messages = m_messages;
		foreach (InputMessagePair inputMessagePair in messages)
		{
			if (Input.GetKeyDown(inputMessagePair.m_key) && (inputMessagePair.m_modifier == KeyCode.None || Input.GetKey(inputMessagePair.m_modifier)))
			{
				gameObject.BroadcastMessage(inputMessagePair.m_messageName);
			}
		}
	}
}
