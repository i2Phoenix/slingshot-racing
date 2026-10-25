using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootScene : MonoBehaviour
{
	public string m_sceneToLoad = "TitleScreen";

	public float m_delay;

	public bool m_collectGarbage = true;

	public bool m_clearPNGsFromDocumentsFolder = true;

	public GameObject[] m_commonPrefabs;

	public GameObject[] m_iOSPrefabs;

	public GameObject[] m_googlePlayPrefabs;

	public GameObject[] m_amazonPrefabs;

	public GameObject m_kamcordPrefab;

	private IEnumerator Start()
	{
		Debug.Log("Reached boot scene");
		KamcordGameSettings.Init();
		PlatformSettings.Init();
		UIUtils.SetLowProfileMode(true);
		Analytics.Init();
		Dictionary<string, string> bootInfo = new Dictionary<string, string>();
		bootInfo["device"] = SystemInfo.deviceModel;
		bootInfo["graphicsDevice"] = SystemInfo.graphicsDeviceName;
		bootInfo["graphicsVendor"] = SystemInfo.graphicsDeviceVendor;
		bootInfo["resolution"] = PlatformSettings.NativeResolution.ToString();
		bootInfo["screenWidth"] = Screen.width.ToString();
		bootInfo["screenHeight"] = Screen.height.ToString();
		bootInfo["operatingSystem"] = SystemInfo.operatingSystem.ToString();
		bootInfo["simdSupported"] = Platform.IsSimdSupported.ToString();
		bootInfo["androidAPI"] = AndroidUtils.ApiLevel.ToString();
		Analytics.LogEvent("Boot", bootInfo);
		AutoRotation.Init();
		GameObject[] commonPrefabs = m_commonPrefabs;
		foreach (GameObject prefab in commonPrefabs)
		{
			UnityEngine.Object.Instantiate(prefab);
		}
		yield return null;
		GameObject[] prefabs = null;
		if (prefabs != null)
		{
			GameObject[] array = prefabs;
			foreach (GameObject prefab2 in array)
			{
				UnityEngine.Object.Instantiate(prefab2);
			}
			yield return null;
		}
		RateAppPrompt.CheckUnlocks();
		if (m_delay > 0f)
		{
			yield return new WaitForSeconds(m_delay);
		}
		yield return YieldCache.WaitForEndOfFrame;
		if (m_collectGarbage)
		{
			GC.Collect();
		}
		Debug.Log(string.Format("Loading '{0}'", m_sceneToLoad));
		Application.LoadLevel(m_sceneToLoad);
	}
}
