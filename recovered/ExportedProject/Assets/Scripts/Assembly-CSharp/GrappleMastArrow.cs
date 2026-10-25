using UnityEngine;

[AddComponentMenu("Graphics/Grapple Mast Arrow")]
public class GrappleMastArrow : MonoBehaviour
{
	private GrapplePoint m_grapplePointComp;

	private GrapplePoint[] m_extraGrapplePointComps;

	private Renderer[] m_renderers;

	private bool m_enabled = true;

	public GameObject m_grapplePoint;

	public GameObject[] m_extraGrapplePoints;

	private void Start()
	{
		m_grapplePointComp = m_grapplePoint.GetComponent<GrapplePoint>();
		if (m_extraGrapplePoints != null)
		{
			m_extraGrapplePointComps = new GrapplePoint[m_extraGrapplePoints.Length];
			for (int i = 0; i < m_extraGrapplePoints.Length; i++)
			{
				m_extraGrapplePointComps[i] = m_extraGrapplePoints[i].GetComponent<GrapplePoint>();
			}
		}
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(true);
		int num = 0;
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			Material[] materials = componentsInChildren[j].materials;
			for (int k = 0; k < materials.Length; k++)
			{
				if (materials[k].name.IndexOf("GR_Arrows_Glow") != -1)
				{
					num++;
					break;
				}
			}
		}
		if (num > 0)
		{
			m_renderers = new Renderer[num];
			int num2 = 0;
			for (int l = 0; l < componentsInChildren.Length; l++)
			{
				Material[] materials2 = componentsInChildren[l].materials;
				for (int m = 0; m < materials2.Length; m++)
				{
					if (materials2[m].name.IndexOf("GR_Arrows_Glow") != -1)
					{
						m_renderers[num2++] = componentsInChildren[l];
						break;
					}
				}
			}
		}
		Apply();
	}

	private void Update()
	{
		bool flag = m_grapplePointComp.NumGrapplingObjects > 0;
		if (m_extraGrapplePointComps != null)
		{
			for (int i = 0; i < m_extraGrapplePointComps.Length; i++)
			{
				flag |= m_extraGrapplePointComps[i].NumGrapplingObjects > 0;
			}
		}
		if (flag != m_enabled)
		{
			m_enabled = flag;
			Apply();
		}
	}

	private void Apply()
	{
		if (m_renderers != null)
		{
			for (int i = 0; i < m_renderers.Length; i++)
			{
				m_renderers[i].enabled = m_enabled;
			}
		}
	}
}
