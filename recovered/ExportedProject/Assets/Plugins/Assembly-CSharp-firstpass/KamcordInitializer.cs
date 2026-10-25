using UnityEngine;

public class KamcordInitializer : MonoBehaviour
{
	public string developerKey = "Kamcord developer key";

	public string developerSecret = "Kamcord developer secret";

	public string appName = "Application name";

	public Kamcord.DeviceOrientation deviceOrientation;

	public Kamcord.VideoResolution videoResolution = Kamcord.VideoResolution.Medium;

	public bool disableIpod4Gen;

	public bool disableIpod5Gen;

	public bool disableIphone3GS;

	public bool disableIphone4;

	public bool disableIpad1;

	public bool disableIpad2;

	public bool disableIpadMini;

	public bool enableAutoAudioListener;
}
