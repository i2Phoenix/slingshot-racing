using UnityEngine;

public class PlayableArea : MonoBehaviour
{
	public float m_1pCameraTilt = 10f;

	public float m_lookAheadScale = 1f;

	private void Awake()
	{
		base.transform.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
	}
}
