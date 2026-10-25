using UnityEngine;

[AddComponentMenu("Graphics/Apply Track Lighting")]
public class ApplyTrackLighting : MonoBehaviour
{
	public float m_yRange = 2f;

	private void Start()
	{
		Color color = TrackUtils.SampleTrackLighting(base.transform.position, m_yRange);
		ParticleSystem[] componentsInChildren = GameObjectUtils.GetComponentsInChildren<ParticleSystem>(base.gameObject);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].startColor *= color;
		}
		ParticleAnimator[] componentsInChildren2 = GameObjectUtils.GetComponentsInChildren<ParticleAnimator>(base.gameObject);
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			Color[] colorAnimation = componentsInChildren2[j].colorAnimation;
			for (int k = 0; k < colorAnimation.Length; k++)
			{
				colorAnimation[k] *= color;
			}
			componentsInChildren2[j].colorAnimation = colorAnimation;
		}
		SharedParticleEmitter[] componentsInChildren3 = GameObjectUtils.GetComponentsInChildren<SharedParticleEmitter>(base.gameObject);
		for (int l = 0; l < componentsInChildren3.Length; l++)
		{
			componentsInChildren3[l].m_colour *= color;
		}
		MeshFilter[] componentsInChildren4 = GameObjectUtils.GetComponentsInChildren<MeshFilter>(base.gameObject);
		for (int m = 0; m < componentsInChildren4.Length; m++)
		{
			MeshRenderer component = componentsInChildren4[m].GetComponent<MeshRenderer>();
			if (component != null)
			{
			}
			Mesh mesh = componentsInChildren4[m].mesh;
			Color[] array = new Color[mesh.vertexCount];
			for (int n = 0; n < array.Length; n++)
			{
				array[n] = color;
			}
			mesh.colors = array;
		}
	}
}
