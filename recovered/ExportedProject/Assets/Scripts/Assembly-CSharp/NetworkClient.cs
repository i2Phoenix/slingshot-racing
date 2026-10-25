using UnityEngine;

public class NetworkClient : MonoBehaviour
{
	public static NetworkServer s_server;

	private NetworkView m_netView;

	private void Awake()
	{
		m_netView = GetComponent<NetworkView>();
		base.enabled = false;
	}

	[RPC]
	public void SpawnVehicle(NetworkViewID viewID)
	{
		if (s_server != null)
		{
			int num = s_server.AllocatePlayer(m_netView.viewID);
			m_netView.RPC("ClientSpawnVehicle", RPCMode.AllBuffered, viewID, num);
		}
		else
		{
			m_netView.RPC("SpawnVehicle", RPCMode.Server, viewID);
		}
	}

	[RPC]
	public void ClientSpawnVehicle(NetworkViewID viewID, int playerIndex)
	{
		FinishLine finishLine = FinishLine.Get();
		Vector3 position = finishLine.transform.position;
		Quaternion rotation = finishLine.transform.rotation;
		GameObject gameObject;
		if (viewID.isMine)
		{
			Vehicle vehicle = VehicleFactory.Create(VehicleFactory.Type.Default, playerIndex, playerIndex, position, rotation);
			gameObject = vehicle.gameObject;
			GameObject gameObject2 = Object.Instantiate(FinishLine.Get().DynamicCamera) as GameObject;
			DynamicCamera component = gameObject2.GetComponent<DynamicCamera>();
			Object.Destroy(GameObject.FindWithTag("MainCamera"));
			component.TrackObject(vehicle, true);
		}
		else
		{
			gameObject = VehicleFactory.CreateGhost(playerIndex, playerIndex, position, rotation).gameObject;
		}
		NetworkView networkView = gameObject.AddComponent<NetworkView>();
		networkView.viewID = viewID;
		networkView.stateSynchronization = NetworkStateSynchronization.Off;
		if (viewID.isMine)
		{
			Vehicle component2 = gameObject.GetComponent<Vehicle>();
			if (Network.isServer)
			{
				VehicleFactory.AddAIController(component2, AIDifficulty.Hard, false);
			}
			else
			{
				VehicleController vehicleController = gameObject.AddComponent<VehicleController>();
				vehicleController.buttonName = "Grapple0";
			}
			networkView.observed = gameObject.AddComponent<LocalNetworkGhost>();
		}
		else
		{
			networkView.observed = gameObject.AddComponent<RemoteNetworkGhost>();
		}
	}
}
