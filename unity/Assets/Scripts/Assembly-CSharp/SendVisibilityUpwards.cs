using UnityEngine;

[AddComponentMenu("Graphics/Send Visibility Upwards")]
public class SendVisibilityUpwards : MonoBehaviour
{
	private void OnBecameVisible()
	{
		SendMessageUpwards("OnChildBecameVisible", this, SendMessageOptions.DontRequireReceiver);
	}

	private void OnBecameInvisible()
	{
		SendMessageUpwards("OnChildBecameInvisible", this, SendMessageOptions.DontRequireReceiver);
	}
}
