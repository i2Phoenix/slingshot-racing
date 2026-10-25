using System.Collections;
using UnityEngine;

public class Tickerboard : MonoBehaviour
{
	private Animation m_visualsAnim;

	private TransitionPlayer m_transitions;

	private bool m_allowStart;

	private AudioPlayer m_audioPlayer;

	public GameObject[] m_jetStreams;

	public float m_fastStartTime;

	public Material m_tickerboardMaterial;

	public Material m_jetstreamMaterial;

	private bool StandAlone
	{
		get
		{
			return base.transform.parent == null;
		}
	}

	private void Start()
	{
		m_visualsAnim = base.transform.FindChild("Tickerboard").animation;
		m_transitions = GetComponent<TransitionPlayer>();
		m_audioPlayer = base.gameObject.AddComponent<AudioPlayer>();
		m_visualsAnim.gameObject.SetActiveRecursively(false);
		ActivateJetStreams(false);
		if (StandAlone)
		{
			StartCoroutine(Countdown(true));
		}
		m_tickerboardMaterial.renderQueue = 4000;
		m_jetstreamMaterial.renderQueue = 4001;
	}

	public IEnumerator Countdown(bool _fast)
	{
		base.gameObject.SetActiveRecursively(true);
		ActivateJetStreams(false);
		m_allowStart = false;
		if (_fast)
		{
			base.animation["TickerboardEvents"].time = m_fastStartTime;
			m_visualsAnim["TickerboardAnim"].time = m_fastStartTime;
		}
		base.animation.Play();
		m_visualsAnim.Play();
		m_transitions.Play(0, false);
		while (!m_allowStart)
		{
			yield return null;
		}
		StartCoroutine(EndSequence());
	}

	public void AbortCountdown()
	{
		base.animation.Stop();
		m_visualsAnim.Stop();
		StopAllCoroutines();
	}

	private IEnumerator EndSequence()
	{
		EZTransition transition = m_transitions.Play(1, false);
		while (transition.IsRunning())
		{
			yield return null;
		}
		m_visualsAnim.gameObject.SetActiveRecursively(false);
		ActivateJetStreams(false);
	}

	private void PlayAudio(AudioClip _clip)
	{
		m_audioPlayer.Play(_clip);
	}

	private void FireJetSteams()
	{
		ActivateJetStreams(true);
	}

	private void StartRace()
	{
		m_allowStart = true;
	}

	private void ActivateJetStreams(bool _active)
	{
		GameObject[] jetStreams = m_jetStreams;
		foreach (GameObject gameObject in jetStreams)
		{
			gameObject.SetActiveRecursively(_active);
		}
	}
}
