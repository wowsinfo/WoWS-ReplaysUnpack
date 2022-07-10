namespace Nodsoft.WowsReplaysUnpack.Core.Extensions;

public static class StringExtensions
{
	public static string GetStringBeforeIndex(this string str, char before)
	  => str[..str.IndexOf(before)];

	public static string GetStringAfterIndex(this string str, char after)
	  => str[(str.IndexOf(after) + 1)..];

	public static string GetStringBeforeIndex(this string str, string before)
	  => str[..str.IndexOf(before)];

	public static string GetStringAfterLength(this string str, string after)
	  => str[after.Length..];
}
