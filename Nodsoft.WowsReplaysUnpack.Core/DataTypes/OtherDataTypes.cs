using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Security;
using Razorvine.Pickle;
using System.Collections;
using System.Text;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public class BlobDataType : ADataTypeBase
{
	public BlobDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(ArrayList))
	{
	}

	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
	{
		var bytes = reader.ReadBytes(GetSizeFromHeader(reader));
		CVEChecks.ScanForCVE_2022_31265(bytes, XmlNode?.Name);
		using Unpickler unpickler = new();
		using MemoryStream buffer = new(bytes);
		return unpickler.load(buffer) as ArrayList;
	}

	protected override int GetSizeFromHeader(BinaryReader reader)
	{
		var size = base.GetSizeFromHeader(reader);
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
	public StringDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(string))
	{
		DataSize = Consts.Infinity;
	}

	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
		=> Encoding.UTF8.GetString(reader.ReadBytes(GetSizeFromHeader(reader)));
}

public class UnicodeStringDataType : ADataTypeBase
{
	public UnicodeStringDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(string))
	{
		DataSize = Consts.Infinity;
	}

	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
		=> Encoding.Unicode.GetString(reader.ReadBytes(GetSizeFromHeader(reader)));
}

public class MailboxDataType : ADataTypeBase
{
	public MailboxDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(object))
	{
	}
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		=> "<Mailbox>";
}

public class ChildDataType : ADataTypeBase
{
	public ADataTypeBase ChildType { get; set; }
	public ChildDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(object))
	{
		var typeNode = xmlNode.SelectSingleNode("Type");
		ChildType = typeNode is null ? new BlobDataType(version, definitionStore, xmlNode) : definitionStore.GetDataType(version, typeNode);
		ClrType = ChildType.ClrType;
	}

	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
	{
		// Always read header otherwise we will get a padding error
		if (ChildType is not BlobDataType)
			_ = reader.ReadBytes(headerSize);
		return ChildType.GetValue(reader, propertyOrArgumentNode, headerSize);
	}
}

public class ArrayDataType : ADataTypeBase
{
	public ADataTypeBase ElementType { get; }
	public bool AllowNone { get; }
	public int? ItemCount { get; }
	public ArrayDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(Array))
	{
		var ofNode = xmlNode.SelectSingleNode("of")!;
		var allowNoneNode = xmlNode.SelectSingleNode("AllowNone");
		var sizeNode = xmlNode.SelectSingleNode("size");

		ElementType = definitionStore.GetDataType(version, ofNode);
		AllowNone = allowNoneNode is not null && allowNoneNode.TrimmedText() == "true";
		ClrType = Array.CreateInstance(ElementType.ClrType, 0).GetType();
		if (sizeNode is not null)
		{
			ItemCount = int.Parse(sizeNode.TrimmedText());
			DataSize = ElementType.DataSize * ItemCount.Value;
		}
	}
	public override object? GetDefaultValue(XmlNode? propertyOrArgumentNode, bool forArray = false)
		=> propertyOrArgumentNode?.SelectXmlNodes("Default/item").Select(node => ElementType.GetDefaultValue(node, true)).ToArray();
	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
	{
		var size = ItemCount ?? reader.ReadByte();
		return new FixedList(ElementType, Enumerable.Range(0, size).Select(_ => ElementType.GetValue(reader, propertyOrArgumentNode, headerSize)).ToArray());
	}
}

public class FixedDictDataType : ADataTypeBase
{
	public bool AllowNone { get; }
	public Dictionary<string, ADataTypeBase> PropertyTypes { get; set; } = new();
	public FixedDictDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(Dictionary<string, object?>))
	{
		var allowNoneNode = xmlNode.SelectSingleNode("AllowNone");
		AllowNone = allowNoneNode is not null && allowNoneNode.TrimmedText() == "true";

		foreach (var propertyNode in xmlNode.SelectSingleNode("Properties")!.ChildNodes())
			PropertyTypes.Add(propertyNode.Name, definitionStore.GetDataType(version, propertyNode.SelectSingleNode("Type")!));

		if (!AllowNone)
		{
			DataSize = PropertyTypes.Select(p => p.Value.DataSize).Sum();
		}
	}

	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
	{
		var originalStreamPosition = reader.BaseStream.Position;
		if (AllowNone)
		{
			var flag = reader.ReadByte();
			if (flag == 0x00) // empty dict
				return null;
			else if (flag == 0x01) { } // non empty dict
			else
				reader.BaseStream.Seek(originalStreamPosition, SeekOrigin.Begin);
		}

		return new FixedDictionary(PropertyTypes, PropertyTypes.ToDictionary(kv => kv.Key, kv => kv.Value.GetValue(reader, propertyOrArgumentNode, headerSize)));
	}
}
