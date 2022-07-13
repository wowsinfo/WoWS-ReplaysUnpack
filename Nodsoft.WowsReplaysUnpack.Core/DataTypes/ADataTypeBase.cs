using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public abstract class ADataTypeBase
{
	protected Version Version { get; }
	protected DefinitionStore DefinitionStore { get; }
	protected XmlNode XmlNode { get; }

	public virtual int DataSize { get; protected set; } = Consts.Infinity;

	public ADataTypeBase(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
	{
		Version = version;
		DefinitionStore = definitionStore;
		XmlNode = xmlNode;
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
	protected virtual object? GetDefaultValue(XmlNode propertyOrArgumentNode)
		=> propertyOrArgumentNode.SelectSingleNodeText("Default");
	protected virtual int GetActualDataSize(BinaryReader reader)
		=> reader.ReadByte();
}
