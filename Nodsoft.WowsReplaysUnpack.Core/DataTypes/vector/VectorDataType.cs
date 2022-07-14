using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

internal class VectorDataType : ADataTypeBase
{
	private readonly int _itemCount;

	protected VectorDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode, int itemCount, Type clrType)
		: base(version, definitionStore, xmlNode, clrType)
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
	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
	{
		// Size of a float value
		int itemSize = 4;
		var x = Enumerable.Range(0, _itemCount);
		return Enumerable.Range(0, _itemCount)
			.Select((_, index) =>
			{
				var s = x;
				return reader.ReadSingle();
			}).ToArray();
	}

	public override object? GetDefaultValue(XmlNode? propertyOrArgumentNode, bool forArray = false)
	=> propertyOrArgumentNode?.SelectSingleNodeText("Default")?.Split(' ').Select(v => Convert.ToSingle(v)).ToArray();
}
