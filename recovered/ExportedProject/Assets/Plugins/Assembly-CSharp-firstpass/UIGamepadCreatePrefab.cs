using UnityEngine;

public class UIGamepadCreatePrefab : MonoBehaviour
{
	public GameObject m_prefab;

	private void Start()
	{
		if (m_prefab != null && UIGamepadManager.IsGamepadActive)
		{
			GameObject gameObject = Object.Instantiate(m_prefab) as GameObject;
			Vector3 localScale = gameObject.transform.localScale;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = localScale;
		}
	}
}
