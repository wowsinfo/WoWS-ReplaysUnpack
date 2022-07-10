namespace Nodsoft.WowsReplaysUnpack.Core.Extensions;

internal static class StreamExtensions
{
	public static byte[] ReadToArray(this Stream stream, int length)
		=> ReadToArray(stream, 0, length);
	public static byte[] ReadToArray(this Stream stream, int offset, int length)
	{
		byte[] buffer = new byte[length];
		stream.Read(buffer, offset, length);
		return buffer;
	}


	public static async Task<byte[]> ReadToArrayAsync(this Stream stream, int length)
		=> await ReadToArrayAsync(stream, 0, length);
	public static async Task<byte[]> ReadToArrayAsync(this Stream stream, int offset, int length)
	{
		byte[] buffer = new byte[length];
		await stream.ReadAsync(buffer, offset, length);
		return buffer;
	}
}
