using UnityEngine;

public class UIGamepadIconManager : MonoBehaviour
{
	private static UIGamepadIconManager s_instance;

	public GameObject m_iconPrefab;

	public static UIGamepadIconManager Get
	{
		get
		{
			return s_instance;
		}
	}

	private void Awake()
	{
		if (s_instance != null)
		{
			Debug.LogError("Only one instance of UIGamepadIconManager is permitted");
		}
		s_instance = this;
	}

	private void OnDestroy()
	{
		if (s_instance == this)
		{
			s_instance = null;
		}
	}

	public GameObject CreateIcon(string _name)
	{
		GameObject gameObject = Object.Instantiate(m_iconPrefab) as GameObject;
		PackedSprite component = gameObject.GetComponent<PackedSprite>();
		UVAnimation anim = component.GetAnim(_name);
		component.defaultAnim = anim.index;
		return gameObject;
	}
}
