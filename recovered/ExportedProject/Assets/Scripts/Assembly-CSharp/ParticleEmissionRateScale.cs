using UnityEngine;

[AddComponentMenu("Graphics/Particle Emission Rate scale (ParticleEmitter)")]
[RequireComponent(typeof(ParticleEmitter))]
public class ParticleEmissionRateScale : MonoBehaviour
{
	public float m_lowRateScale = 0.8f;

	public float m_mediumRateScale = 0.9f;

	public float m_highRateScale = 1f;

	private void Start()
	{
		ParticleEmitter particleEmitter = base.particleEmitter;
		switch (PlatformSettings.ParticleEffects)
		{
		case PlatformSettings.ParticleDetail.Low:
			particleEmitter.minEmission *= m_lowRateScale;
			particleEmitter.maxEmission *= m_lowRateScale;
			break;
		case PlatformSettings.ParticleDetail.Medium:
			particleEmitter.minEmission *= m_mediumRateScale;
			particleEmitter.maxEmission *= m_mediumRateScale;
			break;
		default:
			particleEmitter.minEmission *= m_highRateScale;
			particleEmitter.maxEmission *= m_highRateScale;
			break;
		}
	}
}
