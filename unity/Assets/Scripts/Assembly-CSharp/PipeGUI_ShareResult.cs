using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class PipeGUI_ShareResult : MonoBehaviour
{
	private enum ScreenshotFormat
	{
		PNG = 0,
		JPG = 1
	}

	private const string c_tempScreenshotPath = "ShareScreenshot.png";

	private const float c_jpgQuality = 95f;

	private const int c_encoderBlocksPerFrame = 64;

	private const float c_startupDelay = 1f;

	private const float c_captureDelay = 0.25f;

	private const float c_postButtonsDelay = 0.25f;

	private const int c_processLinesPerFrame = 8;

	private const int c_twitterCharacterLimit = 140;

	private static readonly string[] c_permissions = new string[1] { "publish_actions" };

	private RaceManager m_raceManager;

	private string m_twitterText;

	private string m_facebookText;

	private bool m_hasPostedTwitter;

	private bool m_hasPostedFacebook;

	private bool m_canRetryTwitterPost;

	private bool m_canRetryFacebookPost;

	private string m_screenshotPath;

	private bool m_captureInProgress;

	private bool m_hasCapturedScreenshot;

	private bool m_enableShare;

	public UIButton m_shareButton;

	public UIButton m_twitterButton;

	public UIButton m_facebookButton;

	public UIBistateInteractivePanel m_twitterPanel;

	public UIBistateInteractivePanel m_facebookPanel;

	public UIBistateInteractivePanel m_picturePanel;

	public KamcordRaceRecorder m_moviePanel;

	public int m_maxCaptureWidth = 512;

	public TrustedDeviceList m_trustedDevices;

	public PipeGUI_Results_ParticleTrigger[] m_particles;

	public float m_captureDelay;

	[SerializeField]
	private bool m_includeTwitterURL;

	[SerializeField]
	private bool m_includeFacebookURL;

	[SerializeField]
	private string m_iosURL;

	[SerializeField]
	private string m_googleURL;

	[SerializeField]
	private string m_amazonURL;

	private string TwitterURL
	{
		get
		{
			if (m_includeTwitterURL)
			{
			}
			return null;
		}
	}

	private string FacebookURL
	{
		get
		{
			if (m_includeFacebookURL)
			{
			}
			return null;
		}
	}

	private void Awake()
	{
		if (SocialManager.IsTwitterInitialised || SocialManager.IsFacebookInitialised)
		{
			if (m_trustedDevices == null)
			{
				m_enableShare = true;
			}
			else
			{
				m_enableShare = m_trustedDevices.IsThisDeviceTrusted();
			}
		}
		else
		{
			m_enableShare = false;
		}
	}

	private void Start()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject != null)
		{
			m_raceManager = gameObject.GetComponent<RaceManager>();
			m_raceManager.OnDisplayResults += OnDisplayResults;
		}
		base.gameObject.SetActiveRecursively(m_enableShare);
		m_shareButton.controlIsEnabled = m_enableShare;
		HidePostButtons();
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnDisplayResults -= OnDisplayResults;
		}
	}

	private void OnEnable()
	{
		m_screenshotPath = null;
		m_hasCapturedScreenshot = false;
		m_hasPostedTwitter = false;
		m_hasPostedFacebook = false;
		HidePostButtons();
		m_picturePanel.Dismiss();
		m_moviePanel.Dismiss();
	}

	public void OnDisplayResults(RaceManager.DisplayResults _displayResults)
	{
		if (!(m_raceManager == null) && _displayResults == RaceManager.DisplayResults.Medals)
		{
			m_shareButton.GetComponent<Renderer>().enabled = m_enableShare;
			m_hasCapturedScreenshot = false;
			m_shareButton.controlIsEnabled = true;
			m_hasPostedTwitter = false;
			m_hasPostedFacebook = false;
			HidePostButtons();
		}
	}

	public void OnShareResults()
	{
		m_picturePanel.BringIn();
		m_moviePanel.BringIn();
		m_twitterPanel.Dismiss();
		m_facebookPanel.Dismiss();
	}

	public void OnPicture()
	{
		if (!m_enableShare)
		{
			return;
		}
		m_picturePanel.Dismiss();
		m_moviePanel.Dismiss();
		if (!m_hasCapturedScreenshot)
		{
			m_shareButton.controlIsEnabled = false;
			if (m_raceManager != null)
			{
				m_raceManager.SuppressEndRaceInput = true;
			}
			StartCoroutine(CaptureScreenShot());
		}
		else
		{
			DisplayPostButtons();
		}
	}

	private IEnumerator CaptureScreenShot()
	{
		float captureDelay = 0.5f;
		if (m_particles != null)
		{
			PipeGUI_Results_ParticleTrigger[] particles = m_particles;
			foreach (PipeGUI_Results_ParticleTrigger particles2 in particles)
			{
				if (particles2.gameObject.active && particles2.enabled)
				{
					particles2.TriggerForCapture();
					captureDelay = m_captureDelay;
				}
			}
		}
		yield return StartCoroutine(WaitForRealSeconds(captureDelay));
		if (TimeEx.IsAvailable)
		{
			TimeEx.RequestPause(PauseLevel.Gameplay);
		}
		m_shareButton.GetComponent<Renderer>().enabled = false;
		yield return null;
		StartCoroutine(CaptureScreenShotFallback());
	}

	private void TidyUpCapture()
	{
		m_captureInProgress = false;
		if (m_raceManager != null)
		{
			m_raceManager.SuppressEndRaceInput = false;
		}
		if (TimeEx.IsAvailable)
		{
			TimeEx.RequestUnPause(PauseLevel.Gameplay);
		}
		m_shareButton.GetComponent<Renderer>().enabled = true;
		m_shareButton.controlIsEnabled = true;
	}

	private void OnCaptured(Texture2D _capture)
	{
		StartCoroutine(CaptureScreenShotCoroutine(_capture));
	}

	private string GetScreenshotPath(ScreenshotFormat _format)
	{
		string text = ((_format == ScreenshotFormat.JPG) ? ".jpg" : ".png");
		return Application.temporaryCachePath + "/" + Path.GetRandomFileName() + text;
	}

	private void PrepareToPostScreenshot(string _screenshotPath)
	{
		m_screenshotPath = _screenshotPath;
		if (File.Exists(m_screenshotPath))
		{
			m_hasCapturedScreenshot = true;
			DisplayPostButtons();
			Debug.Log(string.Format("Share: Screenshot taken: {0}", _screenshotPath));
		}
		else
		{
			m_screenshotPath = null;
			Debug.Log(string.Format("Share: Failed to take screenshot: {0}", _screenshotPath));
		}
	}

	private IEnumerator CaptureScreenShotCoroutine(Texture2D _capture)
	{
		if (!m_enableShare)
		{
			TidyUpCapture();
			UnityEngine.Object.Destroy(_capture);
			yield break;
		}
		if (m_captureInProgress)
		{
			TidyUpCapture();
			UnityEngine.Object.Destroy(_capture);
			yield break;
		}
		if (!m_hasCapturedScreenshot)
		{
			yield return null;
			JPGEncoder jpgEncoder = new JPGEncoder(_capture, 95f);
			yield return StartCoroutine(jpgEncoder.doEncoding(64));
			byte[] bytes = jpgEncoder.GetBytes();
			UnityEngine.Object.Destroy(_capture);
			_capture = null;
			yield return null;
			string screenshotPath = GetScreenshotPath(ScreenshotFormat.JPG);
			try
			{
				File.WriteAllBytes(screenshotPath, bytes);
			}
			catch (Exception ex)
			{
				Exception e = ex;
				Debug.Log(e.ToString());
				TidyUpCapture();
				yield break;
			}
			PrepareToPostScreenshot(screenshotPath);
		}
		else
		{
			UnityEngine.Object.Destroy(_capture);
			_capture = null;
		}
		TidyUpCapture();
	}

	private IEnumerator WaitForRealSeconds(float _seconds)
	{
		if (TimeEx.IsAvailable)
		{
			for (float startTime = 0f; startTime < _seconds; startTime += TimeEx.DeltaTime(PauseLevel.Game))
			{
				yield return null;
			}
			yield break;
		}
		float time = Time.realtimeSinceStartup;
		float endTime = time + _seconds;
		while (time < endTime)
		{
			yield return null;
			time = Time.realtimeSinceStartup;
		}
	}

	private IEnumerator CaptureScreenShotFallback()
	{
		if (!m_enableShare)
		{
			TidyUpCapture();
			yield break;
		}
		if (m_captureInProgress)
		{
			TidyUpCapture();
			yield break;
		}
		m_captureInProgress = true;
		if (!m_hasCapturedScreenshot)
		{
			string tempScreenshotPath = Application.persistentDataPath + "/ShareScreenshot.png";
			if (File.Exists(tempScreenshotPath))
			{
				try
				{
					File.Delete(tempScreenshotPath);
				}
				catch (Exception ex)
				{
					Exception e = ex;
					Debug.Log(e.ToString());
					TidyUpCapture();
					yield break;
				}
			}
			UnityEngine.ScreenCapture.CaptureScreenshot("ShareScreenshot.png");
			Debug.Log(string.Format("Share: Waiting for {0}", tempScreenshotPath));
			yield return null;
			int numWaits = 10;
			while (numWaits-- > 0 && !File.Exists(tempScreenshotPath))
			{
				yield return StartCoroutine(WaitForRealSeconds(0.5f));
			}
			if (!File.Exists(tempScreenshotPath))
			{
				Debug.Log("Share: Screenshot failed");
				TidyUpCapture();
				yield break;
			}
			byte[] srcBytes = null;
			while (numWaits-- > 0)
			{
				try
				{
					using (FileStream fileStream = File.Open(tempScreenshotPath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						if (fileStream == null || !fileStream.CanRead)
						{
							throw new Exception("Waiting for file...");
						}
						long length = fileStream.Length;
						if (length > int.MaxValue)
						{
							throw new Exception("File too big");
						}
						int offset = 0;
						int size = (int)length;
						byte[] array = new byte[size];
						while (size > 0)
						{
							int readSize = fileStream.Read(array, offset, size);
							if (readSize == 0)
							{
								throw new Exception("Unexpected EOF");
							}
							offset += readSize;
							size -= readSize;
						}
						srcBytes = array;
					}
				}
				catch (Exception ex2)
				{
					Exception e2 = ex2;
					Debug.Log(e2);
					goto IL_030f;
				}
				break;
				IL_030f:
				yield return StartCoroutine(WaitForRealSeconds(0.5f));
			}
			if (srcBytes == null)
			{
				Debug.Log("Share: Screenshot failed to load");
				TidyUpCapture();
				yield break;
			}
			int screenWidth = Screen.width;
			int screenHeight = Screen.height;
			float aspect = (float)screenWidth / (float)screenHeight;
			int captureWidth = Math.Min(m_maxCaptureWidth, screenWidth);
			int captureHeight = Mathf.RoundToInt((float)captureWidth / aspect);
			ScreenshotFormat format;
			byte[] bytes;
			if (captureWidth == screenWidth && captureHeight == screenHeight)
			{
				format = ScreenshotFormat.PNG;
				bytes = srcBytes;
			}
			else
			{
				yield return null;
				Texture2D texture;
				if (!LoadPNG(srcBytes, out texture))
				{
					TidyUpCapture();
					yield break;
				}
				float rcpCaptureWidth = 1f / (float)captureWidth;
				float rcpCaptureHeight = 1f / (float)captureHeight;
				Color32[] dstPixels = new Color32[captureWidth * captureHeight];
				for (int y = 0; y < captureHeight; y++)
				{
					float yLo = ((float)y + 0.25f) * rcpCaptureHeight;
					float yHi = ((float)y + 0.75f) * rcpCaptureHeight;
					int dstY = y * captureWidth;
					for (int x = 0; x < captureWidth; x++)
					{
						float xLo = ((float)x + 0.25f) * rcpCaptureWidth;
						float xHi = ((float)x + 0.75f) * rcpCaptureWidth;
						Color c0 = texture.GetPixelBilinear(xLo, yLo);
						Color c1 = texture.GetPixelBilinear(xLo, yHi);
						Color c2 = texture.GetPixelBilinear(xHi, yLo);
						Color c3 = texture.GetPixelBilinear(xHi, yHi);
						dstPixels[x + dstY] = (c0 + c1 + c2 + c3) * 0.25f;
					}
					if (y % 8 == 0)
					{
						yield return null;
					}
				}
				UnityEngine.Object.Destroy(texture);
				texture = null;
				format = ScreenshotFormat.JPG;
				JPGEncoder jpgEncoder = new JPGEncoder(dstPixels, captureWidth, captureHeight, 95f);
				yield return StartCoroutine(jpgEncoder.doEncoding(64));
				bytes = jpgEncoder.GetBytes();
			}
			yield return null;
			if (format == ScreenshotFormat.PNG)
			{
				Color32[] srcPixels;
				int srcWidth;
				int srcHeight;
				if (!LoadPNG(bytes, out srcPixels, out srcWidth, out srcHeight))
				{
					TidyUpCapture();
					yield break;
				}
				format = ScreenshotFormat.JPG;
				JPGEncoder jpgEncoder2 = new JPGEncoder(srcPixels, srcWidth, srcHeight, 95f);
				yield return StartCoroutine(jpgEncoder2.doEncoding(64));
				bytes = jpgEncoder2.GetBytes();
			}
			string screenshotPath = GetScreenshotPath(format);
			try
			{
				File.WriteAllBytes(screenshotPath, bytes);
			}
			catch (Exception ex3)
			{
				Debug.Log(ex3.ToString());
				TidyUpCapture();
				yield break;
			}
			PrepareToPostScreenshot(screenshotPath);
		}
		TidyUpCapture();
	}

	private static bool LoadPNG(byte[] _bytes, out Texture2D o_texture)
	{
		Texture2D texture2D = new Texture2D(0, 0);
		try
		{
			if (!texture2D.LoadImage(_bytes))
			{
				o_texture = null;
				Debug.Log("Failed to load texture. No result");
				UnityEngine.Object.Destroy(texture2D);
				return false;
			}
			if (texture2D.width <= 16 || texture2D.height <= 16)
			{
				o_texture = null;
				Debug.Log("Failed to load texture. Bad dimensions");
				UnityEngine.Object.Destroy(texture2D);
				return false;
			}
		}
		catch (Exception ex)
		{
			o_texture = null;
			Debug.Log(ex.ToString());
			UnityEngine.Object.Destroy(texture2D);
			return false;
		}
		o_texture = texture2D;
		return true;
	}

	private static bool LoadPNG(byte[] _bytes, out Color32[] o_pixels, out int o_width, out int o_height)
	{
		Texture2D o_texture;
		if (!LoadPNG(_bytes, out o_texture))
		{
			o_pixels = null;
			o_width = 0;
			o_height = 0;
			return false;
		}
		o_pixels = o_texture.GetPixels32();
		o_width = o_texture.width;
		o_height = o_texture.height;
		UnityEngine.Object.Destroy(o_texture);
		return true;
	}

	private void DisplayPostButtons()
	{
		if (m_enableShare && m_hasCapturedScreenshot)
		{
			if (!m_hasPostedTwitter && SocialManager.IsTwitterInitialised)
			{
				m_twitterPanel.BringIn();
				m_twitterButton.controlIsEnabled = true;
			}
			if (!m_hasPostedFacebook && SocialManager.IsFacebookInitialised)
			{
				m_facebookPanel.BringIn();
				m_facebookButton.controlIsEnabled = true;
			}
		}
	}

	private void HidePostButtons()
	{
		m_twitterPanel.Dismiss();
		m_twitterButton.controlIsEnabled = false;
		m_facebookPanel.Dismiss();
		m_facebookButton.controlIsEnabled = false;
	}

	public void OnTwitter()
	{
		if (m_enableShare)
		{
			m_canRetryTwitterPost = true;
			DoTwitter();
		}
	}

	public void OnFacebook()
	{
		if (m_enableShare)
		{
			m_canRetryFacebookPost = true;
			DoFacebook();
		}
	}

	private void DoTwitter()
	{
		if (m_hasPostedTwitter || m_raceManager == null)
		{
			return;
		}
		GameManager component = m_raceManager.GetComponent<GameManager>();
		if (component == null)
		{
			return;
		}
		string format = Language.Get("ID_ShareResultTwitter");
		RaceTour currentTour = component.CurrentTour;
		if (currentTour != null && component.CurrentCampaign.m_type == RaceCampaign.Type.SinglePlayer)
		{
			m_twitterText = string.Format(format, currentTour.DisplayRaceName(component.CurrentRace));
		}
		else
		{
			m_twitterText = string.Format(format, "Slingshot Racing");
		}
		string twitterURL = TwitterURL;
		if (!string.IsNullOrEmpty(twitterURL))
		{
			string text = m_twitterText + " " + string.Format(twitterURL, AppSettings.AppID);
			if (text.Length <= 140)
			{
				m_twitterText = text;
			}
		}
		if (m_twitterText != null)
		{
			if (TwitterAndroid.isLoggedIn())
			{
				Tweet();
				return;
			}
			TwitterAndroidManager.twitterInitializedEvent += LoginAfterInit;
			TwitterAndroid.init("GVUwZ5qAzmc66zCwgF9XHQ", "E1gXiXqL8CGWJufEh6cl5AmII3d0Qd4itG9BLWbNg");
		}
	}

	private void LoginAfterInit()
	{
		TwitterAndroidManager.twitterInitializedEvent -= LoginAfterInit;
		TwitterAndroidManager.loginDidSucceedEvent += TweetAfterLoginWithUserName;
		TwitterAndroidManager.loginDidFailEvent += TwitterLoginFailed;
		TwitterAndroid.showLoginDialog();
	}

	private void TweetAfterLoginWithUserName(string _userName)
	{
		TweetAfterLogin();
	}

	private void TweetAfterLogin()
	{
		TwitterAndroidManager.loginDidSucceedEvent -= TweetAfterLoginWithUserName;
		TwitterAndroidManager.loginDidFailEvent -= TwitterLoginFailed;
		Tweet();
	}

	private void TwitterLoginFailed(string _error)
	{
		TwitterAndroidManager.loginDidSucceedEvent -= TweetAfterLoginWithUserName;
		TwitterAndroidManager.loginDidFailEvent -= TwitterLoginFailed;
		m_twitterText = null;
		m_canRetryTwitterPost = false;
		Debug.Log(string.Format("Twitter: Failed to log in. {0}", _error));
	}

	private void Tweet()
	{
		TwitterAndroidManager.requestSucceededEvent += TwitterPostSuccessWithObject;
		TwitterAndroidManager.requestFailedEvent += TwitterPostFailed;
		if (m_twitterText != null && !m_hasPostedTwitter)
		{
			if (File.Exists(m_screenshotPath))
			{
				byte[] image = File.ReadAllBytes(m_screenshotPath);
				TwitterAndroid.postUpdateWithImage(m_twitterText, image);
				m_hasPostedTwitter = true;
				Debug.Log(string.Format("Twitter: Posting tweet. {0}", m_twitterText));
			}
			else
			{
				Debug.Log("Twitter: Missing image!");
			}
			m_twitterText = null;
		}
		else
		{
			Debug.Log("Twitter: Nothing to tweet!");
		}
	}

	private void TwitterPostSuccessWithObject(object _result)
	{
		Hashtable hashtable = _result as Hashtable;
		if (hashtable != null && hashtable.ContainsKey("error"))
		{
			TwitterPostFailed(hashtable["error"].ToString());
		}
		else
		{
			TwitterPostSuccess();
		}
	}

	private void TwitterPostSuccess()
	{
		TwitterAndroidManager.requestSucceededEvent -= TwitterPostSuccessWithObject;
		TwitterAndroidManager.requestFailedEvent -= TwitterPostFailed;
		if (m_twitterButton != null)
		{
			m_twitterPanel.Dismiss();
			m_twitterButton.controlIsEnabled = false;
		}
		m_canRetryTwitterPost = false;
		Debug.Log("Twitter: Tweet successful");
	}

	private void TwitterPostFailed(string _error)
	{
		TwitterAndroidManager.requestSucceededEvent -= TwitterPostSuccessWithObject;
		TwitterAndroidManager.requestFailedEvent -= TwitterPostFailed;
		bool flag = m_canRetryTwitterPost && !string.IsNullOrEmpty(_error) && _error.Contains("OAuth");
		m_hasPostedTwitter = false;
		m_canRetryTwitterPost = false;
		Debug.Log(string.Format("Twitter: Tweet failed. {0}", _error));
		if (flag)
		{
			TwitterAndroid.logout();
			DoTwitter();
		}
	}

	private void DoFacebook()
	{
		if (m_hasPostedFacebook || m_raceManager == null)
		{
			return;
		}
		GameManager component = m_raceManager.GetComponent<GameManager>();
		if (component == null)
		{
			return;
		}
		string format = Language.Get("ID_ShareResultFacebook");
		RaceTour currentTour = component.CurrentTour;
		if (currentTour != null && component.CurrentCampaign.m_type == RaceCampaign.Type.SinglePlayer)
		{
			m_facebookText = string.Format(format, currentTour.DisplayRaceName(component.CurrentRace));
		}
		else
		{
			m_facebookText = string.Format(format, "Slingshot Racing");
		}
		string facebookURL = FacebookURL;
		if (!string.IsNullOrEmpty(facebookURL))
		{
			m_facebookText = m_facebookText + "\n\n" + string.Format(facebookURL, AppSettings.AppID);
		}
		if (m_facebookText != null)
		{
			if (FacebookAndroid.isSessionValid())
			{
				PostToFacebook();
				return;
			}
			FacebookManager.loginFailedEvent += FacebookLoginFailed;
			FacebookManager.loginSucceededEvent += PostToFacebookAfterLogin;
			FacebookAndroid.loginWithRequestedPermissions(c_permissions);
		}
	}

	private void ClearLogin()
	{
		FacebookManager.loginSucceededEvent -= PostToFacebookAfterLogin;
		FacebookManager.loginFailedEvent -= FacebookLoginFailed;
	}

	private void PostToFacebookAfterLogin()
	{
		ClearLogin();
		PostToFacebook();
	}

	private void FacebookLoginFailed(string _error)
	{
		ClearLogin();
		m_facebookText = null;
		m_canRetryFacebookPost = false;
		Debug.Log(string.Format("Facebook: Failed to log in. {0}", _error));
	}

	private void PostToFacebook()
	{
		if (m_facebookText != null && !m_hasPostedFacebook)
		{
			if (File.Exists(m_screenshotPath))
			{
				byte[] image = File.ReadAllBytes(m_screenshotPath);
				Facebook.instance.postImage(image, m_facebookText, FacebookCompletionHandler);
				m_hasPostedFacebook = true;
				Debug.Log(string.Format("Facebook: Posting. {0}", m_facebookText));
			}
			else
			{
				Debug.Log("Facebook: Missing image!");
			}
			m_facebookText = null;
		}
		else
		{
			Debug.Log("Facebook: Nothing to post!");
		}
	}

	private bool GetFacebookError(string _error, object _result, out string o_errorString)
	{
		if (!string.IsNullOrEmpty(_error))
		{
			o_errorString = _error;
			return true;
		}
		Hashtable hashtable = _result as Hashtable;
		if (hashtable != null && hashtable.ContainsKey("error"))
		{
			Hashtable hashtable2 = hashtable["error"] as Hashtable;
			if (hashtable2 != null && hashtable2.ContainsKey("message"))
			{
				o_errorString = hashtable2["message"].ToString();
			}
			else
			{
				o_errorString = "Unknown facebook error";
			}
			return true;
		}
		o_errorString = null;
		return false;
	}

	private void FacebookCompletionHandler(string _error, object _result)
	{
		string o_errorString;
		if (GetFacebookError(_error, _result, out o_errorString))
		{
			bool flag = m_canRetryFacebookPost && !string.IsNullOrEmpty(o_errorString) && o_errorString.Contains("FileNotFoundException");
			m_hasPostedFacebook = false;
			m_canRetryFacebookPost = false;
			Debug.Log(_error);
			if (flag)
			{
				FacebookManager.loggedOutEvent += FacebookLogoutRepost;
				FacebookAndroid.logout();
			}
		}
		else
		{
			if (m_facebookButton != null)
			{
				m_facebookPanel.Dismiss();
				m_facebookButton.controlIsEnabled = false;
			}
			m_canRetryFacebookPost = false;
			Debug.Log("Facebook: Post successful");
		}
	}

	private void FacebookLogoutRepost()
	{
		FacebookManager.loggedOutEvent -= FacebookLogoutRepost;
		DoFacebook();
	}
}
