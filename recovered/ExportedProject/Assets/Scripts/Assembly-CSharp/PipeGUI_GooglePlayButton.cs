using System.Collections;
using UnityEngine;

public class PipeGUI_GooglePlayButton : MonoBehaviour
{
	protected SocialManager m_socialManager;

	protected IUIObject m_uiObject;

	protected virtual void Start()
	{
		m_socialManager = GameObjectUtils.FindComponentWithTag<SocialManager>("RaceControl");
		if (m_socialManager == null || !m_socialManager.IsGoogleGameServicesSupported)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		m_uiObject = GetComponent(typeof(IUIObject)) as IUIObject;
		StartCoroutine(MonitorSignIn());
	}

	private IEnumerator MonitorSignIn()
	{
		bool lastAuthenticated = m_socialManager.IsPlayerAuthenticated;
		OnAuthenticationChange(lastAuthenticated);
		while (true)
		{
			bool authenticated = m_socialManager.IsPlayerAuthenticated;
			if (authenticated != lastAuthenticated)
			{
				OnAuthenticationChange(authenticated);
				lastAuthenticated = authenticated;
			}
			yield return YieldCache.WaitForHalfSecond;
		}
	}

	protected virtual void OnAuthenticationChange(bool _authenticated)
	{
	}
}
