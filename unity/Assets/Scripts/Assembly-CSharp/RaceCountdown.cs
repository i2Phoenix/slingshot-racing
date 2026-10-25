using System.Collections;
using UnityEngine;

[AddComponentMenu("Gameplay/Race Countdown")]
public class RaceCountdown : MonoBehaviour
{
	public UIBistateInteractivePanel m_panel;

	public PackedSprite m_trafficLights;

	public AudioClip m_countdownAudio;

	public AudioClip m_startRaceAudio;

	public bool m_test;

	private IEnumerator Start()
	{
		if (m_test)
		{
			yield return YieldCache.WaitForOneSecond;
			StartCoroutine(Countdown());
		}
	}

	public IEnumerator Countdown()
	{
		m_panel.Reveal();
		if (m_countdownAudio != null)
		{
			AudioSource.PlayClipAtPoint(m_countdownAudio, Vector3.zero);
		}
		m_trafficLights.PlayAnim(0);
		yield return YieldCache.WaitForTwoSeconds;
		if (m_countdownAudio != null)
		{
			AudioSource.PlayClipAtPoint(m_countdownAudio, Vector3.zero);
		}
		m_trafficLights.PlayAnim(1);
		yield return YieldCache.WaitForOneSecond;
		if (m_startRaceAudio != null)
		{
			AudioSource.PlayClipAtPoint(m_startRaceAudio, Vector3.zero);
		}
		m_trafficLights.PlayAnim(2);
		StartCoroutine(DelayedHide());
	}

	public void AbortCountdown()
	{
		m_trafficLights.PlayAnim(0);
		m_panel.Hide();
		StopAllCoroutines();
	}

	private IEnumerator DelayedHide()
	{
		yield return YieldCache.WaitForTwoSeconds;
		m_panel.Hide();
	}
}
