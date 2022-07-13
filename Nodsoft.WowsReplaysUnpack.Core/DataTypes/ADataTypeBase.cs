using Nodsoft.WowsReplaysUnpack.Core.Extensions;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

public abstract class ADataTypeBase
{
	public virtual int DataSize { get; } = Consts.Infinity;
	public object? GetValue(BinaryReader reader, string? defaultValue = null)
	{
		object? value = GetValue(reader);
		object? _defaultValue = defaultValue is null ? null : GetDefaultValue(defaultValue);

		if (value is null && _defaultValue is null)
			return null;
		return value ?? _defaultValue;
	}

	protected abstract object? GetValue(BinaryReader reader);
	protected virtual object? GetDefaultValue(string defaultValue)
		=> throw new NotImplementedException();
	protected virtual int GetActualDataSize(BinaryReader reader)
		=> reader.ReadByte();
}
