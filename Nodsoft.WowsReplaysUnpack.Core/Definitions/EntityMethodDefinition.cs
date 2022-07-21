using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public class EntityMethodDefinition
{
	private const int DefaultHeaderSize = 1;
	public string? Name { get; }
	public List<EntityMethodArgumentDefinition> Arguments { get; } = new();
	public int HeaderSize { get; }

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
			{
				Arguments.Add(new(arg, arg.Name, definitionStore.GetDataType(clientVersion, arg)));
			}
		}
		else if (node.SelectNodes("Arg") is not null)
		{
			int index = 0;

			foreach (XmlNode arg in node.SelectXmlNodes("Arg"))
			{
				Arguments.Add(new(arg, index.ToString(), definitionStore.GetDataType(clientVersion, arg)));
				index++;
			}
		}

		// Set Header Size
		XmlNode? variableLengthHeaderSizeNode = node.SelectSingleNode("VariableLengthHeaderSize");

		if (variableLengthHeaderSizeNode is not null)
		{
			try { HeaderSize = int.Parse(variableLengthHeaderSizeNode.FirstChild!.TrimmedText()); }
			catch { HeaderSize = DefaultHeaderSize; }
		}
		else
		{
			HeaderSize = DefaultHeaderSize;
		}

		// Set Data Size
		DataSize = Arguments.Sum(a => a.DataType.DataSize);

		if (DataSize >= Consts.Infinity)
		{
			DataSize = Consts.Infinity + HeaderSize;
		}
		else
		{
			DataSize += HeaderSize;
		}
	}

	public (List<object?> unnamed, Dictionary<string, object?> named) GetValues(BinaryReader reader)
	{
		List<object?> unnamed = new();
		Dictionary<string, object?> named = new();

		foreach (EntityMethodArgumentDefinition argument in Arguments)
		{
			object? value = argument.GetValue(reader);
			named.Add(argument.Name, value);
		}

		return (unnamed, named);
	}

	public override string ToString() => $"{Name} <{DataSize}, {HeaderSize}>";
}

public record EntityMethodArgumentDefinition
{
	private readonly XmlNode _node;

	public string Name { get; }
	public DataTypeBase DataType { get; }

	public EntityMethodArgumentDefinition(XmlNode node, string name, DataTypeBase dataType) => (_node, Name, DataType) = (node, name, dataType);

	public object? GetValue(BinaryReader binaryReader)
	{
		return DataType.GetValue(binaryReader, _node);
	}
}