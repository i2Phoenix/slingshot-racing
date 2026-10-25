using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Graphics/Shared Particles")]
public class SharedParticles : MonoBehaviour
{
	private static readonly string c_sharedParticleSystemsName = "__SharedParticleSystems";

	private static readonly string c_sharedParticleSystemsFind = "/" + c_sharedParticleSystemsName;

	private Dictionary<int, ParticleEmitter> m_emitters = new Dictionary<int, ParticleEmitter>();

	public ParticleEmitter GetEmitter(GameObject _prefab)
	{
		if (_prefab == null)
		{
			return null;
		}
		int instanceID = _prefab.GetInstanceID();
		if (m_emitters.ContainsKey(instanceID))
		{
			return m_emitters[instanceID];
		}
		GameObject gameObject = (GameObject)Object.Instantiate(_prefab);
		gameObject.transform.parent = base.transform;
		ParticleEmitter particleEmitter = gameObject.GetComponent<ParticleEmitter>();
		particleEmitter.emit = false;
		m_emitters[instanceID] = particleEmitter;
		return particleEmitter;
	}

	public static SharedParticles Get()
	{
		GameObject gameObject = GameObject.Find(c_sharedParticleSystemsFind);
		if (gameObject == null)
		{
			gameObject = new GameObject(c_sharedParticleSystemsName);
			gameObject.AddComponent<SharedParticles>();
		}
		return gameObject.GetComponent<SharedParticles>();
	}
}
