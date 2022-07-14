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
}
