using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

/// <summary>
/// Represents a property found in a definition (.def) file.
/// </summary>
public record PropertyDefinition
{
	/// <summary>
	/// Name of this property.
	/// </summary>
	public string Name { get; }
	
	/// <summary>
	/// Data type of this property.
	/// </summary>
	public DataTypeBase DataType { get; }
	
	/// <summary>
	/// Entity flag representing the scope of this property.
	/// </summary>
	public EntityFlag Flag { get; } = EntityFlag.CELL_PRIVATE;
	
	/// <summary>
	/// Data size of this property.
	/// </summary>
	public int DataSize => Math.Min(DataType.DataSize, Consts.Infinity);
	
	/// <summary>
	/// XML node representing this property.
	/// </summary>
	public XmlNode XmlNode { get; }

	public PropertyDefinition(Version clientVersion, IDefinitionStore definitionStore, XmlNode propertyXmlNode)
	{
		XmlNode = propertyXmlNode;
		Name = propertyXmlNode.Name;
		DataType = definitionStore.GetDataType(clientVersion, propertyXmlNode.SelectSingleNode("Type")!);

		string? flag = propertyXmlNode.SelectSingleNodeText("Flags");

		if (!string.IsNullOrEmpty(flag))
		{
			Flag = (EntityFlag)Enum.Parse(typeof(EntityFlag), flag);
		}
	}

	/// <summary>
	/// Gets the value of this property held within a BinaryReader.
	/// </summary>
	/// <param name="reader">BinaryReader to read from.</param>
	/// <param name="propertyNode">XML node representing this property.</param>
	/// <returns>Value of this property.</returns>
	public object? GetValue(BinaryReader reader, XmlNode propertyNode) => DataType.GetValue(reader, propertyNode);

	/// <summary>
	/// Returns the string representation of this property.
	/// </summary>
	/// <returns>String representation of this property.</returns>
	public override string ToString() => $"{Name} <{DataType.GetType().Name},{DataType.ClrType.Name}>";
}