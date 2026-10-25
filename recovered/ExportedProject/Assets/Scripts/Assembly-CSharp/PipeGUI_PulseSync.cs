using UnityEngine;

public class PipeGUI_PulseSync : MonoBehaviour
{
	public float m_gap;

	public PipeGUI_Pulse[] m_clients;

	public float GetDelay(int _index)
	{
		return m_gap * (float)_index;
	}

	public float GetDelay(PipeGUI_Pulse _pulse)
	{
		for (int i = 0; i < m_clients.Length; i++)
		{
			if (_pulse == m_clients[i])
			{
				return GetDelay(i);
			}
		}
		Debug.LogError("supplied object is not in the client list");
		return GetDelay(0);
	}
}
