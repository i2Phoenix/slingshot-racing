using UnityEngine;

[AddComponentMenu("Graphics/Static Light Flare")]
public class StaticLightFlare : MonoBehaviour
{
	public static bool s_staticLightFlaresEnabled = true;

	private bool m_init;

	public float m_intensityScale = 1f;

	public float m_radiusScale = 1f;

	private void Start()
	{
		if (!s_staticLightFlaresEnabled)
		{
			return;
		}
		m_init = true;
		bool flag = true;
		StaticLightFlare[] array = Object.FindObjectsOfType(typeof(StaticLightFlare)) as StaticLightFlare[];
		int num = 0;
		StaticLightFlare[] array2 = array;
		foreach (StaticLightFlare staticLightFlare in array2)
		{
			if (staticLightFlare.enabled)
			{
				num++;
				if (!staticLightFlare.m_init)
				{
					flag = false;
					break;
				}
			}
		}
		if (!flag || num <= 0)
		{
			return;
		}
		GameObject gameObject = new GameObject("StaticLightFlare Batch Object");
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.identity;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = array[0].GetComponent<MeshRenderer>().material;
		meshRenderer.castShadows = false;
		meshRenderer.receiveShadows = false;
		Mesh mesh = new Mesh();
		int num2 = num * 4;
		int num3 = num * 6;
		Vector3[] array3 = new Vector3[num2];
		Color[] array4 = new Color[num2];
		Vector2[] array5 = new Vector2[num2];
		int num4 = 0;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].enabled)
			{
				Vector3 position = array[j].transform.position;
				Color color = Color.white;
				float num5 = 8f;
				Light component = array[j].GetComponent<Light>();
				if (component == null)
				{
					component = array[j].transform.parent.GetComponent<Light>();
				}
				if (component != null)
				{
					color = component.color * 0.5f * array[j].m_intensityScale;
					num5 = component.intensity * 1f * array[j].m_radiusScale;
				}
				float num6 = Mathf.Max(color.r, Mathf.Max(color.g, color.b));
				color.r = num6 - (num6 - color.r) * 0.6f;
				color.g = num6 - (num6 - color.g) * 0.6f;
				color.b = num6 - (num6 - color.b) * 0.6f;
				color.a = num5 / 32f;
				array3[num4] = position;
				array3[num4 + 1] = position;
				array3[num4 + 2] = position;
				array3[num4 + 3] = position;
				array4[num4] = color;
				array4[num4 + 1] = color;
				array4[num4 + 2] = color;
				array4[num4 + 3] = color;
				array5[num4].x = 0f;
				array5[num4].y = 0f;
				array5[num4 + 1].x = 1f;
				array5[num4 + 1].y = 0f;
				array5[num4 + 2].x = 0f;
				array5[num4 + 2].y = 1f;
				array5[num4 + 3].x = 1f;
				array5[num4 + 3].y = 1f;
				num4 += 4;
			}
		}
		mesh.vertices = array3;
		mesh.colors = array4;
		mesh.uv = array5;
		int[] array6 = new int[num3];
		int num7 = 0;
		int num8 = 0;
		for (int k = 0; k < num; k++)
		{
			array6[num8++] = num7;
			array6[num8++] = num7 + 2;
			array6[num8++] = num7 + 1;
			array6[num8++] = num7 + 1;
			array6[num8++] = num7 + 2;
			array6[num8++] = num7 + 3;
			num7 += 4;
		}
		mesh.triangles = array6;
		mesh.bounds = new Bounds
		{
			center = Vector3.zero,
			extents = new Vector3(100000000f, 100000000f, 100000000f)
		};
		meshFilter.mesh = mesh;
	}
}
