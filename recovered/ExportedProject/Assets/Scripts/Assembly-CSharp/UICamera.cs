using UnityEngine;

[AddComponentMenu("GUI/UI Camera")]
[RequireComponent(typeof(Camera))]
public class UICamera : MonoBehaviour
{
	private void Start()
	{
		if (SystemInfo.supportsImageEffects)
		{
			base.gameObject.AddComponent<ScreenCapture>();
		}
		if (Camera.allCameras.Length <= 1)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("MainCamera");
		if (array == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].camera.targetTexture == null)
			{
				num++;
			}
		}
		if (num > 1)
		{
			base.camera.clearFlags = CameraClearFlags.Nothing;
		}
		if (array.Length > 1)
		{
			base.gameObject.tag = "Untagged";
		}
	}
}
