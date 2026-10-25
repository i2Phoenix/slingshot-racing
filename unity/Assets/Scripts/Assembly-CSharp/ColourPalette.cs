using UnityEngine;

public class ColourPalette : ScriptableObject
{
	public Color[] m_colours;

	public Color RandomColour
	{
		get
		{
			if (m_colours == null)
			{
				return new Color(1f, 0f, 1f);
			}
			int num = Random.Range(0, m_colours.Length);
			return m_colours[num];
		}
	}
}
