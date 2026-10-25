using UnityEngine;

public class BobUpAndDown : MonoBehaviour
{
	private float m_offset;

	private float m_randomStart;

	public float m_rate = 1f;

	public float m_minOffset = -0.5f;

	public float m_maxOffset = 0.5f;

	private void Start()
	{
		m_randomStart = Random.Range(0f, 1f);
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		float t = Mathf.PingPong(m_randomStart + Time.time * m_rate, 1f);
		float num = Mathf.SmoothStep(m_minOffset, m_maxOffset, t);
		float num2 = num - m_offset;
		position.y += num2;
		base.transform.position = position;
		m_offset = num;
	}
}
