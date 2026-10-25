using System;
using System.Collections;
using System.Collections.Generic;

public class MapList<TKey, TValue> : IEnumerable<MapList<TKey, TValue>.Item>, IEnumerable where TKey : IComparable<TKey>
{
	public class Item
	{
		private readonly TKey m_key;

		private readonly TValue m_value;

		public TKey Key
		{
			get
			{
				return m_key;
			}
		}

		public TValue Value
		{
			get
			{
				return m_value;
			}
		}

		public Item(TKey _key, TValue _value)
		{
			m_key = _key;
			m_value = _value;
		}
	}

	private List<Item> m_list;

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

	public MapList()
	{
		m_list = new List<Item>();
	}

	public MapList(int _capacity)
	{
		m_list = new List<Item>(_capacity);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool TryAdd(TKey _key, TValue _value)
	{
		int num = BinarySearch(_key);
		if (num < 0)
		{
			num = ~num;
			m_list.Insert(num, new Item(_key, _value));
			return true;
		}
		return false;
	}

	public bool TryRemove(TKey _key)
	{
		int num = BinarySearch(_key);
		if (num < 0)
		{
			return false;
		}
		m_list.RemoveAt(num);
		return true;
	}

	public bool TryGetValue(TKey _key, out TValue o_value)
	{
		int num = BinarySearch(_key);
		if (num < 0)
		{
			o_value = default(TValue);
			return false;
		}
		o_value = m_list[num].Value;
		return true;
	}

	public void Add(TKey _key, TValue _value)
	{
		int num = BinarySearch(_key);
		if (num < 0)
		{
			num = ~num;
			m_list.Insert(num, new Item(_key, _value));
		}
	}

	public void RemoveAt(int _index)
	{
		m_list.RemoveAt(_index);
	}

	public void Clear()
	{
		m_list.Clear();
	}

	public int GetIndexOf(TKey _key)
	{
		return BinarySearch(_key);
	}

	public TKey GetKeyByIndex(int _index)
	{
		return m_list[_index].Key;
	}

	public TValue GetValueByIndex(int _index)
	{
		return m_list[_index].Value;
	}

	public void SetValueByIndex(int _index, TValue _value)
	{
		Item value = new Item(m_list[_index].Key, _value);
		m_list[_index] = value;
	}

	private int BinarySearch(TKey _key)
	{
		int num = 0;
		int num2 = m_list.Count - 1;
		while (num <= num2)
		{
			int num3 = num + (num2 - num >> 1);
			int num4 = m_list[num3].Key.CompareTo(_key);
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

	public IEnumerator<Item> GetEnumerator()
	{
		return m_list.GetEnumerator();
	}
}
