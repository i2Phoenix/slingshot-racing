using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ReadSerialiser : IDisposable, ISerialiser
{
	private Dictionary<string, string> m_currentProperties;

	private BinaryReader m_reader;

	public bool IsReading
	{
		get
		{
			return true;
		}
	}

	public bool IsWriting
	{
		get
		{
			return false;
		}
	}

	public long Position
	{
		get
		{
			return m_reader.BaseStream.Position;
		}
	}

	public ReadSerialiser(Stream _stream)
	{
		if (_stream != null && _stream.CanRead)
		{
			m_reader = new BinaryReader(_stream, Encoding.UTF8);
		}
	}

	public bool IsValid()
	{
		return m_reader != null;
	}

	public void Dispose()
	{
		if (m_reader != null)
		{
			m_reader.Close();
			m_reader = null;
		}
	}

	public bool FourCC(byte[] _fourCC)
	{
		if (_fourCC.Length != 4)
		{
			return false;
		}
		byte b = ReadByte();
		byte b2 = ReadByte();
		byte b3 = ReadByte();
		byte b4 = ReadByte();
		return b == _fourCC[0] && b2 == _fourCC[1] && b3 == _fourCC[2] && b4 == _fourCC[3];
	}

	public bool Read(Dictionary<string, string> _properties)
	{
		int num = ReadInt32();
		for (int i = 0; i < num; i++)
		{
			string key = ReadString();
			string value = ReadString();
			_properties.Add(key, value);
		}
		uint num2 = ReadUInt32();
		return num2 == 3236803774u;
	}

	public bool ReadBoolean()
	{
		return m_reader.ReadBoolean();
	}

	public byte ReadByte()
	{
		return m_reader.ReadByte();
	}

	public byte[] ReadBytes(int _count)
	{
		return m_reader.ReadBytes(_count);
	}

	public char ReadChar()
	{
		return m_reader.ReadChar();
	}

	public ushort ReadUInt16()
	{
		return m_reader.ReadUInt16();
	}

	public short ReadInt16()
	{
		return m_reader.ReadInt16();
	}

	public uint ReadUInt32()
	{
		return m_reader.ReadUInt32();
	}

	public int ReadInt32()
	{
		return m_reader.ReadInt32();
	}

	public ulong ReadUInt64()
	{
		return m_reader.ReadUInt64();
	}

	public long ReadInt64()
	{
		return m_reader.ReadInt64();
	}

	public float ReadSingle()
	{
		return m_reader.ReadSingle();
	}

	public double ReadDouble()
	{
		return m_reader.ReadDouble();
	}

	public string ReadString()
	{
		return m_reader.ReadString();
	}

	public void Serialise(ref bool io_value)
	{
		io_value = ReadBoolean();
	}

	public void Serialise(ref byte io_value)
	{
		io_value = ReadByte();
	}

	public void Serialise(ref char io_value)
	{
		io_value = ReadChar();
	}

	public void Serialise(ref ushort io_value)
	{
		io_value = ReadUInt16();
	}

	public void Serialise(ref short io_value)
	{
		io_value = ReadInt16();
	}

	public void Serialise(ref uint io_value)
	{
		io_value = ReadUInt32();
	}

	public void Serialise(ref int io_value)
	{
		io_value = ReadInt32();
	}

	public void Serialise(ref ulong io_value)
	{
		io_value = ReadUInt64();
	}

	public void Serialise(ref long io_value)
	{
		io_value = ReadInt64();
	}

	public void Serialise(ref float io_value)
	{
		io_value = ReadSingle();
	}

	public void Serialise(ref double io_value)
	{
		io_value = ReadDouble();
	}

	public void Serialise(ref string io_value)
	{
		io_value = ReadString();
	}

	public bool Serialise(Dictionary<string, string> _properties)
	{
		return Read(_properties);
	}

	public bool BeginProperties(out IPropertySerialiser o_propertySerialiser)
	{
		ReadPropertySerialiser o_readPropertySerialiser;
		bool result = BeginReadProperties(out o_readPropertySerialiser);
		o_propertySerialiser = o_readPropertySerialiser;
		return result;
	}

	public bool BeginReadProperties(out ReadPropertySerialiser o_readPropertySerialiser)
	{
		if (m_currentProperties != null)
		{
			o_readPropertySerialiser = null;
			return false;
		}
		m_currentProperties = new Dictionary<string, string>();
		if (!Serialise(m_currentProperties))
		{
			m_currentProperties = null;
			o_readPropertySerialiser = null;
			return false;
		}
		o_readPropertySerialiser = new ReadPropertySerialiser(m_currentProperties);
		return true;
	}

	public bool EndProperties()
	{
		if (m_currentProperties == null)
		{
			return false;
		}
		m_currentProperties = null;
		return true;
	}

	public bool Sentinal()
	{
		uint num = ReadUInt32();
		return num == 3236803774u;
	}

	public bool EndOfSerialise()
	{
		if (m_currentProperties != null)
		{
			return false;
		}
		uint num = ReadUInt32();
		return num == 3305041933u;
	}
}
