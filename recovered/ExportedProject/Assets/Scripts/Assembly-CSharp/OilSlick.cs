using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OilSlick : MonoBehaviour
{
	private AudioPlayer m_audioPlayer;

	public AudioBank m_audioBank;

	public float m_time;

	private void Awake()
	{
		m_audioPlayer = base.gameObject.AddComponent<AudioPlayer>();
	}

	private void OnTriggerEnter(Collider collider)
	{
		PhysicsVehicle component = collider.transform.root.GetComponent<PhysicsVehicle>();
		if (component != null)
		{
			component.OilSlick(this);
			if (m_audioBank != null && component.Visuals != null)
			{
				m_audioPlayer.PlayRandomClip(m_audioBank, component.Visuals.VolumeScale);
			}
		}
	}
}
