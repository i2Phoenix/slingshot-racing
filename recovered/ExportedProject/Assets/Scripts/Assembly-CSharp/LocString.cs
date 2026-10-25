using System;
using UnityEngine;

[Serializable]
public class LocString
{
	private string m_text;

	private object[] m_args;

	private bool m_dirty = true;

	[SerializeField]
	private string m_id;

	[SerializeField]
	private int m_numArgs;

	public static bool s_displayIDs;

	public string Text
	{
		get
		{
			if (m_dirty)
			{
				BuildString();
			}
			return m_text;
		}
	}

	public string ID
	{
		get
		{
			return m_id;
		}
		set
		{
			m_id = value;
			m_dirty = true;
		}
	}

	public object this[int index]
	{
		set
		{
			if (m_args == null)
			{
				m_args = new object[m_numArgs];
			}
			m_args[index] = value;
			m_dirty = true;
		}
	}

	private void BuildString()
	{
		if (m_id.Contains("ID_Dbg_"))
		{
			m_text = m_id;
			m_text = "#" + m_text.Replace("ID_Dbg_", string.Empty);
		}
		else if (m_args != null && m_args.Length > 0)
		{
			m_text = string.Format(Language.Get(m_id), m_args);
		}
		else
		{
			m_text = Language.Get(m_id);
		}
		m_dirty = false;
	}
}
