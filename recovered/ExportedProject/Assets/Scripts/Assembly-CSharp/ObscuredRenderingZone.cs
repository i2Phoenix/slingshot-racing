using UnityEngine;

[AddComponentMenu("Graphics/Obscured Rendering Zone")]
public class ObscuredRenderingZone : MonoBehaviour
{
	private void OnTriggerEnter(Collider _collider)
	{
		Vehicle component = _collider.transform.root.GetComponent<Vehicle>();
		if (component != null && component.Visuals != null)
		{
			component.Visuals.EnterObscuredZone();
		}
	}

	private void OnTriggerExit(Collider _collider)
	{
		Vehicle component = _collider.transform.root.GetComponent<Vehicle>();
		if (component != null && component.Visuals != null)
		{
			component.Visuals.ExitObscuredZone();
		}
	}
}
