using UnityEngine;

[RequireComponent(typeof(UIBtnWWW))]
public class PipeGUI_CMGLink : MonoBehaviour
{
	private void Start()
	{
		UIBtnWWW component = GetComponent<UIBtnWWW>();
		component.URL = AppSettings.CMGURL;
	}
}
