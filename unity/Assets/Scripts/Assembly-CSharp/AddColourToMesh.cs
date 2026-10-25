using UnityEngine;

[AddComponentMenu("Graphics/Add Colour To Mesh")]
[RequireComponent(typeof(MeshFilter))]
public class AddColourToMesh : MonoBehaviour
{
	public Color m_colour;

	private void Start()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		Mesh mesh = component.mesh;
		Color[] array = new Color[mesh.vertexCount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = m_colour;
		}
		mesh.colors = array;
	}
}
