using System.Numerics;

namespace Nodsoft.WowsReplaysUnpack.Core.Extensions;

public static class StreamExtensions
{
	public static IEnumerable<byte[]> Chunkify(this BinaryReader binaryReader, int chunkSize = 8)
	{
		while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
		{
			yield return binaryReader.ReadBytes(chunkSize);
		}
	}

	public static byte[] ReadBytesWithHeader(this BinaryReader binaryReader)
	{
		uint size = binaryReader.ReadUInt32();
		return binaryReader.ReadBytes((int)size);
	}

	public static BinaryReader GetBinaryReader(this byte[] data) => new(new MemoryStream(data));

	public static Vector3 ReadVector3(this BinaryReader binaryReader) => new(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
}
