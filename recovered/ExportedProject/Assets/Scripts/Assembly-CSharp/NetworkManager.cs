using UnityEngine;

public class NetworkManager : MonoBehaviour
{
	public string m_remoteIP = "127.0.0.1";

	public int m_remotePort = 25000;

	public int m_listenPort = 25000;

	public GameObject m_networkClientPrefab;

	public TextAsset m_ghostRecording;

	private void Awake()
	{
		Network.sendRate = 30f;
	}

	private void OnGUI()
	{
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal();
		GUILayout.Space(10f);
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginVertical();
			if (GUILayout.Button("Connect"))
			{
				Network.Connect(m_remoteIP, m_remotePort);
			}
			if (GUILayout.Button("Start Server"))
			{
				Network.InitializeServer(32, m_listenPort, false);
			}
			GUILayout.EndVertical();
			m_remoteIP = GUILayout.TextField(m_remoteIP, GUILayout.MinWidth(100f));
			m_remotePort = int.Parse(GUILayout.TextField(m_remotePort.ToString()).ToString());
		}
		else
		{
			GUILayout.Label("Local IP/port: " + Network.player.ipAddress + "/" + Network.player.port);
			GUILayout.Label(" - External IP/port: " + Network.player.externalIP + "/" + Network.player.externalPort);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Disconnect"))
			{
				Network.Disconnect(200);
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private void OnServerInitialized()
	{
		Debug.Log("==> Local IP/port is " + Network.player.ipAddress + "/" + Network.player.port + ". Use this on clients to connect directly.");
		OnConnectedToServer();
	}

	private void OnPlayerConnected(NetworkPlayer player)
	{
		Debug.Log("Player connected from " + player.ipAddress + ":" + player.port);
	}

	private void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log("Player disconnected from " + player.ipAddress + ":" + player.port);
	}

	private void OnConnectedToServer()
	{
		Debug.Log("OnConnectedToServer");
		GameObject gameObject = Network.Instantiate(m_networkClientPrefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
		NetworkClient component = gameObject.GetComponent<NetworkClient>();
		if (Network.isServer)
		{
			NetworkClient.s_server = gameObject.AddComponent<NetworkServer>();
		}
		component.SpawnVehicle(Network.AllocateViewID());
	}

	private void OnDisconnectedFromServer()
	{
		Debug.Log("OnDisconnectedFromServer");
		Application.LoadLevel(Application.loadedLevel);
		NetworkClient.s_server = null;
	}
}
