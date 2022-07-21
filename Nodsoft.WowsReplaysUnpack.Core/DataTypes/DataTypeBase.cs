using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public abstract class DataTypeBase
{
	protected Version Version { get; }
	protected IDefinitionStore DefinitionStore { get; }
	protected XmlNode XmlNode { get; }

	public virtual int DataSize { get; protected init; } = Consts.Infinity;
	public Type ClrType { get; protected init; }

	protected DataTypeBase(Version version, IDefinitionStore definitionStore, XmlNode typeOrArgXmlNode, Type clrType)
		=> (Version, DefinitionStore, XmlNode, ClrType) = (version, definitionStore, typeOrArgXmlNode, clrType);

	public object? GetValue(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize = 1)
		=> GetValueInternal(reader, propertyOrArgumentNode, headerSize)
			?? GetDefaultValue(propertyOrArgumentNode);

	protected abstract object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize);

	public virtual object? GetDefaultValue(XmlNode? propertyOrArgumentNode, bool forArray = false)
		=> propertyOrArgumentNode is null
			? null
			: !forArray
				? propertyOrArgumentNode.SelectSingleNodeText("Default")
				: propertyOrArgumentNode.TrimmedText();

	protected virtual int GetSizeFromHeader(BinaryReader reader) => reader.ReadByte();
}