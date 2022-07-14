using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodsoft.WowsReplaysUnpack.Core.Extensions;

public static class DictionaryExtensions
{
	public static void GetOrAddValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value,
		Func<TValue> getValueFunc)
		where TKey : notnull
	{
#pragma warning disable CS8601 // Possible null reference assignment.
		if (!dict.TryGetValue(key, out value))
		{
			value = getValueFunc();
			dict.Add(key, value);
		}
#pragma warning restore CS8601 // Possible null reference assignment.
	}
}
