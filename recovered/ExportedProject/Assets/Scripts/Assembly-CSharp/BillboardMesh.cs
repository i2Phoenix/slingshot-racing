using UnityEngine;

[AddComponentMenu("Graphics/Billboard Mesh")]
public class BillboardMesh : MonoBehaviour
{
	private MeshFilter m_meshFilter;

	private MeshRenderer m_meshRenderer;

	private Mesh m_mesh;

	private Vector3[] m_vertices;

	private Vector2[] m_uvs;

	private Color[] m_colours;

	private int[] m_triangles;

	private int m_billboardCount;

	private bool m_verticesDirty;

	private bool m_uvsDirty;

	private bool m_coloursDirty;

	public static BillboardMesh Create(int _billboardCount)
	{
		GameObject gameObject = new GameObject("BillboardMesh");
		BillboardMesh billboardMesh = gameObject.AddComponent<BillboardMesh>();
		billboardMesh.Initialise(_billboardCount);
		return billboardMesh;
	}

	private void Awake()
	{
		m_meshFilter = base.gameObject.AddComponent<MeshFilter>();
		m_meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		m_meshRenderer.castShadows = false;
		m_meshRenderer.receiveShadows = false;
		m_mesh = m_meshFilter.mesh;
	}

	private void LateUpdate()
	{
		if (m_verticesDirty)
		{
			m_mesh.vertices = m_vertices;
			m_mesh.RecalculateBounds();
			m_verticesDirty = false;
		}
		if (m_uvsDirty)
		{
			m_mesh.uv = m_uvs;
			m_uvsDirty = false;
		}
		if (m_coloursDirty)
		{
			m_mesh.colors = m_colours;
			m_coloursDirty = false;
		}
	}

	private void OnDestroy()
	{
		m_vertices = null;
		m_uvs = null;
		m_colours = null;
		m_triangles = null;
		m_meshFilter = null;
		m_meshRenderer = null;
		m_mesh.Clear();
		m_mesh = null;
	}

	public void SetBillboard(int _billboardIndex, Vector3 _position, Vector3 _up, Vector3 _right)
	{
		int num = BaseVertexIndex(_billboardIndex);
		m_vertices[num] = _position + _up - _right;
		m_vertices[num + 1] = _position + _up + _right;
		m_vertices[num + 2] = _position - _up - _right;
		m_vertices[num + 3] = _position - _up + _right;
		m_verticesDirty = true;
	}

	public void SetBillboardUVs(int _billboardIndex, Rect _uvRect)
	{
		int num = BaseVertexIndex(_billboardIndex);
		m_uvs[num] = new Vector2(_uvRect.xMin, _uvRect.yMax);
		m_uvs[num + 1] = new Vector2(_uvRect.xMax, _uvRect.yMax);
		m_uvs[num + 2] = new Vector2(_uvRect.xMin, _uvRect.yMin);
		m_uvs[num + 3] = new Vector2(_uvRect.xMax, _uvRect.yMin);
		m_uvsDirty = true;
	}

	public void SetBillboardColour(int _billboardIndex, Color _colour)
	{
		int num = BaseVertexIndex(_billboardIndex);
		m_colours[num] = _colour;
		m_colours[num + 1] = _colour;
		m_colours[num + 2] = _colour;
		m_colours[num + 3] = _colour;
		m_coloursDirty = true;
	}

	public void SetBillboardColour(int _billboardIndex, Color _colourAtTop, Color _colourAtBottom)
	{
		int num = BaseVertexIndex(_billboardIndex);
		m_colours[num] = _colourAtTop;
		m_colours[num + 1] = _colourAtTop;
		m_colours[num + 2] = _colourAtBottom;
		m_colours[num + 3] = _colourAtBottom;
		m_coloursDirty = true;
	}

	public void Destroy()
	{
		Object.Destroy(base.gameObject);
	}

	private void Initialise(int _billboardCount)
	{
		int num = VertexCount(_billboardCount);
		m_vertices = new Vector3[num];
		m_uvs = new Vector2[num];
		m_colours = new Color[num];
		int num2 = IndexCount(_billboardCount);
		m_triangles = new int[num2];
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < _billboardCount; i++)
		{
			m_triangles[num4] = num3;
			m_triangles[num4 + 1] = num3 + 2;
			m_triangles[num4 + 2] = num3 + 1;
			m_triangles[num4 + 3] = num3 + 1;
			m_triangles[num4 + 4] = num3 + 2;
			m_triangles[num4 + 5] = num3 + 3;
			num3 += 4;
			num4 += 6;
		}
		m_mesh.vertices = m_vertices;
		m_mesh.uv = m_uvs;
		m_mesh.colors = m_colours;
		m_mesh.triangles = m_triangles;
	}

	private static int BaseVertexIndex(int _billboardIndex)
	{
		return _billboardIndex * 4;
	}

	private static int VertexCount(int _billboardCount)
	{
		return _billboardCount * 4;
	}

	private static int IndexCount(int _billboardCount)
	{
		return _billboardCount * 6;
	}
}
