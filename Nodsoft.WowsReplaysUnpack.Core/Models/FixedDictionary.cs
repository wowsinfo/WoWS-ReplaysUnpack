using Nodsoft.WowsReplaysUnpack.Core.DataTypes;

namespace Nodsoft.WowsReplaysUnpack.Core.Models;

public class FixedDictionary : Dictionary<string, object?>, IFixedLength
{
	private readonly Dictionary<string, ADataTypeBase> _dataTypes;

	public int Length => base.Count;

	public FixedDictionary(Dictionary<string, ADataTypeBase> dataTypes, Dictionary<string, object?> values)
		: base(values)
	{
		_dataTypes = dataTypes;
	}
	//public string GetKeyForIndex(int index) => _dataTypes.ElementAt(index).Key;

	public ADataTypeBase GetDataTypeForIndex(int index) => _dataTypes.ElementAt(index).Value;

	public object? ElementValueAt(int index) => this.ElementAt(index).Value;
}