using UnityEngine;

public class GlobalSave : CloudSave
{
	private const int c_saveVersion = 1;

	private const string c_filename = "GameData.gsv";

	private const string c_saveDataTagName = "SaveData";

	private const string c_vehicleStylePropertyName = "VehicleStyle";

	private const string c_pickupsStrategyGuidePropertyName = "PickupsStrategyGuide";

	private const string c_timeTrialStrategyGuidePropertyName = "TimeTrialStrategyGuide";

	private const string c_deviceIDPropertyName = "DeviceID";

	private static GlobalSave s_instance;

	private bool[] m_vehicleStyles = new bool[9] { true, true, true, true, false, false, false, false, false };

	private bool m_pickupsStrategyGuide;

	private bool m_timeTrialStrategyGuide;

	protected override int SaveVersion
	{
		get
		{
			return 1;
		}
	}

	protected override string Filename
	{
		get
		{
			return "GameData.gsv";
		}
	}

	protected override bool ReadyForCloudSync
	{
		get
		{
			return true;
		}
	}

	private static GlobalSave Instance
	{
		get
		{
			if (s_instance == null)
			{
				GameObject gameObject = new GameObject("__GlobalSave");
				gameObject.AddComponent<GlobalSave>();
				Object.DontDestroyOnLoad(gameObject);
			}
			return s_instance;
		}
	}

	public static void DoCloudSync()
	{
		Instance.CloudSync();
	}

	public static bool IsVehicleStyleUnlocked(int _style)
	{
		return Instance.m_vehicleStyles[_style];
	}

	public static void UnlockVehicleStyle(int _style)
	{
		Instance.m_vehicleStyles[_style] = true;
		Instance.Save();
	}

	public static void RevokeVehicleStyle(int _style)
	{
		Instance.m_vehicleStyles[_style] = false;
		Instance.Save();
	}

	public static bool IsPickupsStrategyGuideUnlocked()
	{
		return Instance.m_pickupsStrategyGuide;
	}

	public static void UnlockPickupsStrategyGuide()
	{
		Instance.m_pickupsStrategyGuide = true;
		Instance.Save();
	}

	public static bool IsTimeTrialStrategyGuideUnlocked()
	{
		return Instance.m_timeTrialStrategyGuide;
	}

	public static void UnlockTimeTrialStrategyGuide()
	{
		Instance.m_timeTrialStrategyGuide = true;
		Instance.Save();
	}

	public static void StartCloudDownload()
	{
		if (CloudUtils.DocumentStoreAvailable)
		{
			CloudUtils.StartDownload("GameData.gsv");
		}
	}

	protected override void Awake()
	{
		base.Awake();
		s_instance = this;
		Load();
	}

	protected override void OnDestroy()
	{
		Save();
		s_instance = null;
		base.OnDestroy();
	}

	protected override void OnSaveDataUpdated()
	{
	}

	protected override LoadStatus Load(ReadSerialiser _serialiser, bool _additive)
	{
		SerialiseNode o_node;
		if (!SerialiseNode.ReadGraph(_serialiser, out o_node))
		{
			Debug.LogError("GlobalSave: Unable to load data");
			return LoadStatus.Corrupted;
		}
		if (!_serialiser.EndOfSerialise())
		{
			Debug.LogError("GlobalSave: Failed to end load");
			return LoadStatus.Corrupted;
		}
		ReadPropertySerialiser readProperties = o_node.ReadProperties;
		string o_value;
		if (!_additive && readProperties.TryRead("DeviceID", out o_value) && SystemInfo.deviceUniqueIdentifier != o_value)
		{
			return LoadStatus.Failed;
		}
		for (int i = 0; i < m_vehicleStyles.Length; i++)
		{
			string key = "VehicleStyle" + i;
			bool o_value2;
			if (readProperties.TryRead(key, out o_value2))
			{
				if (_additive)
				{
					m_vehicleStyles[i] |= o_value2;
				}
				else
				{
					m_vehicleStyles[i] = o_value2;
				}
			}
		}
		bool o_value3;
		if (readProperties.TryRead("PickupsStrategyGuide", out o_value3))
		{
			m_pickupsStrategyGuide = ((!_additive) ? o_value3 : (o_value3 | m_pickupsStrategyGuide));
		}
		bool o_value4;
		if (readProperties.TryRead("TimeTrialStrategyGuide", out o_value4))
		{
			m_timeTrialStrategyGuide = ((!_additive) ? o_value4 : (o_value4 | m_timeTrialStrategyGuide));
		}
		return LoadStatus.Success;
	}

	protected override bool Save(WriteSerialiser _serialiser)
	{
		SerialiseNodeWriter serialiseNodeWriter = new SerialiseNodeWriter(_serialiser);
		if (!serialiseNodeWriter.BeginGraph())
		{
			Debug.LogError("GlobalSave: Failed to begin save data");
			return false;
		}
		serialiseNodeWriter.BeginNode("SaveData");
		WritePropertySerialiser o_writePropertiesSerialiser;
		serialiseNodeWriter.BeginProperties(out o_writePropertiesSerialiser);
		o_writePropertiesSerialiser.TryWrite("DeviceID", SystemInfo.deviceUniqueIdentifier);
		for (int i = 0; i < m_vehicleStyles.Length; i++)
		{
			string key = "VehicleStyle" + i;
			o_writePropertiesSerialiser.TryWrite(key, m_vehicleStyles[i]);
		}
		o_writePropertiesSerialiser.TryWrite("PickupsStrategyGuide", m_pickupsStrategyGuide);
		o_writePropertiesSerialiser.TryWrite("TimeTrialStrategyGuide", m_timeTrialStrategyGuide);
		serialiseNodeWriter.EndProperties();
		serialiseNodeWriter.EndNode();
		if (!serialiseNodeWriter.EndGraph())
		{
			Debug.LogError("GlobalSave: Failed to end save data");
			return false;
		}
		return true;
	}
}
