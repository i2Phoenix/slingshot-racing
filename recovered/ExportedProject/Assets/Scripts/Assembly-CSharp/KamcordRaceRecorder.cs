using UnityEngine;

public class KamcordRaceRecorder : MonoBehaviour
{
	public UIBistateInteractivePanel m_panel;

	public GameObject m_button;

	public float m_additionalCaptureTime = 2f;

	private void Awake()
	{
		if (!Settings.KamcordEnabled && m_button != null)
		{
			Object.Destroy(m_button);
			m_button = null;
		}
	}

	public void BringIn()
	{
		m_panel.BringIn();
	}

	public void Dismiss()
	{
		m_panel.Dismiss();
	}
}
