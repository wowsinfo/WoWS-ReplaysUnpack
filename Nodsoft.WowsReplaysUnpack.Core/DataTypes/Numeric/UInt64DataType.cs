using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

/// <summary>
/// C# ulong
/// </summary>
internal class UInt64DataType : DataTypeBase
{
	public UInt64DataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode) : base(version, definitionStore, xmlNode, typeof(ulong))
	{
	}

	public override int DataSize => 8;
	protected override object GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize) => reader.ReadUInt64();
}
