using UnityEngine;

public class EnableChildRenderersAtNight : MonoBehaviour
{
	private void Start()
	{
		if (SceneSettings.RenderSettings.m_night)
		{
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = true;
			}
		}
	}
}
