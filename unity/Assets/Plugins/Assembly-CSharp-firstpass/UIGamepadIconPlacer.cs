using UnityEngine;

public class UIGamepadIconPlacer : MonoBehaviour
{
	private GameObject m_icon;

	public string m_iconName;

	public UIGamepadTargetSimple m_target;

	private void Start()
	{
		SetIcon(m_iconName);
		if (m_target != null)
		{
			m_target.OnFocusEvent += OnTargetFocus;
			m_target.OnLostFocusEvent += OnTargetLostFocus;
			if (m_icon != null)
			{
				m_icon.SetActiveRecursively(false);
			}
		}
	}

	private void OnDestroy()
	{
		if (m_target != null)
		{
			m_target.OnFocusEvent -= OnTargetFocus;
			m_target.OnLostFocusEvent -= OnTargetLostFocus;
		}
	}

	private void OnTargetFocus()
	{
		if (m_icon != null)
		{
			m_icon.SetActiveRecursively(true);
		}
	}

	private void OnTargetLostFocus()
	{
		if (m_icon != null)
		{
			m_icon.SetActiveRecursively(false);
		}
	}

	public void SetIcon(string _iconName)
	{
		if (m_icon != null)
		{
			Object.Destroy(m_icon);
		}
		m_iconName = _iconName;
		if (UIGamepadManager.IsGamepadActive)
		{
			GameObject gameObject = UIGamepadIconManager.Get.CreateIcon(m_iconName);
			Vector3 localScale = gameObject.transform.localScale;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = localScale;
			m_icon = gameObject;
		}
	}
}
