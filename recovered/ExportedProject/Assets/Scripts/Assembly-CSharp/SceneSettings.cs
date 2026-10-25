using UnityEngine;

public class SceneSettings : MonoBehaviour
{
	private static GameObject s_sceneSettingsGameObject;

	private static RenderSettings s_renderSettings;

	public static RenderSettings RenderSettings
	{
		get
		{
			if (s_renderSettings == null)
			{
				GetGameObject();
			}
			return s_renderSettings;
		}
	}

	public static GameObject GetGameObject()
	{
		GameObject gameObject = s_sceneSettingsGameObject;
		if (gameObject == null)
		{
			GameObject original = (GameObject)Resources.Load("Prefabs/SceneSettings");
			gameObject = (GameObject)Object.Instantiate(original);
		}
		return gameObject;
	}

	private void Awake()
	{
		s_sceneSettingsGameObject = base.gameObject;
		s_renderSettings = GetComponent<RenderSettings>();
	}

	private void OnDestroy()
	{
		if (s_sceneSettingsGameObject == base.gameObject)
		{
			s_sceneSettingsGameObject = null;
		}
	}
}
