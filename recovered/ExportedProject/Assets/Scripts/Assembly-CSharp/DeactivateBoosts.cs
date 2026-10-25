using UnityEngine;

public class DeactivateBoosts : MonoBehaviour
{
	public int m_generationsToDeactivate;

	private void Start()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("BoostBox");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			BoostBox component = gameObject.GetComponent<BoostBox>();
			if (component == null || component.m_generation <= m_generationsToDeactivate)
			{
				gameObject.SetActiveRecursively(false);
			}
		}
	}
}
