public class PipeGUI_ShowAchievementsButton : PipeGUI_GooglePlayButton
{
	private void OnSelected()
	{
		if (m_socialManager != null)
		{
			m_socialManager.ShowAchievements();
		}
	}

	protected override void OnAuthenticationChange(bool _authenticated)
	{
		base.renderer.enabled = _authenticated;
		if (m_uiObject != null)
		{
			m_uiObject.controlIsEnabled = _authenticated;
		}
	}
}
