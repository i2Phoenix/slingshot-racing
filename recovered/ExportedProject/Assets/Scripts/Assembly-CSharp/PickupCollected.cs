using UnityEngine;

public class PickupCollected : MonoBehaviour
{
	private ParticleEmitter[] m_emitters;

	private void Awake()
	{
		Object original = Resources.Load("Prefabs/PickupCollected");
		GameObject gameObject = Object.Instantiate(original) as GameObject;
		gameObject.transform.parent = base.transform;
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.identity;
		m_emitters = gameObject.GetComponentsInChildren<ParticleEmitter>();
	}

	private void OnCollected(Pickup _pickup)
	{
		Quaternion collectedRotation = _pickup.CollectedRotation;
		Vector3 position = _pickup.transform.position;
		ParticleEmitter[] emitters = m_emitters;
		foreach (ParticleEmitter emitter in emitters)
		{
			ParticleUtils.Emit(emitter, position, collectedRotation, 0f);
		}
	}
}
