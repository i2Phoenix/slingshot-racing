public class PipeGUI_ShowLeaderboardsButton : PipeGUI_GooglePlayButton
{
	private bool m_authenticated;

	public PackedSprite m_sprite;

	private void OnSelected()
	{
		if (m_socialManager != null)
		{
			if (m_authenticated)
			{
				m_socialManager.ShowLeaderboards();
			}
			else
			{
				m_socialManager.AuthenticatePlayer();
			}
		}
	}

	protected override void OnAuthenticationChange(bool _authenticated)
	{
		m_authenticated = _authenticated;
		if (m_sprite != null)
		{
			m_sprite.PlayAnim(m_authenticated ? 1 : 0);
			m_sprite.defaultAnim = (m_authenticated ? 1 : 0);
		}
	}
}
