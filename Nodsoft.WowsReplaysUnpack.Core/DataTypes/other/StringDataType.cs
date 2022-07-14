using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using System.Text;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public class StringDataType : ADataTypeBase
{
	public StringDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(string))
	{
		DataSize = Consts.Infinity;
	}

	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
		=> Encoding.UTF8.GetString(reader.ReadBytes(GetSizeFromHeader(reader)));
}
