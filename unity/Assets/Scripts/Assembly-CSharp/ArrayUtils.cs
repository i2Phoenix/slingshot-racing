using System.Collections.Generic;

public static class ArrayUtils
{
	public static bool IsOrdered<T>(T[] _array, IComparer<T> _comparer)
	{
		for (int i = 1; i < _array.Length; i++)
		{
			if (_comparer.Compare(_array[i - 1], _array[i]) > 0)
			{
				return false;
			}
		}
		return true;
	}

	public static int Length<T>(T[] _array)
	{
		return (_array != null) ? _array.Length : 0;
	}

	public static List<object> ToObjectList<T>(List<T> _list) where T : class
	{
		if (_list == null)
		{
			return null;
		}
		List<object> list = new List<object>();
		for (int i = 0; i < _list.Count; i++)
		{
			list.Add(_list[i]);
		}
		return list;
	}
}
