using System.Collections;
using UnityEngine;

[AddComponentMenu("Audio/Background Audio")]
public class BackgroundAudio : MonoBehaviour
{
	private RaceManager m_raceManager;

	private AudioSource m_audioSource;

	public AudioClip m_audioClip;

	public float m_volume;

	public Playlist m_inGameMusic;

	private void Start()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnRacePrepared += OnRacePrepared;
		}
		else
		{
			OnRacePrepared();
		}
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnRacePrepared -= OnRacePrepared;
		}
	}

	private void OnRacePrepared()
	{
		MusicPlayer musicPlayer = MusicPlayer.Get();
		if (musicPlayer != null && Settings.InRaceMusic)
		{
			musicPlayer.TransitionToPlaylist(m_inGameMusic);
		}
		else if (musicPlayer != null)
		{
			musicPlayer.FadeOut(OnMusicFadedOut);
		}
		else
		{
			OnMusicFadedOut();
		}
		if (m_raceManager != null)
		{
			m_raceManager.OnRacePrepared -= OnRacePrepared;
		}
	}

	private void OnMusicFadedOut()
	{
		m_audioSource = base.gameObject.AddComponent<AudioSource>();
		m_audioSource.clip = m_audioClip;
		m_audioSource.volume = m_volume;
		m_audioSource.bypassEffects = true;
		m_audioSource.loop = true;
		if (!Platform.IsMusicPlaying() && m_audioSource != null)
		{
			m_audioSource.Play();
		}
	}

	private IEnumerator OnApplicationPause(bool _pause)
	{
		if (_pause)
		{
			yield break;
		}
		yield return null;
		if (!(m_audioSource == null))
		{
			bool isMusicPlaying = Platform.IsMusicPlaying();
			if (!m_audioSource.isPlaying && !isMusicPlaying)
			{
				m_audioSource.Play();
			}
			if (m_audioSource.isPlaying && isMusicPlaying)
			{
				m_audioSource.Stop();
			}
		}
	}
}
