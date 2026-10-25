using System;
using UnityEngine;

internal class AndroidUtils
{
	private static int s_apiLevel;

	public static int ApiLevel
	{
		get
		{
			if (Application.platform != RuntimePlatform.Android) return 0;
			if (s_apiLevel == 0)
			{
				IntPtr clazz = AndroidJNI.FindClass("android/os/Build$VERSION");
				IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, "SDK_INT", "I");
				s_apiLevel = (int)AndroidJNI.GetStaticIntField(clazz, staticFieldID);
				if (AndroidJNI.ExceptionOccurred() != IntPtr.Zero)
				{
					AndroidJNI.ExceptionClear();
					s_apiLevel = 8;
				}
			}
			return s_apiLevel;
		}
	}

	public static bool IsAlwaysFinishActivitiesEnabled
	{
		get
		{
			int val;
			if (GetSettingsInt("System", "ALWAYS_FINISH_ACTIVITIES", out val, 0) && val != 0)
			{
				Debug.Log("Found Settings.System.ALWAYS_FINISH_ACTIVITIES is enabled");
				return true;
			}
			return false;
		}
	}

	public static bool IsGooglePlayServicesAvailable
	{
		get
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			if (androidJavaClass == null)
			{
				Debug.Log("Unable to get 'com.unity3d.player.UnityPlayer'");
				return false;
			}
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			if (androidJavaObject == null)
			{
				Debug.Log("Unable to get 'currentActivity'");
				return false;
			}
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
			if (androidJavaObject2 == null)
			{
				Debug.Log("Unable to get application context");
				return false;
			}
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.google.android.gms.common.GooglePlayServicesUtil");
			if (androidJavaClass2 == null)
			{
				Debug.Log("Unable to get GooglePlayServicesUtil");
				return false;
			}
			int num = androidJavaClass2.CallStatic<int>("isGooglePlayServicesAvailable", new object[1] { androidJavaObject2 });
			Debug.Log("isGooglePlayServicesAvailable returned " + num);
			return num == 0;
		}
	}

	public static bool GetSettingsInt(string subSystem, string key, out int val, int defaultVal)
	{
		if (Application.platform != RuntimePlatform.Android) { val = defaultVal; return false; }
		AndroidJavaClass settingsClass = GetSettingsClass(subSystem);
		AndroidJavaObject contentResolver = GetContentResolver();
		if (settingsClass != null && contentResolver != null)
		{
			val = settingsClass.CallStatic<int>("getInt", new object[3]
			{
				contentResolver,
				settingsClass.GetStatic<string>(key),
				defaultVal
			});
			if (AndroidJNI.ExceptionOccurred() == IntPtr.Zero)
			{
				return true;
			}
			AndroidJNI.ExceptionClear();
		}
		val = defaultVal;
		return false;
	}

	public static bool PutSettingsInt(string subSystem, string key, int val)
	{
		if (Application.platform != RuntimePlatform.Android) return false;
		AndroidJavaClass settingsClass = GetSettingsClass(subSystem);
		AndroidJavaObject contentResolver = GetContentResolver();
		if (settingsClass != null && contentResolver != null)
		{
			bool result = settingsClass.CallStatic<bool>("putInt", new object[3]
			{
				contentResolver,
				settingsClass.GetStatic<string>(key),
				val
			});
			if (AndroidJNI.ExceptionOccurred() == IntPtr.Zero)
			{
				return result;
			}
			AndroidJNI.ExceptionClear();
		}
		return false;
	}

	private static AndroidJavaClass GetSettingsClass(string subSystem)
	{
		string text = "android.provider.Settings$" + subSystem;
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(text);
		if (androidJavaClass == null)
		{
			Debug.Log("Unable to get '" + text + "'");
		}
		return androidJavaClass;
	}

	private static AndroidJavaObject GetContentResolver()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		if (androidJavaClass == null)
		{
			Debug.Log("Unable to get 'com.unity3d.player.UnityPlayer'");
			return null;
		}
		AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		if (androidJavaObject == null)
		{
			Debug.Log("Unable to get 'currentActivity'");
			return null;
		}
		AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getContentResolver", new object[0]);
		if (androidJavaObject2 == null)
		{
			Debug.Log("Unable to get content resolver");
			return null;
		}
		return androidJavaObject2;
	}
}
