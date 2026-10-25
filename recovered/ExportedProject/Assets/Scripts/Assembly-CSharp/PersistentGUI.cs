using UnityEngine;

public class PersistentGUI : MonoBehaviour
{
	public BusyIcon m_busyIcon;

	public LoadscreenManager m_loadscreen;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
	}
}
