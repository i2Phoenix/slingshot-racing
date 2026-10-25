using UnityEngine;

[AddComponentMenu("GUI/PipeGUI/OverlayButton")]
public class PipeGUI_OverlayButton : MonoBehaviour
{
	private void Update()
	{
		BoxCollider componentInChildren = GetComponentInChildren<BoxCollider>();
		PackedSprite component = GetComponent<PackedSprite>();
		componentInChildren.center = -componentInChildren.transform.localPosition;
		componentInChildren.size = new Vector3(component.width, component.height, 0f);
	}
}
