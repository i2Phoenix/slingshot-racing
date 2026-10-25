using System.Runtime.InteropServices;
using UnityEngine;

public class BypassMesh : BypassMeshBase
{
	private static Vector2[] s_dummyVector2 = new Vector2[1];

	private static Vector3[] s_dummyVector3 = new Vector3[1];

	private static Color[] s_dummyColor = new Color[1];

	private static ColourRGBA32[] s_dummyColourRGBA32 = new ColourRGBA32[1];

	public ushort[] triangles
	{
		set
		{
			m_indices = value;
			m_numIndices = ((m_indices != null) ? ((uint)m_indices.Length) : 0u);
		}
	}

	public Vector3[] vertices
	{
		set
		{
			m_vertices = value;
		}
	}

	public Vector3[] normals
	{
		set
		{
			m_normals = value;
		}
	}

	public Color[] colors
	{
		set
		{
			m_colours = value;
			m_colour32s = null;
		}
	}

	public ColourRGBA32[] colors32
	{
		set
		{
			m_colour32s = value;
			m_colours = null;
		}
	}

	public Vector2[] uv
	{
		set
		{
			m_texCoords = value;
		}
	}

	public void DrawRange(uint matAttributeMask, uint startIndex, uint indexCount)
	{
		if (indexCount != 0)
		{
			uint num = 0u;
			if (m_normals != null)
			{
				num |= 4;
			}
			if (m_colours != null)
			{
				num |= 2;
			}
			if (m_colour32s != null)
			{
				num |= 1;
			}
			if (m_texCoords != null)
			{
				num |= 8;
			}
			if ((matAttributeMask & 4) != 0 && (num & 4) == 0)
			{
				Debug.LogError("Material requires normals but BypassMesh doesn't provide them");
			}
			if ((matAttributeMask & 2) != 0 && (num & 2) == 0 && (num & 1) == 0)
			{
				Debug.LogError("Material requires colours but BypassMesh doesn't provide them");
			}
			if ((matAttributeMask & 8) != 0 && (num & 8) == 0)
			{
				Debug.LogError("Material requires uvs but BypassMesh doesn't provide them");
			}
			BypassMesh_DrawRange(ref m_indices[0], ref m_vertices[0], ref ((m_normals == null) ? s_dummyVector3 : m_normals)[0], ref ((m_colours == null) ? s_dummyColor : m_colours)[0], ref ((m_colour32s == null) ? s_dummyColourRGBA32 : m_colour32s)[0], ref ((m_texCoords == null) ? s_dummyVector2 : m_texCoords)[0], matAttributeMask, num, startIndex, indexCount);
		}
	}

	[DllImport("GrappleRacerNative")]
	private static extern void BypassMesh_DrawRange(ref ushort indices, ref Vector3 vertices, ref Vector3 normals, ref Color colours, ref ColourRGBA32 colours32, ref Vector2 texCoords, uint matAttributeMask, uint meshFlags, uint startIndex, uint indexCount);
}
