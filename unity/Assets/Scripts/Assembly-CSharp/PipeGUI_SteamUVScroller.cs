using UnityEngine;

[AddComponentMenu("GUI/PipeGUI/SteamUVScroller")]
public class PipeGUI_SteamUVScroller : MonoBehaviour
{
	private Material m_clonedMaterial;

	private Vector2 m_texOffset;

	private Vector4 m_tex2ST;

	private Vector3 m_designScale;
	private Vector2 m_designTiling;
	private float m_designSecondTiling;
	private float m_designSecondTilingY;
	private float m_designWidth;
	private float m_designHeight;
	private Camera m_uiCamera;

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
		m_designScale = transform.localScale;
		m_designTiling = m_clonedMaterial.mainTextureScale;
		m_designSecondTiling = m_tex2ST.x;
		m_designSecondTilingY = m_tex2ST.y;
		m_designWidth = GetComponent<Renderer>().bounds.size.x;
		m_designHeight = GetComponent<Renderer>().bounds.size.y;
		m_uiCamera = UIManager.instance.uiCameras[0].camera;
	}

	private void Update()
	{
		// Cover both wide and near-square viewports. The imported plane is rotated:
		// uniform scaling also covers its vertical axis without assuming local XY.
		if (m_uiCamera != null && m_designWidth > 0)
		{
			float width = 2f * m_uiCamera.orthographicSize * m_uiCamera.aspect;
			float heightScale = m_designHeight > 0f ? (2f * m_uiCamera.orthographicSize + 1f) / m_designHeight : 1f;
			float scale = Mathf.Max(1f, (width + 1f) / m_designWidth, heightScale);
			transform.localScale = m_designScale * scale;
			Vector3 position = transform.position;
			position.x = m_uiCamera.transform.position.x;
			position.y = m_uiCamera.transform.position.y;
			transform.position = position;
			m_clonedMaterial.mainTextureScale = m_designTiling * scale;
			m_tex2ST.x = m_designSecondTiling * scale;
			m_tex2ST.y = m_designSecondTilingY * scale;
		}
		float deltaTime = Time.deltaTime;
		m_texOffset.x = Mathf.Repeat(m_texOffset.x + m_scrollRate.x * deltaTime, 1f);
		m_texOffset.y = Mathf.Repeat(m_texOffset.y + m_scrollRate.y * deltaTime, 1f);
		m_clonedMaterial.mainTextureOffset = m_texOffset;
		m_tex2ST.z = Mathf.Repeat(m_tex2ST.z + m_scrollRate2.x * deltaTime, 1f);
		m_tex2ST.w = Mathf.Repeat(m_tex2ST.w + m_scrollRate2.y * deltaTime, 1f);
		m_clonedMaterial.SetVector("_MainTex2_ST", m_tex2ST);
	}

	private void OnDestroy()
	{
		if (m_clonedMaterial != null) Object.Destroy(m_clonedMaterial);
	}
}
