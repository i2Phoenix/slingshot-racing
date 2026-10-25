using System.Diagnostics;
using UnityEngine;

public class DynamicMesh
{
	public enum Attributes
	{
		Triangles = 1,
		Vertices = 2,
		Colours = 0x20,
		UV = 0x40,
		UV1 = 0x80
	}

	private int m_numVertices;

	private int m_numTriangles;

	private int[] m_triangles;

	private Vector3[] m_vertices;

	private Color32[] m_colours;

	private Vector2[] m_uvs;

	private Vector2[] m_uv1;

	private Attributes m_locked;

	private Attributes m_dynamic;

	private Attributes m_newDirty;

	private Mesh m_newMesh;

	private Attributes m_oldDirty;

	private Mesh m_oldMesh;

	public Mesh Mesh
	{
		get
		{
			if (m_oldDirty != 0)
			{
				SyncNew();
				Mesh oldMesh = m_oldMesh;
				m_oldMesh = m_newMesh;
				m_newMesh = oldMesh;
				m_newDirty = m_oldDirty;
				m_oldDirty = (Attributes)0;
			}
			return m_oldMesh;
		}
	}

	public Bounds Bounds
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

	public Vector3[] Vertices
	{
		get
		{
			return m_vertices;
		}
	}

	public Color32[] Colours
	{
		get
		{
			return m_colours;
		}
	}

	public Vector2[] UVs
	{
		get
		{
			return m_uvs;
		}
	}

	public Vector2[] UV1
	{
		get
		{
			return m_uv1;
		}
	}

	public DynamicMesh(int _numVertices, int _numTriangles, Attributes _dynamicMask)
	{
		m_oldMesh = new Mesh();
		m_dynamic = _dynamicMask;
		m_newMesh = new Mesh();
		m_numVertices = _numVertices;
		m_numTriangles = _numTriangles;
		if ((m_dynamic & Attributes.Vertices) != 0)
		{
			m_vertices = new Vector3[m_numVertices];
		}
		if ((m_dynamic & Attributes.Colours) != 0)
		{
			m_colours = new Color32[m_numVertices];
		}
		if ((m_dynamic & Attributes.UV) != 0)
		{
			m_uvs = new Vector2[m_numVertices];
		}
		if ((m_dynamic & Attributes.UV1) != 0)
		{
			m_uv1 = new Vector2[m_numVertices];
		}
		SetDirty(m_dynamic);
	}

	public void SetDirty(Attributes _attributes)
	{
		m_newDirty |= _attributes;
		m_oldDirty |= _attributes;
	}

	public void SetDirtyVertices()
	{
		SetDirty(Attributes.Vertices);
	}

	public void SetDirtyColours()
	{
		SetDirty(Attributes.Colours);
	}

	public void SetDirtyUVs()
	{
		SetDirty(Attributes.UV);
	}

	public void SetDirtyUV1()
	{
		SetDirty(Attributes.UV1);
	}

	public Vector3[] LockVertices()
	{
		if (m_vertices == null)
		{
			m_vertices = m_newMesh.vertices;
			if (m_vertices == null)
			{
				m_vertices = new Vector3[m_numVertices];
			}
		}
		return m_vertices;
	}

	public Color32[] LockColours()
	{
		if (m_colours == null)
		{
			m_colours = m_newMesh.colors32;
			if (m_colours == null)
			{
				m_colours = new Color32[m_numVertices];
			}
		}
		return m_colours;
	}

	public Vector2[] LockUVs()
	{
		if (m_uvs == null)
		{
			m_uvs = m_newMesh.uv;
			if (m_uvs == null)
			{
				m_uvs = new Vector2[m_numVertices];
			}
		}
		return m_uvs;
	}

	public void UnlockVertices()
	{
		if ((m_dynamic & Attributes.Vertices) == 0)
		{
			SyncBoth();
			m_vertices = null;
		}
	}

	public void UnlockColours()
	{
		if ((m_dynamic & Attributes.Colours) == 0)
		{
			SyncBoth();
			m_colours = null;
		}
	}

	public void UnlockUVs()
	{
		if ((m_dynamic & Attributes.UV) == 0)
		{
			SyncBoth();
			m_uvs = null;
		}
	}

	public int[] LockTriangles()
	{
		if (m_triangles == null)
		{
			m_triangles = m_newMesh.triangles;
			if (m_triangles.Length != m_numTriangles)
			{
				m_triangles = new int[m_numTriangles * 3];
			}
		}
		return m_triangles;
	}

	public void UnlockTriangles()
	{
		SyncBoth();
		m_newMesh.triangles = m_triangles;
		m_oldMesh.triangles = m_triangles;
		m_triangles = null;
	}

	public void RecalculateBounds()
	{
		SyncNewVertices();
		m_newMesh.RecalculateBounds();
		m_oldMesh.bounds = m_newMesh.bounds;
	}

	private void SyncNewVertices()
	{
		if ((m_newDirty & Attributes.Vertices) != 0)
		{
			m_newMesh.vertices = m_vertices;
			m_newDirty &= (Attributes)(-3);
		}
	}

	private void SyncBoth()
	{
		SyncOld();
		SyncNew();
	}

	private void SyncNew()
	{
		if (m_newDirty != 0)
		{
			Sync(m_newMesh, m_newDirty);
			m_newDirty = (Attributes)0;
		}
	}

	private void SyncOld()
	{
		if (m_oldDirty != 0)
		{
			Sync(m_oldMesh, m_oldDirty);
			m_oldDirty = (Attributes)0;
		}
	}

	private void Sync(Mesh mesh, Attributes dirty)
	{
		if ((dirty & Attributes.Vertices) != 0)
		{
			mesh.vertices = m_vertices;
		}
		if ((dirty & Attributes.Colours) != 0)
		{
			mesh.colors32 = m_colours;
		}
		if ((dirty & Attributes.UV) != 0)
		{
			mesh.uv = m_uvs;
		}
		if ((dirty & Attributes.UV1) != 0)
		{
			mesh.uv2 = m_uv1;
		}
	}

	[Conditional("UNITY_EDITOR")]
	private void Lock(Attributes _attribute)
	{
		m_locked |= _attribute;
	}

	[Conditional("UNITY_EDITOR")]
	private void Unlock(Attributes _attribute)
	{
		m_locked &= ~_attribute;
		SetDirty(_attribute);
	}

	public static implicit operator Mesh(DynamicMesh _mesh)
	{
		return _mesh.Mesh;
	}
}
