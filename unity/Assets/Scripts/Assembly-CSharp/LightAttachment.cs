using System;
using UnityEngine;

[AddComponentMenu("Graphics/Light Attachment")]
public class LightAttachment : MonoBehaviour
{
	public static bool s_lightAttachmentsEnabled = true;

	private bool m_init;

	public GameObject m_attachment;

	public bool m_enableAutoLink = true;

	private void Start()
	{
		if (!s_lightAttachmentsEnabled)
		{
			return;
		}
		m_init = true;
		bool flag = true;
		LightAttachment[] array = UnityEngine.Object.FindObjectsOfType(typeof(LightAttachment)) as LightAttachment[];
		int num = 0;
		LightAttachment[] array2 = array;
		foreach (LightAttachment lightAttachment in array2)
		{
			if (lightAttachment.enabled && lightAttachment.m_attachment != null)
			{
				num++;
				if (!lightAttachment.m_init)
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
		GameObject gameObject = new GameObject("LightAttachment Batch Object");
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.identity;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = array[0].GetComponent<MeshRenderer>().material;
		meshRenderer.castShadows = false;
		meshRenderer.receiveShadows = false;
		Mesh mesh = new Mesh();
		int num2 = 6;
		int num3 = 3;
		int num4 = num * num2;
		int num5 = num * (num3 * 6);
		Vector3[] array3 = new Vector3[num4];
		Color[] array4 = new Color[num4];
		Vector2[] array5 = new Vector2[num4];
		Vector3[] array6 = new Vector3[num4];
		float num6 = 0.15f;
		float num7 = -(float)Math.PI / 2f;
		float num8 = (float)Math.PI * 2f / 3f;
		Vector2 vector = new Vector2(Mathf.Cos(num7), Mathf.Sin(num7));
		Vector3 vector2 = new Vector2(Mathf.Cos(num7 + num8), Mathf.Sin(num7 + num8));
		Vector3 vector3 = new Vector2(Mathf.Cos(num7 + num8 * 2f), Mathf.Sin(num7 + num8 * 2f));
		int num9 = 0;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].enabled && array[j].m_attachment != null)
			{
				Vector3 position = array[j].transform.position;
				Vector3 position2 = array[j].m_attachment.transform.position;
				Vector3 normalized = (position2 - position).normalized;
				Vector3 normalized2 = Vector3.Cross(normalized, Vector3.forward).normalized;
				Vector3 vector4 = Vector3.Cross(normalized2, normalized);
				position2 -= normalized * 0.1f;
				Vector3 vector5 = normalized2 * vector.x + vector4 * vector.y;
				Vector3 vector6 = normalized2 * vector2.x + vector4 * vector2.y;
				Vector3 vector7 = normalized2 * vector3.x + vector4 * vector3.y;
				Vector3 vector8 = vector5 * num6;
				Vector3 vector9 = vector6 * num6;
				Vector3 vector10 = vector7 * num6;
				Color white = Color.white;
				array3[num9] = position + vector8;
				array6[num9] = vector5;
				array4[num9] = white;
				array5[num9].x = 0f;
				array5[num9].y = 0f;
				num9++;
				array3[num9] = position + vector9;
				array6[num9] = vector6;
				array4[num9] = white;
				array5[num9].x = 0.5f;
				array5[num9].y = 0f;
				num9++;
				array3[num9] = position + vector10;
				array6[num9] = vector7;
				array4[num9] = white;
				array5[num9].x = 1f;
				array5[num9].y = 0f;
				num9++;
				array3[num9] = position2 + vector8;
				array6[num9] = vector5;
				array4[num9] = white;
				array5[num9].x = 0f;
				array5[num9].y = 1f;
				num9++;
				array3[num9] = position2 + vector9;
				array6[num9] = vector6;
				array4[num9] = white;
				array5[num9].x = 0.5f;
				array5[num9].y = 1f;
				num9++;
				array3[num9] = position2 + vector10;
				array6[num9] = vector7;
				array4[num9] = white;
				array5[num9].x = 1f;
				array5[num9].y = 1f;
				num9++;
			}
		}
		mesh.vertices = array3;
		mesh.colors = array4;
		mesh.uv = array5;
		mesh.normals = array6;
		int[] array7 = new int[num5];
		int num10 = 0;
		int num11 = 0;
		for (int k = 0; k < num; k++)
		{
			array7[num11++] = num10 + 3;
			array7[num11++] = num10 + 4;
			array7[num11++] = num10;
			array7[num11++] = num10;
			array7[num11++] = num10 + 4;
			array7[num11++] = num10 + 1;
			array7[num11++] = num10 + 4;
			array7[num11++] = num10 + 5;
			array7[num11++] = num10 + 1;
			array7[num11++] = num10 + 1;
			array7[num11++] = num10 + 5;
			array7[num11++] = num10 + 2;
			array7[num11++] = num10 + 5;
			array7[num11++] = num10 + 3;
			array7[num11++] = num10 + 2;
			array7[num11++] = num10 + 2;
			array7[num11++] = num10 + 3;
			array7[num11++] = num10;
			num10 += 6;
		}
		mesh.triangles = array7;
		mesh.bounds = new Bounds
		{
			center = Vector3.zero,
			extents = new Vector3(100000000f, 100000000f, 100000000f)
		};
		meshFilter.mesh = mesh;
	}
}
