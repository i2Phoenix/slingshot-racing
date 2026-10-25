using UnityEngine;

public class BypassMaterial
{
	public Material m_material;

	public int m_program;

	public uint m_attributeMask;

	public Mesh m_dummyMesh;

	public BypassMaterial(Material material)
	{
		m_material = material;
	}
}
