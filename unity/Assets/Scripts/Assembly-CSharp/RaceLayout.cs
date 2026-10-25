using UnityEngine;

public class RaceLayout : ScriptableObject
{
	public enum Variant
	{
		Standard = 0,
		Night = 1
	}

	[HideInInspector]
	public string[] m_scenes;

	public bool m_reverseTrack;

	public bool m_debug;

	public bool m_noReverse;

	public bool m_noVariants;

	public string DisplayName
	{
		get
		{
			return (!m_debug) ? Language.Get("ID_" + m_scenes[0]) : m_scenes[0];
		}
	}

	public string ID
	{
		get
		{
			return m_scenes[0];
		}
	}

	public int NumScenes
	{
		get
		{
			return m_scenes.Length;
		}
	}

	public bool CanReverse
	{
		get
		{
			return !m_noReverse;
		}
	}

	public bool HasVariants
	{
		get
		{
			return !m_noVariants;
		}
	}

	public string GetScene(int _index, Variant _variant)
	{
		string text = m_scenes[_index];
		if (_index == 0 && HasVariants && _variant == Variant.Night)
		{
			text = text.Replace("Blockout", "Night");
		}
		return text;
	}
}
