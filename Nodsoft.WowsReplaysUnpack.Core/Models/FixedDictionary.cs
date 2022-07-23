using Nodsoft.WowsReplaysUnpack.Core.DataTypes;

namespace Nodsoft.WowsReplaysUnpack.Core.Models;

/// <summary>
/// Represents a fixed-length dictionary
/// </summary>
public class FixedDictionary : Dictionary<string, object?>, IFixedLength
{
	private readonly Dictionary<string, DataTypeBase> _dataTypes;
	
	/// <inheritdoc />
	public int Length => Count;

	public FixedDictionary(Dictionary<string, DataTypeBase> dataTypes, Dictionary<string, object?> values) : base(values) => _dataTypes = dataTypes;
	//public string GetKeyForIndex(int index) => _dataTypes.ElementAt(index).Key;

	/// <summary>
	/// Gets a data type for a given index.
	/// </summary>
	/// <param name="index">Index of the data type</param>
	/// <returns>Data type</returns>
	public DataTypeBase GetDataType(int index) => _dataTypes.ElementAt(index).Value;

	/// <summary>
	/// Gets the value for a given index.
	/// </summary>
	/// <param name="index">Index of the value</param>
	/// <returns>Value at index</returns>
	public object? ElementValueAt(int index) => this.ElementAt(index).Value;
}