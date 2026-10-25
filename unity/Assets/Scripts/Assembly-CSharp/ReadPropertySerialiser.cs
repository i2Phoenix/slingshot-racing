using System.Collections.Generic;

public class ReadPropertySerialiser : IPropertySerialiser
{
	private readonly Dictionary<string, string> m_properties;

	public ReadPropertySerialiser(Dictionary<string, string> _properties)
	{
		m_properties = _properties;
	}

	public bool TryRead(string _key, out bool o_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			o_value = false;
			return false;
		}
		return bool.TryParse(value, out o_value);
	}

	public bool TryRead(string _key, out byte o_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			o_value = 0;
			return false;
		}
		return byte.TryParse(value, out o_value);
	}

	public bool TryRead(string _key, out char o_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			o_value = '\0';
			return false;
		}
		return char.TryParse(value, out o_value);
	}

	public bool TryRead(string _key, out ushort o_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			o_value = 0;
			return false;
		}
		return ushort.TryParse(value, out o_value);
	}

	public bool TryRead(string _key, out short o_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			o_value = 0;
			return false;
		}
		return short.TryParse(value, out o_value);
	}

	public bool TryRead(string _key, out uint o_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			o_value = 0u;
			return false;
		}
		return uint.TryParse(value, out o_value);
	}

	public bool TryRead(string _key, out int o_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			o_value = 0;
			return false;
		}
		return int.TryParse(value, out o_value);
	}

	public bool TryRead(string _key, out ulong o_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			o_value = 0uL;
			return false;
		}
		return ulong.TryParse(value, out o_value);
	}

	public bool TryRead(string _key, out long o_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			o_value = 0L;
			return false;
		}
		return long.TryParse(value, out o_value);
	}

	public bool TryRead(string _key, out float o_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			o_value = 0f;
			return false;
		}
		return float.TryParse(value, out o_value);
	}

	public bool TryRead(string _key, out double o_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			o_value = 0.0;
			return false;
		}
		return double.TryParse(value, out o_value);
	}

	public bool TryRead(string _key, out string o_value)
	{
		return m_properties.TryGetValue(_key, out o_value);
	}

	public bool TrySerialise(string _key, ref bool io_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			return false;
		}
		return bool.TryParse(value, out io_value);
	}

	public bool TrySerialise(string _key, ref byte io_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			return false;
		}
		return byte.TryParse(value, out io_value);
	}

	public bool TrySerialise(string _key, ref char io_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			return false;
		}
		return char.TryParse(value, out io_value);
	}

	public bool TrySerialise(string _key, ref ushort io_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			return false;
		}
		return ushort.TryParse(value, out io_value);
	}

	public bool TrySerialise(string _key, ref short io_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			return false;
		}
		return short.TryParse(value, out io_value);
	}

	public bool TrySerialise(string _key, ref uint io_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			return false;
		}
		return uint.TryParse(value, out io_value);
	}

	public bool TrySerialise(string _key, ref int io_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			return false;
		}
		return int.TryParse(value, out io_value);
	}

	public bool TrySerialise(string _key, ref ulong io_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			return false;
		}
		return ulong.TryParse(value, out io_value);
	}

	public bool TrySerialise(string _key, ref long io_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			return false;
		}
		return long.TryParse(value, out io_value);
	}

	public bool TrySerialise(string _key, ref float io_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			return false;
		}
		return float.TryParse(value, out io_value);
	}

	public bool TrySerialise(string _key, ref double io_value)
	{
		string value;
		if (!m_properties.TryGetValue(_key, out value))
		{
			return false;
		}
		return double.TryParse(value, out io_value);
	}

	public bool TrySerialise(string _key, ref string io_value)
	{
		return m_properties.TryGetValue(_key, out io_value);
	}
}
