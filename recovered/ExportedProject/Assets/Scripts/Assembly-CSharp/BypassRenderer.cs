using System;
using System.Text;
using UnityEngine;

public class BypassRenderer : BypassRendererBase
{
	private struct MeshEntry
	{
		internal uint m_attributeMask;

		internal Mesh m_mesh;
	}

	private const string c_vertexAttribName = "_glesVertex";

	private const string c_colorAttribName = "_glesColor";

	private const string c_normalAttribName = "_glesNormal";

	private const string c_texCoordAttribName = "_glesMultiTexCoord0";

	private static MeshEntry[] s_meshes = new MeshEntry[0];

	private BypassMaterial m_currMaterial;

	private void OnPostRender()
	{
		for (int i = 0; i != m_numActiveMeshes; i++)
		{
			MeshState meshState = m_activeMeshes[i];
			ChangeState(meshState.material, meshState.matrix);
			meshState.mesh.DrawRange(meshState.material.m_attributeMask, meshState.startIndex, meshState.indexCount);
		}
		m_currMaterial = null;
	}

	private static Mesh GetMesh(uint _attributeMask)
	{
		int num = ((s_meshes != null) ? s_meshes.Length : 0);
		for (int i = 0; i != num; i++)
		{
			if (s_meshes[i].m_attributeMask == _attributeMask)
			{
				return s_meshes[i].m_mesh;
			}
		}
		Mesh mesh = new Mesh();
		if ((_attributeMask & 1) != 0)
		{
			mesh.vertices = new Vector3[1];
		}
		if ((_attributeMask & 2) != 0)
		{
			mesh.colors = new Color[1];
		}
		if ((_attributeMask & 4) != 0)
		{
			mesh.normals = new Vector3[1];
		}
		if ((_attributeMask & 8) != 0)
		{
			mesh.uv = new Vector2[1];
		}
		mesh.triangles = new int[3];
		Array.Resize(ref s_meshes, num + 1);
		s_meshes[num].m_attributeMask = _attributeMask;
		s_meshes[num].m_mesh = mesh;
		return mesh;
	}

	private void ChangeState(BypassMaterial material, Matrix4x4 matrix)
	{
		if (m_currMaterial != material)
		{
			m_currMaterial = material;
			material.m_material.SetPass(0);
		}
		if (material.m_program == 0)
		{
			Graphics.DrawMeshNow(GetMesh(15u), matrix);
			int _params = 0;
			OpenGL.glGetIntegerv(35725u, out _params);
			material.m_program = _params;
			int _params2 = 0;
			OpenGL.glGetProgramiv((uint)_params, 35721u, out _params2);
			int _params3 = 0;
			OpenGL.glGetProgramiv((uint)_params, 35722u, out _params3);
			for (int i = 0; i != _params2; i++)
			{
				int length = 0;
				int size = 0;
				uint type = 0u;
				StringBuilder stringBuilder = new StringBuilder(_params3);
				OpenGL.glGetActiveAttrib((uint)_params, (uint)i, _params3, out length, out size, out type, stringBuilder);
				string text = stringBuilder.ToString(0, length);
				switch (text)
				{
				case "_glesVertex":
					if (OpenGL.glGetAttribLocation((uint)_params, text) != 0)
					{
						Debug.LogError("Mismatched vertex attribute mapping, inform Tim he'll have to dynamically bind them!");
					}
					material.m_attributeMask |= 1u;
					break;
				case "_glesColor":
				{
					int num2 = OpenGL.glGetAttribLocation((uint)_params, text);
					if (num2 != 1)
					{
						Debug.LogError("Mismatched color attribute mapping, inform Tim he'll have to dynamically bind them!");
					}
					material.m_attributeMask |= 2u;
					break;
				}
				case "_glesNormal":
				{
					int num3 = OpenGL.glGetAttribLocation((uint)_params, text);
					if (num3 != 2)
					{
						Debug.LogError("Mismatched normal attribute mapping, inform Tim he'll have to dynamically bind them!");
					}
					material.m_attributeMask |= 4u;
					break;
				}
				case "_glesMultiTexCoord0":
				{
					int num = OpenGL.glGetAttribLocation((uint)_params, text);
					if (num != 3)
					{
						Debug.LogError("Mismatched texCoord attribute mapping, inform Tim he'll have to dynamically bind them!");
					}
					material.m_attributeMask |= 8u;
					break;
				}
				default:
					Debug.LogError("Inform Tim there's an unknown attribute : " + text);
					break;
				}
			}
			material.m_dummyMesh = GetMesh(material.m_attributeMask);
		}
		Graphics.DrawMeshNow(material.m_dummyMesh, matrix);
		OpenGL.glBindBuffer(34962u, 0u);
		OpenGL.glBindBuffer(34963u, 0u);
	}
}
