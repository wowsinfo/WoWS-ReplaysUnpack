using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public class ChildDataType : DataTypeBase
{
	public DataTypeBase ChildType { get; }
	public ChildDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode) : base(version, definitionStore, xmlNode, typeof(object))
	{
		XmlNode? typeNode = XmlNode.SelectSingleNode("Type");
		ChildType = typeNode is null ? new BlobDataType(Version, DefinitionStore, xmlNode) : DefinitionStore.GetDataType(Version, typeNode);
		ClrType = ChildType.ClrType;
	}

	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
	{
		// Always read header otherwise we will get a padding error
		if (ChildType is not BlobDataType)
		{
			_ = reader.ReadBytes(headerSize);
		}

		return ChildType.GetValue(reader, propertyOrArgumentNode, headerSize);
	}
}
