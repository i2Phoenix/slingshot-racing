using UnityEngine;

public class CreatePrefabChild : MonoBehaviour
{
	public GameObject m_prefab;

	private void Start()
	{
		GameObject gameObject = Object.Instantiate(m_prefab) as GameObject;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = m_prefab.transform.localPosition;
		gameObject.transform.localRotation = m_prefab.transform.localRotation;
	}
}
