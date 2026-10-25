using System;
using System.Reflection;
using UnityEngine;

public class AppSettingsBehaviour : MonoBehaviour
{
	public string m_methodName;

	public bool m_delete;

	private void Awake()
	{
		Type typeFromHandle = typeof(AppSettings);
		PropertyInfo property = typeFromHandle.GetProperty(m_methodName);
		if (property != null)
		{
			if (!(bool)property.GetValue(null, null))
			{
				if (m_delete)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				else
				{
					base.gameObject.SetActiveRecursively(false);
				}
			}
		}
		else
		{
			Debug.Log(m_methodName + " does not exist on AppSettings");
		}
	}
}
