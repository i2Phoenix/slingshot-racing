using UnityEngine;

public class ShadowManager : MonoBehaviour
{
	public struct LightData
	{
		public Vector3 m_dir;

		public float m_weight;
	}

	private const int c_numQuadsPerShadow = 1;

	private const DynamicMesh.Attributes c_dynamicAttributes = (DynamicMesh.Attributes)226;

	private static readonly string c_shadowManagerName = "__ShadowManager";

	private static readonly string c_shadowManagerFind = "/" + c_shadowManagerName;

	private static Bounds c_maxBounds = new Bounds(Vector3.zero, Vector3.one * float.MaxValue);

	private MeshFilter m_meshFilter;

	private DynamicMesh m_mesh;

	private int m_nextShadow;

	private int m_numShadows;

	private int m_numActiveShadows;

	private int m_maxShadows;

	private Vector3[] m_vertices;

	private Vector2[] m_uvs;

	private Vector2[] m_uv1;

	private Color32[] m_colours;

	public Light[] m_nightLights;

	public LightData[] m_tempLightData;

	private void Awake()
	{
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.identity;
	}

	private void Start()
	{
		m_meshFilter = base.gameObject.AddComponent<MeshFilter>();
		Texture2D texture2D = null;
		TextureA8[] array = new TextureA8[4]
		{
			(TextureA8)Resources.Load("Textures/VehicleShadow" + VehicleFactory.GetStyle(0)),
			(TextureA8)Resources.Load("Textures/VehicleShadow" + VehicleFactory.GetStyle(1)),
			(TextureA8)Resources.Load("Textures/VehicleShadow" + VehicleFactory.GetStyle(2)),
			(TextureA8)Resources.Load("Textures/VehicleShadow" + VehicleFactory.GetStyle(3))
		};
		int width = array[0].m_width;
		int height = array[0].m_height;
		int width2 = width;
		int num = height * 4;
		Color[] array2 = new Color[width];
		texture2D = new Texture2D(width2, num, TextureFormat.Alpha8, true);
		for (int i = 0; i < 4; i++)
		{
			int num2 = num - (i + 1) * height;
			byte[] data = array[i].m_data;
			int j = 0;
			int num3 = 0;
			for (; j < height; j++)
			{
				int num4 = 0;
				while (num4 < width)
				{
					array2[num4].a = (float)(int)data[num3] * (1f / 255f);
					num4++;
					num3++;
				}
				texture2D.SetPixels(0, num2 + j, width, 1, array2);
			}
		}
		texture2D.Apply(true, true);
		Resources.UnloadUnusedAssets();
		Material material = (Material)Resources.Load("Materials/Shadow");
		material.mainTexture = texture2D;
		MeshRenderer meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		meshRenderer.castShadows = false;
		meshRenderer.receiveShadows = false;
		meshRenderer.sharedMaterial = material;
		if (!SceneSettings.RenderSettings.m_night || !Shadow.s_enableNightShadows)
		{
			return;
		}
		GameObject gameObject = GameObject.Find("Lights");
		if (gameObject != null)
		{
			Light[] componentsInChildren = gameObject.GetComponentsInChildren<Light>();
			m_nightLights = new Light[componentsInChildren.Length + 1];
			GameObject gameObject2 = GameObject.FindGameObjectWithTag("MainLight");
			if (gameObject2 != null)
			{
				Light component = gameObject2.GetComponent<Light>();
				m_nightLights[0] = component;
			}
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				m_nightLights[k + 1] = componentsInChildren[k];
			}
			m_tempLightData = new LightData[m_nightLights.Length];
		}
	}

	public int AddShadow(out Vector3[] o_vertices, out Vector2[] o_uvs, out Vector2[] o_uv1, out Color32[] o_colours)
	{
		if (m_maxShadows != m_numShadows)
		{
			m_maxShadows = m_numShadows;
			m_numActiveShadows = 0;
			int num = 1 * m_numShadows;
			int numVertices = num * 4;
			m_mesh = new DynamicMesh(numVertices, num * 2, (DynamicMesh.Attributes)226);
			m_vertices = m_mesh.Vertices;
			m_uvs = m_mesh.UVs;
			m_uv1 = m_mesh.UV1;
			m_colours = m_mesh.Colours;
			int[] array = m_mesh.LockTriangles();
			for (int i = 0; i != num; i++)
			{
				array[i * 6] = i * 4;
				array[i * 6 + 1] = i * 4 + 1;
				array[i * 6 + 2] = i * 4 + 2;
				array[i * 6 + 3] = i * 4 + 2;
				array[i * 6 + 4] = i * 4 + 1;
				array[i * 6 + 5] = i * 4 + 3;
			}
			m_mesh.UnlockTriangles();
			m_meshFilter.sharedMesh = m_mesh.Mesh;
			m_mesh.Bounds = c_maxBounds;
		}
		o_vertices = m_vertices;
		o_uvs = m_uvs;
		o_uv1 = m_uv1;
		o_colours = m_colours;
		return m_nextShadow++ * 4;
	}

	private void LateUpdate()
	{
		if (m_nextShadow > 0)
		{
			if (m_nextShadow < m_numActiveShadows)
			{
				int num = m_nextShadow * 4;
				int num2 = m_numActiveShadows * 4;
				for (int i = num; i != num2; i++)
				{
					m_vertices[i].x = 0f;
					m_vertices[i].y = 0f;
					m_vertices[i].z = 0f;
				}
			}
			m_numActiveShadows = m_nextShadow;
			m_mesh.SetDirty((DynamicMesh.Attributes)226);
			m_meshFilter.sharedMesh = m_mesh;
			m_nextShadow = 0;
		}
		else
		{
			m_meshFilter.sharedMesh = null;
		}
	}

	public static ShadowManager Register()
	{
		GameObject gameObject = GameObject.Find(c_shadowManagerFind);
		if (gameObject == null)
		{
			gameObject = new GameObject(c_shadowManagerName);
			gameObject.AddComponent<ShadowManager>();
		}
		ShadowManager component = gameObject.GetComponent<ShadowManager>();
		component.m_numShadows++;
		return component;
	}

	public void Unregister()
	{
		m_numShadows--;
	}
}
