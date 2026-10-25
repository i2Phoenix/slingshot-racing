using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class WriteSerialiser : IDisposable, ISerialiser
{
	private Dictionary<string, string> m_currentProperties;

	private BinaryWriter m_writer;

	public bool IsReading
	{
		get
		{
			return false;
		}
	}

	public bool IsWriting
	{
		get
		{
			return true;
		}
	}

	public long Position
	{
		get
		{
			return m_writer.BaseStream.Position;
		}
	}

	public WriteSerialiser(Stream _stream)
	{
		if (_stream != null && _stream.CanWrite)
		{
			m_writer = new BinaryWriter(_stream, Encoding.UTF8);
		}
	}

	public bool IsValid()
	{
		return m_writer != null;
	}

	public void Dispose()
	{
		if (m_writer != null)
		{
			m_writer.Close();
			m_writer = null;
		}
	}

	public bool FourCC(byte[] _fourCC)
	{
		if (_fourCC.Length != 4)
		{
			return false;
		}
		Write(_fourCC[0]);
		Write(_fourCC[1]);
		Write(_fourCC[2]);
		Write(_fourCC[3]);
		return true;
	}

	public void Write(Dictionary<string, string> _properties)
	{
		Write(_properties.Count);
		foreach (KeyValuePair<string, string> _property in _properties)
		{
			Write(_property.Key);
			Write(_property.Value);
		}
		Write(3236803774u);
	}

	public void Write(bool _value)
	{
		m_writer.Write(_value);
	}

	public void Write(byte _value)
	{
		m_writer.Write(_value);
	}

	public void Write(byte[] _value)
	{
		m_writer.Write(_value);
	}

	public void Write(char _value)
	{
		m_writer.Write(_value);
	}

	public void Write(ushort _value)
	{
		m_writer.Write(_value);
	}

	public void Write(short _value)
	{
		m_writer.Write(_value);
	}

	public void Write(uint _value)
	{
		m_writer.Write(_value);
	}

	public void Write(int _value)
	{
		m_writer.Write(_value);
	}

	public void Write(ulong _value)
	{
		m_writer.Write(_value);
	}

	public void Write(long _value)
	{
		m_writer.Write(_value);
	}

	public void Write(float _value)
	{
		m_writer.Write(_value);
	}

	public void Write(double _value)
	{
		m_writer.Write(_value);
	}

	public void Write(string _value)
	{
		m_writer.Write(_value);
	}

	public void Serialise(ref bool io_value)
	{
		Write(io_value);
	}

	public void Serialise(ref byte io_value)
	{
		Write(io_value);
	}

	public void Serialise(ref char io_value)
	{
		Write(io_value);
	}

	public void Serialise(ref ushort io_value)
	{
		Write(io_value);
	}

	public void Serialise(ref short io_value)
	{
		Write(io_value);
	}

	public void Serialise(ref uint io_value)
	{
		Write(io_value);
	}

	public void Serialise(ref int io_value)
	{
		Write(io_value);
	}

	public void Serialise(ref ulong io_value)
	{
		Write(io_value);
	}

	public void Serialise(ref long io_value)
	{
		Write(io_value);
	}

	public void Serialise(ref float io_value)
	{
		Write(io_value);
	}

	public void Serialise(ref double io_value)
	{
		Write(io_value);
	}

	public void Serialise(ref string io_value)
	{
		Write(io_value);
	}

	public bool Serialise(Dictionary<string, string> _properties)
	{
		Write(_properties);
		return true;
	}

	public bool BeginProperties(out IPropertySerialiser o_propertySerialiser)
	{
		WritePropertySerialiser o_writePropertySerialiser;
		bool result = BeginWriteProperties(out o_writePropertySerialiser);
		o_propertySerialiser = o_writePropertySerialiser;
		return result;
	}

	public bool BeginWriteProperties(out WritePropertySerialiser o_writePropertySerialiser)
	{
		if (m_currentProperties != null)
		{
			o_writePropertySerialiser = null;
			return false;
		}
		m_currentProperties = new Dictionary<string, string>();
		o_writePropertySerialiser = new WritePropertySerialiser(m_currentProperties);
		return true;
	}

	public bool EndProperties()
	{
		if (m_currentProperties == null)
		{
			return false;
		}
		Write(m_currentProperties);
		m_currentProperties = null;
		return true;
	}

	public bool Sentinal()
	{
		Write(3236803774u);
		return true;
	}

	public bool EndOfSerialise()
	{
		if (m_currentProperties != null)
		{
			return false;
		}
		Write(3305041933u);
		m_writer.Flush();
		return true;
	}
}
