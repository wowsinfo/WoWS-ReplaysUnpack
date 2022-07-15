using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using System.Numerics;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

internal class Vector3DataType : VectorDataType
{
	public override int DataSize => 12;
	public Vector3DataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, 3, typeof(Vector3)) { }

	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
	{
		float[]? values = (float[]?)base.GetValueInternal(reader, propertyOrArgumentNode, headerSize);
		return values is null ? null : new Vector3(values[0], values[1], values[2]);
	}
}
