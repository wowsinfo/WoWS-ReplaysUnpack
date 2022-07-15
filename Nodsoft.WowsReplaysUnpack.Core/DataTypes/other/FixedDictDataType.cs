using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public class FixedDictDataType : ADataTypeBase
{
	public bool AllowNone { get; }
	public Dictionary<string, ADataTypeBase> PropertyTypes { get; set; } = new();
	public FixedDictDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(Dictionary<string, object?>))
	{
		XmlNode? allowNoneNode = xmlNode.SelectSingleNode("AllowNone");
		AllowNone = allowNoneNode is not null && allowNoneNode.TrimmedText() == "true";

		foreach (XmlNode propertyNode in xmlNode.SelectSingleNode("Properties")!.ChildNodes())
			PropertyTypes.Add(propertyNode.Name, definitionStore.GetDataType(version, propertyNode.SelectSingleNode("Type")!));

		if (!AllowNone)
		{
			DataSize = PropertyTypes.Select(p => p.Value.DataSize).Sum();
		}
	}

	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
	{
		long originalStreamPosition = reader.BaseStream.Position;
		if (AllowNone)
		{
			byte flag = reader.ReadByte();
			if (flag == 0x00) // empty dict
				return null;
			else if (flag == 0x01) { } // non empty dict
			else
				reader.BaseStream.Seek(originalStreamPosition, SeekOrigin.Begin);
		}

		return new FixedDictionary(PropertyTypes, PropertyTypes.ToDictionary(kv => kv.Key, kv => kv.Value.GetValue(reader, propertyOrArgumentNode, headerSize)));
	}
}
