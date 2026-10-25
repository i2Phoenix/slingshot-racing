using UnityEngine;

public class LightShaft : MonoBehaviour
{
	private bool m_init;

	public float m_verticalAngle;

	public float m_length = 30f;

	public float m_width = 7f;

	public Color m_colour = Color.white;

	public bool m_colourFromLight = true;

	public float m_intensity = 1f;

	private void Start()
	{
		if (!PlatformSettings.EnableLightShafts)
		{
			return;
		}
		Color color = m_colour;
		float verticalAngle = m_verticalAngle;
		float y = 0f;
		float z = 0f;
		GameObject gameObject = GameObject.FindGameObjectWithTag("MainLight");
		if (gameObject != null)
		{
			y = gameObject.transform.rotation.eulerAngles.y;
			if (m_colourFromLight)
			{
				Light component = gameObject.GetComponent<Light>();
				if (component != null)
				{
					color = component.color;
					color.r *= component.intensity;
					color.g *= component.intensity;
					color.b *= component.intensity;
				}
			}
		}
		Vector3 vector = Quaternion.Euler(verticalAngle, y, z) * Vector3.forward;
		Vector3 vector2 = -Vector3.Cross(Vector3.up, vector).normalized;
		color.r *= m_intensity;
		color.g *= m_intensity;
		color.b *= m_intensity;
		Mesh mesh = new Mesh();
		int num = 8;
		int num2 = 6;
		int num3 = (num + 1) * (num2 + 1);
		int num4 = num * num2 * 6;
		int num5 = 2;
		Vector3[] array = new Vector3[num3];
		Color[] array2 = new Color[num3];
		float num6 = 22f / 127f;
		float num7 = (float)num5 / (float)num2;
		float num8 = 3.5f;
		float num9 = 4f;
		float num10 = 3f;
		int num11 = 0;
		for (int i = 0; i < num2 + 1; i++)
		{
			float num12 = (float)i / (float)num2;
			num12 = ((!(num12 < num7)) ? ((num12 - num7) / (1f - num7) * (1f - num6) + num6) : (num12 / num7 * num6));
			float num13;
			if (num12 > num6)
			{
				num13 = (num12 - num6) / (1f - num6);
				num13 = Mathf.Pow(2.718f, (0f - num10) * (num13 * num13));
			}
			else
			{
				num13 = num12 / num6;
				num13 = Mathf.Pow(2.718f, (0f - num9) * ((1f - num13) * (1f - num13)));
			}
			if (i == 0 || i == num2)
			{
				num13 = 0f;
			}
			for (int j = 0; j < num + 1; j++)
			{
				float num14 = (float)j / (float)num;
				float num15 = Mathf.Abs(num14 - 0.5f) * 2f;
				num15 = Mathf.Pow(2.718f, (0f - num8) * (num15 * num15));
				if (j == 0 || j == num)
				{
					num15 = 0f;
				}
				array[num11] = vector2 * (m_width * (num14 - 0.5f)) + vector * (m_length * num12);
				array2[num11] = color * (num15 * num13);
				num11++;
			}
		}
		mesh.vertices = array;
		mesh.colors = array2;
		int[] array3 = new int[num4];
		int num16 = 0;
		int num17 = 0;
		for (int k = 0; k < num2; k++)
		{
			for (int l = 0; l < num; l++)
			{
				bool flag = false;
				if (l >= num / 2)
				{
					flag = true;
				}
				if (k >= num5)
				{
					flag = !flag;
				}
				if (flag)
				{
					array3[num17++] = num16 + 1;
					array3[num17++] = num16 + 1 + (num + 1);
					array3[num17++] = num16;
					array3[num17++] = num16;
					array3[num17++] = num16 + 1 + (num + 1);
					array3[num17++] = num16 + (num + 1);
				}
				else
				{
					array3[num17++] = num16;
					array3[num17++] = num16 + 1;
					array3[num17++] = num16 + (num + 1);
					array3[num17++] = num16 + (num + 1);
					array3[num17++] = num16 + 1;
					array3[num17++] = num16 + 1 + (num + 1);
				}
				num16++;
			}
			num16++;
		}
		mesh.triangles = array3;
		MeshFilter meshFilter = base.gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		mesh.RecalculateBounds();
		m_init = true;
		bool flag2 = true;
		LightShaft[] array4 = Object.FindObjectsOfType(typeof(LightShaft)) as LightShaft[];
		LightShaft[] array5 = array4;
		foreach (LightShaft lightShaft in array5)
		{
			if (!lightShaft.m_init)
			{
				flag2 = false;
				break;
			}
		}
		if (flag2)
		{
			GameObject[] array6 = new GameObject[array4.Length];
			for (int n = 0; n < array4.Length; n++)
			{
				array6[n] = array4[n].gameObject;
			}
			GameObject staticBatchRoot = new GameObject("LightShaft Batch Object");
			StaticBatchingUtility.Combine(array6, staticBatchRoot);
		}
	}
}
