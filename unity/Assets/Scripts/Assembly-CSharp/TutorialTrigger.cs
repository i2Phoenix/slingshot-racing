using System.Collections;
using UnityEngine;

[AddComponentMenu("Gameplay/Tutorial Trigger")]
public class TutorialTrigger : MonoBehaviour
{
	public enum GrappleZone
	{
		StartBegin = 0,
		StartEnd = 1,
		FinishBegin = 2,
		FinishEnd = 3
	}

	private bool m_allowTrigger;

	public GameObject m_target;

	public GrappleZone m_zone;

	public int m_triggerID;

	private void Start()
	{
		if (m_target == null)
		{
			m_target = GameObject.FindGameObjectWithTag("UIController");
		}
		m_allowTrigger = true;
	}

	private void OnTriggerEnter(Collider collider)
	{
		Vehicle component = collider.transform.root.GetComponent<Vehicle>();
		if (component != null && m_target != null && m_allowTrigger)
		{
			m_target.BroadcastMessage("TutorialEvent_EnterTrigger", this, SendMessageOptions.DontRequireReceiver);
			StartCoroutine(SuppressTrigger());
		}
	}

	private IEnumerator SuppressTrigger()
	{
		m_allowTrigger = false;
		yield return YieldCache.WaitForSeconds(0.2f);
		m_allowTrigger = true;
	}
}
