using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("In App Purchase/Manager")]
public class InAppPurchaseManager : MonoBehaviour
{
	public enum PurchaseStatus
	{
		Purchased = 0,
		Cancelled = 1,
		Error = 2
	}

	public delegate void PurchaseEvent(PurchaseStatus _status);

	private static InAppPurchaseManager s_instance;

	protected bool m_isTrusted;

	protected PurchaseEvent m_callback;

	protected Action<bool> m_restorePurchasesCallback;

	public ProductInfo[] m_products;

	public TrustedDeviceList m_trustedDevices;

	protected string[] ProductIDs
	{
		get
		{
			List<string> list = new List<string>();
			for (int i = 0; i < m_products.Length; i++)
			{
				string iD = m_products[i].ID;
				if (iD.Length > 0)
				{
					list.Add(iD);
				}
			}
			return list.ToArray();
		}
	}

	public static InAppPurchaseManager Get
	{
		get
		{
			return s_instance;
		}
	}

	protected virtual void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		if (s_instance == null)
		{
			s_instance = this;
		}
		if (m_trustedDevices == null)
		{
			m_isTrusted = true;
		}
		else
		{
			m_isTrusted = m_trustedDevices.IsThisDeviceTrusted();
		}
		if (m_isTrusted)
		{
			Debug.Log("InAppPurchase: Device is trusted");
		}
		else
		{
			Debug.Log("InAppPurchase: Device is not trusted");
		}
	}

	protected virtual void OnDestroy()
	{
		if (s_instance == this)
		{
			s_instance = null;
		}
	}

	public virtual bool IsProductAvailable(ProductInfo _productInfo)
	{
		return false;
	}

	public virtual bool IsPurchased(ProductInfo _productInfo)
	{
		return false;
	}

	public virtual bool Purchase(ProductInfo _productInfo, PurchaseEvent _callback)
	{
		return false;
	}

	public virtual bool RestoreCompletedTransactions(Action<bool> _callback)
	{
		return false;
	}
}
