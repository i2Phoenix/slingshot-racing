using System.IO;
using UnityEngine;

public static class CloudUtils
{
	public static bool DocumentStoreAvailable
	{
		get
		{
			return false;
		}
	}

	public static string LocalFilePath(string _fileName)
	{
		string path = string.Format("./{0}", _fileName);
		return Path.Combine(Application.persistentDataPath, path);
	}

	public static string CloudFilePath(string _fileName)
	{
		return null;
	}

	public static void EvictCloudFile(string _fileName)
	{
	}

	public static void DeleteUnmanagedCloudFile(string _fileName)
	{
	}

	public static bool CopyFileToCloud(string _fileName)
	{
		return false;
	}

	public static void StartDownload(string _fileName)
	{
	}

	public static bool IsFileInCloud(string _fileName)
	{
		return false;
	}

	public static bool IsFileDownloaded(string _fileName)
	{
		return false;
	}
}
