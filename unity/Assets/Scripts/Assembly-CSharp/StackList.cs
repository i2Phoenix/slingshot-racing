using System.Collections.Generic;

public class StackList<T>
{
	private List<T> m_list;

	public int Count
	{
		get
		{
			return m_list.Count;
		}
	}

	public StackList()
	{
		m_list = new List<T>();
	}

	public StackList(int _capacity)
	{
		m_list = new List<T>(_capacity);
	}

	public void Push(T _item)
	{
		m_list.Add(_item);
	}

	public T Pop()
	{
		int index = m_list.Count - 1;
		T result = m_list[index];
		m_list.RemoveAt(index);
		return result;
	}

	public T Peek()
	{
		int index = m_list.Count - 1;
		return m_list[index];
	}
}
