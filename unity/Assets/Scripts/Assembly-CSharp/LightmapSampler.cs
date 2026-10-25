using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Graphics/Lightmap Sampler")]
public class LightmapSampler : MonoBehaviour
{
	private struct MaterialPair
	{
		public Material original;

		public Material clone;

		public MaterialPair(Material original, Material clone)
		{
			this.original = original;
			this.clone = clone;
		}
	}

	private const string p_lightmap = "_Lightmap";

	private const string p_lmMatR1 = "p_lmMatR1";

	private const string p_lmMatR2 = "p_lmMatR2";

	private const string p_lightDirOS = "p_lightDirOS";

	private const string p_halfVecOS = "p_halfVecOS";

	private bool m_constantExtraction;

	private Vector4 m_row1;

	private Vector4 m_row2;

	private Transform m_transform;

	private Material[] m_materials;

	private Texture2D[] m_lightmapTextures;

	private int m_currentLightmapIndex = -1;

	private Color m_coarseLighting;

	public Color CoarseLighting
	{
		get
		{
			return m_coarseLighting;
		}
	}

	private void Awake()
	{
		m_transform = base.transform;
		m_coarseLighting = new Color(1f, 1f, 1f, 1f);
	}

	private void Start()
	{
		List<Renderer> list = new List<Renderer>();
		list.AddRange(GetComponentsInChildren<MeshRenderer>());
		list.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());
		List<MaterialPair> list2 = new List<MaterialPair>();
		int num = 0;
		foreach (Renderer item in list)
		{
			Material[] sharedMaterials = item.sharedMaterials;
			bool flag = false;
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				Material material = sharedMaterials[i];
				if (!material.HasProperty("_Lightmap"))
				{
					continue;
				}
				Material material2 = null;
				foreach (MaterialPair item2 in list2)
				{
					if (material == item2.original)
					{
						material2 = item2.clone;
						break;
					}
				}
				if (material2 == null)
				{
					material2 = ((!material.name.EndsWith("(Clone)")) ? new Material(material) : material);
					list2.Add(new MaterialPair(material, material2));
				}
				sharedMaterials[i] = material2;
				flag = true;
			}
			if (flag)
			{
				item.sharedMaterials = sharedMaterials;
				num++;
			}
		}
		m_materials = new Material[list2.Count];
		int num2 = 0;
		foreach (MaterialPair item3 in list2)
		{
			m_materials[num2++] = item3.clone;
		}
		LightmapData[] lightmaps = LightmapSettings.lightmaps;
		m_lightmapTextures = new Texture2D[lightmaps.Length];
		for (int j = 0; j < m_lightmapTextures.Length; j++)
		{
			m_lightmapTextures[j] = lightmaps[j].lightmapColor;
		}
	}

	public void UpdateObjectSpaceParams(Transform rendererTransform)
	{
		Transform transform = MainCamera.Transform;
		if (!(transform == null))
		{
			m_constantExtraction = true;
			Vector3 position = rendererTransform.position;
			Quaternion q = rendererTransform.rotation;
			q.x = 0f - q.x;
			q.y = 0f - q.y;
			q.z = 0f - q.z;
			RenderSettings get = RenderSettings.Get;
			Vector3 highlightDirection = get.HighlightDirection;
			QuaternionUtils.QuaternionMul(ref highlightDirection.x, ref q);
			Vector3 vector = MainCamera.Transform.position - position;
			QuaternionUtils.QuaternionMul(ref vector.x, ref q);
			Vector3 normalized = (vector.normalized - highlightDirection).normalized;
			Vector4 row = m_row1;
			Vector4 row2 = m_row2;
			row.w = row.x * position.x + row.y * position.y + row.z * position.z + row.w;
			row2.w = row2.x * position.x + row2.y * position.y + row2.z * position.z + row2.w;
			QuaternionUtils.QuaternionMul(ref row.x, ref q);
			QuaternionUtils.QuaternionMul(ref row2.x, ref q);
			Material[] materials = m_materials;
			foreach (Material material in materials)
			{
				material.SetVector("p_lightDirOS", highlightDirection);
				material.SetVector("p_halfVecOS", normalized);
				material.SetVector("p_lmMatR1", row);
				material.SetVector("p_lmMatR2", row2);
			}
		}
	}

	public void UpdateForTrackSpline(TrackSpline _trackSpline, float _t)
	{
		Vector3 o_point;
		Vector3 o_tangent;
		Vector3 o_normal;
		float o_width;
		_trackSpline.InfoAtT(_t, out o_point, out o_tangent, out o_normal, out o_width);
		o_tangent.Normalize();
		UpdateForTrackSpline(_trackSpline, _t, o_point, o_normal, o_tangent, o_width);
	}

	public void UpdateForTrackSpline(TrackSpline _trackSpline, float _t, Vector3 _point, Vector3 _normal, Vector3 _tangent, float _width)
	{
		TrackLighting lighting = _trackSpline.Lighting;
		if (lighting == null)
		{
			return;
		}
		Vector4 trackUVScaleOffset = lighting.trackUVScaleOffset;
		Vector4 lightmapTilingOffset = lighting.lightmapTilingOffset;
		float num = _trackSpline.ReParamTtoS(_t) + lighting.m_lightmapPadding;
		float num2 = 1f / (_trackSpline.Length + 2f * lighting.m_lightmapPadding);
		float num3 = num * num2;
		if (lighting.Reversed)
		{
			num3 = 1f - num3;
			_normal = -_normal;
			_tangent = -_tangent;
		}
		Vector4 zero = Vector4.zero;
		Vector3 lhs = m_transform.position - _point;
		float num4 = Vector3.Dot(lhs, _normal) / _width;
		float num5 = Mathf.Clamp01(num4 * 0.5f + 0.5f);
		float num6 = Mathf.Clamp01(num3);
		float num7 = num5 * trackUVScaleOffset.x + trackUVScaleOffset.z;
		float num8 = num6 * trackUVScaleOffset.y + trackUVScaleOffset.w;
		if (lighting.HasCoarseSamples)
		{
			m_coarseLighting = lighting.CoarseSample(num5, num6);
		}
		zero.z = lightmapTilingOffset.z + lightmapTilingOffset.x * num7;
		zero.w = lightmapTilingOffset.w + lightmapTilingOffset.y * num8;
		int lightmapIndex = lighting.lightmapIndex;
		if (m_lightmapTextures != null && lightmapIndex < m_lightmapTextures.Length)
		{
			float num9 = 0.5f / _width * trackUVScaleOffset.x * lightmapTilingOffset.x;
			float num10 = (0.5f * trackUVScaleOffset.x + trackUVScaleOffset.z) * lightmapTilingOffset.x + lightmapTilingOffset.z;
			m_row1.x = _normal.x * num9;
			m_row1.y = _normal.y * num9;
			m_row1.z = _normal.z * num9;
			m_row1.w = num10 - num9 * Vector3.Dot(_normal, _point);
			num9 = num2 * trackUVScaleOffset.y * lightmapTilingOffset.y;
			num10 = (num3 * trackUVScaleOffset.y + trackUVScaleOffset.w) * lightmapTilingOffset.y + lightmapTilingOffset.w;
			m_row2.x = _tangent.x * num9;
			m_row2.y = _tangent.y * num9;
			m_row2.z = _tangent.z * num9;
			m_row2.w = num10 - num9 * Vector3.Dot(_tangent, _point);
			if (lightmapIndex != m_currentLightmapIndex)
			{
				m_currentLightmapIndex = lightmapIndex;
				UpdateLightmapTexture(m_lightmapTextures[lightmapIndex]);
			}
			UpdateLightmapMatrix();
		}
	}

	public void UpdateForTrackHub(TrackHub _trackHub)
	{
		TrackLighting lighting = _trackHub.Lighting;
		if (lighting == null)
		{
			return;
		}
		Transform transform = _trackHub.transform;
		Vector4 trackUVScaleOffset = lighting.trackUVScaleOffset;
		Vector4 lightmapTilingOffset = lighting.lightmapTilingOffset;
		Vector4 zero = Vector4.zero;
		Vector3 vector = transform.InverseTransformPoint(m_transform.position);
		float num = Mathf.Clamp01(vector.x * trackUVScaleOffset.x + trackUVScaleOffset.z);
		float num2 = Mathf.Clamp01(vector.z * trackUVScaleOffset.y + trackUVScaleOffset.w);
		if (lighting.HasCoarseSamples)
		{
			m_coarseLighting = lighting.CoarseSample(num, num2);
		}
		zero.z = lightmapTilingOffset.z + lightmapTilingOffset.x * num;
		zero.w = lightmapTilingOffset.w + lightmapTilingOffset.y * num2;
		int lightmapIndex = lighting.lightmapIndex;
		if (m_lightmapTextures != null && lightmapIndex < m_lightmapTextures.Length)
		{
			Vector3 right = transform.right;
			float num3 = trackUVScaleOffset.x * lightmapTilingOffset.x;
			float num4 = trackUVScaleOffset.z * lightmapTilingOffset.x + lightmapTilingOffset.z;
			m_row1.x = right.x * num3;
			m_row1.y = right.y * num3;
			m_row1.z = right.z * num3;
			m_row1.w = num4 - num3 * Vector3.Dot(right, transform.position);
			right = transform.forward;
			num3 = trackUVScaleOffset.y * lightmapTilingOffset.y;
			num4 = trackUVScaleOffset.w * lightmapTilingOffset.y + lightmapTilingOffset.w;
			m_row2.x = right.x * num3;
			m_row2.y = right.y * num3;
			m_row2.z = right.z * num3;
			m_row2.w = num4 - num3 * Vector3.Dot(right, transform.position);
			if (lightmapIndex != m_currentLightmapIndex)
			{
				m_currentLightmapIndex = lightmapIndex;
				UpdateLightmapTexture(m_lightmapTextures[lightmapIndex]);
			}
			UpdateLightmapMatrix();
		}
	}

	private void UpdateLightmapMatrix()
	{
		if (!m_constantExtraction)
		{
			Material[] materials = m_materials;
			foreach (Material material in materials)
			{
				material.SetVector("p_lmMatR1", m_row1);
				material.SetVector("p_lmMatR2", m_row2);
			}
		}
	}

	private void UpdateLightmapTexture(Texture lightmap)
	{
		Material[] materials = m_materials;
		foreach (Material material in materials)
		{
			material.SetTexture("_Lightmap", lightmap);
		}
	}
}
