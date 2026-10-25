using UnityEngine;

[AddComponentMenu("Graphics/Particle Emission Rate (ParticleSystem)")]
[RequireComponent(typeof(ParticleSystem))]
public class ParticleEmissionRate : MonoBehaviour
{
	public float m_lowRate;

	public float m_mediumRate;

	public float m_highRate;

	private void Start()
	{
		ParticleSystem component = GetComponent<ParticleSystem>();
		switch (PlatformSettings.ParticleEffects)
		{
		case PlatformSettings.ParticleDetail.Low:
			component.emissionRate = m_lowRate;
			break;
		case PlatformSettings.ParticleDetail.Medium:
			component.emissionRate = m_mediumRate;
			break;
		default:
			component.emissionRate = m_highRate;
			break;
		}
	}
}
