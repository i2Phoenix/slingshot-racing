using UnityEngine;

public class ActivateForTrackDirection : MonoBehaviour
{
	private bool m_reverseTrack;

	public bool m_recursiveActivate = true;

	private void Start()
	{
		string text = ((!m_reverseTrack) ? "Forward" : "Reverse");
		string text2 = (m_reverseTrack ? "Forward" : "Reverse");
		Transform transform = base.transform.Find(text);
		if (transform != null)
		{
			if (m_recursiveActivate)
			{
				transform.gameObject.SetActiveRecursively(false);
				transform.gameObject.SetActiveRecursively(true);
			}
			else
			{
				transform.gameObject.active = true;
			}
		}
		Transform transform2 = base.transform.Find(text2);
		if (transform2 != null)
		{
			transform2.gameObject.SetActiveRecursively(false);
		}
	}

	public void OnReverseTrack()
	{
		m_reverseTrack = true;
	}
}
