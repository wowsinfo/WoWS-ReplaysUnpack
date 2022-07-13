using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Security;
using Razorvine.Pickle;
using System.Collections;
using System.Text;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public class BlobDataType : ADataTypeBase
{
	public BlobDataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode)
	{
	}

	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
	{
		var bytes = reader.ReadBytes(GetActualDataSize(reader));
		CVEChecks.ScanForCVE_2022_31265(bytes, XmlNode.Name);
		using Unpickler unpickler = new();
		using MemoryStream buffer = new(bytes);
		return unpickler.load(buffer) as ArrayList;
	}

	protected override int GetActualDataSize(BinaryReader reader)
	{
		var size = base.GetActualDataSize(reader);
		if (size is 0xff) // 255
		{
			size = reader.ReadInt16();
			_ = reader.ReadByte(); // padding
		}
		return size;
	}
}

public class StringDataType : ADataTypeBase
{
	public StringDataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode)
	{
		DataSize = Consts.Infinity;
	}

	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
		=> Encoding.UTF8.GetString(reader.ReadBytes(GetActualDataSize(reader)));
}

public class UnicodeStringDataType : ADataTypeBase
{
	public UnicodeStringDataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode)
	{
		DataSize = Consts.Infinity;
	}

	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
		=> Encoding.Unicode.GetString(reader.ReadBytes(GetActualDataSize(reader)));
}

public class MailboxDataType : ADataTypeBase
{
	public MailboxDataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode)
	{
	}
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		=> "<Mailbox>";
}

public class UserType : ADataTypeBase
{
	public ADataTypeBase ChildType { get; set; }
	public UserType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode)
	{
		var typeNode = xmlNode.SelectSingleNode("Type");
		ChildType = typeNode is null ? new BlobDataType(version, definitionStore, xmlNode) : definitionStore.GetDataType(version, typeNode);
	}

	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
	{
		// Always read header otherwise we will get a padding error
		if (ChildType is not BlobDataType)
			_ = reader.ReadBytes(headerSize);
		return ChildType.GetValue(reader, propertyOrArgumentNode, headerSize);
	}
}
