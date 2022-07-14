using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public class ArrayDataType : ADataTypeBase
{
	public ADataTypeBase ElementType { get; }
	public bool AllowNone { get; }
	public int? ItemCount { get; }
	public ArrayDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(Array))
	{
		var ofNode = xmlNode.SelectSingleNode("of")!;
		var allowNoneNode = xmlNode.SelectSingleNode("AllowNone");
		var sizeNode = xmlNode.SelectSingleNode("size");

		ElementType = definitionStore.GetDataType(version, ofNode);
		AllowNone = allowNoneNode is not null && allowNoneNode.TrimmedText() == "true";
		ClrType = Array.CreateInstance(ElementType.ClrType, 0).GetType();
		if (sizeNode is not null)
		{
			ItemCount = int.Parse(sizeNode.TrimmedText());
			DataSize = ElementType.DataSize * ItemCount.Value;
		}
	}
	public override object? GetDefaultValue(XmlNode? propertyOrArgumentNode, bool forArray = false)
		=> propertyOrArgumentNode?.SelectXmlNodes("Default/item").Select(node => ElementType.GetDefaultValue(node, true)).ToArray();
	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
	{
		var size = ItemCount ?? reader.ReadByte();
		return new FixedList(ElementType, Enumerable.Range(0, size).Select(_ => ElementType.GetValue(reader, propertyOrArgumentNode, headerSize)).ToArray());
	}
}
