using System;
using UnityEngine;

[RequireComponent(typeof(Animation))]
[AddComponentMenu("GUI/Race Identifier")]
public class RaceIdentifier : MonoBehaviour
{
	private const string c_fadeIn = "AnimatedArrow";

	private const string c_fadeInQuick = "AnimatedArrowQuick";

	private const string c_fadeOut = "AnimatedArrowFadeOut";

	private const string c_idle = "AnimatedArrowIdle";

	private RaceManager m_raceManager;

	public void Init(RaceManager _raceManager, bool _quick)
	{
		if (_quick)
		{
			base.GetComponent<Animation>().Play("AnimatedArrowQuick");
		}
		else
		{
			AnimationClip clip = base.GetComponent<Animation>().GetClip("AnimatedArrow");
			base.GetComponent<Animation>().Play("AnimatedArrow");
			Invoke("Idle", clip.length);
		}
		m_raceManager = _raceManager;
		m_raceManager.OnRaceLaunchEvent += FadeOnEvent;
		m_raceManager.OnRaceReplayEvent += KillOnEvent;
		m_raceManager.OnRaceAbortedEvent += KillOnEvent;
	}

	public void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceLaunchEvent -= FadeOnEvent;
			m_raceManager.OnRaceReplayEvent -= KillOnEvent;
			m_raceManager.OnRaceAbortedEvent -= KillOnEvent;
		}
	}

	private void KillOnEvent(object _sender, EventArgs _args)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void FadeOnEvent(object _sender, EventArgs _args)
	{
		AnimationClip clip = base.GetComponent<Animation>().GetClip("AnimatedArrowFadeOut");
		base.GetComponent<Animation>().Play("AnimatedArrowFadeOut");
		CancelInvoke();
		Invoke("FadedOut", clip.length);
	}

	private void Idle()
	{
		base.GetComponent<Animation>().Play("AnimatedArrowIdle", AnimationPlayMode.Mix);
	}

	private void FadedOut()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
