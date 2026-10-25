using System.Runtime.InteropServices;
using UnityEngine;

public static class Platform
{
	public delegate void OnDialogResult(bool _confirmed);

	private static OnDialogResult s_dialogCallback;

	public static bool IsMoviePlayerAvailable
	{
		get
		{
			if (AndroidUtils.IsAlwaysFinishActivitiesEnabled)
			{
				return false;
			}
			return SystemInfo.supportsRenderTextures;
		}
	}

	public static bool IsAmazonStore
	{
		get
		{
			return false;
		}
	}

	public static bool DisableSimd
	{
		set
		{
			Platform_DisableSimd(value);
		}
	}

	public static bool IsSimdSupported
	{
		get
		{
			return Platform_IsSimdSupported();
		}
	}

	public static bool IsMusicPlaying()
	{
		return false;
	}

	public static void ShowActivity()
	{
		EtceteraAndroid.showProgressDialog(string.Empty, Language.Get("ID_Working"));
	}

	public static void HideActivity()
	{
		EtceteraAndroid.hideProgressDialog();
	}

	public static bool ShowDialog(string _title, string _contents, string _cancel, string _ok, OnDialogResult _callback)
	{
		if (s_dialogCallback != null)
		{
			Debug.LogError("Multiple open dialogs are not supported");
			return false;
		}
		if (GameObject.Find("PlatformManager") == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<PlatformManager>();
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.snowbolt.SystemAlert"))
		{
			AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			s_dialogCallback = _callback;
			androidJavaObject.Call("showDialog", _title, _contents, (_cancel == null) ? string.Empty : _cancel, (_ok == null) ? string.Empty : _ok);
		}
		return true;
	}

	public static void _OnDialogConfirm()
	{
		if (s_dialogCallback != null)
		{
			s_dialogCallback(true);
		}
		s_dialogCallback = null;
	}

	public static void _OnDialogCancel()
	{
		if (s_dialogCallback != null)
		{
			s_dialogCallback(false);
		}
		s_dialogCallback = null;
	}

	[DllImport("GrappleRacerNative")]
	private static extern bool Platform_IsSimdSupported();

	[DllImport("GrappleRacerNative")]
	private static extern void Platform_DisableSimd(bool disable);
}
