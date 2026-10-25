using System;
using UnityEngine;

public static class Settings
{
	public enum RatedType
	{
		NotYetRated = 0,
		HasRated = 1,
		WillNotRate = 2
	}

	private static string m_vehicleType = "Default";

	private static bool m_loaded;

	private static bool m_dynamicCamera = true;

	private static bool m_cameraTrackAIPlayers;

	private static bool m_devFreeplay;

	private static int m_freePlayNumPlayers = 1;

	private static int m_freePlayNumLaps = -1;

	private static bool m_freePlayEnableAIs;

	private static float m_soundVolume = 1f;

	private static float m_musicVolume = 1f;

	private static bool m_cloudSync = true;

	private static bool m_inRaceMusic = true;

	private static bool m_highlightPlayer = true;

	private static TouchInputManager.SinglePlayerMode m_singlePlayerInputMode = TouchInputManager.SinglePlayerMode.Screen;

	private static string m_lastVersion = string.Empty;

	private static int m_playedCount = 0;

	private static RatedType m_rated;

	private static bool m_raceButtonOnRight;

	private static int m_vehicleStyleIndex = -1;

	private static int[] m_vehicleColourIndices = new int[9] { 0, 1, 2, 3, 1, 0, 3, 2, 0 };

	private static float m_timeSinceReviewPrompt;

	private static bool m_reviewOffered;

	private static bool m_kamcordEnabled = true;

	public static bool DynamicCamera
	{
		get
		{
			LazyLoad();
			return m_dynamicCamera;
		}
		set
		{
			LazyLoad();
			if (m_dynamicCamera != value)
			{
				m_dynamicCamera = value;
				SettingsUpdated();
			}
		}
	}

	public static bool DynamicCameraTrackAI
	{
		get
		{
			LazyLoad();
			return m_cameraTrackAIPlayers;
		}
		set
		{
			LazyLoad();
			if (m_cameraTrackAIPlayers != value)
			{
				m_cameraTrackAIPlayers = value;
				SettingsUpdated();
			}
		}
	}

	public static bool DevFreeplay
	{
		get
		{
			LazyLoad();
			return m_devFreeplay;
		}
		set
		{
			LazyLoad();
			if (m_devFreeplay != value)
			{
				m_devFreeplay = value;
				SettingsUpdated();
			}
		}
	}

	public static string VehicleType
	{
		get
		{
			LazyLoad();
			return m_vehicleType;
		}
		set
		{
			LazyLoad();
			if (m_vehicleType != value)
			{
				m_vehicleType = value;
				SettingsUpdated();
			}
		}
	}

	public static int FreePlayNumPlayers
	{
		get
		{
			LazyLoad();
			return m_freePlayNumPlayers;
		}
		set
		{
			LazyLoad();
			if (m_freePlayNumPlayers != value)
			{
				m_freePlayNumPlayers = value;
				SettingsUpdated();
			}
		}
	}

	public static int FreePlayNumLaps
	{
		get
		{
			LazyLoad();
			return m_freePlayNumLaps;
		}
		set
		{
			LazyLoad();
			if (m_freePlayNumLaps != value)
			{
				m_freePlayNumLaps = value;
				SettingsUpdated();
			}
		}
	}

	public static bool FreePlayEnableAIs
	{
		get
		{
			LazyLoad();
			return m_freePlayEnableAIs;
		}
		set
		{
			LazyLoad();
			if (m_freePlayEnableAIs != value)
			{
				m_freePlayEnableAIs = value;
				SettingsUpdated();
			}
		}
	}

	public static float SoundVolume
	{
		get
		{
			LazyLoad();
			return m_soundVolume;
		}
		set
		{
			LazyLoad();
			if (m_soundVolume != value)
			{
				m_soundVolume = value;
				SettingsUpdated();
			}
		}
	}

	public static float MusicVolume
	{
		get
		{
			LazyLoad();
			return m_musicVolume;
		}
		set
		{
			LazyLoad();
			if (m_musicVolume != value)
			{
				m_musicVolume = value;
				SettingsUpdated();
			}
		}
	}

	public static bool CloudSync
	{
		get
		{
			LazyLoad();
			return m_cloudSync;
		}
		set
		{
			LazyLoad();
			if (m_cloudSync != value)
			{
				m_cloudSync = value;
				SettingsUpdated();
			}
		}
	}

	public static bool InRaceMusic
	{
		get
		{
			LazyLoad();
			return m_inRaceMusic;
		}
		set
		{
			LazyLoad();
			if (m_inRaceMusic != value)
			{
				m_inRaceMusic = value;
				SettingsUpdated();
			}
		}
	}

	public static bool HighlightPlayer
	{
		get
		{
			LazyLoad();
			return m_highlightPlayer;
		}
		set
		{
			LazyLoad();
			if (m_highlightPlayer != value)
			{
				m_highlightPlayer = value;
				SettingsUpdated();
			}
		}
	}

	public static int PlayerColour
	{
		get
		{
			return GetVehicleColourIndex(VehicleStyleIndex);
		}
		set
		{
			SetVehicleColourIndex(VehicleStyleIndex, value);
		}
	}

	public static TouchInputManager.SinglePlayerMode SinglePlayerInputMode
	{
		get
		{
			LazyLoad();
			return m_singlePlayerInputMode;
		}
		set
		{
			LazyLoad();
			if (m_singlePlayerInputMode != value)
			{
				SettingsUpdated();
			}
		}
	}

	public static string LastVersion
	{
		get
		{
			LazyLoad();
			return m_lastVersion;
		}
		set
		{
			LazyLoad();
			if (m_lastVersion != value)
			{
				m_lastVersion = value;
				SettingsUpdated();
			}
		}
	}

	public static int PlayedCount
	{
		get
		{
			LazyLoad();
			return m_playedCount;
		}
		set
		{
			LazyLoad();
			if (m_playedCount != value)
			{
				m_playedCount = value;
				SettingsUpdated();
			}
		}
	}

	public static RatedType Rated
	{
		get
		{
			LazyLoad();
			return m_rated;
		}
		set
		{
			LazyLoad();
			if (m_rated != value)
			{
				m_rated = value;
				SettingsUpdated();
			}
		}
	}

	public static bool RaceButtonOnRight
	{
		get
		{
			LazyLoad();
			return m_raceButtonOnRight;
		}
		set
		{
			LazyLoad();
			if (m_raceButtonOnRight != value)
			{
				m_raceButtonOnRight = value;
				SettingsUpdated();
			}
		}
	}

	public static int VehicleStyleIndex
	{
		get
		{
			LazyLoad();
			return m_vehicleStyleIndex;
		}
		set
		{
			LazyLoad();
			if (m_vehicleStyleIndex != value)
			{
				m_vehicleStyleIndex = value;
				Analytics.LogEvent("Settings", "VehicleStyle", value.ToString(), "Unlocked", GlobalSave.IsVehicleStyleUnlocked(value).ToString());
				SettingsUpdated();
			}
		}
	}

	public static float TimeSinceReviewPrompt
	{
		get
		{
			LazyLoad();
			return m_timeSinceReviewPrompt;
		}
		set
		{
			LazyLoad();
			if (m_timeSinceReviewPrompt != value)
			{
				m_timeSinceReviewPrompt = value;
				SettingsUpdated();
			}
		}
	}

	public static bool ReviewOffered
	{
		get
		{
			LazyLoad();
			return m_reviewOffered;
		}
		set
		{
			LazyLoad();
			if (m_reviewOffered != value)
			{
				m_reviewOffered = value;
				SettingsUpdated();
			}
		}
	}

	public static bool KamcordEnabled
	{
		get
		{
			LazyLoad();
			return m_kamcordEnabled;
		}
		set
		{
			LazyLoad();
			if (m_kamcordEnabled != value)
			{
				m_kamcordEnabled = value;
				SettingsUpdated();
			}
		}
	}

	public static event EventHandler OnSettingsUpdated;

	public static int GetVehicleColourIndex(int _vehicleIndex)
	{
		LazyLoad();
		return m_vehicleColourIndices[_vehicleIndex];
	}

	public static void SetVehicleColourIndex(int _vehicleIndex, int _colourIndex)
	{
		LazyLoad();
		if (m_vehicleColourIndices[_vehicleIndex] != _colourIndex)
		{
			m_vehicleColourIndices[_vehicleIndex] = Mathf.Clamp(_colourIndex, 0, 3);
			SettingsUpdated();
		}
	}

	public static bool GetBool(string name, bool def)
	{
		LazyLoad();
		return PlayerPrefs.GetInt(name, def ? 1 : 0) != 0;
	}

	public static void SetBool(string name, bool val)
	{
		LazyLoad();
		int num = (val ? 1 : 0);
		if (!PlayerPrefs.HasKey(name) || PlayerPrefs.GetInt(name) != num)
		{
			PlayerPrefs.SetInt(name, num);
			SettingsUpdated();
		}
	}

	public static int GetInt(string name, int def)
	{
		LazyLoad();
		return PlayerPrefs.GetInt(name, def);
	}

	public static void SetInt(string name, int val)
	{
		LazyLoad();
		if (!PlayerPrefs.HasKey(name) || PlayerPrefs.GetInt(name) != val)
		{
			PlayerPrefs.SetInt(name, val);
			SettingsUpdated();
		}
	}

	public static string GetString(string name, string def)
	{
		LazyLoad();
		return PlayerPrefs.GetString(name, def);
	}

	public static void SetString(string name, string val)
	{
		LazyLoad();
		if (!PlayerPrefs.HasKey(name) || PlayerPrefs.GetString(name) != val)
		{
			PlayerPrefs.SetString(name, val);
			SettingsUpdated();
		}
	}

	public static void Validate()
	{
		if (!GlobalSave.IsVehicleStyleUnlocked(m_vehicleStyleIndex))
		{
			Debug.Log("Vehicle style " + m_vehicleStyleIndex + " is locked");
			m_vehicleStyleIndex = 0;
		}
	}

	private static void SettingsUpdated()
	{
		Validate();
		Save();
		if (OnSettingsUpdated != null)
		{
			OnSettingsUpdated(null, null);
		}
	}

	private static void Save()
	{
		PlayerPrefs.SetInt("DynamicCamera", m_dynamicCamera ? 1 : 0);
		PlayerPrefs.SetInt("DynamicCameraTrackAI", m_cameraTrackAIPlayers ? 1 : 0);
		PlayerPrefs.SetInt("DevFreeplay", m_devFreeplay ? 1 : 0);
		PlayerPrefs.SetString("VehicleType", m_vehicleType);
		PlayerPrefs.SetInt("CloudSync", m_cloudSync ? 1 : 0);
		PlayerPrefs.SetInt("FreePlayNumPlayers", m_freePlayNumPlayers);
		PlayerPrefs.SetInt("FreePlayNumLaps", m_freePlayNumLaps);
		PlayerPrefs.SetInt("FreePlayEnableAIs", m_freePlayEnableAIs ? 1 : 0);
		PlayerPrefs.SetFloat("SoundVolume", m_soundVolume);
		PlayerPrefs.SetFloat("MusicVolume", m_musicVolume);
		PlayerPrefs.SetInt("InRaceMusic", m_inRaceMusic ? 1 : 0);
		PlayerPrefs.SetInt("HighlightPlayer", m_highlightPlayer ? 1 : 0);
		PlayerPrefs.SetString("LastVersion", m_lastVersion);
		PlayerPrefs.SetInt("PlayedCount", m_playedCount);
		PlayerPrefs.SetInt("Rated", (int)m_rated);
		PlayerPrefs.SetInt("RaceButtonOnRight", m_raceButtonOnRight ? 1 : 0);
		PlayerPrefs.SetInt("VehicleStyleIndex", m_vehicleStyleIndex);
		PlayerPrefs.SetFloat("TimeSinceReviewPrompt", m_timeSinceReviewPrompt);
		PlayerPrefs.SetInt("ReviewOffered", m_reviewOffered ? 1 : 0);
		PlayerPrefs.SetInt("KamcordEnabled", m_kamcordEnabled ? 1 : 0);
		for (int i = 0; i < m_vehicleColourIndices.Length; i++)
		{
			PlayerPrefs.SetInt("VehicleColourIndex" + i, m_vehicleColourIndices[i]);
		}
		PlayerPrefs.Save();
	}

	private static void Load()
	{
		m_dynamicCamera = PlayerPrefs.GetInt("DynamicCamera", m_dynamicCamera ? 1 : 0) != 0;
		m_cameraTrackAIPlayers = PlayerPrefs.GetInt("DynamicCameraTrackAI", m_cameraTrackAIPlayers ? 1 : 0) != 0;
		m_devFreeplay = PlayerPrefs.GetInt("DevFreeplay", m_devFreeplay ? 1 : 0) != 0;
		m_vehicleType = PlayerPrefs.GetString("VehicleType", m_vehicleType);
		m_cloudSync = PlayerPrefs.GetInt("CloudSync", m_cloudSync ? 1 : 0) != 0;
		int num = PlayerPrefs.GetInt("PlayerColour", 0);
		m_freePlayNumPlayers = PlayerPrefs.GetInt("FreePlayNumPlayers", m_freePlayNumPlayers);
		m_freePlayNumLaps = PlayerPrefs.GetInt("FreePlayNumLaps", m_freePlayNumLaps);
		m_freePlayEnableAIs = PlayerPrefs.GetInt("FreePlayEnableAIs", m_freePlayEnableAIs ? 1 : 0) != 0;
		m_soundVolume = PlayerPrefs.GetFloat("SoundVolume", m_soundVolume);
		m_musicVolume = PlayerPrefs.GetFloat("MusicVolume", m_musicVolume);
		m_inRaceMusic = PlayerPrefs.GetInt("InRaceMusic", m_inRaceMusic ? 1 : 0) != 0;
		m_highlightPlayer = PlayerPrefs.GetInt("HighlightPlayer", m_highlightPlayer ? 1 : 0) != 0;
		m_singlePlayerInputMode = (TouchInputManager.SinglePlayerMode)PlayerPrefs.GetInt("SinglePlayerInputMode", (int)m_singlePlayerInputMode);
		m_lastVersion = PlayerPrefs.GetString("LastVersion", m_lastVersion);
		m_playedCount = PlayerPrefs.GetInt("PlayedCount", m_playedCount);
		m_rated = (RatedType)PlayerPrefs.GetInt("Rated", (int)m_rated);
		m_raceButtonOnRight = PlayerPrefs.GetInt("RaceButtonOnRight", m_raceButtonOnRight ? 1 : 0) != 0;
		m_vehicleStyleIndex = Mathf.Clamp(PlayerPrefs.GetInt("VehicleStyleIndex", (m_vehicleStyleIndex >= 0) ? m_vehicleStyleIndex : num), 0, VehicleFactory.NumStyles);
		m_timeSinceReviewPrompt = PlayerPrefs.GetFloat("TimeSinceReviewPrompt", m_timeSinceReviewPrompt);
		m_reviewOffered = PlayerPrefs.GetInt("ReviewOffered", m_reviewOffered ? 1 : 0) != 0;
		m_kamcordEnabled = PlayerPrefs.GetInt("KamcordEnabled", m_kamcordEnabled ? 1 : 0) != 0;
		for (int i = 0; i < m_vehicleColourIndices.Length; i++)
		{
			m_vehicleColourIndices[i] = PlayerPrefs.GetInt("VehicleColourIndex" + i, m_vehicleColourIndices[i]);
		}
	}

	private static void LazyLoad()
	{
		if (!m_loaded)
		{
			Load();
			m_loaded = true;
			Validate();
		}
	}
}
