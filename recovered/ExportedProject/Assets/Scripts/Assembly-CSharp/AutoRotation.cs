using UnityEngine;

public class AutoRotation : MonoBehaviour
{
	private static AutoRotation s_instance;

	public static void Init()
	{
		if (s_instance == null)
		{
			GameObject gameObject = new GameObject("AutoRotation");
			s_instance = gameObject.AddComponent<AutoRotation>();
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		if (s_instance == this)
		{
			s_instance = null;
		}
	}

	private void Start()
	{
		SetupAutoRotation();
	}

	private void OnApplicationPause(bool _pause)
	{
		if (!_pause)
		{
			SetupAutoRotation();
		}
	}

	private void SetupAutoRotation()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.provider.Settings$System"))
		{
			using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				if (androidJavaClass == null)
				{
					Debug.Log("Unable to get 'android.provider.Settings$System'", this);
					return;
				}
				if (androidJavaClass2 == null)
				{
					Debug.Log("Unable to get 'com.unity3d.player.UnityPlayer'", this);
					return;
				}
				AndroidJavaObject androidJavaObject = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
				if (androidJavaObject == null)
				{
					Debug.Log("Unable to get 'currentActivity'", this);
					return;
				}
				AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getContentResolver", new object[0]);
				if (androidJavaObject2 == null)
				{
					Debug.Log("Unable to get content resolver", this);
					return;
				}
				string text = androidJavaClass.GetStatic<string>("ACCELEROMETER_ROTATION");
				if (androidJavaClass.CallStatic<int>("getInt", new object[2] { androidJavaObject2, text }) == 0)
				{
					ScreenOrientation screenOrientation = Screen.orientation;
					if (screenOrientation == ScreenOrientation.AutoRotation)
					{
						screenOrientation = ScreenOrientation.LandscapeLeft;
					}
					Screen.orientation = screenOrientation;
				}
				else
				{
					Screen.orientation = ScreenOrientation.AutoRotation;
				}
			}
		}
	}
}
