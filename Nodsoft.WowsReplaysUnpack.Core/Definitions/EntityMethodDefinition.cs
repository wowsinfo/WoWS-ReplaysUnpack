using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public class EntityMethodDefinition
{
	private const int DEFAULT_HEADER_SIZE = 1;
	public string? Name { get; }
	public List<EntityMethodArgumentDefinition> Arguments { get; } = new();
	public int HeaderSize { get; } = DEFAULT_HEADER_SIZE;

	public int DataSize { get; }
	public EntityMethodDefinition(Version clientVersion, IDefinitionStore definitionStore,
		XmlNode node)
	{
		Name = node.Name;

		// Parse Args
		XmlNode? args = node.SelectSingleNode("Args");
		if (args is not null)
		{
			foreach (XmlNode arg in args.ChildNodes())
				Arguments.Add(new EntityMethodArgumentDefinition(arg, arg.Name, definitionStore.GetDataType(clientVersion, arg)));
		}
		else if (node.SelectNodes("Arg") is not null)
		{
			int index = 0;
			foreach (XmlNode arg in node.SelectXmlNodes("Arg"))
			{
				Arguments.Add(new EntityMethodArgumentDefinition(arg, index.ToString(), definitionStore.GetDataType(clientVersion, arg)));
				index++;
			}
		}

		// Set Header Size
		XmlNode? variableLengthHeaderSizeNode = node.SelectSingleNode("VariableLengthHeaderSize");
		if (variableLengthHeaderSizeNode is not null)
			try { HeaderSize = int.Parse(variableLengthHeaderSizeNode.FirstChild!.TrimmedText()); }
			catch { HeaderSize = DEFAULT_HEADER_SIZE; }
		else
			HeaderSize = DEFAULT_HEADER_SIZE;

		// Set Data Size
		DataSize = Arguments.Sum(a => a.DataType.DataSize);
		if (DataSize >= Consts.Infinity)
			DataSize = Consts.Infinity + HeaderSize;
		else
			DataSize = DataSize + HeaderSize;
	}

	public (List<object?> unnamed, Dictionary<string, object?> named) GetValues(BinaryReader reader)
	{
		List<object?> unnamed = new();
		Dictionary<string, object?> named = new();
		foreach (EntityMethodArgumentDefinition argument in Arguments)
		{
			object? value = argument.GetValue(reader);
			if (argument.Name is null)
				unnamed.Add(value);
			else
				named.Add(argument.Name, value);
		}
		return (unnamed, named);
	}

	public override string ToString() => $"{Name} <{DataSize}, {HeaderSize}>";
}

public class EntityMethodArgumentDefinition
{
	private readonly XmlNode _node;

	public string Name { get; }
	public ADataTypeBase DataType { get; }

	public EntityMethodArgumentDefinition(XmlNode node, string name, ADataTypeBase dataType)
	{
		_node = node;
		Name = name;
		DataType = dataType;
	}

	public object? GetValue(BinaryReader binaryReader)
	{
		return DataType.GetValue(binaryReader, _node);
	}
}
