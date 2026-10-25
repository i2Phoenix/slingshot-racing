using System.Collections.Generic;

public class WritePropertySerialiser : IPropertySerialiser
{
	private readonly Dictionary<string, string> m_properties;

	public WritePropertySerialiser(Dictionary<string, string> _properties)
	{
		m_properties = _properties;
	}

	public bool TryWrite(string _key, bool _value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, _value.ToString());
		return true;
	}

	public bool TryWrite(string _key, byte _value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, _value.ToString());
		return true;
	}

	public bool TryWrite(string _key, char _value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, _value.ToString());
		return true;
	}

	public bool TryWrite(string _key, ushort _value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, _value.ToString());
		return true;
	}

	public bool TryWrite(string _key, short _value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, _value.ToString());
		return true;
	}

	public bool TryWrite(string _key, uint _value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, _value.ToString());
		return true;
	}

	public bool TryWrite(string _key, int _value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, _value.ToString());
		return true;
	}

	public bool TryWrite(string _key, ulong _value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, _value.ToString());
		return true;
	}

	public bool TryWrite(string _key, long _value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, _value.ToString());
		return true;
	}

	public bool TryWrite(string _key, float _value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, _value.ToString());
		return true;
	}

	public bool TryWrite(string _key, double _value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, _value.ToString());
		return true;
	}

	public bool TryWrite(string _key, string _value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, _value);
		return true;
	}

	public bool TrySerialise(string _key, ref bool io_value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, io_value.ToString());
		return true;
	}

	public bool TrySerialise(string _key, ref byte io_value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, io_value.ToString());
		return true;
	}

	public bool TrySerialise(string _key, ref char io_value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, io_value.ToString());
		return true;
	}

	public bool TrySerialise(string _key, ref ushort io_value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, io_value.ToString());
		return true;
	}

	public bool TrySerialise(string _key, ref short io_value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, io_value.ToString());
		return true;
	}

	public bool TrySerialise(string _key, ref uint io_value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, io_value.ToString());
		return true;
	}

	public bool TrySerialise(string _key, ref int io_value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, io_value.ToString());
		return true;
	}

	public bool TrySerialise(string _key, ref ulong io_value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, io_value.ToString());
		return true;
	}

	public bool TrySerialise(string _key, ref long io_value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, io_value.ToString());
		return true;
	}

	public bool TrySerialise(string _key, ref float io_value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, io_value.ToString());
		return true;
	}

	public bool TrySerialise(string _key, ref double io_value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, io_value.ToString());
		return true;
	}

	public bool TrySerialise(string _key, ref string io_value)
	{
		if (m_properties.ContainsKey(_key))
		{
			return false;
		}
		m_properties.Add(_key, io_value);
		return true;
	}
}
