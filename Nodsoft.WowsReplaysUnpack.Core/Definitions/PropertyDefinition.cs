using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public class PropertyDefinition
{
	public string Name { get; }
	public ADataTypeBase DataType { get; }
	public EntityFlag Flag { get; } = EntityFlag.CELL_PRIVATE;
	public int DataSize => Math.Min(DataType.DataSize, Consts.Infinity);
	public XmlNode XmlNode { get; }

	public PropertyDefinition(Version clientVersion, DefinitionStore definitionStore, XmlNode xmlNode)
	{
		XmlNode = xmlNode;
		Name = xmlNode.Name;
		DataType = definitionStore.GetDataType(clientVersion, xmlNode.SelectSingleNode("Type")!);

		var flag = xmlNode.SelectSingleNodeText("Flags");
		if (!string.IsNullOrEmpty(flag))
		{
			Flag = (EntityFlag)Enum.Parse(typeof(EntityFlag), flag);
		}
	}

	public object? GetValue(BinaryReader reader, XmlNode propertyNode)
		=> DataType.GetValue(reader, propertyNode);
}
