using System.Collections.Generic;
using UnityEngine;

public class NetworkServer : MonoBehaviour
{
	private struct Player
	{
		public NetworkPlayer player;

		public NetworkViewID clientViewID;

		public int index;
	}

	private int m_playerMask;

	private List<Player> m_players = new List<Player>();

	public int AllocatePlayer(NetworkViewID clientViewID)
	{
		int num = BitUtils.LowestZeroBitIndex(m_playerMask);
		m_playerMask |= 1 << num;
		Player item = default(Player);
		item.player = clientViewID.owner;
		item.index = num;
		item.clientViewID = clientViewID;
		m_players.Add(item);
		return num;
	}

	public void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log("Player disconnected from " + player.ipAddress + ":" + player.port);
		int count = m_players.Count;
		while (count-- > 0)
		{
			if (m_players[count].player == player)
			{
				Network.RemoveRPCs(m_players[count].clientViewID);
				m_playerMask &= ~(1 << m_players[count].index);
				m_players.RemoveAt(count);
			}
		}
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
}
