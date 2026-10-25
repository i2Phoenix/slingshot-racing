using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class GameObjectUtils
{
	[StructLayout((LayoutKind)0, Size = 1)]
	public struct CompareByName : IComparer<GameObject>, IComparer<UnityEngine.Object>
	{
		public int Compare(GameObject a, GameObject b)
		{
			return a.name.CompareTo(b.name);
		}

		public int Compare(UnityEngine.Object a, UnityEngine.Object b)
		{
			return a.name.CompareTo(b.name);
		}
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	public struct CompareByPosition : IComparer<UnityEngine.Object>
	{
		public int Compare(UnityEngine.Object _a, UnityEngine.Object _b)
		{
			Component component = _a as Component;
			Component component2 = _b as Component;
			Vector3 position = component.transform.position;
			Vector3 position2 = component2.transform.position;
			if (position.x != position2.x)
			{
				return (!(position.x < position2.x)) ? 1 : (-1);
			}
			if (position.z != position2.z)
			{
				return (!(position.z < position2.z)) ? 1 : (-1);
			}
			return 0;
		}
	}

	public static T FindComponentWithTag<T>(string _tag) where T : Component
	{
		GameObject gameObject = GameObject.FindWithTag(_tag);
		if (gameObject == null)
		{
			return (T)null;
		}
		return gameObject.GetComponent<T>();
	}

	public static T[] FindComponentsWithTag<T>(string _tag) where T : Component
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(_tag);
		if (array == null)
		{
			return null;
		}
		List<T> list = new List<T>();
		for (int i = 0; i < array.Length; i++)
		{
			T[] components = array[i].GetComponents<T>();
			if (components != null && components.Length > 0)
			{
				list.AddRange(components);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		return list.ToArray();
	}

	public static T GetComponentInChildren<T>(GameObject _gameObject) where T : Component
	{
		T val = _gameObject.GetComponent<T>();
		if (val != null)
		{
			return val;
		}
		Transform transform = _gameObject.transform;
		for (int i = 0; i < transform.GetChildCount(); i++)
		{
			Transform child = transform.GetChild(i);
			val = GetComponentInChildren<T>(child.gameObject);
			if (val != null)
			{
				return val;
			}
		}
		return val;
	}

	public static T[] GetComponentsInChildren<T>(GameObject _gameObject) where T : Component
	{
		List<T> list = new List<T>();
		list.AddRange(_gameObject.GetComponents<T>());
		Transform transform = _gameObject.transform;
		for (int i = 0; i < transform.GetChildCount(); i++)
		{
			Transform child = transform.GetChild(i);
			GetComponentsInChildren(child.gameObject, list);
		}
		return list.ToArray();
	}

	private static void GetComponentsInChildren<T>(GameObject _gameObject, List<T> o_components) where T : Component
	{
		o_components.AddRange(_gameObject.GetComponents<T>());
		Transform transform = _gameObject.transform;
		for (int i = 0; i < transform.GetChildCount(); i++)
		{
			Transform child = transform.GetChild(i);
			GetComponentsInChildren(child.gameObject, o_components);
		}
	}

	public static Component[] GetComponentsInChildren(GameObject _gameObject, Type _type)
	{
		List<Component> list = new List<Component>();
		list.AddRange(_gameObject.GetComponents(_type));
		Transform transform = _gameObject.transform;
		for (int i = 0; i < transform.GetChildCount(); i++)
		{
			Transform child = transform.GetChild(i);
			GetComponentsInChildren(child.gameObject, _type, list);
		}
		return list.ToArray();
	}

	public static void DestroyAllChildren(GameObject _gameObject)
	{
		Transform transform = _gameObject.transform;
		for (int i = 0; i < transform.GetChildCount(); i++)
		{
			Transform child = transform.GetChild(i);
			UnityEngine.Object.Destroy(child.gameObject);
		}
	}

	private static void GetComponentsInChildren(GameObject _gameObject, Type _type, List<Component> o_components)
	{
		o_components.AddRange(_gameObject.GetComponents(_type));
		Transform transform = _gameObject.transform;
		for (int i = 0; i < transform.GetChildCount(); i++)
		{
			Transform child = transform.GetChild(i);
			GetComponentsInChildren(child.gameObject, _type, o_components);
		}
	}
}
