using System.Collections.Generic;

public class SerialiseNodeWriter
{
	private readonly WriteSerialiser m_writeSerialiser;

	private int m_nodeDepth;

	private bool m_beginGraph;

	private bool m_writingProperties;

	public SerialiseNodeWriter(WriteSerialiser _writeSerialiser)
	{
		m_writeSerialiser = _writeSerialiser;
	}

	public bool BeginGraph()
	{
		if (m_beginGraph)
		{
			return false;
		}
		m_writeSerialiser.Write(3235774464u);
		m_writeSerialiser.Write(1);
		m_beginGraph = true;
		return true;
	}

	public bool BeginNode(string _tag)
	{
		if (!m_beginGraph)
		{
			return false;
		}
		m_writeSerialiser.Write(3235774465u);
		m_writeSerialiser.Write(_tag);
		m_nodeDepth++;
		return true;
	}

	public bool WriteProperties(Dictionary<string, string> _properties)
	{
		if (m_nodeDepth == 0)
		{
			return false;
		}
		m_writeSerialiser.Write(3235774466u);
		m_writeSerialiser.Write(_properties);
		return true;
	}

	public bool BeginProperties(out WritePropertySerialiser o_writePropertiesSerialiser)
	{
		if (m_nodeDepth == 0)
		{
			o_writePropertiesSerialiser = null;
			return false;
		}
		if (m_writingProperties)
		{
			o_writePropertiesSerialiser = null;
			return false;
		}
		if (!m_writeSerialiser.BeginWriteProperties(out o_writePropertiesSerialiser))
		{
			return false;
		}
		m_writeSerialiser.Write(3235774466u);
		m_writingProperties = true;
		return true;
	}

	public bool EndProperties()
	{
		if (!m_writingProperties)
		{
			return false;
		}
		if (!m_writeSerialiser.EndProperties())
		{
			return false;
		}
		m_writingProperties = false;
		return true;
	}

	public bool EndNode()
	{
		if (m_nodeDepth == 0)
		{
			return false;
		}
		m_writeSerialiser.Write(3235774467u);
		m_nodeDepth--;
		return true;
	}

	public bool EndGraph()
	{
		if (!m_beginGraph)
		{
			return false;
		}
		if (m_nodeDepth != 0)
		{
			return false;
		}
		m_writeSerialiser.Write(3235774468u);
		m_writeSerialiser.Sentinal();
		m_beginGraph = false;
		return true;
	}
}
