using UnityEngine;

public class RotateAround : MonoBehaviour
{
	private Transform m_transform;

	public Vector3 m_rate;

	private void Start()
	{
		m_transform = base.transform;
		Vector3 localEulerAngles = m_transform.localEulerAngles;
		for (int i = 0; i < 3; i++)
		{
			if (Mathf.Abs(m_rate[i]) > 0.1f)
			{
				localEulerAngles[i] = Random.Range(0f, 360f);
			}
		}
		m_transform.localEulerAngles = localEulerAngles;
	}

	private void Update()
	{
		m_transform.Rotate(m_rate * Time.deltaTime);
	}
}
