using UnityEngine;

public class PipeGUI_SpriteVariation : MonoBehaviour
{
	private PackedSprite m_sprite;

	private int m_lastIndex;

	private void Start()
	{
		m_sprite = GetComponent<PackedSprite>();
	}

	public void SetAnimIndex(int _index)
	{
		m_lastIndex = _index;
		if (m_sprite != null)
		{
			int num = m_sprite.animations.Length;
			int num2 = _index % num;
			m_sprite.PlayAnim(num2);
			m_sprite.defaultAnim = num2;
		}
	}

	private void OnEnable()
	{
		SetAnimIndex(m_lastIndex);
	}
}
