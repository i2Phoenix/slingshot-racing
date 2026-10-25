using UnityEngine;

[AddComponentMenu("GUI/Localised Text")]
public class LocalisedText : MonoBehaviour
{
	public LocString m_locString;

	public SpriteText m_text;

	public bool m_fitSingleLine;

	private float m_originalCharacterSize;

	public object this[int index]
	{
		set
		{
			m_locString[index] = value;
			Refresh();
		}
	}

	private void Start()
	{
		if (m_text == null)
		{
			m_text = GetComponent<SpriteText>();
		}
		Refresh();
	}

	public void SetTextID(string _id)
	{
		m_locString.ID = _id;
		Refresh();
	}

	public void Refresh()
	{
		if (m_text != null)
		{
			string text = m_locString.Text;
			if (m_fitSingleLine && m_text.maxWidth > 0f)
			{
				if (m_originalCharacterSize == 0f)
					m_originalCharacterSize = m_text.characterSize;
				m_text.SetCharacterSize(m_originalCharacterSize);
				float width = m_text.maxWidthInPixels ? m_text.GetScreenWidth(text) : m_text.GetWidth(text);
				if (width > m_text.maxWidth)
					m_text.SetCharacterSize(m_originalCharacterSize * m_text.maxWidth / width * 0.98f);
				m_text.multiline = false;
			}
			m_text.Text = text;
		}
	}
}
