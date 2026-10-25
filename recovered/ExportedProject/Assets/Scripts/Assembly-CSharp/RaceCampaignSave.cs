using System;
using System.IO;
using UnityEngine;

[AddComponentMenu("Gameplay/Race Campaign Save")]
public class RaceCampaignSave : CloudSave
{
	public interface IManager
	{
		void OnUpdateSaveData(RaceCampaignSave _save);

		int GetTotalPointsScored(RaceCampaignSave _save);

		int GetTotalPointsAvailable(RaceCampaignSave _save);
	}

	public enum AwardType
	{
		None = 0,
		Bronze = 1,
		Silver = 2,
		Gold = 3,
		Platinum = 4
	}

	[Flags]
	public enum UnlockFlags
	{
		Race = 1,
		Tour = 2
	}

	public struct VehicleRaceSummary
	{
		private bool[] m_bonusObjectives;

		public int FinishPosition { get; set; }

		public float BestLap { get; set; }

		public float TotalTime { get; set; }

		public void SetBonusObjectiveCompleted(int _index, bool _completed)
		{
			if (m_bonusObjectives == null)
			{
				m_bonusObjectives = new bool[MaxBonusObjectives];
			}
			m_bonusObjectives[_index] = _completed;
		}

		public bool GetBonusObjectiveCompleted(int _index)
		{
			return m_bonusObjectives != null && m_bonusObjectives[_index];
		}
	}

	public class RaceStatsImproved
	{
		private bool[] m_bonusObjectives;

		public bool ImprovedPosition { get; set; }

		public bool ImprovedBestLap { get; set; }

		public bool ImprovedTotalTime { get; set; }

		public void SetBonusObjectiveImproved(int _index, bool _completed)
		{
			if (m_bonusObjectives == null)
			{
				m_bonusObjectives = new bool[MaxBonusObjectives];
			}
			m_bonusObjectives[_index] = _completed;
		}

		public bool GetBonusObjectiveImproved(int _index)
		{
			return m_bonusObjectives != null && m_bonusObjectives[_index];
		}
	}

	public class RaceEventSaveData
	{
		private RaceCampaignSave m_owner;

		private RaceEvent m_race;

		private bool m_locked = true;

		private int m_bestPosition = MaxPosition;

		private int m_bonusTasks;

		private float m_bestLapTime = MaxTime;

		private float m_bestTotalTime = MaxTime;

		private RaceCampaign.EventUnlockType m_unlockType;

		public bool Locked
		{
			get
			{
				return (m_owner == null || !m_owner.LocksDisabled) && m_locked;
			}
			set
			{
				m_locked = value;
				if (!value)
				{
					m_unlockType = RaceCampaign.EventUnlockType.None;
				}
			}
		}

		public RaceCampaign.EventUnlockType LockReason
		{
			get
			{
				return m_unlockType;
			}
			set
			{
				m_unlockType = value;
			}
		}

		public float BestLapTime
		{
			get
			{
				return m_bestLapTime;
			}
		}

		public float BestTotalTime
		{
			get
			{
				return m_bestTotalTime;
			}
		}

		public int BestPosition
		{
			get
			{
				return GetPosition(m_bestPosition, m_bestTotalTime, m_bestLapTime);
			}
		}

		public RaceEvent Race
		{
			get
			{
				return m_race;
			}
		}

		public int NumBonusObjectives
		{
			get
			{
				return (m_race.m_bonusObjectives != null) ? m_race.m_bonusObjectives.Length : 0;
			}
		}

		public bool AllBonusTasksComplete
		{
			get
			{
				return true;
			}
		}

		public int NumBonusObjectivesComplete
		{
			get
			{
				return 0;
			}
		}

		public AwardType Award
		{
			get
			{
				switch (BestPosition)
				{
				case 1:
					return AwardType.Gold;
				case 2:
					return AwardType.Silver;
				case 3:
					return AwardType.Bronze;
				default:
					return AwardType.None;
				}
			}
		}

		public int PointsScored
		{
			get
			{
				return RaceCampaign.GetPointsForPosition(BestPosition);
			}
		}

		public float PercentageComplete
		{
			get
			{
				switch (BestPosition)
				{
				case 1:
					return 100f;
				case 2:
					return 66f;
				case 3:
					return 33f;
				default:
					return 0f;
				}
			}
		}

		public bool Played
		{
			get
			{
				return m_bestTotalTime < MaxTimeThreshold;
			}
		}

		public RaceEventSaveData Clone()
		{
			return MemberwiseClone() as RaceEventSaveData;
		}

		public void InitSaveData(RaceCampaignSave _owner, RaceEvent _event)
		{
			m_owner = _owner;
			m_race = _event;
		}

		public void SerialiseProperties(IPropertySerialiser _serialiser)
		{
			_serialiser.TrySerialise("BestPosition", ref m_bestPosition);
			_serialiser.TrySerialise("BestLapTime", ref m_bestLapTime);
			_serialiser.TrySerialise("BestTotalTime", ref m_bestTotalTime);
		}

		public void LoadPropertiesAdditive(ReadPropertySerialiser _serialiser)
		{
			int o_value;
			if (_serialiser.TryRead("BestPosition", out o_value))
			{
				TryUpdatePosition(o_value);
			}
			float o_value2;
			if (_serialiser.TryRead("BestLapTime", out o_value2))
			{
				TryUpdateBestLapTime(o_value2);
			}
			float o_value3;
			if (_serialiser.TryRead("BestTotalTime", out o_value3))
			{
				TryUpdateBestTotalTime(o_value3);
			}
		}

		private bool TryUpdatePosition(int _position)
		{
			if (_position > 0 && _position < m_bestPosition)
			{
				m_bestPosition = _position;
				return true;
			}
			return false;
		}

		private bool TryUpdateBestLapTime(float _bestLapTime)
		{
			if (_bestLapTime > 0f && _bestLapTime < m_bestLapTime)
			{
				m_bestLapTime = _bestLapTime;
				return true;
			}
			return false;
		}

		private bool TryUpdateBestTotalTime(float _bestTotalTime)
		{
			if (_bestTotalTime > 0f && _bestTotalTime < MaxTimeThreshold)
			{
				bool invTotalTime = m_race.InvTotalTime;
				if ((_bestTotalTime < m_bestTotalTime && !invTotalTime) || (_bestTotalTime > m_bestTotalTime && invTotalTime) || m_bestTotalTime >= MaxTimeThreshold)
				{
					m_bestTotalTime = _bestTotalTime;
					return true;
				}
			}
			return false;
		}

		public RaceStatsImproved UpdateSave(VehicleRaceSummary _summary)
		{
			RaceStatsImproved raceStatsImproved = new RaceStatsImproved();
			raceStatsImproved.ImprovedPosition = TryUpdatePosition(_summary.FinishPosition);
			raceStatsImproved.ImprovedBestLap = TryUpdateBestLapTime(_summary.BestLap);
			raceStatsImproved.ImprovedTotalTime = TryUpdateBestTotalTime(_summary.TotalTime);
			return raceStatsImproved;
		}

		public bool GetBonusTaskComplete(int _index)
		{
			return (m_bonusTasks & (1 << _index)) != 0;
		}

		public void SetBonusTaskComplete(int _index, bool _completed)
		{
			if (_completed)
			{
				m_bonusTasks |= 1 << _index;
			}
		}

		public int GetPosition(int _position, float _raceTime, float _bestLapTime)
		{
			switch (m_race.AwardPointsType)
			{
			default:
				return _position;
			case RaceEvent.PointsType.BestLap:
				return GetPositionForTime(_bestLapTime);
			case RaceEvent.PointsType.TotalTime:
				return GetPositionForTime(_raceTime);
			case RaceEvent.PointsType.InvTotalTime:
				return GetPositionForInvTime(_raceTime);
			case RaceEvent.PointsType.None:
				return 100;
			}
		}

		public AwardType GetAwardType(int _position, float _raceTime, float _bestLapTime)
		{
			switch (GetPosition(_position, _raceTime, _bestLapTime))
			{
			case 1:
				return AwardType.Gold;
			case 2:
				return AwardType.Silver;
			case 3:
				return AwardType.Bronze;
			default:
				return AwardType.None;
			}
		}

		private int GetPositionForTime(float _time)
		{
			for (int i = 0; i < m_race.m_times.Length; i++)
			{
				if (_time <= m_race.m_times[i])
				{
					return i + 1;
				}
			}
			return m_race.m_times.Length + 1;
		}

		private int GetPositionForInvTime(float _time)
		{
			if (_time < MaxTimeThreshold)
			{
				for (int i = 0; i < m_race.m_times.Length; i++)
				{
					if (_time >= m_race.m_times[i])
					{
						return i + 1;
					}
				}
			}
			return m_race.m_times.Length + 1;
		}
	}

	public class RaceTourSaveData
	{
		private RaceTour m_raceTour;

		private MapList<string, RaceEventSaveData> m_raceSaves;

		private bool m_locked = true;

		private RaceCampaignSave m_owner;

		public RaceTour Tour
		{
			get
			{
				return m_raceTour;
			}
		}

		public bool NewUnlock { get; set; }

		public int PointsScored { get; private set; }

		public int PointsAvailable
		{
			get
			{
				int num = 0;
				if (!Locked)
				{
					RaceEvent[] races = m_raceTour.m_races;
					foreach (RaceEvent raceEvent in races)
					{
						if (raceEvent.AwardPointsType != RaceEvent.PointsType.None)
						{
							num += 3;
						}
					}
				}
				return num;
			}
		}

		public bool Locked
		{
			get
			{
				return (m_owner == null || !m_owner.LocksDisabled) && m_locked;
			}
		}

		public AwardType Award
		{
			get
			{
				AwardType awardType = AwardType.Gold;
				foreach (MapList<string, RaceEventSaveData>.Item raceSafe in m_raceSaves)
				{
					RaceEventSaveData value = raceSafe.Value;
					awardType = (AwardType)Math.Min((int)awardType, (int)value.Award);
				}
				return awardType;
			}
		}

		public float PercentageComplete
		{
			get
			{
				float num = 0f;
				foreach (MapList<string, RaceEventSaveData>.Item raceSafe in m_raceSaves)
				{
					RaceEventSaveData value = raceSafe.Value;
					num += value.PercentageComplete;
				}
				return num / (float)m_raceSaves.Count;
			}
		}

		public bool Played
		{
			get
			{
				foreach (MapList<string, RaceEventSaveData>.Item raceSafe in m_raceSaves)
				{
					RaceEventSaveData value = raceSafe.Value;
					if (value.Played)
					{
						return true;
					}
				}
				return false;
			}
		}

		public RaceTourSaveData Clone()
		{
			RaceTourSaveData raceTourSaveData = new RaceTourSaveData();
			raceTourSaveData.m_raceTour = m_raceTour;
			raceTourSaveData.m_locked = m_locked;
			raceTourSaveData.m_owner = m_owner;
			raceTourSaveData.m_raceSaves = new MapList<string, RaceEventSaveData>(m_raceSaves.Count);
			foreach (MapList<string, RaceEventSaveData>.Item raceSafe in m_raceSaves)
			{
				raceTourSaveData.m_raceSaves.Add(raceSafe.Key, raceSafe.Value.Clone());
			}
			return raceTourSaveData;
		}

		public RaceEventSaveData GetSaveData(RaceEvent _event)
		{
			RaceEventSaveData o_value = null;
			m_raceSaves.TryGetValue(_event.name, out o_value);
			return o_value;
		}

		public void InitSaveData(RaceCampaignSave _owner, RaceTour _tour)
		{
			m_raceTour = _tour;
			m_owner = _owner;
			if (m_raceSaves == null)
			{
				m_raceSaves = new MapList<string, RaceEventSaveData>(_tour.m_races.Length);
			}
			RaceEvent[] races = _tour.m_races;
			foreach (RaceEvent raceEvent in races)
			{
				RaceEventSaveData o_value = null;
				if (!m_raceSaves.TryGetValue(raceEvent.name, out o_value))
				{
					o_value = new RaceEventSaveData();
					m_raceSaves.Add(raceEvent.name, o_value);
				}
				o_value.InitSaveData(_owner, raceEvent);
			}
		}

		public void Save(SerialiseNodeWriter _nodeWriter)
		{
			foreach (MapList<string, RaceEventSaveData>.Item raceSafe in m_raceSaves)
			{
				_nodeWriter.BeginNode("RaceData");
				WritePropertySerialiser o_writePropertiesSerialiser;
				_nodeWriter.BeginProperties(out o_writePropertiesSerialiser);
				o_writePropertiesSerialiser.TryWrite("Name", raceSafe.Key);
				raceSafe.Value.SerialiseProperties(o_writePropertiesSerialiser);
				_nodeWriter.EndProperties();
				_nodeWriter.EndNode();
			}
		}

		public void Load(SerialiseNode _node, bool _additive)
		{
			foreach (SerialiseNode child in _node.Children)
			{
				if (child.Tag == "RaceData")
				{
					string key = child.Properties["Name"];
					RaceEventSaveData o_value;
					if (m_raceSaves.TryGetValue(key, out o_value))
					{
						if (_additive)
						{
							o_value.LoadPropertiesAdditive(child.ReadProperties);
						}
						else
						{
							o_value.SerialiseProperties(child.ReadProperties);
						}
					}
				}
				else
				{
					Debug.LogError(string.Format("RCSave: Unknown tag '{0}'", child.Tag));
				}
			}
		}

		public void UpdatePointsScored()
		{
			int num = 0;
			foreach (MapList<string, RaceEventSaveData>.Item raceSafe in m_raceSaves)
			{
				RaceEventSaveData value = raceSafe.Value;
				num += value.PointsScored;
			}
			PointsScored = num;
		}

		public UnlockFlags UpdateUnlocks(int _totalPoints, RaceCampaign.EventUnlockType _unlockType)
		{
			UnlockFlags unlockFlags = (UnlockFlags)0;
			if (_totalPoints >= m_raceTour.m_pointsToUnlock)
			{
				if (Locked)
				{
					unlockFlags |= UnlockFlags.Tour;
					NewUnlock = true;
				}
				m_locked = false;
				if (_unlockType == RaceCampaign.EventUnlockType.None || m_raceTour.m_races.Length == 1)
				{
					foreach (MapList<string, RaceEventSaveData>.Item raceSafe in m_raceSaves)
					{
						RaceEventSaveData value = raceSafe.Value;
						if (value.Locked)
						{
							unlockFlags |= UnlockFlags.Race;
						}
						value.Locked = false;
					}
				}
				else if (_unlockType == RaceCampaign.EventUnlockType.LockLast)
				{
					int num = 0;
					for (int i = 0; i < m_raceTour.m_races.Length - 1; i++)
					{
						RaceEventSaveData saveData = GetSaveData(m_raceTour.m_races[i]);
						if (saveData.Locked)
						{
							unlockFlags |= UnlockFlags.Race;
						}
						saveData.Locked = false;
						num = Math.Max(num, saveData.BestPosition);
					}
					RaceEventSaveData saveData2 = GetSaveData(m_raceTour.m_races[m_raceTour.m_races.Length - 1]);
					if (num <= 3)
					{
						if (saveData2.Locked)
						{
							unlockFlags |= UnlockFlags.Race;
						}
						saveData2.Locked = false;
					}
					else
					{
						saveData2.LockReason = _unlockType;
					}
				}
			}
			return unlockFlags;
		}
	}

	private const string c_saveDataTagName = "SaveData";

	private const string c_tourDataTagName = "TourData";

	private const string c_raceDataTagName = "RaceData";

	private const string c_namePropertyName = "Name";

	private const string c_totalNutsPropertyName = "TotalNuts";

	private const string c_disabledLocksPropertyName = "DisableLocks";

	private const string c_bestPositionPropertyName = "BestPosition";

	private const string c_bonusTasksPropertyName = "BonusTasks";

	private const string c_bestLapTimePropertyName = "BestLapTime";

	private const string c_bestTotalTimePropertyName = "BestTotalTime";

	private const int c_saveVersion = 1;

	private RaceCampaign m_campaign;

	private IManager m_manager;

	private string m_savedGameFilename;

	private MapList<string, RaceTourSaveData> m_tourSaves;

	private long m_totalNuts;

	private bool m_disableLocks;

	private static bool s_ignoreLocks;

	public static int MaxPosition
	{
		get
		{
			return 100;
		}
	}

	public static float MaxTime
	{
		get
		{
			return 100000f;
		}
	}

	public static float MaxTimeThreshold
	{
		get
		{
			return MaxTime - 1f;
		}
	}

	public static int MaxBonusObjectives
	{
		get
		{
			return 6;
		}
	}

	public bool LocksDisabled
	{
		get
		{
			return s_ignoreLocks || m_disableLocks;
		}
		set
		{
			m_disableLocks = value;
		}
	}

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
			return m_savedGameFilename;
		}
	}

	protected override bool ReadyForCloudSync
	{
		get
		{
			return m_campaign != null;
		}
	}

	public long TotalNuts
	{
		get
		{
			return m_totalNuts;
		}
	}

	public int TotalPointsScored
	{
		get
		{
			return (m_manager == null) ? PointsScored : m_manager.GetTotalPointsScored(this);
		}
	}

	public int TotalPointsAvailable
	{
		get
		{
			return (m_manager == null) ? PointsAvailable : m_manager.GetTotalPointsAvailable(this);
		}
	}

	public int PointsScored { get; private set; }

	public int PointsAvailable { get; private set; }

	public RaceCampaign Campaign
	{
		get
		{
			return m_campaign;
		}
	}

	public bool ContributesTotalNuts
	{
		get
		{
			return Campaign.m_type == RaceCampaign.Type.SinglePlayer;
		}
	}

	protected override LoadStatus Load(ReadSerialiser _serialiser, bool _additive)
	{
		SerialiseNode o_node;
		if (!SerialiseNode.ReadGraph(_serialiser, out o_node))
		{
			Debug.LogError("RCSave: Unable to load data");
			return LoadStatus.Corrupted;
		}
		if (!_serialiser.EndOfSerialise())
		{
			Debug.LogError("RCSave: Failed to end load");
			return LoadStatus.Corrupted;
		}
		ReadPropertySerialiser readProperties = o_node.ReadProperties;
		long o_value;
		if (readProperties.TryRead("TotalNuts", out o_value))
		{
			if (_additive)
			{
				m_totalNuts = Math.Max(m_totalNuts, o_value);
			}
			else
			{
				m_totalNuts = o_value;
			}
		}
		bool o_value2;
		if (readProperties.TryRead("DisableLocks", out o_value2) && !_additive)
		{
			m_disableLocks = o_value2;
		}
		foreach (SerialiseNode child in o_node.Children)
		{
			if (child.Tag == "TourData")
			{
				string key = child.Properties["Name"];
				RaceTourSaveData o_value3;
				if (m_tourSaves.TryGetValue(key, out o_value3))
				{
					o_value3.Load(child, _additive);
				}
			}
			else
			{
				Debug.LogError(string.Format("RCSave: Unknown tag '{0}'", child.Tag));
			}
		}
		return LoadStatus.Success;
	}

	protected override bool Save(WriteSerialiser _serialiser)
	{
		SerialiseNodeWriter serialiseNodeWriter = new SerialiseNodeWriter(_serialiser);
		if (!serialiseNodeWriter.BeginGraph())
		{
			Debug.LogError("RCSave: Failed to begin save data");
			return false;
		}
		serialiseNodeWriter.BeginNode("SaveData");
		WritePropertySerialiser o_writePropertiesSerialiser;
		serialiseNodeWriter.BeginProperties(out o_writePropertiesSerialiser);
		o_writePropertiesSerialiser.TryWrite("TotalNuts", m_totalNuts);
		o_writePropertiesSerialiser.TryWrite("DisableLocks", m_disableLocks);
		serialiseNodeWriter.EndProperties();
		foreach (MapList<string, RaceTourSaveData>.Item tourSafe in m_tourSaves)
		{
			serialiseNodeWriter.BeginNode("TourData");
			WritePropertySerialiser o_writePropertiesSerialiser2;
			serialiseNodeWriter.BeginProperties(out o_writePropertiesSerialiser2);
			o_writePropertiesSerialiser2.TryWrite("Name", tourSafe.Key);
			serialiseNodeWriter.EndProperties();
			tourSafe.Value.Save(serialiseNodeWriter);
			serialiseNodeWriter.EndNode();
		}
		serialiseNodeWriter.EndNode();
		if (!serialiseNodeWriter.EndGraph())
		{
			Debug.LogError("RCSave: Failed to end save data");
			return false;
		}
		return true;
	}

	public static void StartCloudDownloadForCampaign(RaceCampaign _campaign)
	{
		if (CloudUtils.DocumentStoreAvailable)
		{
			string text = CalculateSavedGameFilename(_campaign);
			Debug.Log(string.Format("RCSave: Starting campaign download {0}", text));
			CloudUtils.StartDownload(text);
		}
	}

	public static void DeleteSaveForCampaign(RaceCampaign _campaign)
	{
		string text = CalculateSavedGameFilename(_campaign);
		Debug.Log(string.Format("RCSave: Deleting save {0}", text));
		string path = CloudUtils.LocalFilePath(text);
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		CloudUtils.EvictCloudFile(text);
	}

	private static string CalculateSavedGameFilename(RaceCampaign _campaign)
	{
		return string.Format("{0}.rcsv", _campaign.name);
	}

	public static void DisableAllLocks()
	{
		s_ignoreLocks = true;
	}

	public void SetCampaign(RaceCampaign _campaign, IManager _manager)
	{
		SetCampaign(_campaign, _manager, true);
	}

	public void SetCampaign(RaceCampaign _campaign, IManager _manager, bool _clearNewUnlocks)
	{
		m_campaign = _campaign;
		m_tourSaves = null;
		m_manager = _manager;
		m_savedGameFilename = CalculateSavedGameFilename(m_campaign);
		Debug.Log("Game saved in: " + m_savedGameFilename);
		InitSaveData();
		Load();
		UpdateSaveData();
		if (!_clearNewUnlocks)
		{
			return;
		}
		foreach (MapList<string, RaceTourSaveData>.Item tourSafe in m_tourSaves)
		{
			RaceTourSaveData value = tourSafe.Value;
			value.NewUnlock = false;
		}
	}

	public void Reload()
	{
		m_tourSaves = null;
		long totalNuts = m_totalNuts;
		m_disableLocks = false;
		InitSaveData();
		Load();
		m_totalNuts = totalNuts;
		UpdateSaveData();
		if (1 == 0)
		{
			return;
		}
		foreach (MapList<string, RaceTourSaveData>.Item tourSafe in m_tourSaves)
		{
			RaceTourSaveData value = tourSafe.Value;
			value.NewUnlock = false;
		}
	}

	private void InitSaveData()
	{
		if (m_tourSaves == null)
		{
			m_tourSaves = new MapList<string, RaceTourSaveData>(m_campaign.m_tours.Length);
		}
		RaceTour[] tours = m_campaign.m_tours;
		foreach (RaceTour raceTour in tours)
		{
			RaceTourSaveData o_value = null;
			if (!m_tourSaves.TryGetValue(raceTour.name, out o_value))
			{
				o_value = new RaceTourSaveData();
				m_tourSaves.Add(raceTour.name, o_value);
			}
			o_value.InitSaveData(this, raceTour);
		}
	}

	protected override void OnSaveDataUpdated()
	{
		UpdateSaveData();
	}

	public UnlockFlags UpdateSaveData()
	{
		UnlockFlags result = UpdateSaveDataManaged(false);
		if (m_manager != null)
		{
			m_manager.OnUpdateSaveData(this);
		}
		return result;
	}

	public UnlockFlags UpdateSaveDataManaged(bool _clearNewUnlocks)
	{
		UnlockFlags unlockFlags = (UnlockFlags)0;
		int num = 0;
		int num2 = 0;
		foreach (MapList<string, RaceTourSaveData>.Item tourSafe in m_tourSaves)
		{
			RaceTourSaveData value = tourSafe.Value;
			value.UpdatePointsScored();
			num += value.PointsScored;
		}
		PointsScored = num;
		num = ((m_manager == null) ? num : m_manager.GetTotalPointsScored(this));
		foreach (MapList<string, RaceTourSaveData>.Item tourSafe2 in m_tourSaves)
		{
			RaceTourSaveData value2 = tourSafe2.Value;
			unlockFlags |= value2.UpdateUnlocks(num, m_campaign.m_eventUnlockType);
			num2 += value2.PointsAvailable;
		}
		PointsAvailable = num2;
		Save();
		if (_clearNewUnlocks)
		{
			foreach (MapList<string, RaceTourSaveData>.Item tourSafe3 in m_tourSaves)
			{
				RaceTourSaveData value3 = tourSafe3.Value;
				value3.NewUnlock = false;
			}
		}
		return unlockFlags;
	}

	public RaceEventSaveData GetSaveData(RaceTour _tour, RaceEvent _event)
	{
		RaceTourSaveData saveData = GetSaveData(_tour);
		if (saveData != null)
		{
			return saveData.GetSaveData(_event);
		}
		return null;
	}

	public RaceTourSaveData GetSaveData(RaceTour _tour)
	{
		RaceTourSaveData o_value = null;
		m_tourSaves.TryGetValue(_tour.name, out o_value);
		return o_value;
	}

	public RaceTour GetHighestUnlockedTour()
	{
		RaceTour result = null;
		RaceTour[] tours = m_campaign.m_tours;
		foreach (RaceTour raceTour in tours)
		{
			if (GetSaveData(raceTour).Locked)
			{
				break;
			}
			result = raceTour;
		}
		return result;
	}

	public bool UpdateTotalNuts(long _numNuts)
	{
		if (_numNuts <= 0)
		{
			return false;
		}
		m_totalNuts += _numNuts;
		return true;
	}
}
