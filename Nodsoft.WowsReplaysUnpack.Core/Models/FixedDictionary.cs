using Nodsoft.WowsReplaysUnpack.Core.DataTypes;

namespace Nodsoft.WowsReplaysUnpack.Core.Models;

public class FixedDictionary : Dictionary<string, object?>, IFixedLength
{
	private readonly Dictionary<string, DataTypeBase> _dataTypes;

	public int Length => Count;

	public FixedDictionary(Dictionary<string, DataTypeBase> dataTypes, Dictionary<string, object?> values) : base(values) => _dataTypes = dataTypes;
	//public string GetKeyForIndex(int index) => _dataTypes.ElementAt(index).Key;

	public DataTypeBase GetDataTypeForIndex(int index) => _dataTypes.ElementAt(index).Value;

	public object? ElementValueAt(int index) => this.ElementAt(index).Value;
}