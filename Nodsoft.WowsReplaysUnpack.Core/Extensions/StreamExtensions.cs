using System.Numerics;

namespace Nodsoft.WowsReplaysUnpack.Core.Extensions;

/// <summary>
/// Extension methods for working with Streams.
/// </summary>
public static class StreamExtensions
{
	/// <summary>
	/// Chunks a stream into buffers of the specified size.
	/// </summary>
	/// <param name="binaryReader">The binary reader.</param>
	/// <param name="chunkSize">Size of the chunk in bytes (defaults to 8b).</param>
	/// <returns>An enumerable of buffers.</returns>
	public static IEnumerable<byte[]> Chunkify(this BinaryReader binaryReader, int chunkSize = 8)
	{
		while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
		{
			yield return binaryReader.ReadBytes(chunkSize);
		}
	}

	/// <summary>
	/// Outputs a buffer of the specified binary reader,
	/// accounting for the size of the output buffer using a size header.
	/// </summary>
	/// <param name="binaryReader">The binary reader.</param>
	/// <returns>A buffer containing reader output.</returns>
	public static byte[] ReadBytesWithHeader(this BinaryReader binaryReader)
	{
		uint size = binaryReader.ReadUInt32();
		return binaryReader.ReadBytes((int)size);
	}

	/// <summary>
	/// Gets a new binary reader from the provided buffer.
	/// </summary>
	/// <param name="data">The data buffer to stream.</param>
	/// <returns>A new binary reader.</returns>
	public static BinaryReader GetBinaryReader(this byte[] data) => new(new MemoryStream(data));

	/// <summary>
	/// Deserializes a <see cref="Vector3"/> from the provided binary reader.
	/// </summary>
	/// <param name="binaryReader">The binary reader.</param>
	/// <returns>A deserialized <see cref="Vector3"/>.</returns>
	public static Vector3 ReadVector3(this BinaryReader binaryReader) => new(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
}
