using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

/// <summary>
/// C# float
/// </summary>
internal class Float32DataType : ADataTypeBase
{
	public Float32DataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(float))
	{
	}

	public override int DataSize => 4;
	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
		=> reader.ReadSingle();
}

/// <summary>
/// C# double
/// </summary>
internal class Float64DataType : ADataTypeBase
{
	public Float64DataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(double))
	{
	}

	public override int DataSize => 8;
	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
		=> reader.ReadDouble();
}

/// <summary>
/// C# byte
/// </summary>
internal class Int8DataType : ADataTypeBase
{
	public Int8DataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(byte))
	{
	}

	public override int DataSize => 1;
	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
		=> reader.ReadByte();
}

/// <summary>
/// C# short
/// </summary>
internal class Int16DataType : ADataTypeBase
{
	public Int16DataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(short))
	{
	}

	public override int DataSize => 2;
	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
		=> reader.ReadInt16();
}

/// <summary>
/// C# int
/// </summary>
internal class Int32DataType : ADataTypeBase
{
	public Int32DataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(int))
	{
	}

	public override int DataSize => 4;
	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
		=> reader.ReadInt32();
}

/// <summary>
/// C# long
/// </summary>
internal class Int64DataType : ADataTypeBase
{
	public Int64DataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(long))
	{
	}

	public override int DataSize => 8;
	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
		=> reader.ReadInt64();
}

/// <summary>
/// C# bool
/// </summary>
internal class UInt8DataType : ADataTypeBase
{
	public UInt8DataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(bool))
	{
	}

	public override int DataSize => 1;
	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
		=> reader.ReadByte() == 0x1;

	public override object? GetDefaultValue(XmlNode propertyOrArgumentNode, bool forArray = false)
		=> propertyOrArgumentNode.SelectSingleNodeText("Default")?.ToLower() == "true";
}

/// <summary>
/// C# ushort
/// </summary>
internal class UInt16DataType : ADataTypeBase
{
	public UInt16DataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(ushort))
	{
	}

	public override int DataSize => 2;
	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
		=> reader.ReadUInt16();
}

/// <summary>
/// C# uint
/// </summary>
internal class UInt32DataType : ADataTypeBase
{
	public UInt32DataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(uint))
	{
	}

	public override int DataSize => 4;
	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
		=> reader.ReadUInt32();
}

/// <summary>
/// C# ulong
/// </summary>
internal class UInt64DataType : ADataTypeBase
{
	public UInt64DataType(Version version, DefinitionStore definitionStore, XmlNode xmlNode)
		: base(version, definitionStore, xmlNode, typeof(ulong))
	{
	}

	public override int DataSize => 8;
	protected override object? GetValueInternal(BinaryReader reader, XmlNode propertyOrArgumentNode, int headerSize)
		=> reader.ReadUInt64();
}
