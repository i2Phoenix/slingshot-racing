using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class TurnBasedMatchData : ITBTournament
{
	public enum TBRaceState
	{
		InActive = 0,
		Waiting = 1,
		Completed = 2
	}

	public class RaceEventData : ITBRace
	{
		private readonly TurnBasedMatchData m_matchData;

		private readonly string m_raceEventName;

		private int m_numLaps;

		private TBRaceState m_tbRaceState;

		private bool m_reversed;

		private RaceLayout.Variant m_variant;

		private List<PlayerResultsData> m_results;

		public string RaceEventName
		{
			get
			{
				return m_raceEventName;
			}
		}

		public int NumLaps
		{
			get
			{
				return m_numLaps;
			}
			set
			{
				m_numLaps = value;
			}
		}

		public bool Reversed
		{
			get
			{
				return m_reversed;
			}
			set
			{
				m_reversed = value;
			}
		}

		public RaceLayout.Variant Variant
		{
			get
			{
				return m_variant;
			}
			set
			{
				m_variant = value;
			}
		}

		public TurnBasedRaceState RaceState
		{
			get
			{
				switch (m_tbRaceState)
				{
				default:
					return TurnBasedRaceState.InActive;
				case TBRaceState.Waiting:
					return TurnBasedRaceState.WaitingLocalPlayer;
				case TBRaceState.Completed:
				{
					if (m_results.Count == 0)
					{
						return TurnBasedRaceState.Completed;
					}
					for (int i = 0; i < m_results.Count; i++)
					{
						if (m_results[i].GhostDataSize == 0)
						{
							return TurnBasedRaceState.Completed;
						}
					}
					return TurnBasedRaceState.CompleteWithGhost;
				}
				}
			}
		}

		public TBRaceState TBRaceState
		{
			get
			{
				return m_tbRaceState;
			}
			set
			{
				m_tbRaceState = value;
			}
		}

		public int NumPlayers
		{
			get
			{
				return m_matchData.NumPlayers;
			}
		}

		public ITBPlayer LocalPlayer
		{
			get
			{
				return m_matchData.LocalPlayer;
			}
		}

		public ITBPlayer CurrentPlayer
		{
			get
			{
				return m_matchData.CurrentPlayer;
			}
		}

		public int NumRaceResults
		{
			get
			{
				return (TBRaceState == TBRaceState.Completed) ? m_results.Count : 0;
			}
		}

		public int NumResultDatas
		{
			get
			{
				return m_results.Count;
			}
		}

		public RaceEventData(TurnBasedMatchData _matchData, string _raceEventName)
		{
			m_matchData = _matchData;
			m_raceEventName = _raceEventName;
			m_results = new List<PlayerResultsData>();
		}

		public RaceEventData Clone(TurnBasedMatchData _matchDataClone)
		{
			RaceEventData raceEventData = new RaceEventData(_matchDataClone, RaceEventName);
			raceEventData.NumLaps = NumLaps;
			raceEventData.TBRaceState = TBRaceState;
			raceEventData.Reversed = Reversed;
			raceEventData.Variant = Variant;
			for (int i = 0; i < m_results.Count; i++)
			{
				PlayerResultsData item = m_results[i].Clone(_matchDataClone);
				raceEventData.m_results.Add(item);
			}
			return raceEventData;
		}

		public ITBPlayer GetPlayer(int _playerIndex)
		{
			return m_matchData.GetPlayer(_playerIndex);
		}

		public bool HasResultsForPlayer(string _playerID)
		{
			for (int i = 0; i < m_results.Count; i++)
			{
				PlayerResultsData playerResultsData = m_results[i];
				if (playerResultsData.PlayerID == _playerID)
				{
					return true;
				}
			}
			return false;
		}

		public PlayerResultsData FindResultsForPlayer(string _playerID)
		{
			for (int i = 0; i < m_results.Count; i++)
			{
				PlayerResultsData playerResultsData = m_results[i];
				if (playerResultsData.PlayerID == _playerID)
				{
					return playerResultsData;
				}
			}
			return null;
		}

		public PlayerResultsData AddPlayerResultsData(string _playerID)
		{
			if (HasResultsForPlayer(_playerID))
			{
				throw new Exception("Player results already exists");
			}
			PlayerResultsData playerResultsData = new PlayerResultsData(m_matchData, _playerID);
			m_results.Add(playerResultsData);
			return playerResultsData;
		}

		public ITBPlayerResults GetRaceResults(int _playerIndex)
		{
			if (TBRaceState != TBRaceState.Completed)
			{
				return null;
			}
			return m_results[_playerIndex];
		}

		public PlayerResultsData GetResultData(int _resultIndex)
		{
			return m_results[_resultIndex];
		}

		public static RaceEventData Read(TurnBasedMatchData _matchData, ReadSerialiser _serialiser, int _loadVersion)
		{
			string raceEventName = _serialiser.ReadString();
			RaceEventData raceEventData = new RaceEventData(_matchData, raceEventName);
			raceEventData.NumLaps = _serialiser.ReadInt32();
			raceEventData.TBRaceState = (TBRaceState)_serialiser.ReadInt32();
			raceEventData.Reversed = _serialiser.ReadBoolean();
			raceEventData.Variant = (RaceLayout.Variant)_serialiser.ReadInt32();
			int num = _serialiser.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				PlayerResultsData item = PlayerResultsData.Read(_matchData, _serialiser, _loadVersion);
				raceEventData.m_results.Add(item);
			}
			return raceEventData;
		}

		public void Write(WriteSerialiser _serialiser)
		{
			_serialiser.Write(RaceEventName);
			_serialiser.Write(NumLaps);
			_serialiser.Write((int)TBRaceState);
			_serialiser.Write(Reversed);
			_serialiser.Write((int)Variant);
			_serialiser.Write(m_results.Count);
			for (int i = 0; i < m_results.Count; i++)
			{
				m_results[i].Write(_serialiser);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("RaceEventName: {0}\n", RaceEventName);
			stringBuilder.AppendFormat("NumLaps: {0}\n", NumLaps);
			stringBuilder.AppendFormat("TBRaceState: {0}\n", TBRaceState);
			stringBuilder.AppendFormat("Reversed: {0}\n", Reversed);
			stringBuilder.AppendFormat("Variant: {0}\n", Variant);
			stringBuilder.AppendFormat("Number of results {0}:\n", m_results.Count);
			for (int i = 0; i < m_results.Count; i++)
			{
				stringBuilder.AppendLine(m_results[i].ToString());
			}
			return stringBuilder.ToString();
		}
	}

	public class PlayerData : ITBPlayer
	{
		private readonly TurnBasedMatchData m_matchData;

		private readonly string m_playerID;

		private string m_name;

		private int m_tbPlayerIndex;

		private int m_vehicleStyleIndex = -1;

		public string PlayerID
		{
			get
			{
				return m_playerID;
			}
		}

		public int ColourIndex
		{
			get
			{
				PlayerData localPlayer = m_matchData.LocalPlayer;
				if (localPlayer == null)
				{
					return -1;
				}
				int num = TBPlayerIndex - localPlayer.TBPlayerIndex;
				return MathUtils.Repeat(Settings.PlayerColour + num, 4);
			}
		}

		public int PlayerIndex
		{
			get
			{
				return m_matchData.PlayerIndex(PlayerID);
			}
		}

		public int VehicleStyleIndex
		{
			get
			{
				if (m_vehicleStyleIndex >= 0)
				{
					return m_vehicleStyleIndex;
				}
				return ColourIndex;
			}
			set
			{
				m_vehicleStyleIndex = value;
			}
		}

		public int TBPlayerIndex
		{
			get
			{
				return m_tbPlayerIndex;
			}
			set
			{
				m_tbPlayerIndex = value;
			}
		}

		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public PlayerData(TurnBasedMatchData _matchData, string _playerID)
		{
			m_matchData = _matchData;
			m_playerID = _playerID;
		}

		public PlayerData Clone(TurnBasedMatchData _matchDataClone)
		{
			PlayerData playerData = new PlayerData(_matchDataClone, PlayerID);
			playerData.Name = Name;
			playerData.TBPlayerIndex = TBPlayerIndex;
			return playerData;
		}

		public static PlayerData Read(TurnBasedMatchData _matchData, ReadSerialiser _serialiser, int _loadVersion)
		{
			string playerID = _serialiser.ReadString();
			string name = _serialiser.ReadString();
			int tBPlayerIndex = _serialiser.ReadInt32();
			PlayerData playerData = new PlayerData(_matchData, playerID);
			playerData.Name = name;
			playerData.TBPlayerIndex = tBPlayerIndex;
			if (_loadVersion > 1)
			{
				int vehicleStyleIndex = _serialiser.ReadInt32();
				playerData.m_vehicleStyleIndex = vehicleStyleIndex;
			}
			else
			{
				playerData.m_vehicleStyleIndex = -1;
			}
			return playerData;
		}

		public void Write(WriteSerialiser _serialiser)
		{
			_serialiser.Write(PlayerID);
			_serialiser.Write(Name);
			_serialiser.Write(TBPlayerIndex);
			_serialiser.Write(VehicleStyleIndex);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("PlayerID: {0}\n", PlayerID);
			stringBuilder.AppendFormat("TBPlayerIndex: {0}\n", TBPlayerIndex);
			stringBuilder.AppendFormat("Name: {0}\n", Name);
			return stringBuilder.ToString();
		}
	}

	public class PlayerResultsData : ITBPlayerResults
	{
		private readonly TurnBasedMatchData m_matchData;

		private readonly string m_playerID;

		private int m_position;

		private float m_raceTime;

		private float m_bestLap;

		private byte[] m_ghostData;

		public string PlayerID
		{
			get
			{
				return m_playerID;
			}
		}

		public byte[] GhostData
		{
			get
			{
				return m_ghostData;
			}
			set
			{
				m_ghostData = value;
			}
		}

		public int GhostDataSize
		{
			get
			{
				return (m_ghostData != null) ? m_ghostData.Length : 0;
			}
		}

		public ITBPlayer Player
		{
			get
			{
				return m_matchData.FindPlayerData(PlayerID);
			}
		}

		public int Position
		{
			get
			{
				return m_position;
			}
			set
			{
				m_position = value;
			}
		}

		public float RaceTime
		{
			get
			{
				return m_raceTime;
			}
			set
			{
				m_raceTime = value;
			}
		}

		public float BestLap
		{
			get
			{
				return m_bestLap;
			}
			set
			{
				m_bestLap = value;
			}
		}

		public PlayerResultsData(TurnBasedMatchData _matchData, string _playerID)
		{
			m_matchData = _matchData;
			m_playerID = _playerID;
		}

		public PlayerResultsData Clone(TurnBasedMatchData _matchDataClone)
		{
			PlayerResultsData playerResultsData = new PlayerResultsData(_matchDataClone, PlayerID);
			playerResultsData.Position = Position;
			playerResultsData.RaceTime = RaceTime;
			playerResultsData.BestLap = BestLap;
			if (GhostData != null)
			{
				playerResultsData.GhostData = new byte[GhostData.Length];
				GhostData.CopyTo(playerResultsData.GhostData, 0);
			}
			return playerResultsData;
		}

		public static PlayerResultsData Read(TurnBasedMatchData _matchData, ReadSerialiser _serialiser, int _loadVersion)
		{
			string playerID = _serialiser.ReadString();
			int position = _serialiser.ReadInt32();
			float raceTime = _serialiser.ReadSingle();
			float bestLap = _serialiser.ReadSingle();
			int num = _serialiser.ReadInt32();
			byte[] ghostData = null;
			if (num > 0)
			{
				ghostData = _serialiser.ReadBytes(num);
			}
			PlayerResultsData playerResultsData = new PlayerResultsData(_matchData, playerID);
			playerResultsData.Position = position;
			playerResultsData.RaceTime = raceTime;
			playerResultsData.BestLap = bestLap;
			if (_loadVersion == 2)
			{
				playerResultsData.GhostData = ghostData;
			}
			return playerResultsData;
		}

		public void Write(WriteSerialiser _serialiser)
		{
			_serialiser.Write(PlayerID);
			_serialiser.Write(Position);
			_serialiser.Write(RaceTime);
			_serialiser.Write(BestLap);
			_serialiser.Write(GhostDataSize);
			if (GhostDataSize > 0)
			{
				_serialiser.Write(GhostData);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("PlayerID: {0}\n", PlayerID);
			stringBuilder.AppendFormat("Position: {0}\n", Position);
			stringBuilder.AppendFormat("RaceTime: {0}\n", RaceTime);
			stringBuilder.AppendFormat("BestLap: {0}\n", BestLap);
			stringBuilder.AppendFormat("GhostDataSize: {0}\n", GhostDataSize);
			return stringBuilder.ToString();
		}
	}

	private struct Header
	{
		private static readonly byte[] s_fourCC = CoreUtils.MakeFourCC("TBMD");

		private int m_version;

		public int Version
		{
			get
			{
				return m_version;
			}
		}

		public Header(int _version)
		{
			m_version = _version;
		}

		public bool Serialise(ISerialiser _serialiser)
		{
			if (!_serialiser.FourCC(s_fourCC))
			{
				return false;
			}
			_serialiser.Serialise(ref m_version);
			return _serialiser.Sentinal();
		}
	}

	private const int c_maxGhostDataSize = 61440;

	private const int c_version = 2;

	private readonly TurnBasedMatchManager m_manager;

	private readonly string m_matchID;

	private int m_sequence;

	private List<RaceEventData> m_raceEvents;

	private List<PlayerData> m_perPlayerData;

	public int TotalGhostDataSize
	{
		get
		{
			int num = 0;
			for (int i = 0; i < m_raceEvents.Count; i++)
			{
				RaceEventData raceEventData = m_raceEvents[i];
				for (int j = 0; j < raceEventData.NumResultDatas; j++)
				{
					num += raceEventData.GetResultData(j).GhostDataSize;
				}
			}
			return num;
		}
	}

	public TurnBasedMatchManager Manager
	{
		get
		{
			return m_manager;
		}
	}

	public string MatchID
	{
		get
		{
			return m_matchID;
		}
	}

	public int Sequence
	{
		get
		{
			return m_sequence;
		}
	}

	public RaceEventData CurrentRace
	{
		get
		{
			for (int i = 0; i < m_raceEvents.Count; i++)
			{
				RaceEventData raceEventData = m_raceEvents[i];
				if (raceEventData.TBRaceState != TBRaceState.Completed)
				{
					return raceEventData;
				}
			}
			return null;
		}
	}

	public int NumRaces
	{
		get
		{
			return m_raceEvents.Count;
		}
	}

	public TurnBasedMatchState MatchState
	{
		get
		{
			bool flag = false;
			for (int i = 0; i < m_raceEvents.Count; i++)
			{
				if (m_raceEvents[i].TBRaceState == TBRaceState.Waiting)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return TurnBasedMatchState.Ended;
			}
			return TurnBasedMatchState.InProgress;
		}
	}

	public int NumRaceDatas
	{
		get
		{
			return m_raceEvents.Count;
		}
	}

	public int NumPlayers
	{
		get
		{
			return m_perPlayerData.Count;
		}
	}

	public PlayerData LocalPlayer
	{
		get
		{
			string currentPlayerID = m_manager.CurrentPlayerID;
			for (int i = 0; i < m_perPlayerData.Count; i++)
			{
				PlayerData playerData = m_perPlayerData[i];
				if (playerData.PlayerID == currentPlayerID)
				{
					return playerData;
				}
			}
			return null;
		}
	}

	public PlayerData CurrentPlayer
	{
		get
		{
			string currentPlayerID = m_manager.CurrentPlayerID;
			for (int i = 0; i < m_perPlayerData.Count; i++)
			{
				PlayerData playerData = m_perPlayerData[i];
				if (playerData.PlayerID == currentPlayerID)
				{
					return playerData;
				}
			}
			return null;
		}
	}

	public bool IsLocalPlayerInMatch
	{
		get
		{
			return m_manager.CurrentLocalPlayerInMatch;
		}
	}

	public TurnBasedMatchData(TurnBasedMatchManager _manager, string _matchID)
	{
		m_manager = _manager;
		m_matchID = _matchID;
		m_raceEvents = new List<RaceEventData>();
		m_perPlayerData = new List<PlayerData>();
	}

	public TurnBasedMatchData Clone()
	{
		TurnBasedMatchData turnBasedMatchData = new TurnBasedMatchData(m_manager, m_matchID);
		turnBasedMatchData.m_sequence = m_sequence;
		for (int i = 0; i < m_raceEvents.Count; i++)
		{
			RaceEventData item = m_raceEvents[i].Clone(turnBasedMatchData);
			turnBasedMatchData.m_raceEvents.Add(item);
		}
		for (int j = 0; j < m_perPlayerData.Count; j++)
		{
			PlayerData item2 = m_perPlayerData[j].Clone(turnBasedMatchData);
			turnBasedMatchData.m_perPlayerData.Add(item2);
		}
		return turnBasedMatchData;
	}

	public static TurnBasedMatchData FromByteArray(TurnBasedMatchManager _manager, byte[] _bytes)
	{
		using (MemoryStream stream = new MemoryStream(_bytes, false))
		{
			using (ReadSerialiser readSerialiser = new ReadSerialiser(stream))
			{
				if (!readSerialiser.IsValid())
				{
					Debug.Log("TurnBasedMatchData: Unable to load");
					return null;
				}
				try
				{
					Header header = default(Header);
					if (!header.Serialise(readSerialiser))
					{
						Debug.LogError("TurnBasedMatchData: Bad header");
						return null;
					}
					if (header.Version > 2)
					{
						Debug.LogError("TurnBasedMatchData: Invalid version");
						return null;
					}
					string matchID = readSerialiser.ReadString();
					TurnBasedMatchData turnBasedMatchData = new TurnBasedMatchData(_manager, matchID);
					turnBasedMatchData.m_sequence = readSerialiser.ReadInt32();
					int num = readSerialiser.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						RaceEventData item = RaceEventData.Read(turnBasedMatchData, readSerialiser, header.Version);
						turnBasedMatchData.m_raceEvents.Add(item);
					}
					int num2 = readSerialiser.ReadInt32();
					for (int j = 0; j < num2; j++)
					{
						PlayerData item2 = PlayerData.Read(turnBasedMatchData, readSerialiser, header.Version);
						turnBasedMatchData.m_perPlayerData.Add(item2);
					}
					if (!readSerialiser.EndOfSerialise())
					{
						Debug.LogError("TurnBasedMatchData: Failed to end load");
						return null;
					}
					return turnBasedMatchData;
				}
				catch (Exception message)
				{
					Debug.LogError(message);
					return null;
				}
			}
		}
	}

	public byte[] ToByteArray()
	{
		TrimGhostData();
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (WriteSerialiser writeSerialiser = new WriteSerialiser(memoryStream))
			{
				if (!writeSerialiser.IsValid())
				{
					Debug.Log("TurnBasedMatchData: Unable to save");
					return null;
				}
				try
				{
					if (!new Header(2).Serialise(writeSerialiser))
					{
						Debug.LogError("TurnBasedMatchData: Bad header");
						return null;
					}
					writeSerialiser.Write(MatchID);
					writeSerialiser.Write(m_sequence);
					int count = m_raceEvents.Count;
					writeSerialiser.Write(count);
					for (int i = 0; i < count; i++)
					{
						m_raceEvents[i].Write(writeSerialiser);
					}
					int count2 = m_perPlayerData.Count;
					writeSerialiser.Write(count2);
					for (int j = 0; j < count2; j++)
					{
						m_perPlayerData[j].Write(writeSerialiser);
					}
					if (!writeSerialiser.EndOfSerialise())
					{
						Debug.LogError("TurnBasedMatchData: Failed to end save");
						return null;
					}
					memoryStream.Flush();
					return memoryStream.ToArray();
				}
				catch (Exception message)
				{
					Debug.LogError(message);
					return null;
				}
			}
		}
	}

	private void TrimGhostData()
	{
		int totalGhostDataSize = TotalGhostDataSize;
		if (totalGhostDataSize <= 61440)
		{
			return;
		}
		int num = totalGhostDataSize - 61440;
		for (int i = 0; i < m_raceEvents.Count; i++)
		{
			RaceEventData raceEventData = m_raceEvents[i];
			for (int j = 0; j < raceEventData.NumResultDatas; j++)
			{
				PlayerResultsData resultData = raceEventData.GetResultData(j);
				num -= resultData.GhostDataSize;
				resultData.GhostData = null;
			}
			if (num <= 0)
			{
				break;
			}
		}
	}

	public void IncrementSequence()
	{
		m_sequence++;
	}

	public void SetMatchEnded()
	{
		for (int i = 0; i < m_raceEvents.Count; i++)
		{
			RaceEventData raceEventData = m_raceEvents[i];
			if (raceEventData.TBRaceState == TBRaceState.Waiting)
			{
				raceEventData.TBRaceState = TBRaceState.InActive;
			}
		}
	}

	public bool HasDataForRaceEvent(string _raceEventName)
	{
		for (int i = 0; i < m_raceEvents.Count; i++)
		{
			RaceEventData raceEventData = m_raceEvents[i];
			if (raceEventData.RaceEventName == _raceEventName)
			{
				return true;
			}
		}
		return false;
	}

	public RaceEventData FindRaceEventData(string _raceEventName)
	{
		for (int i = 0; i < m_raceEvents.Count; i++)
		{
			RaceEventData raceEventData = m_raceEvents[i];
			if (raceEventData.RaceEventName == _raceEventName)
			{
				return raceEventData;
			}
		}
		return null;
	}

	public RaceEventData AddRaceEventData(string _raceEventName)
	{
		if (HasDataForRaceEvent(_raceEventName))
		{
			throw new Exception("Race event data already exists");
		}
		RaceEventData raceEventData = new RaceEventData(this, _raceEventName);
		m_raceEvents.Add(raceEventData);
		return raceEventData;
	}

	public bool RemoveRaceEventData(string _raceEventName)
	{
		for (int i = 0; i < m_raceEvents.Count; i++)
		{
			RaceEventData raceEventData = m_raceEvents[i];
			if (raceEventData.RaceEventName == _raceEventName)
			{
				m_raceEvents.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public ITBRace GetRace(int _index)
	{
		return m_raceEvents[_index];
	}

	public RaceEventData GetRaceData(int _index)
	{
		return m_raceEvents[_index];
	}

	public bool HasDataForPlayer(string _playerID)
	{
		for (int i = 0; i < m_perPlayerData.Count; i++)
		{
			PlayerData playerData = m_perPlayerData[i];
			if (playerData.PlayerID == _playerID)
			{
				return true;
			}
		}
		return false;
	}

	public PlayerData FindPlayerData(string _playerID)
	{
		for (int i = 0; i < m_perPlayerData.Count; i++)
		{
			PlayerData playerData = m_perPlayerData[i];
			if (playerData.PlayerID == _playerID)
			{
				return playerData;
			}
		}
		return null;
	}

	public PlayerData AddPlayerData(string _playerID)
	{
		if (HasDataForPlayer(_playerID))
		{
			throw new Exception("Player data already exists");
		}
		PlayerData playerData = new PlayerData(this, _playerID);
		m_perPlayerData.Add(playerData);
		return playerData;
	}

	public bool RemovePlayerData(string _playerID)
	{
		for (int i = 0; i < m_perPlayerData.Count; i++)
		{
			PlayerData playerData = m_perPlayerData[i];
			if (playerData.PlayerID == _playerID)
			{
				m_perPlayerData.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public PlayerData GetPlayer(int _playerIndex)
	{
		return m_perPlayerData[_playerIndex];
	}

	public int PlayerIndex(string _playerID)
	{
		PlayerData playerData = FindPlayerData(_playerID);
		if (playerData == null)
		{
			return -1;
		}
		int num = -1;
		for (int i = 0; i < m_raceEvents.Count && m_raceEvents[i].TBRaceState == TBRaceState.Completed; i++)
		{
			num = i;
		}
		if (num == -1)
		{
			PlayerData localPlayer = LocalPlayer;
			if (localPlayer == null)
			{
				return -1;
			}
			int i2 = playerData.TBPlayerIndex - localPlayer.TBPlayerIndex;
			return MathUtils.Repeat(i2, NumPlayers);
		}
		RaceEventData raceEventData = m_raceEvents[num];
		if (raceEventData.HasResultsForPlayer(_playerID))
		{
			PlayerData localPlayer2 = LocalPlayer;
			if (localPlayer2 == null)
			{
				return -1;
			}
			int numResultDatas = raceEventData.NumResultDatas;
			int num2 = 0;
			int num3 = 0;
			for (int j = 0; j < numResultDatas; j++)
			{
				PlayerResultsData resultData = raceEventData.GetResultData(j);
				if (resultData.PlayerID == _playerID)
				{
					num3 = j;
				}
				if (resultData.PlayerID == localPlayer2.PlayerID)
				{
					num2 = j;
				}
			}
			int i3 = num3 - num2;
			return MathUtils.Repeat(i3, numResultDatas);
		}
		bool[] array = new bool[NumPlayers];
		for (int k = 0; k < NumPlayers; k++)
		{
			PlayerData playerData2 = m_perPlayerData[k];
			array[playerData2.TBPlayerIndex] = raceEventData.HasResultsForPlayer(playerData2.PlayerID);
		}
		int num4 = raceEventData.NumResultDatas;
		for (int num5 = num - 1; num5 >= 0; num5--)
		{
			RaceEventData raceEventData2 = m_raceEvents[num5];
			for (int l = 0; l < NumPlayers; l++)
			{
				PlayerData playerData3 = m_perPlayerData[l];
				if (!array[playerData3.TBPlayerIndex] && raceEventData2.HasResultsForPlayer(playerData3.PlayerID))
				{
					if (playerData3 == playerData)
					{
						return num4;
					}
					array[playerData3.TBPlayerIndex] = true;
					num4++;
				}
			}
		}
		for (int m = 0; m < NumPlayers; m++)
		{
			PlayerData playerData4 = m_perPlayerData[m];
			if (!array[playerData4.TBPlayerIndex])
			{
				if (playerData4 == playerData)
				{
					return num4;
				}
				array[playerData4.TBPlayerIndex] = true;
				num4++;
			}
		}
		return -1;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("MatchID: {0}\n", m_matchID);
		stringBuilder.AppendFormat("Sequence: {0}\n", m_sequence);
		stringBuilder.AppendFormat("Number of players {0}:\n", m_perPlayerData.Count);
		for (int i = 0; i < m_perPlayerData.Count; i++)
		{
			stringBuilder.AppendLine(m_perPlayerData[i].ToString());
		}
		stringBuilder.AppendFormat("Number of races {0}:\n", m_raceEvents.Count);
		for (int j = 0; j < m_raceEvents.Count; j++)
		{
			stringBuilder.AppendLine(m_raceEvents[j].ToString());
		}
		return stringBuilder.ToString();
	}
}
