using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class PlatformSettings
{
	public enum Generation
	{
		Unknown = 0,
		Unsupported = 1,
		iOS_3rdGenPhone = 2,
		iOS_4thGenPhone = 3,
		iOS_1stGenTablet = 4,
		iOS_5thGenPhone = 5,
		iOS_2ndGenTablet = 6,
		iOS_3rdGenTablet = 7,
		iOS_6thGenPhone = 8
	}

	public enum Resolution
	{
		Low = 0,
		Medium = 1,
		High = 2,
		ExtraHigh = 3
	}

	public enum ShaderLod
	{
		Low = 150,
		Medium = 200,
		High = 250
	}

	public enum ParticleDetail
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	private static bool s_init;

	private static ParticleDetail s_particleEffects = ParticleDetail.High;

	private static bool s_constantExtraction;

	private static bool s_enableLightShafts = true;

	private static bool s_enableClouds = true;

	private static int s_maxBackbufferPixels;

	private static int s_originalWidth;

	private static int s_originalHeight;

	private static int s_settingsVersion;

	public static bool ConstantExtraction
	{
		get
		{
			return s_constantExtraction;
		}
	}

	public static bool EnableLightShafts
	{
		get
		{
			return s_enableLightShafts;
		}
	}

	public static bool EnableClouds
	{
		get
		{
			return s_enableClouds;
		}
	}

	public static ParticleDetail ParticleEffects
	{
		get
		{
			return s_particleEffects;
		}
	}

	public static int SettingsVersion
	{
		get
		{
			return s_settingsVersion;
		}
	}

	public static Resolution NativeResolution
	{
		get
		{
			if (s_originalWidth > 1280)
			{
				return Resolution.ExtraHigh;
			}
			if (s_originalWidth > 800)
			{
				return Resolution.High;
			}
			if (s_originalWidth > 480)
			{
				return Resolution.Medium;
			}
			return Resolution.Low;
		}
	}

	private static string PersistentSettingsPath
	{
		get
		{
			return Application.persistentDataPath + "/slingshotsettings.txt";
		}
	}

	private static string OverrideSettingsPath
	{
		get
		{
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Environment"))
				{
					using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]))
					{
						return androidJavaObject.Call<string>("getAbsolutePath", new object[0]) + "/slingshotoverrides.txt";
					}
				}
			}
			catch
			{
				return string.Empty;
			}
		}
	}

	private static string ResourceSettingsPath
	{
		get
		{
			return "slingshotsettings_android";
		}
	}

	public static void Init()
	{
		if (s_init)
		{
			return;
		}
		s_init = true;
		s_originalWidth = Screen.width;
		s_originalHeight = Screen.height;
		Debug.Log("Device: " + SystemInfo.deviceModel + ", GPU: " + SystemInfo.graphicsDeviceName + ", GPU Vendor: " + SystemInfo.graphicsDeviceVendor);
		bool flag = true;
		if (File.Exists(OverrideSettingsPath))
		{
			Debug.Log("Loading override platform settings");
			ParseAndApplySettings(File.ReadAllText(OverrideSettingsPath));
			s_settingsVersion = -1;
			flag = false;
		}
		if (flag)
		{
			Debug.Log("Loading default platform settings");
			TextAsset textAsset = Resources.Load(ResourceSettingsPath) as TextAsset;
			if (textAsset != null)
			{
				ParseAndApplySettings(textAsset.text);
			}
			Resources.UnloadAsset(textAsset);
		}
	}

	public static void ApplyResolution()
	{
		Init();
		if (s_maxBackbufferPixels == 0)
		{
			return;
		}
		int num = s_originalWidth;
		int num2 = s_originalHeight;
		int num3;
		for (num3 = 8; num3 != 1; num3--)
		{
			int num4 = num * num2 * num3 * num3 / 64;
			if (num4 <= s_maxBackbufferPixels)
			{
				break;
			}
		}
		Screen.SetResolution(num * num3 / 8, num2 * num3 / 8, true);
	}

	public static void ApplySettings()
	{
		Init();
	}

	private static void ParseAndApplySettings(string text)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["antiAliasing"] = 0;
		hashtable["frameRate"] = 0;
		hashtable["shaderLod"] = ShaderLod.High;
		hashtable["particleEffects"] = ParticleDetail.High;
		hashtable["clouds"] = true;
		hashtable["trailShadows"] = true;
		hashtable["windSnow"] = true;
		hashtable["nightLightEffects"] = true;
		hashtable["grappleSteam"] = true;
		hashtable["lightShafts"] = true;
		hashtable["constantExtraction"] = false;
		hashtable["maxBackbufferPixels"] = 0;
		hashtable["disableSimd"] = true;
		hashtable["frameRateSmoothing"] = false;
		ParseSettings(hashtable, text);
		int num = (int)hashtable["frameRate"];
		QualitySettings.vSyncCount = ((num <= 0) ? 1 : 0);
		Application.targetFrameRate = ((num <= 0) ? (-1) : num);
		ScreenSettings.AntiAliasingMode = (int)hashtable["antiAliasing"];
		Shader.globalMaximumLOD = (int)hashtable["shaderLod"];
		s_particleEffects = (ParticleDetail)(int)hashtable["particleEffects"];
		s_enableClouds = (bool)hashtable["clouds"];
		VehicleVisuals.s_renderTrailShadows = (bool)hashtable["trailShadows"];
		WindSnow.s_windSnowEnabled = (bool)hashtable["windSnow"];
		StaticLightFlare.s_staticLightFlaresEnabled = (bool)hashtable["nightLightEffects"];
		StaticSpotlightFan.s_staticSpotlightFansEnabled = (bool)hashtable["nightLightEffects"];
		LightAttachment.s_lightAttachmentsEnabled = (bool)hashtable["nightLightEffects"];
		GrappleSteamRenderer.s_enabled = (bool)hashtable["grappleSteam"];
		s_enableLightShafts = (bool)hashtable["lightShafts"];
		s_constantExtraction = (bool)hashtable["constantExtraction"];
		s_maxBackbufferPixels = (int)hashtable["maxBackbufferPixels"];
		Platform.DisableSimd = (bool)hashtable["disableSimd"];
		TimeManager.EnableSmoothing = (bool)hashtable["frameRateSmoothing"];
		Shader.EnableKeyword((!ConstantExtraction) ? "DISABLE_CONSTANT_EXTRACTION" : "ENABLE_CONSTANT_EXTRACTION");
		Shader.DisableKeyword((!ConstantExtraction) ? "ENABLE_CONSTANT_EXTRACTION" : "DISABLE_CONSTANT_EXTRACTION");
		ScreenSettings.Apply();
	}

	private static void ParseSettings(Hashtable newSettings, string text)
	{
		string text2 = SystemInfo.graphicsDeviceName;
		int num = text2.IndexOf(" / Chainfire3D");
		if (num >= 0)
		{
			text2 = text2.Substring(0, num);
		}
		Hashtable hashtable = new Hashtable();
		hashtable["device"] = SystemInfo.deviceModel.ToLower();
		hashtable["gpu"] = text2.ToLower();
		hashtable["gpuVendor"] = SystemInfo.graphicsDeviceVendor.ToLower();
		hashtable["resolution"] = NativeResolution.ToString().ToLower();
		hashtable["iPhoneGeneration"] = string.Empty;
		Hashtable hashtable2 = MiniJSON.jsonDecode(text) as Hashtable;
		if (hashtable2 == null)
		{
			Debug.Log("Error parsing slingshotsettings JSON: " + MiniJSON.getLastErrorSnippet());
			return;
		}
		s_settingsVersion = ((!hashtable2.ContainsKey("version")) ? (-1) : ((int)(double)hashtable2["version"]));
		ArrayList arrayList = hashtable2["platformSettings"] as ArrayList;
		if (arrayList == null)
		{
			Debug.Log("Bad platformSettings list");
			return;
		}
		foreach (object item in arrayList)
		{
			Hashtable hashtable3 = item as Hashtable;
			ArrayList arrayList2;
			Hashtable hashtable4;
			if (hashtable3 == null || (arrayList2 = hashtable3["match"] as ArrayList) == null || (hashtable4 = hashtable3["settings"] as Hashtable) == null)
			{
				Debug.Log("Bad platformSettings entry");
				continue;
			}
			bool flag = false;
			foreach (object item2 in arrayList2)
			{
				Hashtable hashtable5 = item2 as Hashtable;
				if (hashtable5 == null)
				{
					Debug.Log("Bad platformSettings match table");
					continue;
				}
				int num2 = 0;
				foreach (object key in hashtable5.Keys)
				{
					if (!hashtable.ContainsKey(key))
					{
						Debug.Log("Bad match key '" + key.ToString() + "'");
						break;
					}
					if (hashtable5[key].ToString().ToLower() == hashtable[key].ToString())
					{
						num2++;
					}
				}
				if (num2 != hashtable5.Keys.Count)
				{
					continue;
				}
				Debug.Log("Matched: " + MiniJSON.jsonEncode(hashtable5));
				flag = true;
				break;
			}
			if (!flag)
			{
				continue;
			}
			foreach (object key2 in hashtable4.Keys)
			{
				if (!newSettings.ContainsKey(key2))
				{
					Debug.Log("Unknown settings key '" + key2.ToString() + "'");
					continue;
				}
				object obj = hashtable4[key2];
				Type type = newSettings[key2].GetType();
				Type type2 = obj.GetType();
				if (type2 != type)
				{
					if (type.IsEnum)
					{
						obj = Enum.Parse(type, obj.ToString(), true);
					}
					else if (type == typeof(int) && type2 == typeof(double))
					{
						obj = (int)(double)obj;
					}
				}
				if (obj.GetType() != type)
				{
					Debug.Log(string.Concat("Bad type for '", key2, ": ", obj));
				}
				else if (flag)
				{
					newSettings[key2] = obj;
				}
			}
		}
	}
}
