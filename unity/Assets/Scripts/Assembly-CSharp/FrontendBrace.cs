using UnityEngine;

public class FrontendBrace : MonoBehaviour
{
	private Joint[] m_joints;

	private Vector3[] m_offsets;

	public GameObject m_prefab;

	public int VehicleIndex { get; set; }

	public UIListItemContainer ListItem { get; set; }

	public FrontendHoist Hoist { get; private set; }

	private void Start()
	{
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		GameObject gameObject = Object.Instantiate(m_prefab, position, rotation) as GameObject;
		if (gameObject != null)
		{
			FrontendHoist component = gameObject.GetComponent<FrontendHoist>();
			if (ListItem != null)
			{
				component.ListItem = ListItem;
			}
			component.VehicleIndex = VehicleIndex;
			Hoist = component;
		}
		gameObject.transform.parent = base.transform;
		gameObject.SetActiveRecursively(true);
		m_joints = base.gameObject.GetComponentsInChildren<Joint>();
		m_offsets = new Vector3[m_joints.Length];
		for (int i = 0; i < m_joints.Length; i++)
		{
			Joint joint = m_joints[i];
			m_offsets[i] = joint.transform.position - position;
			joint.connectedBody = gameObject.GetComponent<Rigidbody>();
		}
	}

	public void SetPosition(Vector3 _position)
	{
		if (m_joints != null)
		{
			for (int i = 0; i < m_joints.Length; i++)
			{
				Joint joint = m_joints[i];
				// MovePosition/position do not update the Transform of a frozen
				// dynamic anchor in modern PhysX. Preserve its free rotation and
				// original joint/sway physics, and move the rail explicitly.
				joint.transform.position = _position + m_offsets[i];
			}
			Physics.SyncTransforms();
		}
	}
}
