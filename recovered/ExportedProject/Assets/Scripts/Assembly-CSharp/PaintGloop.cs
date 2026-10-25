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
			base.transform.FindChild("Bone0"),
			base.transform.FindChild("Bone1"),
			base.transform.FindChild("Bone2"),
			base.transform.FindChild("Bone3")
		};
		skinnedMeshRenderer.sharedMesh = component.sharedMesh;
	}

	private void Update()
	{
	}
}
