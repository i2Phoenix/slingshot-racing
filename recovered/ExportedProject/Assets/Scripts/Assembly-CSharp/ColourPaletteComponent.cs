using UnityEngine;

[AddComponentMenu("Graphics/Colour Palette Component")]
public class ColourPaletteComponent : MonoBehaviour
{
	public ColourPalette m_colourPalette;

	public Color RandomColour
	{
		get
		{
			return m_colourPalette.RandomColour;
		}
	}
}
