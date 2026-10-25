using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BoostBox : MonoBehaviour
{
	private Vector3 m_forward;

	public float m_impulse = 12f;

	public bool m_isDirectional = true;

	public int m_generation;

	private void Awake()
	{
		m_forward = base.transform.forward;
	}

	private void OnTriggerEnter(Collider _collider)
	{
		PhysicsVehicle component = _collider.transform.root.GetComponent<PhysicsVehicle>();
		if (component != null)
		{
			float num = m_impulse;
			if (m_isDirectional)
			{
				num *= Mathf.Clamp01(Vector3.Dot(component.Forward, m_forward));
			}
			component.TriggerBoost(num);
		}
	}
}
