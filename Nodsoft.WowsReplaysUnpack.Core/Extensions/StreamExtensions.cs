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
		var size = binaryReader.ReadUInt32();
		return binaryReader.ReadBytes((int)size);
	}
}
