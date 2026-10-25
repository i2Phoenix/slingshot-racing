using System.Collections.Generic;

public class QueueList<T>
{
	private List<T> m_list;

	public int Count
	{
		get
		{
			return m_list.Count;
		}
	}

	public QueueList()
	{
		m_list = new List<T>();
	}

	public QueueList(int _capacity)
	{
		m_list = new List<T>(_capacity);
	}

	public void Enqueue(T _t)
	{
		m_list.Add(_t);
	}

	public T Dequeue()
	{
		T result = m_list[0];
		m_list.RemoveAt(0);
		return result;
	}
}
