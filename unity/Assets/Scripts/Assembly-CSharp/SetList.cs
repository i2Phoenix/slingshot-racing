using System;
using System.Collections;
using System.Collections.Generic;

public class SetList<T> : IEnumerable<T>, IEnumerable where T : IComparable<T>
{
	private List<T> m_list;

	public int Capacity
	{
		get
		{
			return m_list.Capacity;
		}
		set
		{
			m_list.Capacity = value;
		}
	}

	public int Count
	{
		get
		{
			return m_list.Count;
		}
	}

	public SetList()
	{
		m_list = new List<T>();
	}

	public SetList(int _capacity)
	{
		m_list = new List<T>(_capacity);
	}

	public SetList(SetList<T> _other)
	{
		m_list = new List<T>(_other.m_list);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool TryAdd(T _item)
	{
		int num = BinarySearch(_item);
		if (num < 0)
		{
			num = ~num;
			m_list.Insert(num, _item);
			return true;
		}
		return false;
	}

	public bool TryRemove(T _item)
	{
		int num = BinarySearch(_item);
		if (num < 0)
		{
			return false;
		}
		m_list.RemoveAt(num);
		return true;
	}

	public void Add(T _item)
	{
		int num = BinarySearch(_item);
		if (num < 0)
		{
			num = ~num;
			m_list.Insert(num, _item);
		}
	}

	public bool Contains(T _item)
	{
		return BinarySearch(_item) >= 0;
	}

	public void RemoveAt(int _index)
	{
		m_list.RemoveAt(_index);
	}

	public void Clear()
	{
		m_list.Clear();
	}

	public int GetIndexOf(T _item)
	{
		return BinarySearch(_item);
	}

	public T GetByIndex(int _index)
	{
		return m_list[_index];
	}

	private int BinarySearch(T _item)
	{
		int num = 0;
		int num2 = m_list.Count - 1;
		while (num <= num2)
		{
			int num3 = num + (num2 - num >> 1);
			int num4 = m_list[num3].CompareTo(_item);
			if (num4 == 0)
			{
				return num3;
			}
			if (num4 < 0)
			{
				num = num3 + 1;
			}
			else
			{
				num2 = num3 - 1;
			}
		}
		return ~num;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return m_list.GetEnumerator();
	}
}
