using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

internal class VectorDataType : ADataTypeBase
{
	private readonly int _itemCount;

	protected VectorDataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode, int itemCount)
		: base(version, definitionStore, xmlNode)
	{
		_itemCount = itemCount;
	}

	/// <summary>
	/// Reads the python tuple
	/// Equivalent to tuple(struct.unpack())
	/// </summary>
	/// <param name="reader"></param>
	/// <param name="itemCount"></param>
	/// <returns></returns>
	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
	{
		// Size of a float value
		int itemSize = 4;
		byte[] data = reader.ReadBytes(_itemCount * itemSize);
		float[] values = new float[_itemCount];
		for (int i = 0; i < _itemCount; i++)
		{
			var index = i * itemSize;
			values[i] = BitConverter.ToSingle(data, index);
		}

		return values;
	}

	protected override object? GetDefaultValue(XmlNode propertyOrArgumentNode)
	=> propertyOrArgumentNode.SelectSingleNodeText("Default")?.Split(' ').Select(v => Convert.ToSingle(v)).ToArray();
}

/// <summary>
/// Array of floats
/// </summary>
internal class Vector2DataType : VectorDataType
{
	public override int DataSize => 8;
	public Vector2DataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode) 
		: base(version, definitionStore, xmlNode, 2) { }
}

internal class Vector3DataType : VectorDataType
{
	public override int DataSize => 12;
	public Vector3DataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode) 
		: base(version, definitionStore, xmlNode, 3) { }
}

internal class Vector4DataType : VectorDataType
{
	public override int DataSize => 16;
	public Vector4DataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode) 
		: base(version, definitionStore, xmlNode, 4) { }
}
