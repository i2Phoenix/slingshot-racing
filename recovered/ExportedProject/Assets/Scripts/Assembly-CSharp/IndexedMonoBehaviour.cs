using System;
using UnityEngine;

public class IndexedMonoBehaviour<T> : MonoBehaviour where T : IndexedMonoBehaviour<T>
{
	private int m_index;

	private static T[] s_all;

	public static T[] All
	{
		get
		{
			if (s_all == null)
			{
				BuildAll();
			}
			return s_all;
		}
	}

	public int Index
	{
		get
		{
			return m_index;
		}
	}

	protected virtual void Awake()
	{
		s_all = null;
	}

	protected virtual void OnDestroy()
	{
		s_all = null;
	}

	protected virtual void Start()
	{
		if (s_all == null)
		{
			BuildAll();
		}
	}

	private static void BuildAll()
	{
		Type typeFromHandle = typeof(T);
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeFromHandle);
		if (typeFromHandle == typeof(TrackObstacle))
		{
			Array.Sort(array, 0, array.Length, default(GameObjectUtils.CompareByPosition));
		}
		else
		{
			Array.Sort(array, 0, array.Length, default(GameObjectUtils.CompareByName));
		}
		s_all = new T[array.Length];
		for (int i = 0; i != array.Length; i++)
		{
			T val = array[i] as T;
			val.m_index = i;
			s_all[i] = val;
		}
	}
}
