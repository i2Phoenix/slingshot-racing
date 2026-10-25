using System;
using System.Collections;
using UnityEngine;

public class MoviePlayer : MonoBehaviour
{
	public string[] m_movies;

	public GameObject m_raceControlPrefab;

	private IEnumerator Start()
	{
		if (m_movies != null && Platform.IsMoviePlayerAvailable)
		{
			for (int index = 0; index < m_movies.Length; index++)
			{
				string name = m_movies[index];
				yield return null;
				yield return null;
				Handheld.PlayFullScreenMovie(name, Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
				if (index < m_movies.Length - 1)
				{
					yield return YieldCache.WaitForOneSecond;
				}
			}
			GC.Collect();
		}
		yield return null;
		GameObject raceControl = GameObject.FindGameObjectWithTag("RaceControl");
		if (raceControl == null)
		{
			raceControl = UnityEngine.Object.Instantiate(m_raceControlPrefab) as GameObject;
		}
		GameManager gameManager = raceControl.GetComponent<GameManager>();
		gameManager.BootToTitleScreen();
	}
}
