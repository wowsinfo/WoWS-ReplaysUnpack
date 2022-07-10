namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

/// <summary>
/// C# float
/// </summary>
internal class Float32DataType : ADataTypeBase
{
	public override int DataSize => 4;
	protected override object? GetValue(BinaryReader reader)
		=> reader.ReadSingle();
}

/// <summary>
/// C# double
/// </summary>
internal class Float64DataType : ADataTypeBase
{
	public override int DataSize => 8;
	protected override object? GetValue(BinaryReader reader)
		=> reader.ReadDouble();
}

/// <summary>
/// C# byte
/// </summary>
internal class Int8DataType : ADataTypeBase
{
	public override int DataSize => 1;
	protected override object? GetValue(BinaryReader reader)
		=> reader.ReadByte();
}

/// <summary>
/// C# short
/// </summary>
internal class Int16DataType : ADataTypeBase
{
	public override int DataSize => 2;
	protected override object? GetValue(BinaryReader reader)
		=> reader.ReadInt16();
}

/// <summary>
/// C# int
/// </summary>
internal class Int32DataType : ADataTypeBase
{
	public override int DataSize => 4;
	protected override object? GetValue(BinaryReader reader)
		=> reader.ReadInt32();
}

/// <summary>
/// C# long
/// </summary>
internal class Int64DataType : ADataTypeBase
{
	public override int DataSize => 8;
	protected override object? GetValue(BinaryReader reader)
		=> reader.ReadInt64();
}

/// <summary>
/// C# bool
/// </summary>
internal class UInt8DataType : ADataTypeBase
{
	public override int DataSize => 1;
	protected override object? GetValue(BinaryReader reader)
		=> reader.ReadByte() == 0x1;

	protected override object? GetDefaultValue(string defaultValue) 
		=> defaultValue.ToLower() == "true";
}

/// <summary>
/// C# ushort
/// </summary>
internal class UInt16DataType : ADataTypeBase
{
	public override int DataSize => 2;
	protected override object? GetValue(BinaryReader reader)
		=> reader.ReadUInt16();
}

/// <summary>
/// C# uint
/// </summary>
internal class UInt32DataType : ADataTypeBase
{
	public override int DataSize => 4;
	protected override object? GetValue(BinaryReader reader)
		=> reader.ReadUInt32();
}

/// <summary>
/// C# ulong
/// </summary>
internal class UInt64DataType : ADataTypeBase
{
	public override int DataSize => 8;
	protected override object? GetValue(BinaryReader reader)
		=> reader.ReadUInt64();
}
