using UnityEngine;

public class Playlist : ScriptableObject
{
	public MusicTrack[] m_tracks;

	public float m_silenceBetweenTracks = 5f;
}
