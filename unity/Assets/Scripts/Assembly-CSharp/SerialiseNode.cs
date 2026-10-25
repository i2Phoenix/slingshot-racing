using System.Collections.Generic;

public class SerialiseNode
{
	private string m_tag;

	private Dictionary<string, string> m_properties = new Dictionary<string, string>();

	private List<SerialiseNode> m_children = new List<SerialiseNode>();

	public string Tag
	{
		get
		{
			return m_tag;
		}
		set
		{
			m_tag = value;
		}
	}

	public Dictionary<string, string> Properties
	{
		get
		{
			return m_properties;
		}
	}

	public ReadPropertySerialiser ReadProperties
	{
		get
		{
			return new ReadPropertySerialiser(m_properties);
		}
	}

	public WritePropertySerialiser WriteProperties
	{
		get
		{
			return new WritePropertySerialiser(m_properties);
		}
	}

	public List<SerialiseNode> Children
	{
		get
		{
			return m_children;
		}
	}

	public SerialiseNode()
	{
		m_tag = string.Empty;
	}

	public SerialiseNode(string _tag)
	{
		m_tag = _tag;
	}

	public static bool ReadGraph(ReadSerialiser _readSerialiser, out SerialiseNode o_node)
	{
		o_node = null;
		uint num = _readSerialiser.ReadUInt32();
		if (num != 3235774464u)
		{
			return false;
		}
		int num2 = _readSerialiser.ReadInt32();
		if (num2 != 1)
		{
			return false;
		}
		StackList<SerialiseNode> stackList = new StackList<SerialiseNode>();
		do
		{
			num = _readSerialiser.ReadUInt32();
			switch (num)
			{
			case 3235774465u:
			{
				string tag = _readSerialiser.ReadString();
				SerialiseNode serialiseNode = new SerialiseNode(tag);
				if (stackList.Count == 0)
				{
					if (o_node != null)
					{
						return false;
					}
					o_node = serialiseNode;
				}
				else
				{
					SerialiseNode serialiseNode2 = stackList.Peek();
					serialiseNode2.Children.Add(serialiseNode);
				}
				stackList.Push(serialiseNode);
				break;
			}
			case 3235774466u:
			{
				if (stackList.Count == 0)
				{
					return false;
				}
				SerialiseNode serialiseNode3 = stackList.Peek();
				if (!_readSerialiser.Read(serialiseNode3.m_properties))
				{
					return false;
				}
				break;
			}
			case 3235774467u:
				if (stackList.Count == 0)
				{
					return false;
				}
				stackList.Pop();
				break;
			case 3235774468u:
				if (stackList.Count != 0)
				{
					return false;
				}
				break;
			default:
				return false;
			}
		}
		while (num != 3235774468u);
		if (!_readSerialiser.Sentinal())
		{
			return false;
		}
		return true;
	}

	public bool WriteGraph(WriteSerialiser _writeSerialiser)
	{
		SerialiseNodeWriter serialiseNodeWriter = new SerialiseNodeWriter(_writeSerialiser);
		if (!serialiseNodeWriter.BeginGraph())
		{
			return false;
		}
		if (!WriteNode(serialiseNodeWriter))
		{
			return false;
		}
		return serialiseNodeWriter.EndGraph();
	}

	private bool WriteNode(SerialiseNodeWriter _writer)
	{
		if (!_writer.BeginNode(m_tag))
		{
			return false;
		}
		if (m_properties.Count > 0 && !_writer.WriteProperties(m_properties))
		{
			return false;
		}
		for (int i = 0; i < m_children.Count; i++)
		{
			if (!m_children[i].WriteNode(_writer))
			{
				return false;
			}
		}
		return _writer.EndNode();
	}
}
