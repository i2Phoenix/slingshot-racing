using UnityEngine;

public class MusicTrack : ScriptableObject
{
	public AudioClip[] m_clips;

	public AudioClip m_fanfare;

	public bool m_loopLastClip;
}
