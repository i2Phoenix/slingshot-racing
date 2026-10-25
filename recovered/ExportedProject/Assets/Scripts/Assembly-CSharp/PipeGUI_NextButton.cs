using UnityEngine;

public class PipeGUI_NextButton : MonoBehaviour
{
	private bool m_isActive;

	public GameObject m_objects;

	public UIButton m_button;

	private void Start()
	{
		m_isActive = true;
	}

	private void Update()
	{
		if (m_objects != null && m_isActive != m_button.controlIsEnabled)
		{
			m_isActive = m_button.controlIsEnabled;
			m_objects.SetActiveRecursively(m_isActive);
		}
	}
}
