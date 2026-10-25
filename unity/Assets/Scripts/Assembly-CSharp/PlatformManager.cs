using UnityEngine;

public class PlatformManager : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.name = GetType().ToString();
		Object.DontDestroyOnLoad(this);
	}

	private void OnConfirm(string _param)
	{
		Platform._OnDialogConfirm();
	}

	private void OnCancel(string _param)
	{
		Platform._OnDialogCancel();
	}
}
