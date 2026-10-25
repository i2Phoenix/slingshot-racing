using UnityEngine;

public class ActivateAtNight : MonoBehaviour
{
	private bool m_hasStarted;

	private void Start()
	{
		base.gameObject.active = SceneSettings.RenderSettings.m_night;
		m_hasStarted = true;
	}

	private void OnEnable()
	{
		if (m_hasStarted)
		{
			base.gameObject.active = SceneSettings.RenderSettings.m_night;
		}
	}
}
