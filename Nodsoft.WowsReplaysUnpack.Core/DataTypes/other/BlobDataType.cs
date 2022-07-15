using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public class BlobDataType : ADataTypeBase
{
	public BlobDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(byte[]))
	{
	}

	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
		=> reader.ReadBytes(GetSizeFromHeader(reader));

	protected override int GetSizeFromHeader(BinaryReader reader)
	{
		int size = base.GetSizeFromHeader(reader);
		// hack for arenaStateReceived Method
		if (size is 0xff) // 255
		{
			size = reader.ReadInt16();
			_ = reader.ReadByte(); // padding
		}
		return size;
	}
}
