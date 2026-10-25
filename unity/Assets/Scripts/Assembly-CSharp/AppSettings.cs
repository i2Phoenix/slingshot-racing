public static class AppSettings
{
	public static string Version
	{
		get
		{
			return UnityEngine.Application.version;
		}
	}

	public static string VersionTrimmed
	{
		get
		{
			return Version;
		}
	}

	public static string AppID
	{
		get
		{
			return "516349924";
		}
	}

	public static string AndroidPackageName
	{
		get
		{
			return "ccom.crescentmoongames.slingshotracing";
		}
	}

	public static string AndroidPublicKey
	{
		get
		{
			return "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAu8bOAX5KoiESaePjoMc/5GblNnAACU7oBW06UKzUTP5bHdNGjTgT4YiFnII8dHfCFcfIFVBmzGSiv5Gsx3NA5yNsgWwbt6AR3BaxkpHe2yloo2G3t5wvX1KAb9foUayGD0k0LNlyMkCbeGkGko7K0OC6CF0ciq9YdUhg2tOgimiw1kvmeqdcUt8qhVGcasw9Hj8XuujC+bEPaQsja++UdEJty4FdoMSPU6ARgjP/HvC3CBL4n15hvzvbPdWKmZ4cj4TIcD5IGdZB9pABCyJEl2B11YkYDBQCzcaPguoej+l1i3s2pwIBoRSnmXjm/6T5fI1YyU+KvHBbbpcQ8CZIAQIDAQAB";
		}
	}

	public static string FAQURL
	{
		get
		{
			return "http://www.snowbolt.com/slingshotandroidfaq.htm";
		}
	}

	public static string CMGURL
	{
		get
		{
			return "http://crescentmoongames.com/wordpress/other-games/";
		}
	}

	public static bool SupportsResultSharing
	{
		get
		{
			return false;
		}
	}

	public static bool SupportsWebBrowser
	{
		get
		{
			return true;
		}
	}

	public static bool SupportsAppReviews
	{
		get
		{
			return false;
		}
	}

	public static bool SupportsIAP
	{
		get
		{
			return false;
		}
	}

	public static string LocSuffixGamepad
	{
		get
		{
			return "_Gamepad";
		}
	}
}
