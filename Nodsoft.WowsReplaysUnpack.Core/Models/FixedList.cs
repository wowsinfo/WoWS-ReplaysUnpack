using Nodsoft.WowsReplaysUnpack.Core.DataTypes;

namespace Nodsoft.WowsReplaysUnpack.Core.Models;

public class FixedList : List<object?>, IFixedLength
{
	public ADataTypeBase ElementType { get; }

	public int Length => Count;

	public FixedList(ADataTypeBase elementType, IEnumerable<object?> values)
		: base(values)
	{
		ElementType = elementType;
	}

	public void AddAndExtend(int index, object? value)
	{
		if (Count < index+1)
			AddRange(new object?[(index+1)-Count]);
		this[index] = value;
	}
}
