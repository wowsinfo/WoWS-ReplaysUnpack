using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

/// <summary>
/// C# long
/// </summary>
internal class Int64DataType : ADataTypeBase
{
	public Int64DataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(long))
	{
	}

	public override int DataSize => 8;
	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
		=> reader.ReadInt64();
}
