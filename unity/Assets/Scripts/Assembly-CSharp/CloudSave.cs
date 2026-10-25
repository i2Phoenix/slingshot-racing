using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public abstract class CloudSave : MonoBehaviour
{
	protected enum LoadStatus
	{
		Failed = 0,
		Corrupted = 1,
		Success = 2
	}

	protected struct SaveHeader
	{
		private static readonly byte[] s_fourCC = CoreUtils.MakeFourCC("RCSV");

		private int m_saveVersion;

		public int SaveVersion
		{
			get
			{
				return m_saveVersion;
			}
		}

		public SaveHeader(int _saveVersion)
		{
			m_saveVersion = _saveVersion;
		}

		public bool Serialise(ISerialiser _serialiser)
		{
			if (!_serialiser.FourCC(s_fourCC))
			{
				return false;
			}
			_serialiser.Serialise(ref m_saveVersion);
			return _serialiser.Sentinal();
		}
	}

	protected bool m_canSaveToCloud;

	protected abstract int SaveVersion { get; }

	protected abstract string Filename { get; }

	protected abstract bool ReadyForCloudSync { get; }

	protected virtual void Awake()
	{
	}

	protected virtual void OnDestroy()
	{
	}

	protected virtual void Load()
	{
		CloudUtils.DeleteUnmanagedCloudFile(Filename);
		string text = CloudUtils.LocalFilePath(Filename);
		string text2 = CloudUtils.CloudFilePath(Filename);
		bool documentStoreAvailable = CloudUtils.DocumentStoreAvailable;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (documentStoreAvailable)
		{
			if (File.Exists(text2))
			{
				flag = CloudUtils.IsFileInCloud(Filename);
				flag2 = CloudUtils.IsFileDownloaded(Filename);
			}
			if (!File.Exists(text) && flag && flag2)
			{
				try
				{
					Debug.Log(string.Format("Save: Attempting to restore save from cloud {0}", Filename));
					File.Copy(text2, text);
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.ToString());
				}
				if (File.Exists(text))
				{
					flag3 = true;
					Debug.Log("Save: Save restored");
				}
				else
				{
					Debug.Log("Save: Failed to restore save");
				}
			}
		}
		LoadStatus loadStatus = Load(text, false);
		LoadStatus loadStatus2 = LoadStatus.Failed;
		if (flag3)
		{
			loadStatus2 = loadStatus;
		}
		else if (flag && flag2)
		{
			loadStatus2 = Load(text2, true);
		}
		m_canSaveToCloud = documentStoreAvailable && (!flag || loadStatus2 != LoadStatus.Failed);
		if (m_canSaveToCloud)
		{
			Debug.Log("Save: Cloud saving is enabled");
		}
		else
		{
			Debug.Log("Save: Cloud saving is disabled");
		}
	}

	protected virtual LoadStatus Load(string _path, bool _additive)
	{
		if (File.Exists(_path))
		{
			Debug.Log(string.Format("Save: Attempting to load file {0} (additive={1})", _path, _additive));
			using (FileStream fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (CryptoStream stream = ((fileStream != null) ? new CryptoStream(fileStream, new TripleDESCryptoServiceProvider().CreateDecryptor(Crypto.s_key, Crypto.s_IV), CryptoStreamMode.Read) : null))
				{
					using (ReadSerialiser readSerialiser = new ReadSerialiser(stream))
					{
						if (!readSerialiser.IsValid())
						{
							Debug.Log("Save: Unable to read file");
							return LoadStatus.Failed;
						}
						try
						{
							SaveHeader saveHeader = default(SaveHeader);
							if (!saveHeader.Serialise(readSerialiser))
							{
								Debug.LogError("Save: Bad header");
								return LoadStatus.Corrupted;
							}
							if (saveHeader.SaveVersion != SaveVersion)
							{
								Debug.LogError("Save: Invalid save version");
								return LoadStatus.Failed;
							}
							LoadStatus loadStatus = Load(readSerialiser, _additive);
							if (loadStatus != LoadStatus.Success)
							{
								return loadStatus;
							}
							Debug.Log("Save: Finished loading");
						}
						catch (CryptographicException ex)
						{
							Debug.LogError(ex.ToString());
							return LoadStatus.Corrupted;
						}
						catch (Exception ex2)
						{
							Debug.LogError(ex2.ToString());
							return LoadStatus.Failed;
						}
					}
				}
			}
			return LoadStatus.Success;
		}
		Debug.Log(string.Format("Save: Save does not exist {0}", _path));
		return LoadStatus.Failed;
	}

	protected virtual void Save()
	{
		string text = CloudUtils.LocalFilePath(Filename);
		Debug.Log(string.Format("Save: Attempting to save file {0}", text));
		using (FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write))
		{
			using (CryptoStream cryptoStream = ((fileStream != null) ? new CryptoStream(fileStream, new TripleDESCryptoServiceProvider().CreateEncryptor(Crypto.s_key, Crypto.s_IV), CryptoStreamMode.Write) : null))
			{
				using (WriteSerialiser writeSerialiser = new WriteSerialiser(cryptoStream))
				{
					if (!writeSerialiser.IsValid())
					{
						Debug.Log("Save: Unable to write file");
						return;
					}
					try
					{
						if (!new SaveHeader(SaveVersion).Serialise(writeSerialiser))
						{
							Debug.LogError("Save: Bad header");
							return;
						}
						if (!Save(writeSerialiser))
						{
							Debug.LogError("Save: Failed to save");
							return;
						}
						if (!writeSerialiser.EndOfSerialise())
						{
							Debug.LogError("Save: Failed to end save");
							return;
						}
						cryptoStream.FlushFinalBlock();
						Debug.Log("Save: Finished saving");
					}
					catch (Exception ex)
					{
						Debug.LogError(ex.ToString());
					}
				}
			}
		}
		if (!m_canSaveToCloud || !CloudUtils.DocumentStoreAvailable)
		{
			return;
		}
		try
		{
			Debug.Log(string.Format("Save: Attempting to copy file to cloud {0}", Filename));
			if (CloudUtils.CopyFileToCloud(Filename))
			{
				Debug.Log("Save: File copied to cloud");
			}
			else
			{
				Debug.Log("Save: Failed to copy file to cloud");
			}
		}
		catch (Exception ex2)
		{
			Debug.LogError(ex2.ToString());
		}
	}

	public virtual void CloudSync()
	{
		if (!CloudUtils.DocumentStoreAvailable || !ReadyForCloudSync || m_canSaveToCloud)
		{
			return;
		}
		CloudUtils.DeleteUnmanagedCloudFile(Filename);
		string path = CloudUtils.CloudFilePath(Filename);
		bool flag = false;
		bool flag2 = false;
		LoadStatus loadStatus = LoadStatus.Failed;
		if (File.Exists(path))
		{
			flag = CloudUtils.IsFileInCloud(Filename);
			flag2 = CloudUtils.IsFileDownloaded(Filename);
			if (flag && flag2)
			{
				loadStatus = Load(path, true);
			}
		}
		m_canSaveToCloud = !flag || loadStatus != LoadStatus.Failed;
		if (m_canSaveToCloud)
		{
			Debug.Log("Save: Cloud saving is enabled");
			OnSaveDataUpdated();
		}
		else
		{
			Debug.Log("Save: Cloud saving is disabled");
		}
	}

	protected abstract LoadStatus Load(ReadSerialiser _serialiser, bool _additive);

	protected abstract bool Save(WriteSerialiser _serialiser);

	protected abstract void OnSaveDataUpdated();
}
