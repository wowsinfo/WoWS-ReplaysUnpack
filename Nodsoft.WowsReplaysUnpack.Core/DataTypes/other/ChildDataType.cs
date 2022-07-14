using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public class ChildDataType : ADataTypeBase
{
	public ADataTypeBase ChildType { get; set; }
	public ChildDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(object))
	{
		var typeNode = xmlNode.SelectSingleNode("Type");
		ChildType = typeNode is null ? new BlobDataType(version, definitionStore, xmlNode) : definitionStore.GetDataType(version, typeNode);
		ClrType = ChildType.ClrType;
	}

	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
	{
		// Always read header otherwise we will get a padding error
		if (ChildType is not BlobDataType)
			_ = reader.ReadBytes(headerSize);
		return ChildType.GetValue(reader, propertyOrArgumentNode, headerSize);
	}
}
