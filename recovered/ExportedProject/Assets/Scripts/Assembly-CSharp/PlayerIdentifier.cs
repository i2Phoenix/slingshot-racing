using UnityEngine;

public class PlayerIdentifier : MonoBehaviour
{
	private struct Data
	{
		public float m_alpha;

		public float m_width;

		public float m_height;
	}

	public static float s_width = 0f;

	public static float s_height = 0f;

	public static float[] s_zOffset = new float[4];

	private static float s_halfWidth = s_width * 0.5f;

	private static float s_halfHeight = s_height * 0.5f;

	private Transform m_meshTransform;

	private int m_index;

	private static bool s_init = false;

	private static Material s_material;

	private static Mesh[] s_mesh;

	private static Data[] s_data;

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
		if (!s_material)
		{
			s_material = (Material)Resources.Load("Materials/PlayerIdentifier");
		}
		if (s_mesh == null)
		{
			s_mesh = new Mesh[4];
			for (int i = 0; i < 4; i++)
			{
				s_mesh[i] = new Mesh();
				int num = 1;
				int num2 = num * 4;
				int num3 = num * 6;
				Vector3[] array = new Vector3[num2];
				float num4 = s_halfWidth;
				float num5 = s_halfHeight;
				array[0].x = 0f - num4;
				array[0].z = s_zOffset[i] + num5;
				array[1].x = num4;
				array[1].z = s_zOffset[i] + num5;
				array[2].x = 0f - num4;
				array[2].z = s_zOffset[i] - num5;
				array[3].x = num4;
				array[3].z = s_zOffset[i] - num5;
				s_mesh[i].vertices = array;
				Vector2[] array2 = new Vector2[num2];
				float x = 0f;
				float x2 = 1f;
				float y = 1f;
				float y2 = 0f;
				array2[0].x = x;
				array2[0].y = y;
				array2[1].x = x2;
				array2[1].y = y;
				array2[2].x = x;
				array2[2].y = y2;
				array2[3].x = x2;
				array2[3].y = y2;
				s_mesh[i].uv = array2;
				Color[] array3 = new Color[num2];
				Color brightColourForPlayer = Vehicle.GetBrightColourForPlayer(i);
				switch (i)
				{
				case 0:
					brightColourForPlayer.a = 0f;
					break;
				case 1:
					brightColourForPlayer.a = 0.3333f;
					break;
				case 2:
					brightColourForPlayer.a = 0.6666f;
					break;
				case 3:
					brightColourForPlayer.a = 1f;
					break;
				}
				array3[0] = brightColourForPlayer;
				array3[1] = brightColourForPlayer;
				array3[2] = brightColourForPlayer;
				array3[3] = brightColourForPlayer;
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
		if (s_data == null)
		{
			s_data = new Data[4];
			for (int j = 0; j < 4; j++)
			{
				s_data[j].m_alpha = 0f;
				s_data[j].m_width = 0f;
				s_data[j].m_height = 0f;
			}
			SetShaderData();
		}
		s_init = true;
	}

	private static void SetShaderData()
	{
		Vector4 zero = Vector4.zero;
		zero.x = s_data[0].m_alpha;
		zero.y = s_data[1].m_alpha;
		zero.z = s_data[2].m_alpha;
		zero.w = s_data[3].m_alpha;
		Shader.SetGlobalVector("g_playerIdentifierAlpha", zero);
		Vector4 zero2 = Vector4.zero;
		zero2.x = s_data[0].m_width;
		zero2.y = s_data[1].m_width;
		zero2.z = s_data[2].m_width;
		zero2.w = s_data[3].m_width;
		Shader.SetGlobalVector("g_playerIdentifierWidth", zero2);
		Vector4 zero3 = Vector4.zero;
		zero3.x = s_data[0].m_height;
		zero3.y = s_data[1].m_height;
		zero3.z = s_data[2].m_height;
		zero3.w = s_data[3].m_height;
		Shader.SetGlobalVector("g_playerIdentifierHeight", zero3);
	}

	private void Start()
	{
		Init();
		GameObject gameObject = new GameObject("PlayerIdentifier");
		m_meshTransform = gameObject.transform;
		m_meshTransform.rotation = Quaternion.identity;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = s_mesh[m_index];
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.castShadows = false;
		meshRenderer.receiveShadows = false;
		meshRenderer.sharedMaterial = s_material;
	}

	private void OnEnable()
	{
		if (m_meshTransform != null)
		{
			m_meshTransform.gameObject.active = true;
		}
	}

	private void OnDisable()
	{
		if (m_meshTransform != null)
		{
			m_meshTransform.gameObject.active = false;
		}
	}

	private void OnDestroy()
	{
		if ((bool)m_meshTransform)
		{
			Object.Destroy(m_meshTransform.gameObject);
		}
	}

	public void SetData(Vector3 _position, Vector3 _trackUp, Vector3 _vehicleForward)
	{
		if (m_meshTransform != null)
		{
			m_meshTransform.position = _position;
			m_meshTransform.rotation = Quaternion.LookRotation(_trackUp, _vehicleForward);
		}
	}

	public void SetData(Vector3 _position)
	{
		if (m_meshTransform != null)
		{
			m_meshTransform.position = _position;
		}
	}

	public void SetExtraData(float _alpha, float _width, float _height)
	{
		Init();
		s_data[m_index].m_alpha = _alpha;
		s_data[m_index].m_width = _width;
		s_data[m_index].m_height = _height;
		SetShaderData();
	}
}
