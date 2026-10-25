using UnityEngine;

public class PaintEmitter : MonoBehaviour
{
	private Vector3 m_pos;

	private Vector3 m_vel;

	private ParticleSys m_particleSys;

	public ParticleEmitter m_particleEmitter;

	public Color m_colour;

	private void Start()
	{
		m_particleSys = m_particleEmitter.gameObject.GetComponent<ParticleSys>();
	}

	private void Update()
	{
	}

	private void EmitPressedEffect()
	{
		Transform transform = base.transform.FindChild("PaintGloop");
		if (!(transform != null))
		{
			return;
		}
		Transform transform2 = transform.FindChild("Bone1");
		if (transform2 != null)
		{
			Vector3 position = transform2.position;
			Vector3 vector = transform2.rotation * Vector3.up;
			vector.y *= 16f;
			vector.Normalize();
			int num = 32;
			for (int i = 0; i < num; i++)
			{
				Vector3 pos = position;
				pos += RandomTables.InsideUnitSphere * 0.1f;
				Vector3 vel = vector;
				vel *= RandomTables.Value * 1.5f + 2f;
				vel += RandomTables.InsideUnitSphere * 0.75f;
				vel *= 1.05f;
				float num2 = ((float)i + 0.5f) / (float)num;
				num2 = num2 * 0.5f + 0.5f;
				vel *= num2;
				float size = RandomTables.Value * 0.04f + 0.085f;
				float energy = RandomTables.Value * 0.4f + 0.8f;
				float rotation = RandomTables.Value * 360f;
				float angularVelocity = (RandomTables.Value - 0.5f) * 100f;
				m_particleSys.Emit(pos, vel, size, energy, m_colour, rotation, angularVelocity);
			}
		}
	}
}
