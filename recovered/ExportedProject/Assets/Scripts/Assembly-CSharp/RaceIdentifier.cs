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
			base.animation.Play("AnimatedArrowQuick");
		}
		else
		{
			AnimationClip clip = base.animation.GetClip("AnimatedArrow");
			base.animation.Play("AnimatedArrow");
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
		AnimationClip clip = base.animation.GetClip("AnimatedArrowFadeOut");
		base.animation.Play("AnimatedArrowFadeOut");
		CancelInvoke();
		Invoke("FadedOut", clip.length);
	}

	private void Idle()
	{
		base.animation.Play("AnimatedArrowIdle", AnimationPlayMode.Mix);
	}

	private void FadedOut()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
