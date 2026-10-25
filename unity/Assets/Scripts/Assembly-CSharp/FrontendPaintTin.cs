using UnityEngine;

public class FrontendPaintTin : MonoBehaviour
{
	private void Start()
	{
		AutoSpriteControlBase component = base.gameObject.GetComponent<AutoSpriteControlBase>();
		if (component != null)
		{
			component.AddValueChangedDelegate(OnValueChanged);
		}
		ControlBase component2 = base.gameObject.GetComponent<ControlBase>();
		if (component2 != null)
		{
			component2.AddValueChangedDelegate(OnValueChanged);
		}
	}

	private void OnValueChanged(IUIObject obj)
	{
	}
}
