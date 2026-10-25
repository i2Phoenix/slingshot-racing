using UnityEngine;

// The original plugin submitted client-memory arrays to GLES2. Unity now owns
// these buffers so the same rendering path works with Metal, GLES3 and Vulkan.
public class BypassMesh : BypassMeshBase
{
    private Mesh mesh;
    private int[] indices;
    private Color32[] colours;
    private readonly string meshName;
    public BypassMesh(string name = "Slingshot dynamic effects") { meshName = name; }
    public ushort[] triangles { set { m_indices = value; m_numIndices = value == null ? 0u : (uint)value.Length; } }
    public Vector3[] vertices { set { m_vertices = value; } }
    public Vector3[] normals { set { m_normals = value; } }
    public Color[] colors { set { m_colours = value; m_colour32s = null; } }
    public ColourRGBA32[] colors32 { set { m_colour32s = value; m_colours = null; } }
    public Vector2[] uv { set { m_texCoords = value; } }

    public void DrawRange(Matrix4x4 matrix, uint startIndex, uint indexCount)
    {
        if (indexCount == 0 || m_vertices == null || m_indices == null) return;
        if (mesh == null) { mesh = new Mesh { name = meshName }; mesh.MarkDynamic(); }
        mesh.Clear(false);
        mesh.vertices = m_vertices;
        if (m_normals != null) mesh.normals = m_normals;
        if (m_texCoords != null) mesh.uv = m_texCoords;
        if (m_colours != null) mesh.colors = m_colours;
        else if (m_colour32s != null)
        {
            if (colours == null || colours.Length != m_colour32s.Length) colours = new Color32[m_colour32s.Length];
            for (int i = 0; i < colours.Length; i++) colours[i] = m_colour32s[i];
            mesh.colors32 = colours;
        }
        if (indices == null || indices.Length != indexCount) indices = new int[indexCount];
        for (int i = 0; i < indices.Length; i++) indices[i] = m_indices[startIndex + i];
        mesh.SetIndices(indices, MeshTopology.Triangles, 0, false);
        Graphics.DrawMeshNow(mesh, matrix);
    }

    public void Dispose()
    {
        if (mesh != null) Object.Destroy(mesh);
        mesh = null;
    }
}
