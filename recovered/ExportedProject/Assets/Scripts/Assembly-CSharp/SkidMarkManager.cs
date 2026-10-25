using UnityEngine;

[AddComponentMenu("Graphics/Skid Mark Manager")]
public class SkidMarkManager : MonoBehaviour
{
	private const int c_maxSkidMarks = 8;

	private const int c_pointCount = 32;

	private static readonly string c_skidMarkManagerName = "__SkidMarkManager";

	private static readonly string c_skidMarkManagerFind = "/" + c_skidMarkManagerName;

	private SkidMark[] m_freeSkidMarks;

	private int m_numFreeSkidMarks;

	private Material m_material;

	private void Awake()
	{
		m_material = (Material)Resources.Load("Materials/SkidMark");
		m_freeSkidMarks = new SkidMark[8];
		m_numFreeSkidMarks = 8;
		for (int i = 0; i != 8; i++)
		{
			GameObject gameObject = new GameObject("SkidMark");
			SkidMark skidMark = gameObject.AddComponent<SkidMark>();
			skidMark.Initialise(32);
			skidMark.renderer.sharedMaterial = m_material;
			gameObject.transform.parent = base.transform;
			m_freeSkidMarks[i] = skidMark;
		}
	}

	public SkidMark GetFreeSkidMark()
	{
		if (m_numFreeSkidMarks > 0)
		{
			int num = --m_numFreeSkidMarks;
			SkidMark result = m_freeSkidMarks[num];
			m_freeSkidMarks[num] = null;
			return result;
		}
		return null;
	}

	public static SkidMarkManager Get()
	{
		GameObject gameObject = GameObject.Find(c_skidMarkManagerFind);
		if (gameObject == null)
		{
			gameObject = new GameObject(c_skidMarkManagerName);
			gameObject.AddComponent<SkidMarkManager>();
		}
		return gameObject.GetComponent<SkidMarkManager>();
	}

	public void SkidMark_FadedOut(SkidMark _skidMark)
	{
		m_freeSkidMarks[m_numFreeSkidMarks++] = _skidMark;
	}
}
