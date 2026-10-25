using UnityEngine;

[AddComponentMenu("Graphics/Material UV Scroller")]
public class MaterialUVScroller : MonoBehaviour
{
	private Material m_clonedMaterial;

	public Material m_material;

	public Vector2 m_scrollRate;

	public Vector2 m_initialOffset;

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
		if (m_initialOffset != Vector2.zero)
		{
			m_clonedMaterial.mainTextureOffset = m_initialOffset;
		}
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		Vector2 mainTextureOffset = m_clonedMaterial.mainTextureOffset;
		mainTextureOffset.x = Mathf.Repeat(mainTextureOffset.x + m_scrollRate.x * deltaTime, 1f);
		mainTextureOffset.y = Mathf.Repeat(mainTextureOffset.y + m_scrollRate.y * deltaTime, 1f);
		m_clonedMaterial.mainTextureOffset = mainTextureOffset;
	}
}
