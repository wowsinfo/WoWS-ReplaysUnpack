using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

/// <summary>
/// C# bool
/// </summary>
internal class UInt8DataType : ADataTypeBase
{
	public UInt8DataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(bool))
	{
	}

	public override int DataSize => 1;
	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
		=> reader.ReadByte() == 0x1;

	public override object? GetDefaultValue(XmlNode? propertyOrArgumentNode, bool forArray = false)
		=> propertyOrArgumentNode?.SelectSingleNodeText("Default")?.ToLower() == "true";
}
