using UnityEngine;

public class PaintGloop : MonoBehaviour
{
	public Material m_material;

	private void Start()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		if (skinnedMeshRenderer == null)
		{
			skinnedMeshRenderer = base.gameObject.AddComponent<SkinnedMeshRenderer>();
		}
		skinnedMeshRenderer.material = m_material;
		skinnedMeshRenderer.bones = new Transform[4]
		{
			base.transform.Find("Bone0"),
			base.transform.Find("Bone1"),
			base.transform.Find("Bone2"),
			base.transform.Find("Bone3")
		};
		skinnedMeshRenderer.sharedMesh = component.sharedMesh;
	}

	private void Update()
	{
	}
}
