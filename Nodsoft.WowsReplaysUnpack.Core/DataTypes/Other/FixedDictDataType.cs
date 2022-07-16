using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public sealed class FixedDictDataType : DataTypeBase
{
	public bool AllowNone { get; }
	public Dictionary<string, DataTypeBase> PropertyTypes { get; } = new();

	public FixedDictDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode) : base(version, definitionStore, xmlNode, typeof(Dictionary<string, object?>))
	{
		XmlNode? allowNoneNode = XmlNode.SelectSingleNode("AllowNone");
		AllowNone = allowNoneNode?.TrimmedText() is "true";

		foreach (XmlNode propertyNode in XmlNode.SelectSingleNode("Properties")!.ChildNodes())
		{
			PropertyTypes.Add(propertyNode.Name, DefinitionStore.GetDataType(Version, propertyNode.SelectSingleNode("Type")!));
		}

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

			if (flag is 0x00) // empty dict
			{
				return null;
			}

			if (flag is 0x01) { } // non empty dict
			else
			{
				reader.BaseStream.Seek(originalStreamPosition, SeekOrigin.Begin);
			}
		}

		return new FixedDictionary(PropertyTypes, PropertyTypes.ToDictionary(
			kv => kv.Key, 
			kv => kv.Value.GetValue(reader, propertyOrArgumentNode, headerSize)));
	}
}