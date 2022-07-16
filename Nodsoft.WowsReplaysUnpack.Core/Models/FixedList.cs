using Nodsoft.WowsReplaysUnpack.Core.DataTypes;

namespace Nodsoft.WowsReplaysUnpack.Core.Models;

public class FixedList : List<object?>, IFixedLength
{
	public DataTypeBase ElementType { get; }

	public int Length => Count;

	public FixedList(DataTypeBase elementType, IEnumerable<object?> values) : base(values) => ElementType = elementType;

	public void Slice(int start, int end, IEnumerable<object?> newItems)
	{
		List<object?> newList = this.Take(start).Concat(newItems).Concat(this.Skip(end)).ToList();
		Clear();
		AddRange(newList);
	}

	public override string ToString() => $"[{string.Join(Consts.Comma + " ", this)}]";
}
