using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Audio/Music Player")]
public class MusicPlayer : MonoBehaviour
{
	private struct Source
	{
		private AudioSource m_audioSource;

		private int m_clipIndex;

		public bool IsOK
		{
			get
			{
				return m_audioSource != null;
			}
		}

		public bool IsPlaying
		{
			get
			{
				return m_audioSource.isPlaying;
			}
		}

		public AudioClip Clip
		{
			get
			{
				return m_audioSource.clip;
			}
		}

		public float Length
		{
			get
			{
				return (!(m_audioSource.clip == null)) ? m_audioSource.clip.length : 0f;
			}
		}

		public int ClipIndex
		{
			get
			{
				return m_clipIndex;
			}
		}

		public float Time
		{
			get
			{
				return m_audioSource.time;
			}
		}

		public int TimeSamples
		{
			get
			{
				return m_audioSource.timeSamples;
			}
		}

		public Source(AudioSource _source)
		{
			m_audioSource = _source;
			m_audioSource.playOnAwake = false;
			m_audioSource.bypassEffects = true;
			m_audioSource.ignoreListenerVolume = true;
			m_audioSource.loop = false;
			m_audioSource.volume = 0f;
			m_clipIndex = -1;
		}

		public void Play(MusicTrack _track, int _clipIndex, bool _loop, ulong _delay)
		{
			m_clipIndex = _clipIndex;
			m_audioSource.clip = ((_clipIndex < 0) ? _track.m_fanfare : _track.m_clips[_clipIndex]);
			m_audioSource.loop = _loop;
			m_audioSource.Play(_delay);
		}

		public void Resume(MusicTrack _track, int _clipIndex, int _timeSamples, bool _loop)
		{
			m_clipIndex = _clipIndex;
			m_audioSource.clip = _track.m_clips[_clipIndex];
			m_audioSource.loop = _loop;
			m_audioSource.Play();
			m_audioSource.timeSamples = _timeSamples;
		}

		public void Stop()
		{
			m_audioSource.Stop();
			m_audioSource.clip = null;
			m_clipIndex = -1;
		}

		public void SetVolume(float _volume)
		{
			m_audioSource.volume = _volume;
		}
	}

	private const string c_musicPlayerName = "__MusicPlayer";

	private const string c_musicPlayerFind = "/__MusicPlayer";

	private const float c_queueRetryTime = 0.1f;

	private const float c_defaultFadeTime = 2f;

	private Source[] m_sources = new Source[2];

	private Playlist m_currentPlaylist;

	private int m_currentTrackIndex = -1;

	private int m_queuedClipIndex = -1;

	private int m_resumeClipIndex = -1;

	private int m_resumeClipTimeSamples = -1;

	private float m_volume;

	private float m_fade = 1f;

	public float m_volumeScale;

	private bool AnySourcesPlaying
	{
		get
		{
			for (int i = 0; i < m_sources.Length; i++)
			{
				if (m_sources[i].IsPlaying)
				{
					return true;
				}
			}
			return false;
		}
	}

	private int FreeSourceIndex
	{
		get
		{
			for (int i = 0; i < m_sources.Length; i++)
			{
				if (!m_sources[i].IsPlaying)
				{
					return i;
				}
			}
			return -1;
		}
	}

	private int PlayingSourceIndex
	{
		get
		{
			int result = -1;
			float num = float.MinValue;
			for (int i = 0; i < m_sources.Length; i++)
			{
				if (m_sources[i].IsPlaying && m_sources[i].Time > num)
				{
					result = i;
					num = m_sources[i].Time;
				}
			}
			return result;
		}
	}

	private int InProgressSourceIndex
	{
		get
		{
			int result = -1;
			float num = 0f;
			for (int i = 0; i < m_sources.Length; i++)
			{
				float time = m_sources[i].Time;
				if (m_sources[i].ClipIndex >= 0 && time > num && time < m_sources[i].Length)
				{
					result = i;
					num = time;
				}
			}
			return result;
		}
	}

	public float Volume
	{
		get
		{
			return m_volume;
		}
		set
		{
			m_volume = value;
			ApplyVolume();
		}
	}

	public float Fade
	{
		get
		{
			return m_fade;
		}
		set
		{
			m_fade = value;
			ApplyVolume();
		}
	}

	private MusicTrack CurrentTrack
	{
		get
		{
			if (m_currentPlaylist != null && m_currentTrackIndex >= 0)
			{
				return m_currentPlaylist.m_tracks[m_currentTrackIndex];
			}
			return null;
		}
	}

	private void Awake()
	{
		base.gameObject.name = "__MusicPlayer";
		for (int i = 0; i < m_sources.Length; i++)
		{
			AudioSource source = base.gameObject.AddComponent<AudioSource>();
			m_sources[i] = new Source(source);
		}
		Volume = Settings.MusicVolume * m_volumeScale;
	}

	private void Start()
	{
		Settings.OnSettingsUpdated += OnSettingsUpdated;
	}

	private void OnDestroy()
	{
		Settings.OnSettingsUpdated -= OnSettingsUpdated;
		for (int i = 0; i < m_sources.Length; i++)
		{
			m_sources[i] = default(Source);
		}
		m_currentPlaylist = null;
	}

	private void OnSettingsUpdated(object _sender, EventArgs _args)
	{
		Volume = Settings.MusicVolume * m_volumeScale;
	}

	private IEnumerator OnApplicationPause(bool _pause)
	{
		if (_pause)
		{
			int inProgressSourceIndex = InProgressSourceIndex;
			if (inProgressSourceIndex < 0)
			{
				m_resumeClipIndex = -1;
				m_resumeClipTimeSamples = -1;
				Stop();
			}
			else
			{
				m_resumeClipIndex = m_sources[inProgressSourceIndex].ClipIndex;
				m_resumeClipTimeSamples = m_sources[inProgressSourceIndex].TimeSamples;
				StopTrack();
			}
			yield break;
		}
		yield return null;
		for (int i = 0; i < m_sources.Length; i++)
		{
			if (!m_sources[i].IsOK)
			{
				yield break;
			}
		}
		bool isMusicPlaying = Platform.IsMusicPlaying();
		if (!AnySourcesPlaying && !isMusicPlaying)
		{
			Resume();
		}
		if (isMusicPlaying)
		{
			Stop();
		}
	}

	private void PlayNextTrack()
	{
		if (!IsValidPlaylist(m_currentPlaylist))
		{
			StopPlaylist();
			return;
		}
		int num = m_currentPlaylist.m_tracks.Length;
		int num2 = ((m_currentTrackIndex >= 0) ? ((m_currentTrackIndex + 1) % num) : UnityEngine.Random.Range(0, num));
		if (num2 < num)
		{
			StopTrack();
			if (Platform.IsMusicPlaying())
			{
				return;
			}
			Fade = 1f;
			m_currentTrackIndex = num2;
			MusicTrack musicTrack = m_currentPlaylist.m_tracks[m_currentTrackIndex];
			if (musicTrack.m_clips == null || musicTrack.m_clips.Length == 0)
			{
				Debug.LogError("Music track has no clips");
				return;
			}
			int freeSourceIndex = FreeSourceIndex;
			if (freeSourceIndex < 0)
			{
				Debug.LogError("No free sources");
				return;
			}
			bool flag = musicTrack.m_loopLastClip && musicTrack.m_clips.Length == 1;
			m_sources[freeSourceIndex].Play(musicTrack, 0, flag, 0uL);
			m_queuedClipIndex = 0;
			if (!flag)
			{
				if (musicTrack.m_clips.Length > 1)
				{
					QueueNextTrackSection();
				}
				else
				{
					Invoke("TrackComplete", m_sources[freeSourceIndex].Length + m_currentPlaylist.m_silenceBetweenTracks);
				}
			}
		}
		else
		{
			Stop();
		}
	}

	private void Resume()
	{
		if (!IsValidPlaylist(m_currentPlaylist))
		{
			StopPlaylist();
			return;
		}
		if (m_currentTrackIndex < 0 || m_resumeClipIndex < 0)
		{
			PlayNextTrack();
			return;
		}
		if (m_currentTrackIndex < m_currentPlaylist.m_tracks.Length)
		{
			StopTrack();
			if (!Platform.IsMusicPlaying())
			{
				MusicTrack musicTrack = m_currentPlaylist.m_tracks[m_currentTrackIndex];
				if (musicTrack.m_clips == null || musicTrack.m_clips.Length <= m_resumeClipIndex)
				{
					Debug.LogError("Unable to resume clip");
					return;
				}
				int freeSourceIndex = FreeSourceIndex;
				if (freeSourceIndex < 0)
				{
					Debug.LogError("No free sources");
					return;
				}
				bool flag = m_resumeClipIndex == musicTrack.m_clips.Length - 1;
				bool flag2 = flag && musicTrack.m_loopLastClip;
				m_sources[freeSourceIndex].Resume(musicTrack, m_resumeClipIndex, m_resumeClipTimeSamples, flag2);
				m_queuedClipIndex = m_resumeClipIndex;
				if (!flag2)
				{
					if (flag)
					{
						float num = Mathf.Max(0f, m_sources[freeSourceIndex].Length - m_sources[freeSourceIndex].Time);
						Invoke("TrackComplete", num + m_currentPlaylist.m_silenceBetweenTracks);
					}
					else
					{
						QueueNextTrackSection();
					}
				}
			}
		}
		else
		{
			Stop();
		}
		m_resumeClipIndex = -1;
		m_resumeClipTimeSamples = -1;
	}

	private void QueueNextTrackSection()
	{
		int freeSourceIndex = FreeSourceIndex;
		if (freeSourceIndex < 0)
		{
			Invoke("QueueNextTrackSection", 0.1f);
			return;
		}
		MusicTrack musicTrack = m_currentPlaylist.m_tracks[m_currentTrackIndex];
		int num = m_queuedClipIndex + 1;
		bool flag = num == musicTrack.m_clips.Length - 1;
		bool flag2 = flag && musicTrack.m_loopLastClip;
		AudioClip audioClip = musicTrack.m_clips[num];
		int playingSourceIndex = PlayingSourceIndex;
		float num2;
		ulong delay;
		if (playingSourceIndex < 0)
		{
			num2 = 0f;
			delay = 0uL;
		}
		else
		{
			AudioClip clip = m_sources[playingSourceIndex].Clip;
			ulong num3 = (ulong)clip.samples - (ulong)m_sources[playingSourceIndex].TimeSamples;
			num2 = Mathf.Max(0f, clip.length - m_sources[playingSourceIndex].Time);
			delay = (ulong)((long)num3 * (long)audioClip.frequency) / (ulong)clip.frequency + 1;
		}
		m_sources[freeSourceIndex].Play(musicTrack, num, flag2, delay);
		m_queuedClipIndex = num;
		if (!flag2)
		{
			if (flag)
			{
				Invoke("TrackComplete", num2 + audioClip.length + m_currentPlaylist.m_silenceBetweenTracks);
			}
			else
			{
				Invoke("QueueNextTrackSection", num2);
			}
		}
	}

	private void TrackComplete()
	{
		int playingSourceIndex = PlayingSourceIndex;
		if (playingSourceIndex < 0)
		{
			PlayNextTrack();
			return;
		}
		float num = Mathf.Max(0f, m_sources[playingSourceIndex].Length - m_sources[playingSourceIndex].Time);
		Invoke("TrackComplete", num + m_currentPlaylist.m_silenceBetweenTracks);
	}

	public void Play(Playlist _playlist)
	{
		Stop();
		if (IsValidPlaylist(_playlist))
		{
			m_currentPlaylist = _playlist;
			m_currentTrackIndex = -1;
			PlayNextTrack();
		}
	}

	public void Stop()
	{
		StopTrack();
		m_currentTrackIndex = -1;
	}

	public void StopPlaylist()
	{
		Stop();
		m_currentPlaylist = null;
	}

	private void StopTrack()
	{
		for (int i = 0; i < m_sources.Length; i++)
		{
			m_sources[i].Stop();
		}
		m_queuedClipIndex = -1;
		CancelInvoke();
	}

	public void TransitionToPlaylist(Playlist _playlist)
	{
		TransitionToPlaylist(_playlist, 2f);
	}

	public void TransitionToPlaylist(Playlist _playlist, float _fadeTime)
	{
		if (IsValidPlaylist(_playlist))
		{
			if (m_currentPlaylist == null || m_currentTrackIndex < 0)
			{
				Play(_playlist);
				return;
			}
			StopAllCoroutines();
			StartCoroutine(FadeTransitionToPlaylist(_playlist, _fadeTime));
		}
	}

	public void TryTransitionToPlaylist(Playlist _playlist)
	{
		TryTransitionToPlaylist(_playlist, 2f);
	}

	public void TryTransitionToPlaylist(Playlist _playlist, float _fadeTime)
	{
		if (IsValidPlaylist(_playlist) && !(_playlist == m_currentPlaylist))
		{
			if (m_currentPlaylist == null || m_currentTrackIndex < 0)
			{
				Play(_playlist);
				return;
			}
			StopAllCoroutines();
			StartCoroutine(FadeTransitionToPlaylist(_playlist, _fadeTime));
		}
	}

	private IEnumerator FadeTransitionToPlaylist(Playlist _playlist, float _fadeTime)
	{
		float initialFade = Fade;
		float currentFadeTime = _fadeTime;
		while (currentFadeTime > 0f)
		{
			yield return null;
			currentFadeTime -= Time.smoothDeltaTime;
			float fade = Mathf.InverseLerp(0f, _fadeTime, currentFadeTime) * initialFade;
			Fade = fade;
		}
		Fade = 0f;
		yield return null;
		Play(_playlist);
	}

	public void FadeOut(Action _onFadedOut)
	{
		FadeOut(_onFadedOut, 2f);
	}

	public void FadeOut(Action _onFadedOut, float _fadeTime)
	{
		if (m_currentPlaylist == null)
		{
			Stop();
			if (_onFadedOut != null)
			{
				_onFadedOut();
			}
		}
		else
		{
			StopAllCoroutines();
			StartCoroutine(FadeOutImpl(_onFadedOut, _fadeTime));
		}
	}

	private IEnumerator FadeOutImpl(Action _onFadedOut, float _fadeTime)
	{
		float initialFade = Fade;
		float currentFadeTime = _fadeTime;
		while (currentFadeTime > 0f)
		{
			yield return null;
			currentFadeTime -= Time.smoothDeltaTime;
			float fade = Mathf.InverseLerp(0f, _fadeTime, currentFadeTime) * initialFade;
			Fade = fade;
		}
		Fade = 0f;
		yield return null;
		Stop();
		if (_onFadedOut != null)
		{
			_onFadedOut();
		}
	}

	public void DuckVolume(float _fadeLevel, float _inDuration, float _duckDuration, float _outDuration)
	{
		if (m_currentPlaylist != null)
		{
			StopAllCoroutines();
			StartCoroutine(DuckVolumeImpl(_fadeLevel, _inDuration, _duckDuration, _outDuration));
		}
	}

	private IEnumerator DuckVolumeImpl(float _fadeLevel, float _inDuration, float _duckDuration, float _outDuration)
	{
		float fadeSize = Fade - _fadeLevel;
		float currentFadeTime = _inDuration;
		while (currentFadeTime > 0f)
		{
			yield return null;
			currentFadeTime -= Time.smoothDeltaTime;
			float fade = Mathf.InverseLerp(0f, _inDuration, currentFadeTime) * fadeSize;
			Fade = _fadeLevel + fade;
		}
		Fade = _fadeLevel;
		yield return new WaitForSeconds(_duckDuration);
		float fadeSize2 = 1f - _fadeLevel;
		float currentFadeTime2 = _outDuration;
		while (currentFadeTime2 > 0f)
		{
			yield return null;
			currentFadeTime2 -= Time.smoothDeltaTime;
			float fade2 = Mathf.InverseLerp(_outDuration, 0f, currentFadeTime2) * fadeSize2;
			Fade = _fadeLevel + fade2;
		}
	}

	public bool PlayFanfare()
	{
		MusicTrack currentTrack = CurrentTrack;
		if (currentTrack == null || currentTrack.m_fanfare == null)
		{
			return false;
		}
		StopAllCoroutines();
		StartCoroutine(PlayFanfareImpl(currentTrack, 0.066f));
		return true;
	}

	private IEnumerator FanfareFadeDown(float _fadeLevel, float _inDuration)
	{
		float fadeSize = Fade - _fadeLevel;
		float currentFadeTime = _inDuration;
		while (currentFadeTime > 0f)
		{
			yield return null;
			currentFadeTime -= Time.smoothDeltaTime;
			float fade = Mathf.InverseLerp(0f, _inDuration, currentFadeTime) * fadeSize;
			Fade = _fadeLevel + fade;
		}
		Fade = _fadeLevel;
	}

	private IEnumerator PlayFanfareImpl(MusicTrack _track, float _fadeDuration)
	{
		yield return StartCoroutine(FanfareFadeDown(0f, _fadeDuration));
		int inProgressSourceIndex = InProgressSourceIndex;
		if (inProgressSourceIndex >= 0)
		{
			m_resumeClipIndex = m_sources[inProgressSourceIndex].ClipIndex;
			m_resumeClipTimeSamples = m_sources[inProgressSourceIndex].TimeSamples;
		}
		StopTrack();
		yield return null;
		Fade = 1f;
		int sourceIndex = FreeSourceIndex;
		if (sourceIndex < 0)
		{
			Debug.LogError("No free sources");
			Resume();
		}
		else
		{
			m_sources[sourceIndex].Play(_track, -1, false, 0uL);
			Invoke("TrackComplete", m_sources[sourceIndex].Length + 0.1f);
		}
	}

	private bool IsValidPlaylist(Playlist _playlist)
	{
		if (_playlist == null)
		{
			return false;
		}
		if (_playlist.m_tracks == null)
		{
			return false;
		}
		return _playlist.m_tracks.Length > 0;
	}

	private void ApplyVolume()
	{
		for (int i = 0; i < m_sources.Length; i++)
		{
			m_sources[i].SetVolume(m_volume * m_fade);
		}
	}

	public static MusicPlayer Get()
	{
		GameObject gameObject = GameObject.Find("/__MusicPlayer");
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent<MusicPlayer>();
	}
}
