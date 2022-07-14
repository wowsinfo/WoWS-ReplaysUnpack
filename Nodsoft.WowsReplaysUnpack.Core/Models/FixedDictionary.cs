using Nodsoft.WowsReplaysUnpack.Core.DataTypes;

namespace Nodsoft.WowsReplaysUnpack.Core.Models;

public class FixedDictionary : Dictionary<string, object?>, IFixedLength
{
	private readonly Dictionary<string, ADataTypeBase> _dataTypes;

	public int Length => Length;

	public FixedDictionary(Dictionary<string, ADataTypeBase> dataTypes, Dictionary<string, object?> values)
		: base(values)
	{
		_dataTypes = dataTypes;
	}
	//public string GetKeyForIndex(int index) => _dataTypes.ElementAt(index).Key;

	public ADataTypeBase GetDataTypeForIndex(int index) => _dataTypes.ElementAt(index).Value;
}