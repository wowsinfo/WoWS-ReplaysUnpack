using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

/// <summary>
/// Base class for all game client data types.
/// </summary>
public abstract class DataTypeBase
{
	/// <summary>
	/// Game client version of this data type.
	/// </summary>
	protected Version Version { get; }

	/// <summary>
	/// Definition store for this data type.
	/// </summary>
	protected IDefinitionStore DefinitionStore { get; }

	/// <summary>
	/// XML node associated with this data type.
	/// </summary>
	protected XmlNode XmlNode { get; }

	/// <summary>
	/// Data size of this data type.
	/// </summary>
	public virtual int DataSize { get; protected init; } = Consts.Infinity;

	/// <summary>
	/// CLR type that this data type maps to.
	/// </summary>
	public Type ClrType { get; protected init; }

	protected DataTypeBase(Version version, IDefinitionStore definitionStore, XmlNode typeOrArgXmlNode, Type clrType)
		=> (Version, DefinitionStore, XmlNode, ClrType) = (version, definitionStore, typeOrArgXmlNode, clrType);

	public object? GetValue(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize = 1)
		=> GetValueInternal(reader, propertyOrArgumentNode, headerSize) ?? GetDefaultValue(propertyOrArgumentNode);

	/// <summary>
	/// Internal method for getting the value from the binary reader.
	/// Setting the default value to the XML node if present.
	/// </summary>
	/// <param name="reader">Binary reader to read from.</param>
	/// <param name="propertyOrArgumentNode">XML node to get the value from.</param>
	/// <param name="headerSize">Size of the header to skip.</param>
	/// <returns>Value of the XML node.</returns>
	protected abstract object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize);

	/// <summary>
	/// Gets the default value of a given XML node, based on this data type's defaults.
	/// </summary>
	/// <param name="propertyOrArgumentNode">XML node to get the default value of.</param>
	/// <param name="forArray">Whether the default value is for an array.</param>
	/// <returns>Default value of the XML node.</returns>
	public virtual object? GetDefaultValue(XmlNode? propertyOrArgumentNode, bool forArray = false) => forArray
			? propertyOrArgumentNode?.TrimmedText()
			: propertyOrArgumentNode?.SelectSingleNodeText("Default");

	/// <summary>
	/// Reads the size of this data type by its header, from the binary reader.
	/// </summary>
	/// <param name="reader">Binary reader to read from.</param>
	/// <returns>Size of the data type.</returns>
	protected virtual int GetSizeFromHeader(BinaryReader reader) => reader.ReadByte();
}