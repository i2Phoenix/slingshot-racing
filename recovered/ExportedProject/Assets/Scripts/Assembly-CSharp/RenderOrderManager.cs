using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Graphics/Render Order Manager")]
public class RenderOrderManager : MonoBehaviour
{
	private void Start()
	{
		MeshRenderer[] componentsInChildren = GameObjectUtils.GetComponentsInChildren<MeshRenderer>(base.gameObject);
		Array.Sort((UnityEngine.Object[])componentsInChildren, (IComparer<UnityEngine.Object>)default(GameObjectUtils.CompareByName));
		for (int i = 0; i != componentsInChildren.Length; i++)
		{
			MeshRenderer meshRenderer = componentsInChildren[i];
			Material sharedMaterial = meshRenderer.sharedMaterial;
			sharedMaterial.renderQueue = sharedMaterial.shader.renderQueue + i - componentsInChildren.Length;
		}
	}
}
