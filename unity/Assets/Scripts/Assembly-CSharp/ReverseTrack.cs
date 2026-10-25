using UnityEngine;

public class ReverseTrack : MonoBehaviour
{
	private void Awake()
	{
		TrackUtils.ReverseTrack();
	}

	private void Start()
	{
		Object.Destroy(base.gameObject);
	}
}
