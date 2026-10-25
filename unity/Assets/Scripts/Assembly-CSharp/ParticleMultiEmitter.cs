using UnityEngine;

[AddComponentMenu("Graphics/Particle Multi Emitter")]
public class ParticleMultiEmitter : MonoBehaviour
{
	private Transform m_transform;

	private float m_emissionRemainder;

	public ParticleEmitter m_emitter;

	public Color m_colour = Color.white;

	public float m_radius;

	private void Awake()
	{
		m_transform = base.transform;
	}

	private void Update()
	{
		float num = m_emissionRemainder + Random.Range(m_emitter.minEmission, m_emitter.maxEmission) * Time.deltaTime;
		int num2 = Mathf.FloorToInt(num);
		if (num2 > 0)
		{
			ParticleUtils.Emit(m_emitter, m_transform, m_radius, m_colour, num2);
		}
		m_emissionRemainder = num - (float)num2;
	}

	public void ActivateOnLaunch_Replay()
	{
		m_emitter.ClearParticles();
	}
}
