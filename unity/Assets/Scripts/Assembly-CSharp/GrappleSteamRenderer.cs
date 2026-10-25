using UnityEngine;

[AddComponentMenu("Graphics/Grapple Steam Renderer")]
public class GrappleSteamRenderer : MonoBehaviour
{
	private struct Data
	{
		public Vector3 m_pos;

		public Vector3 m_dir;

		public Color m_colour;

		public float m_time;
	}

	private const uint c_maxQuads = 8u;

	private const float c_maxTime = 0.15f;

	private const int c_numFramesX = 4;

	private const int c_numFramesY = 4;

	private const float c_halfWidth = 1.5f;

	private const float c_length = 3f;

	private const int c_numFrames = 16;

	private const float c_numFramesF = 16f;

	private const float c_numFramesXRecip = 0.25f;

	private const float c_numFramesYRecip = 0.25f;

	private static readonly string c_name = "__GrappleSteamRenderer";

	private static readonly string c_find = "/" + c_name;

	public static bool s_enabled = true;

	private Data[] m_data;

	private Camera m_camera;

	private BypassRenderer m_renderer;

	private BypassMaterial m_material;

	private BypassMesh m_mesh;

	private Vector3[] m_vertices;

	private Vector2[] m_texCoords;

	private Color[] m_colours;

	private void Awake()
	{
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.identity;
		if (m_data == null)
		{
			m_data = new Data[8];
			for (int i = 0; (long)i < 8L; i++)
			{
				m_data[i].m_time = 0.15f;
			}
		}
	}

	private void Start()
	{
		Material material = (Material)Resources.Load("Materials/GrappleSteam");
		m_material = ((!material) ? null : new BypassMaterial(material));
		uint num = 8u;
		uint num2 = num * 4;
		m_mesh = new BypassMesh(name + " grapple steam");
		m_vertices = new Vector3[num2];
		m_texCoords = new Vector2[num2];
		m_colours = new Color[num2];
		ushort[] array = new ushort[num * 6];
		for (uint num3 = 0u; num3 != num; num3++)
		{
			array[num3 * 6] = (ushort)(num3 * 4);
			array[num3 * 6 + 1] = (ushort)(num3 * 4 + 1);
			array[num3 * 6 + 2] = (ushort)(num3 * 4 + 2);
			array[num3 * 6 + 3] = (ushort)(num3 * 4 + 2);
			array[num3 * 6 + 4] = (ushort)(num3 * 4 + 1);
			array[num3 * 6 + 5] = (ushort)(num3 * 4 + 3);
		}
		m_mesh.triangles = array;
		base.enabled = false;
	}

	public void Emit(int _index)
	{
		m_data[_index].m_time = 0f;
		base.enabled = true;
	}

	public void SetData(int _index, Vector3 _pos, Vector3 _dir, Color _colour)
	{
		m_data[_index].m_pos = _pos;
		m_data[_index].m_dir = _dir;
		m_data[_index].m_colour = _colour;
	}

	public bool IsActive(int _index)
	{
		return m_data[_index].m_time < 0.15f;
	}

	private void LateUpdate()
	{
		Camera camera = MainCamera.Get;
		if (camera == null)
		{
			Camera[] allCameras = Camera.allCameras;
			if (allCameras.Length > 0)
			{
				camera = allCameras[0];
			}
		}
		if (camera != m_camera)
		{
			m_camera = camera;
			m_renderer = BypassRendererBase.Get(m_camera);
		}
		if (m_material != null && m_camera != null)
		{
			float deltaTime = Time.deltaTime;
			uint num = 0u;
			uint num2 = 0u;
			for (int i = 0; (long)i < 8L; i++)
			{
				if (m_data[i].m_time < 0.15f)
				{
					int num3 = Mathf.FloorToInt(m_data[i].m_time / 0.15f * 16f);
					int num4 = num3 % 4;
					int num5 = num3 / 4;
					float num6 = (float)num4 * 0.25f;
					float num7 = 1f - (float)num5 * 0.25f;
					float x = num6 + 0.25f;
					float y = num7 - 0.25f;
					Vector3 zero = Vector3.zero;
					zero.x = m_data[i].m_dir.z;
					zero.z = 0f - m_data[i].m_dir.x;
					m_vertices[num2] = m_data[i].m_pos + m_data[i].m_dir * 3f + zero * 1.5f;
					m_vertices[num2 + 1] = m_data[i].m_pos + m_data[i].m_dir * 3f + zero * -1.5f;
					m_vertices[num2 + 2] = m_data[i].m_pos + zero * 1.5f;
					m_vertices[num2 + 3] = m_data[i].m_pos + zero * -1.5f;
					m_texCoords[num2].x = num6;
					m_texCoords[num2].y = num7;
					m_texCoords[num2 + 1].x = x;
					m_texCoords[num2 + 1].y = num7;
					m_texCoords[num2 + 2].x = num6;
					m_texCoords[num2 + 2].y = y;
					m_texCoords[num2 + 3].x = x;
					m_texCoords[num2 + 3].y = y;
					m_colours[num2] = m_data[i].m_colour;
					m_colours[num2 + 1] = m_data[i].m_colour;
					m_colours[num2 + 2] = m_data[i].m_colour;
					m_colours[num2 + 3] = m_data[i].m_colour;
					m_data[i].m_time += deltaTime;
					num++;
					num2 += 4;
				}
			}
			if (num != 0)
			{
				m_mesh.vertices = m_vertices;
				m_mesh.uv = m_texCoords;
				m_mesh.colors = m_colours;
				m_renderer.DrawRange(m_mesh, m_material, 0u, num * 6);
			}
			else
			{
				base.enabled = false;
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	public static GrappleSteamRenderer Get()
	{
		if (s_enabled)
		{
			GameObject gameObject = GameObject.Find(c_find);
			if (gameObject == null)
			{
				gameObject = new GameObject(c_name);
				gameObject.AddComponent<GrappleSteamRenderer>();
			}
			return gameObject.GetComponent<GrappleSteamRenderer>();
		}
		return null;
	}
    private void OnDestroy() { if (m_mesh != null) m_mesh.Dispose(); }
}
