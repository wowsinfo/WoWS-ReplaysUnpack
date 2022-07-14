using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using System.Numerics;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

internal class Vector4DataType : VectorDataType
{
	public override int DataSize => 16;
	public Vector4DataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, 4, typeof(Vector4)) { }

	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
	{
		var values = (float[]?)base.GetValueInternal(reader, propertyOrArgumentNode, headerSize);
		return values is null ? null : new Vector4(values[0], values[1], values[2], values[3]);
	}
}
