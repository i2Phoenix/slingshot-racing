using UnityEngine;

[AddComponentMenu("")]
public class LineMeshBase : MonoBehaviour
{
	protected enum DirtyFlags
	{
		Bounds = 1,
		Vertices = 2,
		Colours = 4,
		UVs = 8,
		UVRect = 16,
		UVSlide = 32,
		Mesh = 14
	}

	private float m_boundsPadding = 1f;

	protected GameObject m_gameObject;

	protected MeshFilter m_meshFilter;

	protected MeshRenderer m_meshRenderer;

	protected DynamicMesh m_mesh;

	protected Rect m_uvRect;

	protected float m_uvSlide;

	protected bool m_verticalUVRect;

	protected Vector3[] m_vertices;

	protected Vector2[] m_uvs;

	protected Color32[] m_colours;

	private DirtyFlags m_dirtyFlags;

	public float BoundsPadding
	{
		get
		{
			return m_boundsPadding;
		}
		set
		{
			m_boundsPadding = value;
			MarkDirty(DirtyFlags.Bounds);
		}
	}

	private void Awake()
	{
		m_gameObject = base.gameObject;
		m_meshFilter = m_gameObject.AddComponent<MeshFilter>();
		m_meshRenderer = m_gameObject.AddComponent<MeshRenderer>();
		m_meshRenderer.castShadows = false;
		m_meshRenderer.receiveShadows = false;
	}

	private void LateUpdate()
	{
		if (m_dirtyFlags == (DirtyFlags)0)
		{
			base.enabled = false;
		}
		else if ((m_dirtyFlags & DirtyFlags.Bounds) != 0)
		{
			RecalculateBounds();
		}
	}

	private void OnWillRenderObject()
	{
		if ((m_dirtyFlags & DirtyFlags.UVRect) != 0)
		{
			if (m_verticalUVRect)
			{
				UpdateUVRectVertical();
			}
			else
			{
				UpdateUVRectHorizontal();
			}
			m_dirtyFlags &= (DirtyFlags)(-49);
			m_dirtyFlags |= DirtyFlags.UVs;
		}
		else if ((m_dirtyFlags & DirtyFlags.UVSlide) != 0)
		{
			if (m_verticalUVRect)
			{
				UpdateUVSlideVertical();
			}
			else
			{
				UpdateUVSlideHorizontal();
			}
			m_dirtyFlags &= (DirtyFlags)(-33);
			m_dirtyFlags |= DirtyFlags.UVs;
		}
		if ((m_dirtyFlags & DirtyFlags.Mesh) != 0)
		{
			UpdateMesh();
		}
	}

	private void RecalculateBounds()
	{
		Vector3 a = m_vertices[0];
		Vector3 b = m_vertices[1];
		int num = m_vertices.Length;
		Vector3 c = m_vertices[num - 2];
		Vector3 d = m_vertices[num - 1];
		Vector3 min = Vector3Utils.Min(a, b, c, d);
		Vector3 max = Vector3Utils.Max(a, b, c, d);
		Bounds o_bounds;
		BoundsUtils.BoundsFromMinMax(min, max, out o_bounds);
		o_bounds.Expand(m_boundsPadding);
		m_mesh.Bounds = o_bounds;
		m_dirtyFlags &= (DirtyFlags)(-2);
	}

	private void UpdateMesh()
	{
		if ((m_dirtyFlags & DirtyFlags.Vertices) != 0)
		{
			m_mesh.SetDirtyVertices();
		}
		if ((m_dirtyFlags & DirtyFlags.Colours) != 0)
		{
			m_mesh.SetDirtyColours();
		}
		if ((m_dirtyFlags & DirtyFlags.UVs) != 0)
		{
			m_mesh.SetDirtyUVs();
		}
		m_meshFilter.sharedMesh = m_mesh;
		m_dirtyFlags &= (DirtyFlags)(-15);
	}

	private void OnDestroy()
	{
		m_vertices = null;
		m_uvs = null;
		m_colours = null;
		m_meshFilter = null;
		m_meshRenderer = null;
		m_mesh = null;
	}

	public void Destroy()
	{
		Object.Destroy(base.gameObject);
	}

	private void UpdateUVRectVertical()
	{
		float x = m_uvRect.x;
		float y = m_uvRect.y;
		float x2 = x + m_uvRect.width;
		float num = y + m_uvRect.height;
		m_uvs[0].x = x;
		m_uvs[0].y = num;
		m_uvs[1].x = x2;
		m_uvs[1].y = num;
		int num2 = m_uvs.Length;
		int num3 = num2 >> 1;
		float num4 = m_uvRect.height / (float)(num3 - 1);
		float num5 = num - m_uvSlide * num4 - num4;
		for (int i = 2; i != num2; i += 2)
		{
			if (num5 < y)
			{
				num5 = y;
			}
			m_uvs[i].x = x;
			m_uvs[i].y = num5;
			m_uvs[i + 1].x = x2;
			m_uvs[i + 1].y = num5;
			num5 -= num4;
		}
	}

	private void UpdateUVRectHorizontal()
	{
		float x = m_uvRect.x;
		float y = m_uvRect.y;
		float num = x + m_uvRect.width;
		float y2 = y + m_uvRect.height;
		m_uvs[0].x = x;
		m_uvs[0].y = y;
		m_uvs[1].x = x;
		m_uvs[1].y = y2;
		int num2 = m_uvs.Length;
		int num3 = num2 >> 1;
		float num4 = m_uvRect.width / (float)(num3 - 1);
		float num5 = x + m_uvSlide * num4 + num4;
		for (int i = 2; i != num2; i += 2)
		{
			if (num5 > num)
			{
				num5 = num;
			}
			m_uvs[i].x = num5;
			m_uvs[i].y = y;
			m_uvs[i + 1].x = num5;
			m_uvs[i + 1].y = y2;
			num5 += num4;
		}
	}

	private void UpdateUVSlideVertical()
	{
		float y = m_uvRect.y;
		float num = y + m_uvRect.height;
		m_uvs[0].y = num;
		m_uvs[1].y = num;
		int num2 = m_uvs.Length;
		int num3 = num2 >> 1;
		float num4 = m_uvRect.height / (float)(num3 - 1);
		float num5 = num - m_uvSlide * num4 - num4;
		for (int i = 2; i != num2; i += 2)
		{
			if (num5 < y)
			{
				num5 = y;
			}
			m_uvs[i].y = num5;
			m_uvs[i + 1].y = num5;
			num5 -= num4;
		}
	}

	private void UpdateUVSlideHorizontal()
	{
		float x = m_uvRect.x;
		float num = x + m_uvRect.width;
		m_uvs[0].x = x;
		m_uvs[1].x = x;
		int num2 = m_uvs.Length;
		int num3 = num2 >> 1;
		float num4 = m_uvRect.width / (float)(num3 - 1);
		float num5 = x + m_uvSlide * num4 + num4;
		for (int i = 2; i != num2; i += 2)
		{
			if (num5 > num)
			{
				num5 = num;
			}
			m_uvs[i].x = num5;
			m_uvs[i + 1].x = num5;
			num5 += num4;
		}
	}

	public void SetUVRectVertical(Rect _uvRect)
	{
		m_uvRect = _uvRect;
		m_verticalUVRect = true;
		m_dirtyFlags |= DirtyFlags.UVRect;
	}

	public void SetUVRectHorizontal(Rect _uvRect)
	{
		m_uvRect = _uvRect;
		m_verticalUVRect = false;
		m_dirtyFlags |= DirtyFlags.UVRect;
	}

	public void SetUVSlide(float _uvSlide)
	{
		m_uvSlide = _uvSlide;
		m_dirtyFlags |= DirtyFlags.UVSlide;
	}

	protected void Initialise(Vector3 _initialStart, Vector3 _initialEnd, Vector3 _halfWidth, Rect _uvRect, Color32 _initialColour, int _pointCount, Vector3[] o_points)
	{
		int numVertices = VertexCount(_pointCount);
		int num = IndexCount(_pointCount);
		m_mesh = new DynamicMesh(numVertices, num / 3, (DynamicMesh.Attributes)98);
		m_vertices = m_mesh.Vertices;
		m_uvs = m_mesh.UVs;
		m_colours = m_mesh.Colours;
		float x = _uvRect.x;
		float y = _uvRect.y;
		float width = _uvRect.width;
		float height = _uvRect.height;
		float x2 = x + width;
		int num2 = 0;
		for (int i = 0; i < _pointCount; i++)
		{
			float num3 = (float)i / (float)(_pointCount - 1);
			float num4 = Mathf.Clamp01(1f - num3);
			Vector3 vector = Vector3.Lerp(_initialStart, _initialEnd, num3);
			if (o_points != null)
			{
				o_points[i] = vector;
			}
			m_vertices[num2] = vector - _halfWidth;
			m_uvs[num2] = new Vector2(x, y + height * num4);
			m_colours[num2] = _initialColour;
			num2++;
			m_vertices[num2] = vector + _halfWidth;
			m_uvs[num2] = new Vector2(x2, y + height * num4);
			m_colours[num2] = _initialColour;
			num2++;
		}
		int[] array = m_mesh.LockTriangles();
		int num5 = 0;
		int num6 = 0;
		for (int j = 0; j < _pointCount - 1; j++)
		{
			array[num6] = num5;
			array[num6 + 1] = num5 + 1;
			array[num6 + 2] = num5 + 2;
			array[num6 + 3] = num5 + 2;
			array[num6 + 4] = num5 + 1;
			array[num6 + 5] = num5 + 3;
			num5 += 2;
			num6 += 6;
		}
		m_mesh.UnlockTriangles();
		m_uvRect = _uvRect;
		m_uvSlide = 0f;
		m_verticalUVRect = true;
		m_meshFilter.sharedMesh = m_mesh;
	}

	private static int VertexCount(int _pointCount)
	{
		return _pointCount * 2;
	}

	private static int IndexCount(int _pointCount)
	{
		return (_pointCount - 1) * 6;
	}

	protected void MarkDirty(DirtyFlags _flags)
	{
		m_dirtyFlags |= _flags;
		base.enabled = true;
	}
}
