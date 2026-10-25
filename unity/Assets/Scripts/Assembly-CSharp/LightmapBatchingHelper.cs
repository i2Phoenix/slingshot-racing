using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Graphics/Lightmap Batching Helper")]
public class LightmapBatchingHelper : MonoBehaviour
{
	private class KeyComparer : IEqualityComparer<Key>
	{
		public bool Equals(Key _lhs, Key _rhs)
		{
			return _lhs.m_lightmapIndex == _rhs.m_lightmapIndex && _lhs.m_srcMaterial == _rhs.m_srcMaterial;
		}

		public int GetHashCode(Key _key)
		{
			return _key.m_lightmapIndex.GetHashCode() ^ _key.m_srcMaterial.GetHashCode();
		}
	}

	private struct Key
	{
		public Material m_srcMaterial;

		public int m_lightmapIndex;
	}

	private void Start()
	{
		Dictionary<Key, Material> dictionary = new Dictionary<Key, Material>(new KeyComparer());
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			if (meshRenderer.lightmapIndex < 0)
			{
				continue;
			}
			Material[] sharedMaterials = meshRenderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				Material material = sharedMaterials[j];
				Key key = new Key
				{
					m_srcMaterial = material,
					m_lightmapIndex = meshRenderer.lightmapIndex
				};
				if (!dictionary.ContainsKey(key))
				{
					dictionary[key] = new Material(material);
				}
				sharedMaterials[j] = dictionary[key];
			}
			meshRenderer.sharedMaterials = sharedMaterials;
		}
	}
}
