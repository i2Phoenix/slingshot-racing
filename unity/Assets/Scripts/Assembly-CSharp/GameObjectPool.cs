using System;
using UnityEngine;

public class GameObjectPool
{
	private GameObject m_parent;

	private GameObject[] m_freeList;

	private GameObject[] m_usedList;

	private int m_numFree;

	private int m_numUsed;

	public GameObjectPool(GameObject _parent, string _prefabPath, int _size)
	{
		m_parent = _parent;
		GameObject original = (GameObject)Resources.Load(_prefabPath);
		m_freeList = new GameObject[_size];
		m_numFree = _size;
		for (int i = 0; i < _size; i++)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
			gameObject.transform.parent = m_parent.transform;
			gameObject.SetActiveRecursively(false);
			m_freeList[i] = gameObject;
		}
		m_usedList = new GameObject[_size];
		m_numUsed = 0;
	}

	public GameObject AllocateObject()
	{
		if (m_numFree == 0)
		{
			return null;
		}
		GameObject gameObject = m_freeList[--m_numFree];
		m_freeList[m_numFree] = null;
		m_usedList[m_numUsed++] = gameObject;
		gameObject.SetActiveRecursively(true);
		return gameObject;
	}

	public bool FreeObject(GameObject _gameObject)
	{
		int num = Array.IndexOf(m_usedList, _gameObject);
		if (num < 0)
		{
			return false;
		}
		_gameObject.SetActiveRecursively(false);
		m_usedList[num] = m_usedList[--m_numUsed];
		m_usedList[m_numUsed] = null;
		m_freeList[m_numFree++] = _gameObject;
		return true;
	}
}
