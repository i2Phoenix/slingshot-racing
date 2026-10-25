using UnityEngine;

public class DoubleBufferedMesh
{
	private enum DirtyFlags
	{
		Vertices = 1,
		Normals = 2,
		Tangents = 4,
		Colors = 8,
		UV = 0x10,
		UV1 = 0x20,
		UV2 = 0x40
	}

	private DirtyFlags m_newDirty;

	private DirtyFlags m_oldDirty;

	private Mesh m_newMesh;

	private Mesh m_oldMesh;

	public Mesh mesh
	{
		get
		{
			if (m_oldDirty != 0)
			{
				SyncOldToNew();
				Mesh oldMesh = m_oldMesh;
				m_oldMesh = m_newMesh;
				m_newMesh = oldMesh;
				m_newDirty = m_oldDirty;
				m_oldDirty = (DirtyFlags)0;
			}
			return m_oldMesh;
		}
	}

	public Bounds bounds
	{
		get
		{
			return m_newMesh.bounds;
		}
		set
		{
			m_oldMesh.bounds = value;
			m_newMesh.bounds = value;
		}
	}

	public int vertexCount
	{
		get
		{
			return (((m_oldDirty & DirtyFlags.Vertices) == 0) ? m_oldMesh : m_newMesh).vertexCount;
		}
	}

	public int[] triangles
	{
		get
		{
			return GetTriangles(0);
		}
		set
		{
			SetTriangles(value, 0);
		}
	}

	public Vector3[] vertices
	{
		get
		{
			return (((m_oldDirty & DirtyFlags.Vertices) == 0) ? m_oldMesh : m_newMesh).vertices;
		}
		set
		{
			m_newMesh.vertices = value;
			m_oldDirty |= DirtyFlags.Vertices;
		}
	}

	public Color[] colors
	{
		get
		{
			return (((m_oldDirty & DirtyFlags.Colors) == 0) ? m_oldMesh : m_newMesh).colors;
		}
		set
		{
			m_newMesh.colors = value;
			m_oldDirty |= DirtyFlags.Colors;
		}
	}

	public Vector3[] normals
	{
		get
		{
			return (((m_oldDirty & DirtyFlags.Normals) == 0) ? m_oldMesh : m_newMesh).normals;
		}
		set
		{
			m_newMesh.normals = value;
			m_oldDirty |= DirtyFlags.Normals;
		}
	}

	public Vector4[] tangents
	{
		get
		{
			return (((m_oldDirty & DirtyFlags.Tangents) == 0) ? m_oldMesh : m_newMesh).tangents;
		}
		set
		{
			m_newMesh.tangents = value;
			m_oldDirty |= DirtyFlags.Tangents;
		}
	}

	public Vector2[] uv
	{
		get
		{
			return (((m_oldDirty & DirtyFlags.UV) == 0) ? m_oldMesh : m_newMesh).uv;
		}
		set
		{
			m_newMesh.uv = value;
			m_oldDirty |= DirtyFlags.UV;
		}
	}

	public Vector2[] uv1
	{
		get
		{
			return (((m_oldDirty & DirtyFlags.UV1) == 0) ? m_oldMesh : m_newMesh).uv1;
		}
		set
		{
			m_newMesh.uv1 = value;
			m_oldDirty |= DirtyFlags.UV1;
		}
	}

	public Vector2[] uv2
	{
		get
		{
			return (((m_oldDirty & DirtyFlags.UV2) == 0) ? m_oldMesh : m_newMesh).uv2;
		}
		set
		{
			m_newMesh.uv2 = value;
			m_oldDirty |= DirtyFlags.UV2;
		}
	}

	public DoubleBufferedMesh()
	{
		m_oldMesh = new Mesh();
		m_newMesh = new Mesh();
	}

	public void Clear()
	{
		m_oldMesh.Clear();
		m_newMesh.Clear();
		m_oldDirty = (DirtyFlags)0;
		m_newDirty = (DirtyFlags)0;
	}

	public int[] GetTriangles(int submesh)
	{
		return m_newMesh.GetTriangles(submesh);
	}

	public int[] GetTriangleStrip(int submesh)
	{
		return m_newMesh.GetTriangleStrip(submesh);
	}

	public void RecalculateBounds()
	{
		SyncOldToNewVertices();
		m_newMesh.RecalculateBounds();
		m_oldMesh.bounds = m_newMesh.bounds;
	}

	public void RecalculateNormals()
	{
		SyncOldToNewVertices();
		m_newMesh.RecalculateNormals();
		m_oldDirty |= DirtyFlags.Normals;
	}

	public void SetTriangles(int[] triangles, int submesh)
	{
		SyncOldToNew();
		SyncNewToOld();
		m_newMesh.SetTriangles(triangles, submesh);
		m_oldMesh.SetTriangles(triangles, submesh);
	}

	public void SetTriangleStrip(int[] triangles, int submesh)
	{
		SyncOldToNew();
		SyncNewToOld();
		m_newMesh.SetTriangleStrip(triangles, submesh);
		m_oldMesh.SetTriangleStrip(triangles, submesh);
	}

	private void SyncOldToNewVertices()
	{
		if ((m_newDirty & ~m_oldDirty & DirtyFlags.Vertices) != 0)
		{
			m_newMesh.vertices = m_oldMesh.vertices;
			m_newDirty &= (DirtyFlags)(-2);
		}
	}

	private void SyncOldToNew()
	{
		DirtyFlags dirtyFlags = m_newDirty & ~m_oldDirty;
		if (dirtyFlags != 0)
		{
			Sync(m_oldMesh, m_newMesh, dirtyFlags);
		}
		m_newDirty = (DirtyFlags)0;
	}

	private void SyncNewToOld()
	{
		if (m_oldDirty != 0)
		{
			Sync(m_newMesh, m_oldMesh, m_oldDirty);
			m_oldDirty = (DirtyFlags)0;
		}
	}

	private static void Sync(Mesh from, Mesh to, DirtyFlags dirty)
	{
		if ((dirty & DirtyFlags.Vertices) != 0)
		{
			to.vertices = from.vertices;
		}
		if ((dirty & DirtyFlags.Normals) != 0)
		{
			to.normals = from.normals;
		}
		if ((dirty & DirtyFlags.Tangents) != 0)
		{
			to.tangents = from.tangents;
		}
		if ((dirty & DirtyFlags.Colors) != 0)
		{
			to.colors = from.colors;
		}
		if ((dirty & DirtyFlags.UV) != 0)
		{
			to.uv = from.uv;
		}
		if ((dirty & DirtyFlags.UV1) != 0)
		{
			to.uv1 = from.uv1;
		}
		if ((dirty & DirtyFlags.UV2) != 0)
		{
			to.uv2 = from.uv2;
		}
	}

	public static implicit operator Mesh(DoubleBufferedMesh _mesh)
	{
		return (_mesh == null) ? null : _mesh.mesh;
	}
}
