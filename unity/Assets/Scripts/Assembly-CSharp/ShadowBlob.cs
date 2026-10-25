using UnityEngine;

public class ShadowBlob : MonoBehaviour
{
	public static float s_width = 6f;

	public static float s_height = 7f;

	public static float[] s_zOffset = new float[9] { -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f };

	private static float s_halfWidth = s_width * 0.5f;

	private static float s_halfHeight = s_height * 0.5f;

	private Transform m_shadowTransform;

	private ShadowBlobManager m_manager;

	private int m_index;

	private float m_glow;

	private float m_alpha = 1f;

	private static bool s_init = false;

	private static Mesh[] s_mesh;

	public int Index
	{
		set
		{
			m_index = value;
		}
	}

	private static void Init()
	{
		if (s_init)
		{
			return;
		}
		if (s_mesh == null)
		{
			s_mesh = new Mesh[4];
			for (int i = 0; i < 4; i++)
			{
				int num = 1;
				int num2 = num * 4;
				int num3 = num * 6;
				s_mesh[i] = new Mesh();
				Vector3[] array = new Vector3[num2];
				float num4 = s_halfWidth;
				float num5 = s_halfHeight;
				array[0].x = 0f - num4;
				array[0].y = num5 + s_zOffset[i];
				array[1].x = num4;
				array[1].y = num5 + s_zOffset[i];
				array[2].x = 0f - num4;
				array[2].y = 0f - num5 + s_zOffset[i];
				array[3].x = num4;
				array[3].y = 0f - num5 + s_zOffset[i];
				s_mesh[i].vertices = array;
				Vector2[] array2 = new Vector2[num2];
				float num6 = 0f;
				float num7 = 0.5f;
				float num8 = 1f;
				float num9 = 0.5f;
				if ((i & 1) > 0)
				{
					num6 += 0.5f;
					num7 += 0.5f;
				}
				if (i / 2 > 0)
				{
					num8 -= 0.5f;
					num9 -= 0.5f;
				}
				array2[0].x = num6;
				array2[0].y = num8;
				array2[1].x = num7;
				array2[1].y = num8;
				array2[2].x = num6;
				array2[2].y = num9;
				array2[3].x = num7;
				array2[3].y = num9;
				s_mesh[i].uv = array2;
				Color[] array3 = new Color[num2];
				Color black = Color.black;
				black.a = 0f;
				switch (i)
				{
				case 0:
					black.r = 1f;
					break;
				case 1:
					black.g = 1f;
					break;
				case 2:
					black.b = 1f;
					break;
				case 3:
					black.a = 1f;
					break;
				}
				array3[0] = black;
				array3[1] = black;
				array3[2] = black;
				array3[3] = black;
				s_mesh[i].colors = array3;
				int[] array4 = new int[num3];
				array4[0] = 0;
				array4[1] = 2;
				array4[2] = 1;
				array4[3] = 1;
				array4[4] = 2;
				array4[5] = 3;
				s_mesh[i].triangles = array4;
				s_mesh[i].RecalculateBounds();
			}
		}
		s_init = true;
	}

	private void Awake()
	{
		m_manager = ShadowBlobManager.Register();
	}

	private void Start()
	{
		Init();
		GameObject gameObject = new GameObject("ShadowBlob");
		m_shadowTransform = gameObject.transform;
		m_shadowTransform.rotation = Quaternion.identity;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = s_mesh[m_index];
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.castShadows = false;
		meshRenderer.receiveShadows = false;
		meshRenderer.sharedMaterial = m_manager.CombinedMaterial;
		SetGlowData(0f);
	}

	private void OnEnable()
	{
		if (m_shadowTransform != null)
		{
			m_shadowTransform.gameObject.active = true;
		}
	}

	private void OnDisable()
	{
		if (m_shadowTransform != null)
		{
			m_shadowTransform.gameObject.active = false;
		}
	}

	private void OnDestroy()
	{
		if ((bool)m_shadowTransform)
		{
			Object.Destroy(m_shadowTransform.gameObject);
		}
		m_manager.Unregister();
		m_manager = null;
	}

	public void SetTrackData(Vector3 _trackUp, Vector3 _vehicleForward)
	{
		if (m_shadowTransform != null)
		{
			m_shadowTransform.rotation = Quaternion.LookRotation(_trackUp, _vehicleForward);
		}
	}

	public void SetPosition(Vector3 _position)
	{
		if (m_shadowTransform != null)
		{
			m_shadowTransform.position = _position;
		}
	}

	public void SetAlpha(float _alpha)
	{
		m_alpha = _alpha;
		RenderSettings.Get.SetBlobShadowData(m_index, m_glow, m_alpha);
	}

	public void SetGlowData(float _glow)
	{
		m_glow = _glow;
		RenderSettings.Get.SetBlobShadowData(m_index, m_glow, m_alpha);
	}
}
