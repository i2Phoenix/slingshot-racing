using UnityEngine;

public class AppStoreRating : MonoBehaviour
{
	private const string c_templateURL = "http://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id={0}";

	public void OpenAppStoreRatingLink()
	{
		string url = string.Format("http://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id={0}", AppSettings.AppID);
		Application.OpenURL(url);
	}
}
