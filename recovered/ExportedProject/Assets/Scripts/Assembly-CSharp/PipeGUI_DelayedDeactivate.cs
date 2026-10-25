using System.Collections;
using UnityEngine;

public class PipeGUI_DelayedDeactivate : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return null;
		yield return null;
		base.gameObject.SetActiveRecursively(false);
	}
}
