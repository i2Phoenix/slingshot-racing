public class BitOutputStream
{
	private int m_capacity;

	private int m_numWords;

	private int m_numUnusedBits;

	private uint m_word;

	private uint[] m_buf;

	public BitOutputStream(int capacity)
	{
		m_capacity = capacity / 4;
		m_buf = new uint[m_capacity];
		m_numWords = 0;
		m_numUnusedBits = 32;
	}

	public void Reset()
	{
		m_numWords = 0;
		m_numUnusedBits = 32;
	}

	public void Write(uint val, uint numBits)
	{
		int num = (int)numBits - m_numUnusedBits;
		if (num >= 0)
		{
			if (m_capacity == m_numWords)
			{
				m_capacity <<= 1;
				uint[] array = new uint[m_capacity];
				m_buf.CopyTo(array, 0);
				m_buf = array;
			}
			m_buf[m_numWords++] = m_word | (val >> num);
			m_numUnusedBits = 32 - num;
			m_word = val << m_numUnusedBits - 1 << 1;
		}
		else
		{
			m_numUnusedBits = -num;
			m_word |= val << m_numUnusedBits;
		}
	}

	public void Write(int val, uint numBits)
	{
		Write((uint)(val & ((1 << (int)numBits) - 1)), numBits);
	}

	public void Write(uint val)
	{
		if (m_capacity == m_numWords)
		{
			m_capacity <<= 1;
			uint[] array = new uint[m_capacity];
			m_buf.CopyTo(array, 0);
			m_buf = array;
		}
		int num = 32 - m_numUnusedBits;
		m_buf[m_numWords++] = m_word | (val >> num);
		m_word = val << m_numUnusedBits - 1 << 1;
	}

	public void Write(int val)
	{
		Write((uint)val);
	}

	public void Write(ushort val)
	{
		Write((uint)val, 16u);
	}

	public void Write(short val)
	{
		Write((uint)(val & 0xFFFF), 16u);
	}

	public void Write(byte val)
	{
		Write((uint)val, 8u);
	}

	public void Write(sbyte val)
	{
		Write((uint)(val & 0xFF), 8u);
	}

	public void Write(float val)
	{
		Write(FloatUtils.FloatToBits(val));
	}

	public void Write(bool val)
	{
		Write(val ? 1u : 0u, 1u);
	}

	public byte[] ToByteArray()
	{
		if (m_numUnusedBits < 32)
		{
			Write(0u, (uint)m_numUnusedBits);
		}
		byte[] array = new byte[m_numWords * 4];
		int num = 0;
		int num2 = 0;
		while (num != m_numWords)
		{
			uint num3 = m_buf[num];
			array[num2] = (byte)(num3 >> 24);
			array[num2 + 1] = (byte)((num3 >> 16) & 0xFF);
			array[num2 + 2] = (byte)((num3 >> 8) & 0xFF);
			array[num2 + 3] = (byte)(num3 & 0xFF);
			num++;
			num2 += 4;
		}
		return array;
	}
}
