namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

internal class VectorDataType : ADataTypeBase
{
	private readonly int _itemCount;

	protected VectorDataType(int itemCount)
	{
		_itemCount = itemCount;
	}

	/// <summary>
	/// Reads the python tuple
	/// Equivalent to tuple(struct.unpack())
	/// </summary>
	/// <param name="reader"></param>
	/// <param name="itemCount"></param>
	/// <returns></returns>
	protected override object? GetValue(BinaryReader reader)
	{
		// Size of a float value
		int itemSize = 4;
		byte[] data = reader.ReadBytes(_itemCount * itemSize);
		float[] values = new float[_itemCount];
		for (int i = 0; i < _itemCount; i++)
		{
			var index = i * itemSize;
			values[i] = BitConverter.ToSingle(data, index);
		}

		return values;
	}

	protected override object? GetDefaultValue(string defaultValue)
	=> defaultValue.Split(' ').Select(v => Convert.ToSingle(v)).ToArray();
}

/// <summary>
/// Array of floats
/// </summary>
internal class Vector2DataType : VectorDataType
{
	public override int DataSize => 8;
	public Vector2DataType() : base(2) { }
	protected override object? GetValue(BinaryReader reader)
		=> base.GetValue(reader);
}

internal class Vector3DataType : VectorDataType
{
	public override int DataSize => 12;
	public Vector3DataType() : base(3) { }
	protected override object? GetValue(BinaryReader reader)
		=> base.GetValue(reader);
}

internal class Vector4DataType : VectorDataType
{
	public override int DataSize => 16;
	public Vector4DataType() : base(4) { }
	protected override object? GetValue(BinaryReader reader)
		=> base.GetValue(reader);
}
