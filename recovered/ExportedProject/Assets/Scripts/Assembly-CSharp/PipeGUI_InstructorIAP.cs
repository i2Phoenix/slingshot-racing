using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PipeGUI_InstructorSpeech))]
public class PipeGUI_InstructorIAP : MonoBehaviour
{
	private PipeGUI_InstructorSpeech m_speech;

	private InAppPurchaseManager m_iapManager;

	private LoadscreenManager m_loadscreen;

	private bool m_insideIAPBlock;

	public ProductInfo m_productInfo;

	public UIButton m_purchaseButton;

	public bool IAPAvailable
	{
		get
		{
			if (m_productInfo != null && m_iapManager != null && m_iapManager.IsProductAvailable(m_productInfo))
			{
				return true;
			}
			return false;
		}
	}

	private void Start()
	{
		m_iapManager = InAppPurchaseManager.Get;
		m_speech = GetComponent<PipeGUI_InstructorSpeech>();
		GameObject gameObject = GameObject.Find("PersistentGUI");
		if (gameObject != null)
		{
			PersistentGUI component = gameObject.GetComponent<PersistentGUI>();
			if (component != null)
			{
				m_loadscreen = component.m_loadscreen;
			}
		}
	}

	public IEnumerator DisplayAndWait(string _message, string _alternate)
	{
		base.gameObject.active = true;
		m_speech.AllowClickToClose = true;
		bool useIAP = IAPAvailable;
		m_purchaseButton.controlIsEnabled = useIAP;
		string msg = ((!useIAP) ? _alternate : _message);
		yield return StartCoroutine(m_speech.DisplayAndWait(msg, "Tap", useIAP));
	}

	private void OnIAPPurchased()
	{
		if (m_iapManager != null)
		{
			m_insideIAPBlock = true;
			if (m_iapManager.Purchase(m_productInfo, OnPurchase))
			{
				if (m_insideIAPBlock)
				{
					m_speech.AllowClickToClose = false;
					m_purchaseButton.controlIsEnabled = false;
					if (m_loadscreen != null)
					{
						StartCoroutine(m_loadscreen.ShowSystemBusy(true, 0.2f));
					}
				}
				return;
			}
			m_insideIAPBlock = false;
		}
		m_speech.Hide();
	}

	public void OnPurchase(InAppPurchaseManager.PurchaseStatus _status)
	{
		m_speech.AllowClickToClose = true;
		m_speech.Hide();
		if (m_loadscreen != null)
		{
			StartCoroutine(m_loadscreen.ShowSystemBusy(false, 0.2f));
		}
		m_insideIAPBlock = false;
	}
}
