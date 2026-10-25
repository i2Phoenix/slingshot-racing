using System;
using UnityEngine;

public class RateAppPrompt : AppStoreRating
{
	private enum ReviewOption
	{
		Now = 0,
		Postpone = 1,
		Never = 2
	}

	private const int c_vehicleStyleIndex = 8;

	public string m_reviewContext;

	private string[] ButtonNames
	{
		get
		{
			return new string[3]
			{
				Language.Get("ID_RateAppRateNow"),
				Language.Get("ID_RateAppRemind"),
				Language.Get("ID_RateAppNever")
			};
		}
	}

	private bool IsReviewSupported
	{
		get
		{
			if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
			{
				return false;
			}
			return AppSettings.SupportsAppReviews;
		}
	}

	public event Action<Settings.RatedType> OnRatingSelected;

	public static void Reset()
	{
		Settings.Rated = Settings.RatedType.NotYetRated;
		Settings.TimeSinceReviewPrompt = 0f;
		Settings.ReviewOffered = false;
		GlobalSave.RevokeVehicleStyle(8);
	}

	public static void CheckUnlocks()
	{
		if (Settings.Rated == Settings.RatedType.HasRated || !AppSettings.SupportsAppReviews)
		{
			GlobalSave.UnlockVehicleStyle(8);
			Settings.ReviewOffered = true;
		}
		else if (Settings.Rated == Settings.RatedType.WillNotRate)
		{
			Settings.ReviewOffered = true;
		}
	}

	private void Start()
	{
		CheckUnlocks();
	}

	private void OnReviewSelected()
	{
		Log("Review: Post review selected");
		Settings.Rated = Settings.RatedType.HasRated;
		GlobalSave.UnlockVehicleStyle(8);
		LogEvent("HasRated");
		UnregisterReviewEvents();
		if (OnRatingSelected != null)
		{
			OnRatingSelected(Settings.RatedType.HasRated);
		}
	}

	private void OnReviewPostponed()
	{
		Log("Review: Postpone review selected");
		LogEvent("Postponed");
		UnregisterReviewEvents();
		if (OnRatingSelected != null)
		{
			OnRatingSelected(Settings.RatedType.NotYetRated);
		}
	}

	private void OnReviewNever()
	{
		Log("Review: Never review selected");
		Settings.Rated = Settings.RatedType.WillNotRate;
		LogEvent("WillNotRate");
		UnregisterReviewEvents();
		if (OnRatingSelected != null)
		{
			OnRatingSelected(Settings.RatedType.WillNotRate);
		}
	}

	private void OnButtonClicked(string _name)
	{
		string[] buttonNames = ButtonNames;
		if (_name == buttonNames[0])
		{
			OnReviewSelected();
		}
		else if (_name == buttonNames[1])
		{
			OnReviewPostponed();
		}
		else
		{
			OnReviewNever();
		}
	}

	public bool PresentDialog(float _delay)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			Log("Review: player does not have internet connectivity");
			return false;
		}
		Log("Review: Presenting dialog");
		Settings.ReviewOffered = true;
		if (!IsReviewSupported)
		{
			Debug.Log("Review prompt will not be displayed since this will only work on target HW");
			return false;
		}
		RegisterReviewEvents();
		Invoke("_PresentDialog", _delay);
		return true;
	}

	private void _PresentDialog()
	{
		LogEvent("Offered");
		EtceteraAndroid.resetAskForReview();
		EtceteraAndroid.askForReviewNow(Language.Get("ID_RateAppTitle"), Language.Get("ID_RateAppDescriptionOffer"), Platform.IsAmazonStore);
	}

	private void RegisterReviewEvents()
	{
		UIManager.instance.LockInput();
		EtceteraAndroidManager.askForReviewWillOpenMarketEvent += OnReviewSelected;
		EtceteraAndroidManager.askForReviewRemindMeLaterEvent += OnReviewPostponed;
		EtceteraAndroidManager.askForReviewDontAskAgainEvent += OnReviewNever;
	}

	private void UnregisterReviewEvents()
	{
		UIManager.instance.UnlockInput();
		EtceteraAndroidManager.askForReviewWillOpenMarketEvent -= OnReviewSelected;
		EtceteraAndroidManager.askForReviewRemindMeLaterEvent -= OnReviewPostponed;
		EtceteraAndroidManager.askForReviewDontAskAgainEvent -= OnReviewNever;
	}

	private void Log(string _str)
	{
	}

	private void LogEvent(string _type)
	{
		Analytics.LogEvent("AppReview", "Type", _type, "Context", m_reviewContext);
	}
}
