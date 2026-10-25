using UnityEngine;

public class PipeGUI_ChallengeListButton : MonoBehaviour
{
	public LeaderboardInfo m_defaultLeaderboard;

	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnSelected()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (!(gameObject != null))
		{
			return;
		}
		SocialManager component = gameObject.GetComponent<SocialManager>();
		if (component != null)
		{
			if (component.AreChallengesSupported)
			{
				component.ShowChallenges();
			}
			else
			{
				component.ShowLeaderboard(m_defaultLeaderboard);
			}
		}
	}
}
