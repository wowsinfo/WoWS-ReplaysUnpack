using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public abstract class ADataTypeBase
{
	protected Version Version { get; }
	protected IDefinitionStore DefinitionStore { get; }
	protected XmlNode XmlNode { get; }

	public virtual int DataSize { get; protected set; } = Consts.Infinity;
	public Type ClrType { get; protected set; }

	public ADataTypeBase(Version version, IDefinitionStore definitionStore, XmlNode typeOrArgXmlNode,
		Type clrType)
	{
		Version = version;
		DefinitionStore = definitionStore;
		XmlNode = typeOrArgXmlNode;
		ClrType = clrType;
	}

	public object? GetValue(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize = 1)
	{
		object? value = GetValueInternal(reader, propertyOrArgumentNode, headerSize);
		object? _defaultValue = GetDefaultValue(propertyOrArgumentNode);

		if (value is null && _defaultValue is null)
			return null;
		return value ?? _defaultValue;
	}

	protected abstract object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize);
	public virtual object? GetDefaultValue(XmlNode propertyOrArgumentNode, bool forArray = false)
	{
		if (!forArray)
			return propertyOrArgumentNode.SelectSingleNodeText("Default");

		return propertyOrArgumentNode.TrimmedText();
	}
	protected virtual int GetSizeFromHeader(BinaryReader reader)
		=> reader.ReadByte();
}
