using UnityEngine;

[RequireComponent(typeof(ParticleEmitter))]
[AddComponentMenu("Graphics/Activate On Launch Particles")]
public class ActivateOnLaunchParticles : MonoBehaviour
{
	public void ActivateOnLaunch_Replay()
	{
		base.particleEmitter.ClearParticles();
	}
}
