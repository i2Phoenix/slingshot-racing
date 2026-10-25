using System;
using UnityEngine;

public class TrustedDeviceList : ScriptableObject
{
	public string[] m_trustedDevices;

	public bool IsThisDeviceTrusted()
	{
		string deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
		return Array.IndexOf(m_trustedDevices, deviceUniqueIdentifier) >= 0;
	}
}
