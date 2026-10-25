using UnityEngine;

public class ProductInfo : ScriptableObject
{
	public string m_storeKitID;

	public string m_handlerMethod;

	public string m_revokeMethod;

	public string ID
	{
		get
		{
			return m_storeKitID.ToLower();
		}
	}
}
