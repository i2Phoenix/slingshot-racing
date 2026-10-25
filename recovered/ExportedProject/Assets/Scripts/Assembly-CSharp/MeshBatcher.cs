using System.Collections.Generic;
using UnityEngine;

public class MeshBatcher : MonoBehaviour
{
	private int m_doBatch;

	private void Start()
	{
	}

	private void LateUpdate()
	{
		if (m_doBatch > 0)
		{
			m_doBatch--;
			if (m_doBatch <= 0)
			{
				DoBatch();
			}
		}
	}

	public void Batch()
	{
		m_doBatch = 1;
	}

	private void DoBatch()
	{
		List<GameObject> list = new List<GameObject>();
		GameObject[] array = GameObject.FindGameObjectsWithTag("BatchOnStartup");
		for (int i = 0; i < array.Length; i++)
		{
			MeshFilter[] componentsInChildren = GameObjectUtils.GetComponentsInChildren<MeshFilter>(array[i]);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				list.Add(componentsInChildren[j].gameObject);
			}
		}
		GameObject[] array2 = new GameObject[list.Count];
		for (int k = 0; k < list.Count; k++)
		{
			array2[k] = list[k];
		}
		GameObject gameObject = GameObject.Find("Startup Batch Object");
		if (gameObject == null)
		{
			gameObject = new GameObject("Startup Batch Object");
		}
		StaticBatchingUtility.Combine(array2, gameObject);
	}
}
