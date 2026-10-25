using System.Diagnostics;
using UnityEngine;

public class BypassMeshBase
{
	protected uint m_numIndices;

	protected ushort[] m_indices;

	protected Vector3[] m_vertices;

	protected Vector3[] m_normals;

	protected Color[] m_colours;

	protected ColourRGBA32[] m_colour32s;

	protected Vector2[] m_texCoords;

	public uint NumIndices
	{
		get
		{
			return m_numIndices;
		}
	}

	[Conditional("DEBUG")]
	protected void CheckDraw(uint startIndex, uint indexCount)
	{
		uint num = (uint)m_indices.Length;
		uint num2 = (uint)m_vertices.Length;
		for (uint num3 = 0u; num3 != num; num3++)
		{
		}
	}
}
