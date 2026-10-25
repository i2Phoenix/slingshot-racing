using UnityEngine;

[AddComponentMenu("GUI/Localised Text")]
public class LocalisedText : MonoBehaviour
{
	public LocString m_locString;

	public SpriteText m_text;

	public object this[int index]
	{
		set
		{
			m_locString[index] = value;
			m_text.Text = m_locString.Text;
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
			m_text.Text = m_locString.Text;
		}
	}
}
