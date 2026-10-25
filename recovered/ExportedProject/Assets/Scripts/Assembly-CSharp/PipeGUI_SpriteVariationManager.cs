using UnityEngine;

public class PipeGUI_SpriteVariationManager : MonoBehaviour
{
	private PipeGUI_SpriteVariation[] m_sprites;

	private int m_animIndex;

	private void Start()
	{
		m_sprites = GetComponentsInChildren<PipeGUI_SpriteVariation>();
	}

	public void SelectNewVariant()
	{
		m_animIndex++;
		if (m_sprites != null)
		{
			PipeGUI_SpriteVariation[] sprites = m_sprites;
			foreach (PipeGUI_SpriteVariation pipeGUI_SpriteVariation in sprites)
			{
				pipeGUI_SpriteVariation.SetAnimIndex(m_animIndex);
			}
		}
	}
}
