using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerBox : MonoBehaviour
{
	private Vector3 m_forward;

	public float m_impulse = 2f;

	public bool m_isDirectional = true;

	public bool m_dischargeAll = true;

	private void Awake()
	{
		m_forward = base.transform.forward;
	}

	private void OnTriggerEnter(Collider collider)
	{
		PhysicsVehicle component = collider.transform.root.GetComponent<PhysicsVehicle>();
		if (component != null)
		{
			float num = m_impulse;
			if (m_isDirectional)
			{
				num *= Mathf.Clamp01(Vector3.Dot(component.Forward, m_forward));
			}
			component.TriggerPowerBox(num, m_dischargeAll);
		}
	}
}
