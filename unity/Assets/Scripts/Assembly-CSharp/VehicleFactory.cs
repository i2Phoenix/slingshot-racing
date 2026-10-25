using UnityEngine;

public class VehicleFactory
{
	public enum Type
	{
		NotSet = 0,
		Default = 1,
		Heavy = 2,
		Sport = 3
	}

	private static int[] s_vehicleStyles = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };

	public static string[] c_visualsPrefabPaths = new string[9] { "Prefabs/VehicleVisualsA", "Prefabs/VehicleVisualsB", "Prefabs/VehicleVisualsC", "Prefabs/VehicleVisualsD", "Prefabs/VehicleVisualsE", "Prefabs/VehicleVisualsF", "Prefabs/VehicleVisualsG", "Prefabs/VehicleVisualsH", "Prefabs/VehicleVisualsI" };

	public static int NumStyles
	{
		get
		{
			return c_visualsPrefabPaths.Length;
		}
	}

	public static int GetStyle(int _playerIndex)
	{
		return s_vehicleStyles[_playerIndex];
	}

	public static void SetStyle(int _playerIndex, int _style)
	{
		s_vehicleStyles[_playerIndex] = _style;
	}

	public static Vehicle Create(Type type, int _playerIndex, int _colourIndex, Vector3 position, Quaternion rotation)
	{
		if (type == Type.NotSet)
		{
			type = Type.Default;
		}
		return Create(type.ToString(), _playerIndex, _colourIndex, position, rotation);
	}

	public static string GetVisualsPrefabPath(int _playerIndex)
	{
		return c_visualsPrefabPaths[s_vehicleStyles[_playerIndex]];
	}

	public static Vehicle Create(string type, int _playerIndex, int _colourIndex, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = (GameObject)Resources.Load("Prefabs/Vehicles/" + type);
		if (gameObject == null)
		{
			Debug.LogError("No such vehicle type '" + type + "'");
			gameObject = Resources.Load("Prefabs/Vehicles/Default") as GameObject;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject, position, rotation) as GameObject;
		PhysicsVehicle component = gameObject2.GetComponent<PhysicsVehicle>();
		component.PlayerIndex = _playerIndex;
		component.ColourIndex = _colourIndex;
		GameObject original = Resources.Load(GetVisualsPrefabPath(_playerIndex)) as GameObject;
		GameObject gameObject3 = Object.Instantiate(original) as GameObject;
		gameObject3.transform.parent = component.transform;
		gameObject3.transform.localPosition = Vector3.zero;
		gameObject3.transform.localRotation = Quaternion.identity;
		component.SetVisualsGO(gameObject3);
		return component;
	}

	public static GhostVehicle CreateGhost(int _playerIndex, int _colourIndex, Vector3 _position, Quaternion _rotation)
	{
		GameObject gameObject = (GameObject)Resources.Load("Prefabs/Vehicles/Ghost");
		if (gameObject == null)
		{
			Debug.LogError("No Ghost prefab");
			return null;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject, _position, _rotation) as GameObject;
		GhostVehicle component = gameObject2.GetComponent<GhostVehicle>();
		component.PlayerIndex = _playerIndex;
		component.ColourIndex = _colourIndex;
		GameObject original = Resources.Load(GetVisualsPrefabPath(_playerIndex)) as GameObject;
		GameObject gameObject3 = Object.Instantiate(original) as GameObject;
		gameObject3.transform.parent = component.transform;
		gameObject3.transform.localPosition = Vector3.zero;
		gameObject3.transform.localRotation = Quaternion.identity;
		return component;
	}

	public static void AddHumanController(Vehicle vehicle, TouchRegion touchRegion, string _buttonOverride)
	{
		PersistentState persistentState = PersistentState.Get();
		if (persistentState != null && persistentState.m_aiControlHumans)
		{
			AddAIController(vehicle, persistentState.aiDifficulty, false);
		}
		else
		{
			VehicleController vehicleController = vehicle.gameObject.AddComponent<VehicleController>();
			vehicleController.buttonName = ((_buttonOverride == null || _buttonOverride.Length <= 0) ? ("Grapple" + vehicle.PlayerIndex) : _buttonOverride);
			vehicleController.touch = touchRegion;
		}
		vehicle.EnableFrustumCulling = false;
	}

	public static void AddAIController(Vehicle vehicle, AIDifficulty _difficulty, bool _noMistakes)
	{
		VehicleAIController vehicleAIController = vehicle.gameObject.AddComponent<VehicleAIController>();
		vehicleAIController.m_difficulty = _difficulty;
		vehicleAIController.DynamicDifficulty = true;
		vehicleAIController.NoMistakes = _noMistakes;
		vehicle.EnableFrustumCulling = true;
	}

	public static void AddGhostController(Vehicle vehicle)
	{
		vehicle.gameObject.AddComponent<GhostPlayer>();
	}

	public static void AddGhostRecorder(Vehicle vehicle)
	{
		vehicle.gameObject.AddComponent<GhostRecorder>();
	}
}
