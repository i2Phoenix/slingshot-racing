using UnityEngine;

[AddComponentMenu("GUI/PipeGUI/SteamUVScroller")]
public class PipeGUI_SteamUVScroller : MonoBehaviour
{
	private Material m_clonedMaterial;

	private Vector2 m_texOffset;

	private Vector4 m_tex2ST;

	public Material m_material;

	public Vector2 m_scrollRate;

	public Vector2 m_scrollRate2;

	private void Start()
	{
		m_clonedMaterial = new Material(m_material);
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (renderer.sharedMaterial == m_material)
			{
				renderer.sharedMaterial = m_clonedMaterial;
			}
		}
		m_texOffset = m_clonedMaterial.mainTextureOffset;
		m_tex2ST = m_clonedMaterial.GetVector("_MainTex2_ST");
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		m_texOffset.x = Mathf.Repeat(m_texOffset.x + m_scrollRate.x * deltaTime, 1f);
		m_texOffset.y = Mathf.Repeat(m_texOffset.y + m_scrollRate.y * deltaTime, 1f);
		m_clonedMaterial.mainTextureOffset = m_texOffset;
		m_tex2ST.z = Mathf.Repeat(m_tex2ST.z + m_scrollRate2.x * deltaTime, 1f);
		m_tex2ST.w = Mathf.Repeat(m_tex2ST.w + m_scrollRate2.y * deltaTime, 1f);
		m_clonedMaterial.SetVector("_MainTex2_ST", m_tex2ST);
	}
}
