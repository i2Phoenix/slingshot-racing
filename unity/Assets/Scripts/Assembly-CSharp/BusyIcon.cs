using System.Collections;
using UnityEngine;

[AddComponentMenu("GUI/Busy Icon")]
public class BusyIcon : MonoBehaviour
{
	private bool m_busy;

	public Color m_colour;

	public void ShowIcon()
	{
		StopAllCoroutines();
		base.gameObject.active = true;
		StartCoroutine(IconDisplay());
	}

	public void HideIcon()
	{
		m_busy = false;
	}

	public void HideImmediate()
	{
		StopAllCoroutines();
		base.gameObject.SetActiveRecursively(false);
		m_busy = false;
	}

	private IEnumerator IconDisplay()
	{
		m_busy = true;
		base.gameObject.SetActiveRecursively(true);
		StartCoroutine("IconAnimate");
		float fadeTimeRecip = 4f;
		float alpha = 0f;
		Color colour = m_colour;
		while (alpha < 1f)
		{
			alpha += Time.deltaTime * fadeTimeRecip;
			colour.a = Mathf.Min(alpha, 1f);
			base.GetComponent<Renderer>().material.color = colour;
			yield return null;
		}
		while (m_busy)
		{
			yield return null;
		}
		while (alpha > 0f)
		{
			alpha -= Time.deltaTime * fadeTimeRecip;
			colour.a = Mathf.Max(alpha, 0f);
			base.GetComponent<Renderer>().material.color = colour;
			yield return null;
		}
		StopCoroutine("IconAnimate");
		base.gameObject.SetActiveRecursively(false);
	}

	private IEnumerator IconAnimate()
	{
		while (true)
		{
			base.transform.Rotate(Vector3.up * Time.deltaTime * 350f);
			yield return null;
		}
	}

	private void Start()
	{
		base.gameObject.SetActiveRecursively(false);
		base.GetComponent<Renderer>().material.color = m_colour;
	}
}
