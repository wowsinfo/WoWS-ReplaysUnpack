using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

/// <summary>
/// C# float
/// </summary>
internal class Float32DataType : DataTypeBase
{
	public Float32DataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode) : base(version, definitionStore, xmlNode, typeof(float))
	{
	}

	public override int DataSize => 4;
	protected override object GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize) => reader.ReadSingle();
}
