using System;
using UnityEngine;

public class BypassRendererBase : MonoBehaviour
{
	protected struct MeshState
	{
		public BypassMesh mesh;

		public Matrix4x4 matrix;

		public BypassMaterial material;

		public ushort startIndex;

		public ushort indexCount;

		public bool isIdentityMatrix;
	}

	protected int m_numActiveMeshes;

	protected MeshState[] m_activeMeshes = new MeshState[16];

	public static BypassRenderer Get(Camera _camera)
	{
		BypassRenderer bypassRenderer = _camera.gameObject.GetComponent<BypassRenderer>();
		if (bypassRenderer == null)
		{
			bypassRenderer = _camera.gameObject.AddComponent<BypassRenderer>();
		}
		return bypassRenderer;
	}

	public void Draw(BypassMesh mesh, BypassMaterial material)
	{
		if (m_numActiveMeshes == m_activeMeshes.Length)
		{
			Array.Resize(ref m_activeMeshes, m_numActiveMeshes * 2);
		}
		m_activeMeshes[m_numActiveMeshes].mesh = mesh;
		m_activeMeshes[m_numActiveMeshes].matrix = Matrix4x4.identity;
		m_activeMeshes[m_numActiveMeshes].material = material;
		m_activeMeshes[m_numActiveMeshes].startIndex = 0;
		m_activeMeshes[m_numActiveMeshes].indexCount = (ushort)mesh.NumIndices;
		m_activeMeshes[m_numActiveMeshes].isIdentityMatrix = true;
		m_numActiveMeshes++;
	}

	public void Draw(BypassMesh mesh, Matrix4x4 matrix, BypassMaterial material)
	{
		if (m_numActiveMeshes == m_activeMeshes.Length)
		{
			Array.Resize(ref m_activeMeshes, m_numActiveMeshes * 2);
		}
		m_activeMeshes[m_numActiveMeshes].mesh = mesh;
		m_activeMeshes[m_numActiveMeshes].matrix = matrix;
		m_activeMeshes[m_numActiveMeshes].material = material;
		m_activeMeshes[m_numActiveMeshes].startIndex = 0;
		m_activeMeshes[m_numActiveMeshes].indexCount = (ushort)mesh.NumIndices;
		m_activeMeshes[m_numActiveMeshes].isIdentityMatrix = false;
		m_numActiveMeshes++;
	}

	public void DrawRange(BypassMesh mesh, BypassMaterial material, uint startIndex, uint indexCount)
	{
		if (m_numActiveMeshes == m_activeMeshes.Length)
		{
			Array.Resize(ref m_activeMeshes, m_numActiveMeshes * 2);
		}
		m_activeMeshes[m_numActiveMeshes].mesh = mesh;
		m_activeMeshes[m_numActiveMeshes].matrix = Matrix4x4.identity;
		m_activeMeshes[m_numActiveMeshes].material = material;
		m_activeMeshes[m_numActiveMeshes].startIndex = (ushort)startIndex;
		m_activeMeshes[m_numActiveMeshes].indexCount = (ushort)((indexCount == 0) ? (mesh.NumIndices - startIndex) : indexCount);
		m_activeMeshes[m_numActiveMeshes].isIdentityMatrix = true;
		m_numActiveMeshes++;
	}

	public void DrawRange(BypassMesh mesh, Matrix4x4 matrix, BypassMaterial material, uint startIndex, uint indexCount)
	{
		if (m_numActiveMeshes == m_activeMeshes.Length)
		{
			Array.Resize(ref m_activeMeshes, m_numActiveMeshes * 2);
		}
		m_activeMeshes[m_numActiveMeshes].mesh = mesh;
		m_activeMeshes[m_numActiveMeshes].matrix = matrix;
		m_activeMeshes[m_numActiveMeshes].material = material;
		m_activeMeshes[m_numActiveMeshes].startIndex = (ushort)startIndex;
		m_activeMeshes[m_numActiveMeshes].indexCount = (ushort)((indexCount == 0) ? (mesh.NumIndices - startIndex) : indexCount);
		m_activeMeshes[m_numActiveMeshes].isIdentityMatrix = false;
		m_numActiveMeshes++;
	}

	private void Update()
	{
		m_numActiveMeshes = 0;
	}
}
