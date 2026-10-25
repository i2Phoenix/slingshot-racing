using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Social/Manager")]
public class SocialManager : MonoBehaviour
{
	private struct CacheHeader
	{
		private int m_numAchievementUpdates;

		private int m_numLeaderboardUpdates;

		public int NumAchievementUpdates
		{
			get
			{
				return m_numAchievementUpdates;
			}
		}

		public int NumLeaderboardUpdates
		{
			get
			{
				return m_numLeaderboardUpdates;
			}
		}

		public CacheHeader(int _numAchievementUpdates, int _numLeaderboardUpdates)
		{
			m_numAchievementUpdates = _numAchievementUpdates;
			m_numLeaderboardUpdates = _numLeaderboardUpdates;
		}

		public bool Serialise(ISerialiser _serialiser)
		{
			_serialiser.Serialise(ref m_numAchievementUpdates);
			_serialiser.Serialise(ref m_numLeaderboardUpdates);
			return _serialiser.Sentinal();
		}
	}

	private struct AchievementUpdate
	{
		private const string c_playerIDAttribute = "PlayerID";

		private const string c_achievementIDAttribute = "AchievementID";

		private const string c_percentAttribute = "Percent";

		private string m_playerID;

		private string m_achievementID;

		private float m_percentCompleted;

		public string PlayerID
		{
			get
			{
				return m_playerID;
			}
		}

		public string AchievementID
		{
			get
			{
				return m_achievementID;
			}
		}

		public float PercentCompleted
		{
			get
			{
				return m_percentCompleted;
			}
		}

		public AchievementUpdate(string _playerID, string _achievementID, float _percentCompleted)
		{
			m_playerID = _playerID;
			m_achievementID = _achievementID;
			m_percentCompleted = _percentCompleted;
		}

		public bool Serialise(ISerialiser _serialiser)
		{
			_serialiser.Serialise(ref m_playerID);
			_serialiser.Serialise(ref m_achievementID);
			_serialiser.Serialise(ref m_percentCompleted);
			return _serialiser.Sentinal();
		}
	}

	private struct LeaderboardUpdate
	{
		private const string c_playerIDAttribute = "PlayerID";

		private const string c_leaderboardIDAttribute = "LeaderboardID";

		private const string c_scoreAttribute = "Score";

		private const string c_lowerIsBetterAttribute = "LowerIsBetter";

		private string m_playerID;

		private string m_leaderboardID;

		private long m_score;

		private bool m_lowerIsBetter;

		public string PlayerID
		{
			get
			{
				return m_playerID;
			}
		}

		public string LeaderboardID
		{
			get
			{
				return m_leaderboardID;
			}
		}

		public long Score
		{
			get
			{
				return m_score;
			}
		}

		public bool LowerIsBetter
		{
			get
			{
				return m_lowerIsBetter;
			}
		}

		public LeaderboardUpdate(string _playerID, string _leaderboardID, long _score, bool _lowerIsBetter)
		{
			m_playerID = _playerID;
			m_leaderboardID = _leaderboardID;
			m_score = _score;
			m_lowerIsBetter = _lowerIsBetter;
		}

		public bool Serialise(ISerialiser _serialiser)
		{
			_serialiser.Serialise(ref m_playerID);
			_serialiser.Serialise(ref m_leaderboardID);
			_serialiser.Serialise(ref m_score);
			_serialiser.Serialise(ref m_lowerIsBetter);
			return _serialiser.Sentinal();
		}
	}

	private class AchievementData
	{
		public float m_confirmedPercentCompleted = float.NaN;
	}

	private class RetrieveScoresData
	{
		public readonly OnRetrieveScoresDelegate m_onRetrieveScores;

		public readonly LeaderboardInfo m_leaderboardInfo;

		public readonly int m_start;

		public readonly int m_end;

		public RetrieveScoresData(OnRetrieveScoresDelegate _onRetrieveScores, LeaderboardInfo _leaderboardInfo, int _start, int _end)
		{
			m_onRetrieveScores = _onRetrieveScores;
			m_leaderboardInfo = _leaderboardInfo;
			m_start = _start;
			m_end = _end;
		}
	}

	private class PlayerData
	{
		public Dictionary<string, AchievementData> m_currentAchievements = new Dictionary<string, AchievementData>();

		public Dictionary<string, AchievementUpdate> m_pendingAchievements = new Dictionary<string, AchievementUpdate>();

		public Dictionary<string, LeaderboardUpdate> m_pendingLeaderboards = new Dictionary<string, LeaderboardUpdate>();

		public Dictionary<string, AchievementUpdate> m_failedAchievements = new Dictionary<string, AchievementUpdate>();

		public Dictionary<string, LeaderboardUpdate> m_failedLeaderboards = new Dictionary<string, LeaderboardUpdate>();
	}

	public delegate void OnRetrieveScoresDelegate(List<object> _scores, LeaderboardInfo _leaderboardInfo, bool _success);

	private const string c_updateCacheFilename = "Social.cache";

	private const string c_rootTag = "SocialCache";

	private const string c_achievementUpdateTag = "AchievementUpdate";

	private const string c_leaderboardUpdateTag = "LeaderboardUpdate";

	public const string s_twitterConsumerKey = "GVUwZ5qAzmc66zCwgF9XHQ";

	public const string s_twitterConsumerSecret = "E1gXiXqL8CGWJufEh6cl5AmII3d0Qd4itG9BLWbNg";

	public const string s_facebookAppID = "392278377469558";

	public const string s_facebookAppSecret = "b5f5559f99badb5c8b4a142799998263";

	private const int c_cacheVersion = 1;

	private Dictionary<string, PlayerData> m_playerData = new Dictionary<string, PlayerData>();

	private QueueList<RetrieveScoresData> m_retrieveScoresQueue = new QueueList<RetrieveScoresData>();

	private AchievementUpdate m_currentAchievementUpdate;

	private LeaderboardUpdate m_currentLeaderboardUpdate;

	private RetrieveScoresData m_currentRetrieveScores;

	private bool m_isUpdatingAchievement;

	private bool m_isUpdatingLeaderboard;

	private bool m_authenticationDone;

	private bool m_saveCache;

	public bool m_initialiseTwitter;

	public bool m_initialiseFacebook;

	private static bool s_isTwitterInitialised;

	private static bool s_isFacebookInitialised;

	public bool IsGameCenterSupported
	{
		get
		{
			return false;
		}
	}

	public bool IsAmazonGameCircleSupported
	{
		get
		{
			return false;
		}
	}

	public bool IsGoogleGameServicesSupported
	{
		get
		{
			return false;
		}
	}

	public bool IsSupported
	{
		get
		{
			return IsGameCenterSupported || IsAmazonGameCircleSupported || IsGoogleGameServicesSupported;
		}
	}

	public bool AreChallengesSupported
	{
		get
		{
			return false;
		}
	}

	public bool IsPlayerAuthenticated
	{
		get
		{
			return false;
		}
	}

	private bool AchievementsLoaded
	{
		get
		{
			return true;
		}
	}

	public string PlayerID
	{
		get
		{
			if (IsPlayerAuthenticated)
			{
			}
			return string.Empty;
		}
	}

	public string PlayerAlias
	{
		get
		{
			if (IsPlayerAuthenticated)
			{
			}
			return string.Empty;
		}
	}

	public static bool IsTwitterInitialised
	{
		get
		{
			return s_isTwitterInitialised;
		}
	}

	public static bool IsFacebookInitialised
	{
		get
		{
			return s_isFacebookInitialised;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
		DoStartup();
	}

	private void DoStartup()
	{
		if (IsSupported)
		{
			LoadUpdateCache();
			m_saveCache = true;
			if (!IsPlayerAuthenticated)
			{
				m_authenticationDone = false;
			}
		}
	}

	private void DoShutdown()
	{
		if (IsSupported)
		{
			CancelCallbacks();
			if (m_saveCache)
			{
				SaveUpdateCache();
				m_playerData.Clear();
				m_saveCache = false;
			}
		}
	}

	private void OnDestroy()
	{
		DoShutdown();
	}

	private void OnApplicationQuit()
	{
		DoShutdown();
	}

	private IEnumerator OnApplicationPause(bool _pause)
	{
		if (_pause)
		{
			DoShutdown();
			yield break;
		}
		yield return null;
		DoStartup();
	}

	public void AuthenticatePlayer()
	{
	}

	public bool UncoverAchievement(AchievementInfo _achievementInfo, bool _flush = true)
	{
		return UpdateAchievement(_achievementInfo, 0f, _flush);
	}

	public bool UnlockAchievement(AchievementInfo _achievementInfo, bool _flush = true)
	{
		return UpdateAchievement(_achievementInfo, 100f, _flush);
	}

	public bool UpdateAchievement(AchievementInfo _achievementInfo, float _percentCompleted, bool _flush = true)
	{
		if (!IsSupported)
		{
			Debug.Log("Social: Not supported");
			return false;
		}
		if (_achievementInfo.ID.Length == 0)
		{
			Debug.Log("Social: Achievement not supported");
			return false;
		}
		AddAchievementUpdate(new AchievementUpdate(PlayerID, _achievementInfo.ID, _percentCompleted));
		if (_flush)
		{
			FlushAchievements(true);
		}
		return true;
	}

	public bool UpdateScore(LeaderboardInfo _leaderboardInfo, long _score, bool _flush = true)
	{
		if (!IsSupported)
		{
			Debug.Log("Social: Not supported");
			return false;
		}
		if (_leaderboardInfo.ID.Length == 0)
		{
			Debug.Log("Social: Leaderboard not supported");
			return false;
		}
		AddLeaderboardUpdate(new LeaderboardUpdate(PlayerID, _leaderboardInfo.ID, _score, _leaderboardInfo.m_sortOrder == LeaderboardInfo.SortOrder.LowToHigh));
		if (_flush)
		{
			FlushLeaderboards(true);
		}
		return true;
	}

	public bool RetreiveScores(OnRetrieveScoresDelegate _onRetrieveScores, LeaderboardInfo _leaderboardInfo, int _start, int _end)
	{
		if (!IsSupported)
		{
			Debug.Log("Social: Not supported");
			return false;
		}
		if (_leaderboardInfo.ID.Length == 0)
		{
			Debug.Log("Social: Leaderboard not supported");
			return false;
		}
		RetrieveScoresData t = new RetrieveScoresData(_onRetrieveScores, _leaderboardInfo, _start, _end);
		m_retrieveScoresQueue.Enqueue(t);
		FlushRetrieveScores();
		return true;
	}

	public bool ShowAchievements()
	{
		if (!IsSupported)
		{
			Debug.Log("Social: Not supported");
			return false;
		}
		if (!IsPlayerAuthenticated)
		{
			Debug.Log("Social: Player not authenticated");
			return false;
		}
		return true;
	}

	public bool ShowLeaderboard(LeaderboardInfo _leaderboardInfo)
	{
		if (!IsSupported)
		{
			Debug.Log("Social: Not supported");
			return false;
		}
		if (!IsPlayerAuthenticated)
		{
			Debug.Log("Social: Player not authenticated");
			return false;
		}
		if (_leaderboardInfo.ID.Length == 0)
		{
			Debug.Log("Social: Leaderboard not supported");
			return false;
		}
		return true;
	}

	public bool ShowLeaderboards()
	{
		if (!IsSupported)
		{
			Debug.Log("Social: Not supported");
			return false;
		}
		if (!IsPlayerAuthenticated)
		{
			Debug.Log("Social: Player not authenticated");
			return false;
		}
		return true;
	}

	public bool FlushUpdates()
	{
		if (!IsSupported)
		{
			Debug.Log("Social: Not supported");
			return false;
		}
		FlushAchievements(true);
		FlushLeaderboards(true);
		return true;
	}

	public bool ResetAllAchievements()
	{
		if (!IsGameCenterSupported)
		{
			Debug.Log("Social: Not supported");
			return false;
		}
		if (!AchievementsLoaded)
		{
			return false;
		}
		return false;
	}

	public bool IsUpdating(LeaderboardInfo _leaderboardInfo)
	{
		if (!IsSupported)
		{
			Debug.Log("Social: Not supported");
			return false;
		}
		if (_leaderboardInfo.ID.Length == 0)
		{
			Debug.Log("Social: Leaderboard not supported");
			return false;
		}
		if (!m_isUpdatingLeaderboard)
		{
			return false;
		}
		return m_currentLeaderboardUpdate.LeaderboardID == _leaderboardInfo.ID;
	}

	public bool ShowChallenges()
	{
		if (!IsGameCenterSupported)
		{
			Debug.Log("Social: Not supported");
			return false;
		}
		if (!IsPlayerAuthenticated)
		{
			Debug.Log("Social: Player not authenticated");
			return false;
		}
		return false;
	}

	private void FlushAchievements(bool _includeFailed)
	{
		PlayerData value;
		if (!AchievementsLoaded || m_isUpdatingAchievement || !IsPlayerAuthenticated || !m_playerData.TryGetValue(PlayerID, out value))
		{
			return;
		}
		while (value.m_pendingAchievements.Count > 0)
		{
			IEnumerator<KeyValuePair<string, AchievementUpdate>> enumerator = value.m_pendingAchievements.GetEnumerator();
			enumerator.MoveNext();
			string key = enumerator.Current.Key;
			AchievementUpdate value2 = enumerator.Current.Value;
			value.m_pendingAchievements.Remove(key);
			if (ReportProgressInternal(value2))
			{
				return;
			}
		}
		if (!_includeFailed)
		{
			return;
		}
		while (value.m_failedAchievements.Count > 0)
		{
			IEnumerator<KeyValuePair<string, AchievementUpdate>> enumerator2 = value.m_failedAchievements.GetEnumerator();
			enumerator2.MoveNext();
			string key2 = enumerator2.Current.Key;
			AchievementUpdate value3 = enumerator2.Current.Value;
			value.m_failedAchievements.Remove(key2);
			if (ReportProgressInternal(value3))
			{
				break;
			}
		}
	}

	private void FlushLeaderboards(bool _includeFailed)
	{
		PlayerData value;
		if (!m_authenticationDone || m_isUpdatingLeaderboard || !IsPlayerAuthenticated || !m_playerData.TryGetValue(PlayerID, out value))
		{
			return;
		}
		while (value.m_pendingLeaderboards.Count > 0)
		{
			IEnumerator<KeyValuePair<string, LeaderboardUpdate>> enumerator = value.m_pendingLeaderboards.GetEnumerator();
			enumerator.MoveNext();
			string key = enumerator.Current.Key;
			LeaderboardUpdate value2 = enumerator.Current.Value;
			value.m_pendingLeaderboards.Remove(key);
			if (ReportScoreInternal(value2))
			{
				return;
			}
		}
		if (!_includeFailed)
		{
			return;
		}
		while (value.m_failedLeaderboards.Count > 0)
		{
			IEnumerator<KeyValuePair<string, LeaderboardUpdate>> enumerator2 = value.m_failedLeaderboards.GetEnumerator();
			enumerator2.MoveNext();
			string key2 = enumerator2.Current.Key;
			LeaderboardUpdate value3 = enumerator2.Current.Value;
			value.m_failedLeaderboards.Remove(key2);
			if (ReportScoreInternal(value3))
			{
				break;
			}
		}
	}

	private void FlushRetrieveScores()
	{
		if (m_authenticationDone && m_currentRetrieveScores == null && IsPlayerAuthenticated && m_retrieveScoresQueue.Count > 0)
		{
			RetrieveScoresData data = m_retrieveScoresQueue.Dequeue();
			RetrieveScoresInternal(data);
		}
	}

	private void OnAchievementUpdated(string _id)
	{
		if (!m_isUpdatingAchievement)
		{
			Debug.Log("Social: Ignored OnReportAchievement");
			return;
		}
		if (_id != m_currentAchievementUpdate.AchievementID)
		{
			Debug.Log("Social: Unexpected achievement report");
			return;
		}
		PlayerData playerData = EnsurePlayerData(m_currentAchievementUpdate.PlayerID);
		AchievementData achievementData = playerData.m_currentAchievements[_id];
		if (float.IsNaN(achievementData.m_confirmedPercentCompleted) || m_currentAchievementUpdate.PercentCompleted > achievementData.m_confirmedPercentCompleted)
		{
			Debug.Log(string.Format("Social: Confirmed percent completed {0} ({1})", m_currentAchievementUpdate.PercentCompleted, m_currentAchievementUpdate.AchievementID));
			achievementData.m_confirmedPercentCompleted = m_currentAchievementUpdate.PercentCompleted;
		}
		m_isUpdatingAchievement = false;
		m_currentAchievementUpdate = default(AchievementUpdate);
		FlushAchievements(true);
	}

	private void OnAchievementUpdateFailed()
	{
		if (!m_isUpdatingAchievement)
		{
			Debug.Log("Social: Ignored OnReportAchievementFailed");
			return;
		}
		Debug.Log(string.Format("Social: Failed achievement update ({0})", m_currentAchievementUpdate.AchievementID));
		m_isUpdatingAchievement = false;
		m_currentAchievementUpdate = default(AchievementUpdate);
		FlushAchievements(false);
	}

	private void OnScoreUpdated(string _id)
	{
		if (!m_isUpdatingLeaderboard)
		{
			Debug.Log("Social: Ignored OnReportScore");
			return;
		}
		if (_id != m_currentLeaderboardUpdate.LeaderboardID)
		{
			Debug.Log("Social: Unexpected leaderboard report");
			return;
		}
		Debug.Log(string.Format("Social: Confirmed leaderboard score update {0} ({1})", m_currentLeaderboardUpdate.Score, m_currentLeaderboardUpdate.LeaderboardID));
		m_isUpdatingLeaderboard = false;
		m_currentLeaderboardUpdate = default(LeaderboardUpdate);
		FlushLeaderboards(true);
	}

	private void OnScoreUpdateFailed()
	{
		if (!m_isUpdatingLeaderboard)
		{
			Debug.Log("Social: Ignored OnReportScoreFailed");
			return;
		}
		Debug.Log(string.Format("Social: Failed leaderboard update ({0})", m_currentLeaderboardUpdate.LeaderboardID));
		m_isUpdatingLeaderboard = false;
		m_currentLeaderboardUpdate = default(LeaderboardUpdate);
		FlushLeaderboards(false);
	}

	private void OnRetrievedScores(List<object> _scores)
	{
		if (m_currentRetrieveScores == null)
		{
			Debug.Log("Social: Ignored OnRetrieveScores");
			return;
		}
		Debug.Log(string.Format("Social: Retrieved scores ({0})", m_currentRetrieveScores.m_leaderboardInfo.ID));
		if (m_currentRetrieveScores.m_onRetrieveScores != null)
		{
			m_currentRetrieveScores.m_onRetrieveScores(_scores, m_currentRetrieveScores.m_leaderboardInfo, true);
		}
		m_currentRetrieveScores = null;
		FlushRetrieveScores();
	}

	private void OnRetrieveScoresFailed()
	{
		if (m_currentRetrieveScores == null)
		{
			Debug.Log("Social: Ignored OnRetrieveScoresFailed");
			return;
		}
		Debug.Log(string.Format("Social: Failed to retrieve scores ({0})", m_currentRetrieveScores.m_leaderboardInfo.ID));
		if (m_currentRetrieveScores.m_onRetrieveScores != null)
		{
			m_currentRetrieveScores.m_onRetrieveScores(null, m_currentRetrieveScores.m_leaderboardInfo, false);
		}
		m_currentRetrieveScores = null;
		FlushRetrieveScores();
	}

	private bool ReportProgressInternal(AchievementUpdate _achievementUpdate)
	{
		PlayerData playerData = EnsurePlayerData(_achievementUpdate.PlayerID);
		AchievementData achievementData;
		if (playerData.m_currentAchievements.ContainsKey(_achievementUpdate.AchievementID))
		{
			achievementData = playerData.m_currentAchievements[_achievementUpdate.AchievementID];
		}
		else
		{
			achievementData = new AchievementData();
			playerData.m_currentAchievements[_achievementUpdate.AchievementID] = achievementData;
		}
		if (float.IsNaN(achievementData.m_confirmedPercentCompleted) || _achievementUpdate.PercentCompleted > achievementData.m_confirmedPercentCompleted)
		{
			m_isUpdatingAchievement = true;
			m_currentAchievementUpdate = _achievementUpdate;
			return true;
		}
		return false;
	}

	private bool ReportScoreInternal(LeaderboardUpdate _leaderboardUpdate)
	{
		m_currentLeaderboardUpdate = _leaderboardUpdate;
		m_isUpdatingLeaderboard = true;
		return true;
	}

	private void RetrieveScoresInternal(RetrieveScoresData _data)
	{
		m_currentRetrieveScores = _data;
	}

	private void AddAchievementUpdate(AchievementUpdate _achievementUpdate)
	{
		PlayerData playerData = EnsurePlayerData(_achievementUpdate.PlayerID);
		if (playerData.m_pendingAchievements.ContainsKey(_achievementUpdate.AchievementID))
		{
			AchievementUpdate achievementUpdate = playerData.m_pendingAchievements[_achievementUpdate.AchievementID];
			if (_achievementUpdate.PercentCompleted > achievementUpdate.PercentCompleted)
			{
				playerData.m_pendingAchievements[_achievementUpdate.AchievementID] = _achievementUpdate;
			}
		}
		else
		{
			playerData.m_pendingAchievements[_achievementUpdate.AchievementID] = _achievementUpdate;
		}
	}

	private void AddFailedAchievementUpdate(AchievementUpdate _achievementUpdate)
	{
		PlayerData playerData = EnsurePlayerData(_achievementUpdate.PlayerID);
		if (playerData.m_failedAchievements.ContainsKey(_achievementUpdate.AchievementID))
		{
			AchievementUpdate achievementUpdate = playerData.m_failedAchievements[_achievementUpdate.AchievementID];
			if (_achievementUpdate.PercentCompleted > achievementUpdate.PercentCompleted)
			{
				playerData.m_failedAchievements[_achievementUpdate.AchievementID] = _achievementUpdate;
			}
		}
		else
		{
			playerData.m_failedAchievements[_achievementUpdate.AchievementID] = _achievementUpdate;
		}
	}

	private void AddLeaderboardUpdate(LeaderboardUpdate _leaderboardUpdate)
	{
		PlayerData playerData = EnsurePlayerData(_leaderboardUpdate.PlayerID);
		if (playerData.m_pendingLeaderboards.ContainsKey(_leaderboardUpdate.LeaderboardID))
		{
			LeaderboardUpdate leaderboardUpdate = playerData.m_pendingLeaderboards[_leaderboardUpdate.LeaderboardID];
			if ((_leaderboardUpdate.LowerIsBetter && _leaderboardUpdate.Score < leaderboardUpdate.Score) || (!_leaderboardUpdate.LowerIsBetter && _leaderboardUpdate.Score > leaderboardUpdate.Score))
			{
				playerData.m_pendingLeaderboards[_leaderboardUpdate.LeaderboardID] = _leaderboardUpdate;
			}
		}
		else
		{
			playerData.m_pendingLeaderboards[_leaderboardUpdate.LeaderboardID] = _leaderboardUpdate;
		}
	}

	private void AddFailedLeaderboardUpdate(LeaderboardUpdate _leaderboardUpdate)
	{
		PlayerData playerData = EnsurePlayerData(_leaderboardUpdate.PlayerID);
		if (playerData.m_failedLeaderboards.ContainsKey(_leaderboardUpdate.LeaderboardID))
		{
			LeaderboardUpdate leaderboardUpdate = playerData.m_failedLeaderboards[_leaderboardUpdate.LeaderboardID];
			if ((_leaderboardUpdate.LowerIsBetter && _leaderboardUpdate.Score < leaderboardUpdate.Score) || (!_leaderboardUpdate.LowerIsBetter && _leaderboardUpdate.Score > leaderboardUpdate.Score))
			{
				playerData.m_failedLeaderboards[_leaderboardUpdate.LeaderboardID] = _leaderboardUpdate;
			}
		}
		else
		{
			playerData.m_failedLeaderboards[_leaderboardUpdate.LeaderboardID] = _leaderboardUpdate;
		}
	}

	private void CancelCallbacks()
	{
		if (m_isUpdatingAchievement)
		{
			Debug.Log("Social: Cancelling achievement callback");
		}
		if (m_isUpdatingLeaderboard)
		{
			Debug.Log("Social: Cancelling leaderboard callback");
		}
	}

	private void ClaimUpdatesWithNoPlayerID()
	{
		string playerID = PlayerID;
		PlayerData value;
		if (!IsPlayerAuthenticated || string.IsNullOrEmpty(playerID) || !m_playerData.TryGetValue(string.Empty, out value))
		{
			return;
		}
		foreach (KeyValuePair<string, AchievementUpdate> pendingAchievement in value.m_pendingAchievements)
		{
			AchievementUpdate value2 = pendingAchievement.Value;
			AchievementUpdate achievementUpdate = new AchievementUpdate(playerID, value2.AchievementID, value2.PercentCompleted);
			AddAchievementUpdate(achievementUpdate);
		}
		value.m_pendingAchievements.Clear();
		foreach (KeyValuePair<string, AchievementUpdate> failedAchievement in value.m_failedAchievements)
		{
			AchievementUpdate value3 = failedAchievement.Value;
			AchievementUpdate achievementUpdate2 = new AchievementUpdate(playerID, value3.AchievementID, value3.PercentCompleted);
			AddFailedAchievementUpdate(achievementUpdate2);
		}
		value.m_failedAchievements.Clear();
		foreach (KeyValuePair<string, LeaderboardUpdate> pendingLeaderboard in value.m_pendingLeaderboards)
		{
			LeaderboardUpdate value4 = pendingLeaderboard.Value;
			LeaderboardUpdate leaderboardUpdate = new LeaderboardUpdate(playerID, value4.LeaderboardID, value4.Score, value4.LowerIsBetter);
			AddLeaderboardUpdate(leaderboardUpdate);
		}
		value.m_pendingLeaderboards.Clear();
		foreach (KeyValuePair<string, LeaderboardUpdate> failedLeaderboard in value.m_failedLeaderboards)
		{
			LeaderboardUpdate value5 = failedLeaderboard.Value;
			LeaderboardUpdate leaderboardUpdate2 = new LeaderboardUpdate(playerID, value5.LeaderboardID, value5.Score, value5.LowerIsBetter);
			AddFailedLeaderboardUpdate(leaderboardUpdate2);
		}
		value.m_failedLeaderboards.Clear();
	}

	private PlayerData EnsurePlayerData(string _playerID)
	{
		PlayerData playerData;
		if (m_playerData.ContainsKey(_playerID))
		{
			playerData = m_playerData[_playerID];
		}
		else
		{
			playerData = new PlayerData();
			m_playerData.Add(_playerID, playerData);
		}
		return playerData;
	}

	private string GetUpdateCachePath()
	{
		return Application.persistentDataPath + "/Social.cache";
	}

	private void LoadUpdateCache()
	{
	}

	private void SaveUpdateCache()
	{
	}

	private static void OnTwitterInitializedEvent()
	{
		TwitterAndroidManager.twitterInitializedEvent -= OnTwitterInitializedEvent;
		s_isTwitterInitialised = true;
	}
}
