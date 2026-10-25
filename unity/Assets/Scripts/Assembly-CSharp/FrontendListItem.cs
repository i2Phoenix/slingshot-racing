using System.Collections;
using UnityEngine;

public class FrontendListItem : MonoBehaviour
{
	private FrontendBrace m_brace;

	public GameObject m_prefab;

	public int VehicleIndex { get; set; }

	public FrontendBrace Brace
	{
		get
		{
			return m_brace;
		}
	}

	private IEnumerator Start()
	{
		yield return null;
		yield return null;
		UIListItemContainer item = GetComponent<UIListItemContainer>();
		GameObject brace = Object.Instantiate(m_prefab, base.transform.position, base.transform.rotation) as GameObject;
		brace.transform.parent = item.GetScrollList().transform.parent;
		m_brace = brace.GetComponent<FrontendBrace>();
		m_brace.ListItem = item;
		m_brace.VehicleIndex = VehicleIndex;
		// Scene templates were inactive in Unity 3; modern Instantiate copies
		// that state to every child, so the suspended models never start.
		brace.SetActiveRecursively(true);
	}

	private void FixedUpdate()
	{
		if (m_brace != null)
		{
			m_brace.SetPosition(base.transform.position);
		}
	}

	private void OnDestroy()
	{
		if (m_brace != null)
		{
			Object.Destroy(m_brace.gameObject);
			m_brace = null;
		}
	}
}
