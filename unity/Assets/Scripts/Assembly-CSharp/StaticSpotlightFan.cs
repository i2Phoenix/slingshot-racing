using System;
using UnityEngine;

[AddComponentMenu("Graphics/Static Spotlight Fan")]
public class StaticSpotlightFan : MonoBehaviour
{
	public static bool s_staticSpotlightFansEnabled = true;

	private bool m_init;

	public float m_intensityScale = 1f;

	public float m_radiusScale = 1f;

	public float m_angleScale = 1f;

	public bool m_horizontal;

	public float m_horizontalLerp;

	private void Start()
	{
		if (!s_staticSpotlightFansEnabled)
		{
			return;
		}
		m_init = true;
		bool flag = true;
		StaticSpotlightFan[] array = UnityEngine.Object.FindObjectsOfType(typeof(StaticSpotlightFan)) as StaticSpotlightFan[];
		int num = 0;
		StaticSpotlightFan[] array2 = array;
		foreach (StaticSpotlightFan staticSpotlightFan in array2)
		{
			if (staticSpotlightFan.enabled)
			{
				num++;
				if (!staticSpotlightFan.m_init)
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
		GameObject gameObject = new GameObject("StaticSpotlightFan Batch Object");
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.identity;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = array[0].GetComponent<MeshRenderer>().material;
		meshRenderer.castShadows = false;
		meshRenderer.receiveShadows = false;
		Mesh mesh = new Mesh();
		int num2 = 8;
		int num3 = num * (num2 + 1);
		int num4 = num * ((num2 - 1) * 3);
		Vector3[] array3 = new Vector3[num3];
		Color[] array4 = new Color[num3];
		Vector2[] array5 = new Vector2[num3];
		Vector2[] array6 = new Vector2[num3];
		Vector3[] array7 = new Vector3[num3];
		Vector4[] array8 = new Vector4[num3];
		int num5 = 0;
		for (int j = 0; j < array.Length; j++)
		{
			if (!array[j].enabled)
			{
				continue;
			}
			Vector3 position = array[j].transform.position;
			Color color = Color.white;
			float num6 = 8f;
			float num7 = 45f;
			Vector3 vector = Vector3.right;
			Light component = array[j].GetComponent<Light>();
			if (component == null)
			{
				component = array[j].transform.parent.GetComponent<Light>();
			}
			if (component != null)
			{
				color = component.color * component.intensity * 0.075f * array[j].m_intensityScale;
				num6 = component.range * 0.2f * array[j].m_radiusScale;
				num7 = component.spotAngle * array[j].m_angleScale;
				vector = component.transform.forward.normalized;
				vector.y = Mathf.Lerp(vector.y, 0f, array[j].m_horizontalLerp);
				vector.Normalize();
			}
			float num8 = Mathf.Max(color.r, Mathf.Max(color.g, color.b));
			color.r = num8 - (num8 - color.r) * 0.6f;
			color.g = num8 - (num8 - color.g) * 0.6f;
			color.b = num8 - (num8 - color.b) * 0.6f;
			Vector4 vector2 = new Vector4(vector.x, vector.y, vector.z, -1f);
			color.a = 0f;
			array3[num5] = position;
			array4[num5] = color;
			array5[num5].x = 0f;
			array5[num5].y = 0f;
			array6[num5].x = 0f;
			array6[num5].y = 0f;
			array7[num5] = Vector3.right;
			array8[num5] = vector2;
			num5++;
			float f = num7 * 0.5f * ((float)Math.PI / 180f);
			float num9 = Mathf.Cos(f);
			float num10 = Mathf.Sin(f);
			float num11 = num10 * num10;
			num6 *= 1f / num9;
			for (int k = 0; k < num2; k++)
			{
				float num12 = (float)k / (float)(num2 - 1);
				float num13 = (num12 - 0.5f) * num7;
				float f2 = num13 * ((float)Math.PI / 180f);
				Vector3 zero = Vector3.zero;
				zero.x = Mathf.Cos(f2);
				zero.z = Mathf.Sin(f2);
				float x = Mathf.Sqrt(Mathf.Max(num11 - zero.z * zero.z, 1E-06f));
				if (array[j].m_horizontal)
				{
					x = -1f;
				}
				float f3 = num12 * (float)Math.PI * 0.5f;
				color.a = num6 / 32f;
				array3[num5] = position;
				array4[num5] = color;
				array5[num5].x = Mathf.Cos(f3);
				array5[num5].y = Mathf.Sin(f3);
				array6[num5].x = x;
				array6[num5].y = zero.z;
				array7[num5] = zero;
				array8[num5] = vector2;
				num5++;
			}
		}
		mesh.vertices = array3;
		mesh.colors = array4;
		mesh.uv = array5;
		mesh.uv2 = array6;
		mesh.normals = array7;
		mesh.tangents = array8;
		int[] array9 = new int[num4];
		int num14 = 0;
		int num15 = 0;
		for (int l = 0; l < num; l++)
		{
			for (int m = 0; m < num2 - 1; m++)
			{
				array9[num15++] = num14;
				array9[num15++] = num14 + (m + 1);
				array9[num15++] = num14 + (m + 2);
			}
			num14 += num2 + 1;
		}
		mesh.triangles = array9;
		mesh.bounds = new Bounds
		{
			center = Vector3.zero,
			extents = new Vector3(100000000f, 100000000f, 100000000f)
		};
		meshFilter.mesh = mesh;
	}
}
