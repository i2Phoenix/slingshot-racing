using System.Collections.Generic;
using UnityEngine;

public class IAPHandler : MonoBehaviour
{
	private InAppPurchaseManager m_iapManager;

	private ProductInfo[] m_products;

	public RaceCampaign[] m_singlePlayerCampaigns;

	private string StoreName
	{
		get
		{
			return "Google Play";
		}
	}

	private void Start()
	{
		m_iapManager = InAppPurchaseManager.Get;
		if (m_iapManager != null)
		{
			m_products = m_iapManager.m_products;
		}
	}

	private void OnDestroy()
	{
	}

	private void OnPurchaseSucceeded(GooglePurchase _purchase)
	{
		ProductInfo productInfo = FindProduct(_purchase.productId);
		if (productInfo == null)
		{
			Debug.LogError("Unrecognised product purchased: " + _purchase.productId);
			return;
		}
		SendMessage(productInfo.m_handlerMethod, 1);
		LogPurchase(productInfo.ID);
	}

	private void OnQueryInventorySucceeded(List<GooglePurchase> _purchases, List<GoogleSkuInfo> _skus)
	{
		foreach (GooglePurchase _purchase in _purchases)
		{
			ProductInfo productInfo = FindProduct(_purchase.productId);
			if (productInfo == null)
			{
				Debug.LogError(string.Format("Unrecognised product purchased: {0}", _purchase.productId));
				continue;
			}
			SendMessage(productInfo.m_handlerMethod, -1);
			LogRestore(productInfo.ID);
		}
	}

	private ProductInfo FindProduct(string _id)
	{
		ProductInfo[] products = m_products;
		foreach (ProductInfo productInfo in products)
		{
			if (productInfo.ID == _id)
			{
				return productInfo;
			}
		}
		return null;
	}

	private void AccessAllAreas(int _quanitity)
	{
		Debug.Log("AccessAllAreas called");
		bool flag = false;
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject != null)
		{
			GameManager component = gameObject.GetComponent<GameManager>();
			if (component != null)
			{
				flag = component.OnAccessAllAreas();
			}
		}
		if (flag)
		{
			return;
		}
		SaveManager saveManager = SaveManager.Find();
		if (saveManager != null)
		{
			RaceCampaignSave[] saves = saveManager.Saves;
			RaceCampaignSave[] array = saves;
			foreach (RaceCampaignSave raceCampaignSave in array)
			{
				raceCampaignSave.LocksDisabled = true;
				raceCampaignSave.UpdateSaveData();
			}
		}
	}

	private void RevokeAccessAllAreas()
	{
		Debug.Log("RevokeAccessAllAreas called");
		bool flag = false;
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject != null)
		{
			GameManager component = gameObject.GetComponent<GameManager>();
			if (component != null)
			{
				flag = component.OnRevokeAccessAllAreas();
			}
		}
		if (flag)
		{
			return;
		}
		SaveManager saveManager = SaveManager.Find();
		if (saveManager != null)
		{
			RaceCampaignSave[] saves = saveManager.Saves;
			RaceCampaignSave[] array = saves;
			foreach (RaceCampaignSave raceCampaignSave in array)
			{
				raceCampaignSave.LocksDisabled = false;
				raceCampaignSave.UpdateSaveData();
			}
		}
	}

	private void UnlockVehicle(int _quantity, int _styleIndex)
	{
		Debug.Log("UnlockVehicle " + _styleIndex);
		GlobalSave.UnlockVehicleStyle(_styleIndex);
	}

	private void RevokeVehicle(int _styleIndex)
	{
		Debug.Log("RevokeVehicle " + _styleIndex);
		GlobalSave.RevokeVehicleStyle(_styleIndex);
	}

	private void UnlockVehicleE(int _quantity)
	{
		UnlockVehicle(_quantity, 4);
	}

	private void RevokeVehicleE()
	{
		RevokeVehicle(4);
	}

	private void UnlockVehicleF(int _quantity)
	{
		UnlockVehicle(_quantity, 5);
	}

	private void RevokeVehicleF()
	{
		RevokeVehicle(5);
	}

	private void UnlockVehicleG(int _quantity)
	{
		UnlockVehicle(_quantity, 6);
	}

	private void RevokeVehicleG()
	{
		RevokeVehicle(6);
	}

	private void UnlockVehicleH(int _quantity)
	{
		UnlockVehicle(_quantity, 7);
	}

	private void RevokeVehicleH()
	{
		RevokeVehicle(7);
	}

	private void UnlockPickupsStrategyGuide(int _quantity)
	{
		GlobalSave.UnlockPickupsStrategyGuide();
	}

	private void UnlockTimeTrialStrategyGuide(int _quantity)
	{
		GlobalSave.UnlockTimeTrialStrategyGuide();
	}

	private void LogPurchase(string _id)
	{
		Analytics.LogEvent("IAPPurchase", "ID", _id, "Store", StoreName);
	}

	private void LogRestore(string _id)
	{
		Analytics.LogEvent("IAPRestore", "ID", _id, "Store", StoreName);
	}

	private void LogRevoke(string _id)
	{
		Analytics.LogEvent("IAPRevoke", "ID", _id, "Store", StoreName);
	}
}
