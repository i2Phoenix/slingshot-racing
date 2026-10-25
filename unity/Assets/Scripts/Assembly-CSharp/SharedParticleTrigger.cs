using UnityEngine;

[AddComponentMenu("Graphics/Shared Particle Trigger")]
[RequireComponent(typeof(Collider))]
public class SharedParticleTrigger : MonoBehaviour
{
	private ParticleEmitter m_emitter;

	public GameObject m_prefab;

	public float m_radius;

	public float m_speedThreshold;

	public float m_minInterval;

	public Color m_colour = Color.white;

	private void Start()
	{
		SharedParticles sharedParticles = SharedParticles.Get();
		m_emitter = sharedParticles.GetEmitter(m_prefab);
	}

	private void OnTriggerEnter(Collider _collider)
	{
		if (_collider.attachedRigidbody != null && _collider.attachedRigidbody.linearVelocity.magnitude > m_speedThreshold)
		{
			ParticleUtils.Emit(m_emitter, base.transform, m_radius, m_colour);
			base.GetComponent<Collider>().enabled = false;
			Invoke("EnableCollider", m_minInterval);
		}
	}

	private void EnableCollider()
	{
		base.GetComponent<Collider>().enabled = true;
	}
}
