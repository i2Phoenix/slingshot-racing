using System;
using Prime31;
using UnityEngine;

public class FacebookManager : MonoBehaviour
{
	public static event Action loginSucceededEvent;

	public static event Action preLoginSucceededEvent;

	public static event Action<string> loginFailedEvent;

	public static event Action loggedOutEvent;

	public static event Action<DateTime> accessTokenExtendedEvent;

	public static event Action failedToExtendTokenEvent;

	public static event Action sessionInvalidatedEvent;

	public static event Action dialogCompletedEvent;

	public static event Action<string> dialogFailedEvent;

	public static event Action dialogDidNotCompleteEvent;

	public static event Action<string> dialogCompletedWithUrlEvent;

	public static event Action<object> customRequestReceivedEvent;

	public static event Action<string> customRequestFailedEvent;

	public static event Action<bool> facebookComposerCompletedEvent;

	public static event Action<string> facebookReauthorizationWithReadPermissionsFailedEvent;

	public static event Action facebookReauthorizationWithReadPermissionsSucceededEvent;

	public static event Action<string> facebookReauthorizationWithPublishPermissionsFailedEvent;

	public static event Action facebookReauthorizationWithPublishPermissionsSucceededEvent;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void facebookLoginSucceeded(string empty)
	{
		if (preLoginSucceededEvent != null)
		{
			preLoginSucceededEvent();
		}
		if (loginSucceededEvent != null)
		{
			loginSucceededEvent();
		}
	}

	public void facebookLoginDidFail(string error)
	{
		if (loginFailedEvent != null)
		{
			loginFailedEvent(error);
		}
	}

	public void facebookDidLogout(string empty)
	{
		if (loggedOutEvent != null)
		{
			loggedOutEvent();
		}
	}

	public void facebookDidExtendToken(string secondsSinceEpoch)
	{
		if (accessTokenExtendedEvent != null)
		{
			double value = double.Parse(secondsSinceEpoch);
			DateTime obj = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(value);
			accessTokenExtendedEvent(obj);
		}
	}

	public void facebookFailedToExtendToken(string empty)
	{
		if (failedToExtendTokenEvent != null)
		{
			failedToExtendTokenEvent();
		}
	}

	public void facebookSessionInvalidated(string empty)
	{
		if (sessionInvalidatedEvent != null)
		{
			sessionInvalidatedEvent();
		}
	}

	public void facebookDialogDidComplete(string empty)
	{
		if (dialogCompletedEvent != null)
		{
			dialogCompletedEvent();
		}
	}

	public void facebookDialogDidCompleteWithUrl(string url)
	{
		if (dialogCompletedWithUrlEvent != null)
		{
			dialogCompletedWithUrlEvent(url);
		}
	}

	public void facebookDialogDidNotComplete(string empty)
	{
		if (dialogDidNotCompleteEvent != null)
		{
			dialogDidNotCompleteEvent();
		}
	}

	public void facebookDialogDidFailWithError(string error)
	{
		if (dialogFailedEvent != null)
		{
			dialogFailedEvent(error);
		}
	}

	public void facebookDidReceiveCustomRequest(string result)
	{
		if (customRequestReceivedEvent != null)
		{
			object obj = Json.jsonDecode(result);
			customRequestReceivedEvent(obj);
		}
	}

	public void facebookCustomRequestDidFail(string error)
	{
		if (customRequestFailedEvent != null)
		{
			customRequestFailedEvent(error);
		}
	}

	public void facebookComposerCompleted(string result)
	{
		if (facebookComposerCompletedEvent != null)
		{
			facebookComposerCompletedEvent(result == "1");
		}
	}

	public void facebookReauthorizationWithReadPermissionsFailed(string error)
	{
		if (facebookReauthorizationWithReadPermissionsFailedEvent != null)
		{
			facebookReauthorizationWithReadPermissionsFailedEvent(error);
		}
	}

	public void facebookReauthorizationWithReadPermissionsSucceeded(string empty)
	{
		if (facebookReauthorizationWithReadPermissionsSucceededEvent != null)
		{
			facebookReauthorizationWithReadPermissionsSucceededEvent();
		}
	}

	public void facebookReauthorizationWithPublishPermissionsFailed(string error)
	{
		if (facebookReauthorizationWithPublishPermissionsFailedEvent != null)
		{
			facebookReauthorizationWithPublishPermissionsFailedEvent(error);
		}
	}

	public void facebookReauthorizationWithPublishPermissionsSucceeded(string empty)
	{
		if (facebookReauthorizationWithPublishPermissionsSucceededEvent != null)
		{
			facebookReauthorizationWithPublishPermissionsSucceededEvent();
		}
	}
}
