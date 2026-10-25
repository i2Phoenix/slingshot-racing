using UnityEngine;

[RequireComponent(typeof(UIBtnWWW))]
public class PipeGUI_FAQLink : MonoBehaviour
{
	private void Start()
	{
		UIBtnWWW component = GetComponent<UIBtnWWW>();
		component.URL = AppSettings.FAQURL;
	}
}
