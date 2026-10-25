using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GPCloudStore
{
	private enum State
	{
		WaitingForAuthorisation = 0,
		Disabled = 1,
		Authorised = 2,
		WaitingForLUT = 3,
		ProcessingLUT = 4,
		Ready = 5
	}

	private const int c_maxKeys = 4;

	private const string c_keyNotFoundError = "2002";

	private static Dictionary<string, int> s_keys;

	private static Dictionary<string, bool> s_files;

	private static List<string> s_filesToDownload;

	private static bool s_googlePlayGameServicesAvailable;

	private static State s_state;

	private static int s_currentKey;

	private static bool IsReady
	{
		get
		{
			return s_state == State.Ready;
		}
	}

	public static event Action<string, bool> documentStoreUpdatedEvent;

	static GPCloudStore()
	{
		s_keys = new Dictionary<string, int>();
		s_files = new Dictionary<string, bool>();
		s_filesToDownload = new List<string>();
		s_state = State.WaitingForAuthorisation;
		s_googlePlayGameServicesAvailable = AndroidUtils.IsGooglePlayServicesAvailable;
		Directory.CreateDirectory(documentsDirectory());
	}

	public static void Startup()
	{
		GPGManager.authenticationSucceededEvent += OnAuthenticationSucceeded;
		GPGManager.authenticationFailedEvent += OnAuthenticationFailed;
		GPGManager.userSignedOutEvent += OnUserSignedOut;
		GPGManager.loadCloudDataForKeyFailedEvent += OnLoadCloudDataForKeyFailed;
		GPGManager.loadCloudDataForKeySucceededEvent += OnLoadCloudDataForKeySucceeded;
		GPGManager.updateCloudDataForKeyFailedEvent += OnUpdateCloudDataForKeyFailed;
		GPGManager.updateCloudDataForKeySucceededEvent += OnUpdateCloudDataForKeySucceeded;
		GPGManager.clearCloudDataForKeyFailedEvent += OnClearCloudDataForKeyFailed;
		GPGManager.clearCloudDataForKeySucceededEvent += OnClearCloudDataForKeySucceeded;
		GPGManager.deleteCloudDataForKeyFailedEvent += OnDeleteCloudDataForKeyFailed;
		GPGManager.deleteCloudDataForKeySucceededEvent += OnDeleteCloudDataForKeySucceeded;
		Directory.CreateDirectory(documentsDirectory());
	}

	public static void Shutdown()
	{
		GPGManager.authenticationSucceededEvent -= OnAuthenticationSucceeded;
		GPGManager.authenticationFailedEvent -= OnAuthenticationFailed;
		GPGManager.userSignedOutEvent -= OnUserSignedOut;
		GPGManager.loadCloudDataForKeyFailedEvent -= OnLoadCloudDataForKeyFailed;
		GPGManager.loadCloudDataForKeySucceededEvent -= OnLoadCloudDataForKeySucceeded;
		GPGManager.updateCloudDataForKeyFailedEvent -= OnUpdateCloudDataForKeyFailed;
		GPGManager.updateCloudDataForKeySucceededEvent -= OnUpdateCloudDataForKeySucceeded;
		GPGManager.clearCloudDataForKeyFailedEvent -= OnClearCloudDataForKeyFailed;
		GPGManager.clearCloudDataForKeySucceededEvent -= OnClearCloudDataForKeySucceeded;
		GPGManager.deleteCloudDataForKeyFailedEvent -= OnDeleteCloudDataForKeyFailed;
		GPGManager.deleteCloudDataForKeySucceededEvent -= OnDeleteCloudDataForKeySucceeded;
	}

	public static void Refresh()
	{
		Log("GPCloudStore: Refresh");
		ClearCache();
		if (s_state >= State.Authorised)
		{
			s_state = State.WaitingForLUT;
			PlayGameServices.loadCloudDataForKey(0);
		}
	}

	public static bool isiCloudAvailable()
	{
		return s_googlePlayGameServicesAvailable;
	}

	public static bool documentStoreAvailable()
	{
		return isiCloudAvailable() && IsReady;
	}

	public static string documentsDirectory()
	{
		return Path.Combine(Application.persistentDataPath, "CloudFiles/");
	}

	public static bool isFileInCloud(string file)
	{
		Log("GPCloudStore: isFileInCloud: " + file);
		if (s_keys.ContainsKey(file))
		{
			return true;
		}
		return false;
	}

	public static bool isFileDownloaded(string file)
	{
		Log("GPCloudStore: isFileDownloaded: " + file);
		if (s_files.ContainsKey(file))
		{
			Log("GPCloudStore: File already present: " + file);
			return s_files[file];
		}
		DownloadFile(file);
		return false;
	}

	public static bool addFile(string file)
	{
		Log("GPCloudStore: addFile: " + file);
		if (AllocateCloudKey(file) > 0)
		{
			s_files[file] = true;
			return true;
		}
		LogLUT(file);
		return false;
	}

	public static void evictFile(string file, bool _deleteCloudData = true)
	{
		Log("GPCloudStore: evictFile: " + file);
		File.Delete(GetCloudPathForFile(file));
		s_files[file] = false;
		if (_deleteCloudData)
		{
			int key = s_keys[file];
			if (IsValidFileKey(key))
			{
				PlayGameServices.deleteCloudDataForKey(key);
			}
			else
			{
				LogLUT(file);
			}
			s_keys.Remove(file);
			UpdateLUT();
		}
	}

	public static bool CopyFileToCloud(string _srcFile, string file)
	{
		Log("GPCloudStore: CopyFileToCloud: " + _srcFile + " " + file);
		try
		{
			File.Copy(_srcFile, GetCloudPathForFile(file), true);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
			return false;
		}
		int cloudKey = GetCloudKey(file);
		if (IsValidFileKey(cloudKey))
		{
			byte[] bytes = File.ReadAllBytes(GetCloudPathForFile(file));
			string data = GetString(bytes);
			PlayGameServices.setStateData(data, cloudKey);
			s_files[file] = true;
			return true;
		}
		LogLUT(file);
		return false;
	}

	public static void DeleteAllCloudData()
	{
		if (!IsReady)
		{
			Debug.LogError("GPCloudStore: Not ready to delete all cloud data");
			return;
		}
		foreach (KeyValuePair<string, int> s_key in s_keys)
		{
			evictFile(s_key.Key);
		}
		ClearCache();
	}

	private static byte[] GetBytes(string _str)
	{
		byte[] result = null;
		try
		{
			char[] array = _str.ToCharArray();
			result = Convert.FromBase64CharArray(array, 0, array.Length);
		}
		catch (ArgumentNullException)
		{
			Debug.LogError("Base 64 character array is null.");
		}
		catch (FormatException)
		{
			Debug.LogError("Base 64 Char Array length is not 4 or is not an even multiple of 4.");
		}
		return result;
	}

	private static string GetString(byte[] _bytes)
	{
		long num = (long)(1.3333334f * (float)_bytes.Length);
		if (num % 4 != 0L)
		{
			num += 4 - num % 4;
		}
		char[] array = new char[num];
		Convert.ToBase64CharArray(_bytes, 0, _bytes.Length, array, 0);
		return new string(array);
	}

	private static string GetCloudPathForFile(string file)
	{
		return Path.Combine(documentsDirectory(), file);
	}

	private static void ClearCache()
	{
		s_files.Clear();
		s_keys.Clear();
	}

	private static void DownloadFile(string file)
	{
		Log("GPCloudStore: DownloadFile: " + file);
		int cloudKey = GetCloudKey(file);
		if (IsValidFileKey(cloudKey))
		{
			PlayGameServices.loadCloudDataForKey(cloudKey);
		}
		else
		{
			LogLUT(file);
		}
	}

	private static int AllocateCloudKey(string file)
	{
		Log("GPCloudStore: AllocateCloudKey: " + file);
		int num = -1;
		for (int i = 1; i < 4; i++)
		{
			if (!s_keys.ContainsValue(i))
			{
				num = i;
				break;
			}
		}
		if (num > 0)
		{
			s_keys.Add(file, num);
			UpdateLUT();
		}
		else
		{
			LogLUT(file);
		}
		return num;
	}

	private static int GetCloudKey(string file)
	{
		Log("GPCloudStore: GetCloudKey: " + file);
		int num = -1;
		if (!s_keys.ContainsKey(file))
		{
			return AllocateCloudKey(file);
		}
		return s_keys[file];
	}

	private static void BuildLUT(string _json)
	{
		Hashtable hashtable = _json.hashtableFromJson();
		s_keys.Clear();
		foreach (DictionaryEntry item in hashtable)
		{
			string text = item.Key as string;
			int num = (int)(double)item.Value;
			if (IsValidFileKey(num))
			{
				s_keys.Add(text, num);
				s_filesToDownload.Add(text);
			}
			else
			{
				LogLUT(text);
			}
		}
		s_state = State.ProcessingLUT;
		s_currentKey = -1;
		ProcessDownloadQueue(true, -1, string.Empty);
	}

	private static string LUTToJSON()
	{
		Hashtable obj = new Hashtable(s_keys);
		return obj.toJson();
	}

	private static void UpdateLUT()
	{
		if (s_keys.Count > 0)
		{
			string text = LUTToJSON();
			Log("LUT: " + text);
			PlayGameServices.setStateData(text, 0);
		}
		else
		{
			PlayGameServices.deleteCloudDataForKey(0);
		}
	}

	private static bool ProcessDownloadQueue(bool _succeeded, int _key = -1, string _error = "")
	{
		if (s_state != State.ProcessingLUT)
		{
			return false;
		}
		if (s_state == State.Disabled)
		{
			return true;
		}
		if (s_currentKey > 0 && !_succeeded)
		{
			if (string.Compare(_error, "2002", true) != 0)
			{
				s_state = State.Disabled;
				return true;
			}
			string text = NameFromCloudKey(s_currentKey);
			s_keys.Remove(text);
			Log("GPCloudStore: Removing key: " + s_currentKey + ":" + text);
		}
		if (s_filesToDownload.Count > 0)
		{
			string text2 = s_filesToDownload[s_filesToDownload.Count - 1];
			s_currentKey = s_keys[text2];
			s_filesToDownload.Remove(text2);
			DownloadFile(text2);
		}
		else
		{
			s_currentKey = -1;
			s_state = State.Ready;
		}
		return true;
	}

	private static void UpdateCloudFile(int _key, string _data)
	{
		Log("GPCloudStore: UpdateCloudFile: " + _key + " " + _data.Length);
		string text = NameFromCloudKey(_key);
		byte[] bytes = GetBytes(_data);
		File.WriteAllBytes(GetCloudPathForFile(text), bytes);
		s_files[text] = true;
	}

	private static string NameFromCloudKey(int _cloudKey)
	{
		string result = string.Empty;
		foreach (KeyValuePair<string, int> s_key in s_keys)
		{
			if (s_key.Value == _cloudKey)
			{
				result = s_key.Key;
				break;
			}
		}
		return result;
	}

	private static bool IsValidFileKey(int _key)
	{
		return _key > 0 && _key < 4;
	}

	private static void LogLUT(string _data = null)
	{
		string text = LUTToJSON();
		string text2 = ((_data == null) ? "N/A" : _data);
		Analytics.LogEvent("Cloud", "LUT", text, "Data", text2);
		Debug.LogWarning(string.Format("GPCloudStore:\nLUT: {0}\nData: {1}", text, text2));
	}

	private static void OnAuthenticationSucceeded(string param)
	{
		Log("GPCloudStore: OnAuthenticationSucceeded: " + param);
		if (s_state == State.WaitingForAuthorisation)
		{
			s_state = State.Authorised;
			Refresh();
		}
	}

	private static void OnAuthenticationFailed(string error)
	{
		Log("GPCloudStore: OnAuthenticationFailed: " + error);
		s_state = State.WaitingForAuthorisation;
		ClearCache();
	}

	private static void OnUserSignedOut()
	{
		Log("GPCloudStore: OnUserSignedOut");
		s_state = State.WaitingForAuthorisation;
		ClearCache();
	}

	private static void OnLoadCloudDataForKeyFailed(string error)
	{
		Log("GPCloudStore: OnLoadCloudDataForKeyFailed: " + error);
		if (s_state == State.WaitingForLUT && string.Compare(error, "2002", true) == 0)
		{
			Log("This hopefully means there is no LUT in the cloud but the error stuff from P31 is next to useless");
			s_state = State.Ready;
		}
		else
		{
			ProcessDownloadQueue(false, -1, error);
		}
	}

	private static void OnLoadCloudDataForKeySucceeded(int key, string data)
	{
		Log("GPCloudStore: OnLoadCloudDataForKeySucceeded:" + data);
		if (key == 0)
		{
			if (s_state != State.WaitingForLUT)
			{
				Debug.LogError("We've received a LUT update we didn't expect");
			}
			BuildLUT(data);
		}
		else
		{
			UpdateCloudFile(key, data);
			ProcessDownloadQueue(true, key, string.Empty);
		}
	}

	private static void OnUpdateCloudDataForKeyFailed(string error)
	{
		Log("GPCloudStore: OnUpdateCloudDataForKeyFailed: " + error);
	}

	private static void OnUpdateCloudDataForKeySucceeded(int key, string data)
	{
		Log("GPCloudStore: OnUpdateCloudDataForKeySucceeded: " + data);
	}

	private static void OnClearCloudDataForKeyFailed(string error)
	{
		Log("GPCloudStore: OnClearCloudDataForKeyFailed: " + error);
	}

	private static void OnClearCloudDataForKeySucceeded(string param)
	{
		Log("GPCloudStore: OnClearCloudDataForKeySucceeded: " + param);
	}

	private static void OnDeleteCloudDataForKeyFailed(string error)
	{
		Log("GPCloudStore: OnDeleteCloudDataForKeyFailed: " + error);
	}

	private static void OnDeleteCloudDataForKeySucceeded(string param)
	{
		Log("GPCloudStore: OnDeleteCloudDataForKeySucceeded: " + param);
	}

	private static void Log(string _str)
	{
	}
}
