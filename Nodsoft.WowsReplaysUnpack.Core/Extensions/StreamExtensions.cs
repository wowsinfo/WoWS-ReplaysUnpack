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

	public static byte[] ReadRemainingBytes(this BinaryReader binaryReader)
		  => binaryReader.ReadBytes((int)binaryReader.BaseStream.Length - (int)binaryReader.BaseStream.Position);
}
