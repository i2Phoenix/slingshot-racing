using UnityEngine;

public class BoneLink : MonoBehaviour
{
	private Transform[] m_srcTransform;

	private Transform[] m_dstTransform;

	public string[] m_srcName;

	public string[] m_dstName;

	public bool m_searchFromParent;

	public Vector3 m_offset;

	private void Start()
	{
		m_srcTransform = new Transform[m_srcName.Length];
		m_dstTransform = new Transform[m_dstName.Length];
		Transform parent = base.transform;
		if (m_searchFromParent)
		{
			parent = base.transform.parent;
		}
		for (int i = 0; i < m_srcName.Length; i++)
		{
			m_srcTransform[i] = FindChild(parent, m_srcName[i]);
		}
		for (int j = 0; j < m_dstName.Length; j++)
		{
			m_dstTransform[j] = FindChild(parent, m_dstName[j]);
		}
	}

	private void Update()
	{
		for (int i = 0; i < m_srcTransform.Length; i++)
		{
			m_dstTransform[i].position = m_srcTransform[i].position + m_offset;
		}
	}

	private static Transform FindChild(Transform _parent, string _name)
	{
		for (int i = 0; i < _parent.childCount; i++)
		{
			Transform child = _parent.GetChild(i);
			if (child.name == _name)
			{
				return child;
			}
			Transform transform = FindChild(child, _name);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}
}
