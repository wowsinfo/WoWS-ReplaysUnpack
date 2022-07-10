using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public class EntityMethodDefinition
{
	private const int DEFAULT_HEADER_SIZE = 1;
	public string? Name { get; }
	public List<EntityMethodArgumentDefinition> Arguments { get; } = new();
	public int VariableLengthHeaderSize { get; } = DEFAULT_HEADER_SIZE;
	public EntityMethodDefinition(XmlNode node, Alias alias, int headerSize = DEFAULT_HEADER_SIZE)
	{
		Name = node.Name;

		var args = node.SelectSingleNode("Args");
		if (args is not null)
		{
			foreach (var arg in args.ChildNodes.Cast<XmlNode>())
				Arguments.Add(new EntityMethodArgumentDefinition(arg.Name, alias.GetDataType(arg.FirstChild!.InnerText.Trim())));
		}
		else if (node.SelectNodes("Arg") is not null)
		{
			foreach (var arg in node.SelectNodes("Arg")!.Cast<XmlNode>())
			{
				Arguments.Add(new EntityMethodArgumentDefinition(null, alias.GetDataType(arg.FirstChild!.InnerText.Trim())));
			}
		}

		var variableLengthHeaderSizeNode = node.SelectSingleNode("VariableLengthHeaderSize");
		if (variableLengthHeaderSizeNode is not null)
			try { VariableLengthHeaderSize = int.Parse(variableLengthHeaderSizeNode.FirstChild!.InnerText.Trim()); }
			catch { VariableLengthHeaderSize = headerSize; }
		else
			VariableLengthHeaderSize = headerSize;
	}

	public (List<object?> unnamed, Dictionary<string, object?> named) GetValues(BinaryReader reader)
	{
		var unnamed = new List<object?>();
		var named = new Dictionary<string, object?>();
		foreach (var argument in Arguments)
		{
			var value = argument.DataType.GetValue(reader);
			if (argument.Name is null)
				unnamed.Add(value);
			else
				named.Add(argument.Name, value);
		}
		return (unnamed, named);
	}
}

public class EntityMethodArgumentDefinition
{
	public string? Name { get; }
	public ADataTypeBase DataType { get; }
	public int VariableHeaderSize { get; } = 1;

	public EntityMethodArgumentDefinition(string? name, ADataTypeBase dataType)
	{
		Name = name;
		DataType = dataType;
	}

}
