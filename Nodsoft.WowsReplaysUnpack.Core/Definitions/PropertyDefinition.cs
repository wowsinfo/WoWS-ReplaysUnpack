using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public class PropertyDefinition
{
	public string Name { get; }
	public ADataTypeBase DataType { get; }
	public string? DefaultValue { get; }
	public EntityFlag Flag { get; } = EntityFlag.CELL_PRIVATE;
	public PropertyDefinition(XmlNode xmlNode, Alias alias)
	{
		Name = xmlNode.Name;
		DataType = alias.GetDataType(xmlNode.SelectSingleNode("Type")!.InnerText.Trim());
		DefaultValue = xmlNode.SelectSingleNode("Default")?.InnerText.Trim();

		var flag = xmlNode.SelectSingleNode("Flags")?.InnerText?.Trim();
		if (!string.IsNullOrEmpty(flag))
		{
			Flag = (EntityFlag)Enum.Parse(typeof(EntityFlag),flag);
		}
	}

	public object? GetValue(BinaryReader reader)
		=> DataType.GetValue(reader, DefaultValue);
}
