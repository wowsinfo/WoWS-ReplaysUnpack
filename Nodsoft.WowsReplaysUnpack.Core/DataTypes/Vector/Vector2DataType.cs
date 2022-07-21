using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using System.Numerics;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

/// <summary>
/// Array of floats
/// </summary>
internal class Vector2DataType : VectorDataType
{
	public override int DataSize => 8;
	public Vector2DataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode) : base(version, definitionStore, xmlNode, 2, typeof(Vector2)) { }

	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize) =>
		base.GetValueInternal(reader, propertyOrArgumentNode, headerSize) is float[] values 
			? new Vector2(values[0], values[1]) 
			: null;
}
