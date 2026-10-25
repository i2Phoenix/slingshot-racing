using UnityEngine;

[AddComponentMenu("In App Purchase/Debug Purchase Button")]
public class DebugPurchaseButton : MonoBehaviour
{
	private InAppPurchaseManager m_iapManager;

	private UIButton m_uiButton;

	public ProductInfo m_productInfo;

	private void Awake()
	{
		m_iapManager = InAppPurchaseManager.Get;
		m_uiButton = GetComponent<UIButton>();
		if (m_iapManager != null)
		{
			if (!m_iapManager.IsProductAvailable(m_productInfo))
			{
				m_uiButton.Text = "Unavailable";
			}
			else if (m_iapManager.IsPurchased(m_productInfo))
			{
				m_uiButton.Text = "Already Purchased";
			}
		}
		else
		{
			m_uiButton.Text = "No IAP Manager";
		}
	}

	private void OnDestroy()
	{
	}

	private void OnClick()
	{
		if (!(m_iapManager == null) && m_iapManager.IsProductAvailable(m_productInfo) && !m_iapManager.Purchase(m_productInfo, null))
		{
			m_uiButton.Text = "Fail";
		}
	}
}
