using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public sealed class ArrayDataType : DataTypeBase
{
	public DataTypeBase ElementType { get; }
	public bool AllowNone { get; }
	public int? ItemCount { get; }

	public ArrayDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode) : base(version, definitionStore, xmlNode, typeof(Array))
	{
		XmlNode ofNode = XmlNode.SelectSingleNode("of")!;
		XmlNode? allowNoneNode = XmlNode.SelectSingleNode("AllowNone");
		XmlNode? sizeNode = XmlNode.SelectSingleNode("size");

		ElementType = DefinitionStore.GetDataType(Version, ofNode);
		AllowNone = allowNoneNode?.TrimmedText() is "true";
		ClrType = Array.CreateInstance(ElementType.ClrType, 0).GetType();

		if (sizeNode is not null)
		{
			ItemCount = int.Parse(sizeNode.TrimmedText());
			DataSize = ElementType.DataSize * ItemCount.Value;
		}
	}

	public override object? GetDefaultValue(XmlNode? propertyOrArgumentNode, bool forArray = false)
		=> propertyOrArgumentNode?.SelectXmlNodes("Default/item").Select(node => ElementType.GetDefaultValue(node, true)).ToArray();

	protected override object GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
	{
		int size = ItemCount ?? reader.ReadByte();

		return new FixedList(ElementType, Enumerable.Range(0, size).Select(_ => ElementType.GetValue(reader, propertyOrArgumentNode, headerSize)).ToArray());
	}
}