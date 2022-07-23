using System.Diagnostics.CodeAnalysis;

namespace Nodsoft.WowsReplaysUnpack.Core.Extensions;

/// <summary>
/// Extension methods for dealing with Dictionaries.
/// </summary>
public static class DictionaryExtensions
{
	/// <summary>
	/// Gets a value from a dictionary, adding a new value none was found with the specified key.
	/// </summary>
	/// <param name="dict">The dictionary to get the value from.</param>
	/// <param name="key">The key to look for.</param>
	/// <param name="value">The output value.</param>
	/// <param name="getValueFunc">The function to get the value to add, if none was found.</param>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <returns>True if the value was found, false if it was added.</returns>
	public static bool GetOrAddValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, [NotNullWhen(true)] out TValue? value, Func<TValue> getValueFunc)
		where TKey : notnull
	{
		bool exists = dict.TryGetValue(key, out value);

		if (!exists)
		{
			value = getValueFunc();
			dict.Add(key, value);
		}
		
		return !exists;
	}
}
