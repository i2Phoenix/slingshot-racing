using UnityEngine;

public class OpenURLButton : MonoBehaviour
{
	public string m_url;

	public void OpenURL()
	{
		Application.OpenURL(m_url);
	}
}
