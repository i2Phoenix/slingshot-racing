using UnityEngine;

public class ShadowBlobManager : MonoBehaviour
{
	private static readonly string c_shadowBlobManagerName = "__ShadowBlobManager";

	private static readonly string c_shadowBlobManagerFind = "/" + c_shadowBlobManagerName;

	private static Material s_combinedMaterial;

	private int m_numInstances;

	public Material CombinedMaterial
	{
		get
		{
			return s_combinedMaterial;
		}
	}

	private void Awake()
	{
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.identity;
	}

	private void Start()
	{
		Texture2D texture2D = null;
		TextureA8[] array = new TextureA8[4]
		{
			(TextureA8)Resources.Load("Textures/VehicleBlobShadow" + VehicleFactory.GetStyle(0)),
			(TextureA8)Resources.Load("Textures/VehicleBlobShadow" + VehicleFactory.GetStyle(1)),
			(TextureA8)Resources.Load("Textures/VehicleBlobShadow" + VehicleFactory.GetStyle(2)),
			(TextureA8)Resources.Load("Textures/VehicleBlobShadow" + VehicleFactory.GetStyle(3))
		};
		int width = array[0].m_width;
		int height = array[0].m_height;
		int width2 = width * 2;
		int height2 = height * 2;
		texture2D = new Texture2D(width2, height2, TextureFormat.Alpha8, true);
		Color[] array2 = new Color[width];
		for (int i = 0; i < 4; i++)
		{
			int num = 0;
			int num2 = height;
			if ((i & 1) > 0)
			{
				num += width;
			}
			if (i / 2 > 0)
			{
				num2 -= height;
			}
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
				texture2D.SetPixels(num, num2 + j, width, 1, array2);
			}
		}
		texture2D.Apply(true, true);
		Resources.UnloadUnusedAssets();
		s_combinedMaterial.mainTexture = texture2D;
	}

	public static ShadowBlobManager Register()
	{
		GameObject gameObject = GameObject.Find(c_shadowBlobManagerFind);
		if (gameObject == null)
		{
			gameObject = new GameObject(c_shadowBlobManagerName);
			gameObject.AddComponent<ShadowBlobManager>();
			s_combinedMaterial = (Material)Resources.Load("Materials/ShadowBlobBaked");
		}
		ShadowBlobManager component = gameObject.GetComponent<ShadowBlobManager>();
		component.m_numInstances++;
		return component;
	}

	public void Unregister()
	{
		m_numInstances--;
	}
}
