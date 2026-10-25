public class BitInputStream
{
	private int m_numBits;

	private int m_pos;

	private uint m_word;

	private uint[] m_buf;

	public BitInputStream(byte[] buf)
	{
		m_buf = new uint[buf.Length / 4];
		int num = 0;
		int num2 = 0;
		while (num != m_buf.Length)
		{
			m_buf[num] = (uint)((buf[num2] << 24) | (buf[num2 + 1] << 16) | (buf[num2 + 2] << 8) | buf[num2 + 3]);
			num++;
			num2 += 4;
		}
		Reset();
	}

	public void Reset()
	{
		m_pos = 0;
		m_word = 0u;
		m_numBits = 0;
	}

	public uint ReadUInt(uint numBits)
	{
		uint num = m_word >> (int)(32 - numBits);
		int num2 = m_numBits - (int)numBits;
		if (num2 < 0)
		{
			uint num3 = m_buf[m_pos++];
			m_word = num3 << -1 - num2 << 1;
			m_numBits = 32 + num2;
			return num | (num3 >> m_numBits);
		}
		m_word <<= (int)numBits;
		m_numBits = num2;
		return num;
	}

	public int ReadInt(uint numBits)
	{
		int num = (int)(32 - numBits);
		uint num2 = ReadUInt(numBits);
		return (int)(num2 << num) >> num;
	}

	public uint ReadUInt()
	{
		uint word = m_word;
		uint num = m_buf[m_pos++];
		m_word = num << 31 - m_numBits << 1;
		return word | (num >> m_numBits);
	}

	public int ReadInt()
	{
		return (int)ReadUInt();
	}

	public ushort ReadUShort()
	{
		return (ushort)ReadUInt(16u);
	}

	public short ReadShort()
	{
		return (short)ReadInt(16u);
	}

	public byte ReadByte()
	{
		return (byte)ReadUInt(8u);
	}

	public sbyte ReadSByte()
	{
		return (sbyte)ReadInt(8u);
	}

	public float ReadFloat()
	{
		return FloatUtils.BitsToFloat(ReadUInt());
	}

	public bool ReadBool()
	{
		return ReadUInt(1u) != 0;
	}
}
